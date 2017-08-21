
/// <summary>
///*****************************************************************************
/// Copyright 2011, 2012 Chris Banes.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
/// http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// ******************************************************************************
/// </summary>

using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Java.Lang;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Com.Loonytone.Droid.PullToRefresh.Inner;
using Math = System.Math;
using Void = Java.Lang.Void;

namespace Com.Loonytone.Droid.PullToRefresh
{

    public abstract class PullToRefreshBase<T> : LinearLayout, IPullToRefresh<T> where T : View
    {
        // ===========================================================
        // Constants
        // ===========================================================

        internal const bool USE_HW_LAYERS = false;

        public const string LOG_TAG = "PullToRefresh";

        internal const float FRICTION = 2.0f;

        public const int SMOOTH_SCROLL_DURATION_MS = 200;
        public const int SMOOTH_SCROLL_LONG_DURATION_MS = 325;
        internal const int DEMO_SCROLL_INTERVAL = 225;

        internal const string STATE_STATE = "ptr_state";
        internal const string STATE_MODE = "ptr_mode";
        internal const string STATE_CURRENT_MODE = "ptr_current_mode";
        internal const string STATE_SCROLLING_REFRESHING_ENABLED = "ptr_disable_scrolling";
        internal const string STATE_SHOW_REFRESHING_VIEW = "ptr_show_refreshing_view";
        internal const string STATE_SUPER = "ptr_super";

        // ===========================================================
        // Fields
        // ===========================================================

        private int mTouchSlop;
        private float mLastMotionX, mLastMotionY;
        private float mInitialMotionX, mInitialMotionY;

        private bool mIsBeingDragged = false;
        private RefreshState mState = RefreshState.RESET;
        private PullMode mMode = PullMode.PULL_FROM_START;

        private PullMode mCurrentMode;
        protected T mRefreshableView;
        private FrameLayout mRefreshableViewWrapper;

        private bool mShowViewWhileRefreshing = true;
        private bool mScrollingWhileRefreshingEnabled = false;
        private bool mFilterTouchEvents = true;
        private bool mOverScrollEnabled = true;
        private bool mLayoutVisibilityChangesEnabled = true;
        private bool mHasPullDownFriction = true;
        private bool mHasPullUpFriction = true;

        private IInterpolator mScrollAnimationInterpolator;
        private AnimationStyle mLoadingAnimationStyle = AnimationStyle.ROTATE;

        protected LoadingLayoutBase mHeaderLayout;
        protected LoadingLayoutBase mFooterLayout;

        private IOnRefreshListener<T> mOnRefreshListener;
        private IOnPullRefreshListener<T> mOnPullRefreshListener;
        private IOnPullEventListener<T> mOnPullEventListener;



        private SmoothScrollRunnable mCurrentSmoothScrollRunnable;

        // ===========================================================
        // Constructors
        // ===========================================================

        public PullToRefreshBase(Context context) : base(context)
        {
            Init(context, null);
        }

        public PullToRefreshBase(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init(context, attrs);
        }

        public PullToRefreshBase(Context context, PullMode mode) : base(context)
        {
            mMode = mode;
            Init(context, null);
        }

        public PullToRefreshBase(Context context, PullMode mode, AnimationStyle animStyle) : base(context)
        {
            mMode = mode;
            mLoadingAnimationStyle = animStyle;
            Init(context, null);
        }

        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final T refreshableView = getRefreshableView();
            T refreshableView = RefreshableView;

            if (refreshableView is ViewGroup)
            {
                (refreshableView as ViewGroup).AddView(child, index, @params);
            }
            else
            {
                throw new System.NotSupportedException("Refreshable View is not a ViewGroup so can't addView");
            }
        }

        public bool Demo()
        {
            if (mMode.showHeaderLoadingLayout() && ReadyForPullStart)
            {
                SmoothScrollToAndBack(-HeaderSize * 2);
                return true;
            }
            else if (mMode.showFooterLoadingLayout() && ReadyForPullEnd)
            {
                SmoothScrollToAndBack(FooterSize * 2);
                return true;
            }

            return false;
        }

        public PullMode CurrentMode
        {
            get
            {
                return mCurrentMode;
            }
        }

        public bool FilterTouchEvents
        {
            get
            {
                return mFilterTouchEvents;
            }
            set
            {
                mFilterTouchEvents = value;
            }
        }

        public ILoadingLayout LoadingLayoutProxy
        {
            get
            {
                return GetLoadingLayoutProxy(true, true);
            }
        }

        public ILoadingLayout GetLoadingLayoutProxy(bool includeStart, bool includeEnd)
        {
            return CreateLoadingLayoutProxy(includeStart, includeEnd);
        }

        public PullMode Mode
        {
            get
            {
                return mMode;
            }
            set
            {
                if (value != mMode)
                {
                    mMode = value;
                    UpdateUIForMode();
                }
            }
        }

        public T RefreshableView
        {
            get
            {
                return mRefreshableView;
            }
        }

        public bool ShowViewWhileRefreshing
        {
            get
            {
                return mShowViewWhileRefreshing;
            }
            set
            {
                mShowViewWhileRefreshing = value;
            }
        }

        public RefreshState State
        {
            get
            {
                return mState;
            }
        }

        /// @deprecated See <seealso cref="#isScrollingWhileRefreshingEnabled()"/>. 
        public bool DisableScrollingWhileRefreshing
        {
            get
            {
                return !ScrollingWhileRefreshingEnabled;
            }
            set
            {
                ScrollingWhileRefreshingEnabled = !value;
            }
        }

        public bool IsPullToRefreshEnabled
        {
            get
            {
                return mMode.permitsPullToRefresh();
            }
            set
            {
                Mode = value ? PullMode.PULL_FROM_START : PullMode.DISABLED;
            }
        }

        public bool PullToRefreshOverScrollEnabled
        {
            get
            {
                return Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread && mOverScrollEnabled && OverscrollHelper.isAndroidOverScrollEnabled(mRefreshableView);
            }
            set
            {
                mOverScrollEnabled = value;
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return mState == RefreshState.REFRESHING || mState == RefreshState.MANUAL_REFRESHING;
            }
            set
            {
                if (!IsRefreshing)
                {
                    //保证从开始处刷新。
                    if (mCurrentMode != PullMode.PULL_FROM_START)
                    {
                        mCurrentMode = PullMode.PULL_FROM_START;
                    }                    
                    SetState(RefreshState.MANUAL_REFRESHING, value);
                }
            }
        }

        public bool ScrollingWhileRefreshingEnabled
        {
            get
            {
                return mScrollingWhileRefreshingEnabled;
            }
            set
            {
                mScrollingWhileRefreshingEnabled = value;
            }
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {

            if (!IsPullToRefreshEnabled)
            {
                return false;
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int action = event.getAction();
            MotionEventActions action = ev.Action;

            if (action == MotionEventActions.Cancel || action == MotionEventActions.Up)
            {
                mIsBeingDragged = false;
                return false;
            }

            if (action != MotionEventActions.Down && mIsBeingDragged)
            {
                return true;
            }

            switch (action)
            {
                case MotionEventActions.Move:
                    {
                        // If we're refreshing, and the flag is set. Eat all MOVE events
                        if (!mScrollingWhileRefreshingEnabled && IsRefreshing)
                        {
                            return true;
                        }

                        if (ReadyForPull)
                        {
                            float y = ev.GetY(), x = ev.GetX();
                            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                            //ORIGINAL LINE: final float diff, oppositeDiff, absDiff;
                            float diff, oppositeDiff, absDiff;

                            // We need to use the correct values, based on scroll
                            // direction
                            switch (PullToRefreshScrollDirection)
                            {
                                case ScrollOrientation.HORIZONTAL:
                                    diff = x - mLastMotionX;
                                    oppositeDiff = y - mLastMotionY;
                                    break;
                                case ScrollOrientation.VERTICAL:
                                default:
                                    diff = y - mLastMotionY;
                                    oppositeDiff = x - mLastMotionX;
                                    break;
                            }
                            absDiff = Math.Abs(diff);

                            if (absDiff > mTouchSlop && (!mFilterTouchEvents || absDiff > Math.Abs(oppositeDiff)))
                            {
                                if (mMode.showHeaderLoadingLayout() && diff >= 1f && ReadyForPullStart)
                                {
                                    mLastMotionY = y;
                                    mLastMotionX = x;
                                    mIsBeingDragged = true;
                                    if (mMode == PullMode.BOTH)
                                    {
                                        mCurrentMode = PullMode.PULL_FROM_START;
                                    }
                                }
                                else if (mMode.showFooterLoadingLayout() && diff <= -1f && ReadyForPullEnd)
                                {
                                    mLastMotionY = y;
                                    mLastMotionX = x;
                                    mIsBeingDragged = true;
                                    if (mMode == PullMode.BOTH)
                                    {
                                        mCurrentMode = PullMode.PULL_FROM_END;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case MotionEventActions.Down:
                    {
                        if (ReadyForPull)
                        {
                            mLastMotionY = mInitialMotionY = ev.GetY();
                            mLastMotionX = mInitialMotionX = ev.GetX();
                            mIsBeingDragged = false;
                        }
                        break;
                    }
            }

            return mIsBeingDragged;
        }

        public void OnRefreshComplete()
        {
            if (IsRefreshing)
            {
                SetState(RefreshState.RESET);
            }
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {

            if (!IsPullToRefreshEnabled)
            {
                return false;
            }

            // If we're refreshing, and the flag is set. Eat the event
            if (!mScrollingWhileRefreshingEnabled && IsRefreshing)
            {
                return true;
            }

            if (ev.Action == MotionEventActions.Down && ev.EdgeFlags != 0)
            {
                return false;
            }

            switch (ev.Action)
            {
                case MotionEventActions.Move:
                    {
                        if (mIsBeingDragged)
                        {
                            mLastMotionY = ev.GetY();
                            mLastMotionX = ev.GetX();
                            PullEvent();
                            return true;
                        }
                        break;
                    }

                case MotionEventActions.Down:
                    {
                        if (ReadyForPull)
                        {
                            mLastMotionY = mInitialMotionY = ev.GetY();
                            mLastMotionX = mInitialMotionX = ev.GetX();
                            return true;
                        }
                        break;
                    }

                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    {
                        if (mIsBeingDragged)
                        {
                            mIsBeingDragged = false;

                            if (mState == RefreshState.RELEASE_TO_REFRESH && (null != mOnRefreshListener || null != mOnPullRefreshListener))
                            {
                                SetState(RefreshState.REFRESHING, true);
                                return true;
                            }

                            // If we're already refreshing, just scroll back to the top
                            if (IsRefreshing)
                            {
                                SmoothScrollTo(0);
                                return true;
                            }

                            // If we haven't returned by here, then we're not in a state
                            // to pull, so just reset
                            SetState(RefreshState.RESET);

                            return true;
                        }
                        break;
                    }
            }

            return false;
        }




        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy()"/>. 
        public virtual string LastUpdatedLabel
        {
            set
            {
                LoadingLayoutProxy.LastUpdatedLabel = value;
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy()"/>. 
        public virtual Drawable LoadingDrawable
        {
            set
            {
                LoadingLayoutProxy.LoadingDrawable = value;
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy(boolean, boolean)"/>. 
        public virtual void SetLoadingDrawable(Drawable drawable, PullMode mode)
        {
            GetLoadingLayoutProxy(mode.showHeaderLoadingLayout(), mode.showFooterLoadingLayout()).LoadingDrawable = drawable;
        }

        public override bool LongClickable
        {
            set
            {
                RefreshableView.LongClickable = value;
            }
        }


        public virtual IOnPullEventListener<T> OnPullEventListener
        {
            set
            {
                mOnPullEventListener = value;
            }
        }

        public IOnRefreshListener<T> OnRefreshListener
        {
            set
            {
                mOnRefreshListener = value;
                mOnPullRefreshListener = null;
            }
        }

        public IOnPullRefreshListener<T> OnPullRefreshListener
        {
            set
            {
                mOnPullRefreshListener = value;
                mOnRefreshListener = null;
            }
        }

        public virtual LoadingLayoutBase HeaderLayout
        {
            set
            {
                mHeaderLayout = value;
                UpdateUIForMode();
            }
            get
            {
                return mHeaderLayout;
            }
        }

        public virtual LoadingLayoutBase FooterLayout
        {
            set
            {
                mFooterLayout = value;
                UpdateUIForMode();
            }
            get
            {
                return mFooterLayout;
            }
        }

        public virtual View SecondFooterLayout
        {
            set
            {
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy()"/>. 
        public virtual string PullLabel
        {
            set
            {
                LoadingLayoutProxy.PullLabel = value;
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy(boolean, boolean)"/>. 
        public virtual void SetPullLabel(string pullLabel, PullMode mode)
        {
            GetLoadingLayoutProxy(mode.showHeaderLoadingLayout(), mode.showFooterLoadingLayout()).PullLabel = pullLabel;
        }

        public virtual bool HasPullDownFriction
        {
            set
            {
                this.mHasPullDownFriction = value;
            }
        }

        public virtual bool HasPullUpFriction
        {
            set
            {
                this.mHasPullUpFriction = value;
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy()"/>. 
        public virtual string RefreshingLabel
        {
            set
            {
                LoadingLayoutProxy.RefreshingLabel = value;
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy(boolean, boolean)"/>. 
        public virtual void SetRefreshingLabel(string refreshingLabel, PullMode mode)
        {
            GetLoadingLayoutProxy(mode.showHeaderLoadingLayout(), mode.showFooterLoadingLayout()).RefreshingLabel = refreshingLabel;
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy()"/>. 
        public virtual string ReleaseLabel
        {
            set
            {
                SetReleaseLabel(value, PullMode.BOTH);
            }
        }

        /// @deprecated You should now call this method on the result of
        ///             <seealso cref="#getLoadingLayoutProxy(boolean, boolean)"/>. 
        public virtual void SetReleaseLabel(string releaseLabel, PullMode mode)
        {
            GetLoadingLayoutProxy(mode.showHeaderLoadingLayout(), mode.showFooterLoadingLayout()).ReleaseLabel = releaseLabel;
        }

        public virtual IInterpolator ScrollAnimationInterpolator
        {
            set
            {
                mScrollAnimationInterpolator = value;
            }
        }


        /// <returns> Either <seealso cref="ScrollOrientation#VERTICAL"/> or
        ///         <seealso cref="ScrollOrientation#HORIZONTAL"/> depending on the scroll direction. </returns>
        public abstract ScrollOrientation PullToRefreshScrollDirection { get; }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: final void setState(RefreshState state, final boolean... params)
        internal void SetState(RefreshState state, params bool[] @params)
        {
            mState = state;

            switch (mState)
            {
                case RefreshState.RESET:
                    OnReset();
                    break;
                case RefreshState.PULL_TO_REFRESH:
                    OnPullToRefresh();
                    break;
                case RefreshState.RELEASE_TO_REFRESH:
                    OnReleaseToRefresh();
                    break;
                case RefreshState.REFRESHING:
                case RefreshState.MANUAL_REFRESHING:
                    OnRefreshing(@params[0]);
                    break;
                case RefreshState.OVERSCROLLING:
                    // NO-OP
                    break;
            }

            // Call IOnPullEventListener
            if (null != mOnPullEventListener)
            {
                mOnPullEventListener.onPullEvent(this, mState, mCurrentMode);
            }
        }

        /// <summary>
        /// Used internally for adding view. Need because we override addView to
        /// pass-through to the Refreshable View
        /// </summary>
        protected void AddViewInternal(View child, int index, ViewGroup.LayoutParams @params)
        {
            base.AddView(child, index, @params);
        }

        /// <summary>
        /// Used internally for adding view. Need because we override addView to
        /// pass-through to the Refreshable View
        /// </summary>
        protected void AddViewInternal(View child, ViewGroup.LayoutParams @params)
        {
            base.AddView(child, -1, @params);
        }

        protected virtual LoadingLayoutBase CreateLoadingLayout(Context context, PullMode mode, TypedArray attrs)
        {
            LoadingLayoutBase layout = mLoadingAnimationStyle.createLoadingLayout(context, mode, PullToRefreshScrollDirection, attrs);
            layout.Visibility = ViewStates.Invisible;
            return layout;
        }

        /// <summary>
        /// Used internally for <seealso cref="#getLoadingLayoutProxy(boolean, boolean)"/>.
        /// Allows derivative classes to include any extra LoadingLayouts.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected LoadingLayoutProxy createLoadingLayoutProxy(final boolean includeStart, final boolean includeEnd)
        protected virtual LoadingLayoutProxy CreateLoadingLayoutProxy(bool includeStart, bool includeEnd)
        {
            LoadingLayoutProxy proxy = new LoadingLayoutProxy();

            if (includeStart && mMode.showHeaderLoadingLayout())
            {
                proxy.addLayout(mHeaderLayout);
            }
            if (includeEnd && mMode.showFooterLoadingLayout())
            {
                proxy.addLayout(mFooterLayout);
            }

            return proxy;
        }

        /// <summary>
        /// This is implemented by derived classes to return the created View. If you
        /// need to use a custom View (such as a custom ListView), override this
        /// method and return an instance of your custom class.
        /// 
        /// Be sure to set the ID of the view in this method, especially if you're
        /// using a ListActivity or ListFragment.
        /// </summary>
        /// <param name="context"> Context to create view with </param>
        /// <param name="attrs"> IAttributeSet from wrapped class. Means that anything you
        ///            include in the XML layout declaration will be routed to the
        ///            created View </param>
        /// <returns> New instance of the Refreshable View </returns>
        protected abstract T CreateRefreshableView(Context context, IAttributeSet attrs);

        protected void DisableLoadingLayoutVisibilityChanges()
        {
            mLayoutVisibilityChangesEnabled = false;
        }


        protected int FooterSize
        {
            get
            {
                return mFooterLayout.ContentSize;
            }
        }


        protected int HeaderSize
        {
            get
            {
                return mHeaderLayout.ContentSize;
            }
        }

        protected virtual int PullToRefreshScrollDuration
        {
            get
            {
                return SMOOTH_SCROLL_DURATION_MS;
            }
        }

        protected virtual int PullToRefreshScrollDurationLonger
        {
            get
            {
                return SMOOTH_SCROLL_LONG_DURATION_MS;
            }
        }

        protected virtual FrameLayout RefreshableViewWrapper
        {
            get
            {
                return mRefreshableViewWrapper;
            }
        }

        /// <summary>
        /// Allows Derivative classes to handle the XML Attrs without creating a
        /// TypedArray themsevles
        /// </summary>
        /// <param name="a"> - TypedArray of PullToRefresh Attributes </param>
        protected virtual void HandleStyledAttributes(TypedArray a)
        {
        }

        /// <summary>
        /// Implemented by derived class to return whether the View is in a state
        /// where the user can Pull to Refresh by scrolling from the end.
        /// </summary>
        /// <returns> true if the View is currently in the correct state (for example,
        ///         bottom of a ListView) </returns>
        protected abstract bool ReadyForPullEnd { get; }

        /// <summary>
        /// Implemented by derived class to return whether the View is in a state
        /// where the user can Pull to Refresh by scrolling from the start.
        /// </summary>
        /// <returns> true if the View is currently the correct state (for example, top
        ///         of a ListView) </returns>
        protected abstract bool ReadyForPullStart { get; }

        /// <summary>
        /// Called by <seealso cref="#onRestoreInstanceState(Parcelable)"/> so that derivative
        /// classes can handle their saved instance state.
        /// </summary>
        /// <param name="savedInstanceState"> - Bundle which contains saved instance state. </param>
        protected virtual void OnPtrRestoreInstanceState(Bundle savedInstanceState)
        {
        }

        /// <summary>
        /// Called by <seealso cref="#onSaveInstanceState()"/> so that derivative classes can
        /// save their instance state.
        /// </summary>
        /// <param name="saveState"> - Bundle to be updated with saved state. </param>
        protected virtual void OnPtrSaveInstanceState(Bundle saveState)
        {
        }

        /// <summary>
        /// Called when the UI has been to be updated to be in the
        /// <seealso cref="RefreshState#PULL_TO_REFRESH"/> state.
        /// </summary>
        protected virtual void OnPullToRefresh()
        {
            switch (mCurrentMode)
            {
                case PullMode.PULL_FROM_END:
                    mFooterLayout.pullToRefresh();
                    break;
                case PullMode.PULL_FROM_START:
                    mHeaderLayout.pullToRefresh();
                    break;
                default:
                    // NO-OP
                    break;
            }
        }

        /// <summary>
        /// Called when the UI has been to be updated to be in the
        /// <seealso cref="RefreshState#REFRESHING"/> or <seealso cref="RefreshState#MANUAL_REFRESHING"/> state.
        /// </summary>
        /// <param name="doScroll"> - Whether the UI should scroll for this event. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void onRefreshing(final boolean doScroll)
        protected virtual void OnRefreshing(bool doScroll)
        {
            if (mMode.showHeaderLoadingLayout())
            {
                mHeaderLayout.refreshing();
            }
            if (mMode.showFooterLoadingLayout())
            {
                mFooterLayout.refreshing();
            }

            if (doScroll)
            {
                if (mShowViewWhileRefreshing)
                {

                    // Call Refresh Listener when the Scroll has finished
                    IOnSmoothScrollFinishedListener listener = new IOnSmoothScrollFinishedListenerAnonymousInnerClass(this);

                    switch (mCurrentMode)
                    {
                        case PullMode.MANUAL_REFRESH_ONLY:
                        case PullMode.PULL_FROM_END:
                            SmoothScrollTo(FooterSize, listener);
                            break;
                        case PullMode.PULL_FROM_START:
                        default:
                            SmoothScrollTo(-HeaderSize, listener);
                            break;
                    }
                }
                else
                {
                    SmoothScrollTo(0);
                }
            }
            else
            {
                // We're not scrolling, so just call Refresh Listener now
                CallRefreshListener();
            }
        }

        private class IOnSmoothScrollFinishedListenerAnonymousInnerClass : IOnSmoothScrollFinishedListener
        {
            private readonly PullToRefreshBase<T> outerInstance;

            public IOnSmoothScrollFinishedListenerAnonymousInnerClass(PullToRefreshBase<T> outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void onSmoothScrollFinished()
            {
                outerInstance.CallRefreshListener();
            }
        }

        /// <summary>
        /// Called when the UI has been to be updated to be in the
        /// <seealso cref="RefreshState#RELEASE_TO_REFRESH"/> state.
        /// </summary>
        protected virtual void OnReleaseToRefresh()
        {
            switch (mCurrentMode)
            {
                case PullMode.PULL_FROM_END:
                    mFooterLayout.releaseToRefresh();
                    break;
                case PullMode.PULL_FROM_START:
                    mHeaderLayout.releaseToRefresh();
                    break;
                default:
                    // NO-OP
                    break;
            }
        }

        /// <summary>
        /// Called when the UI has been to be updated to be in the
        /// <seealso cref="RefreshState#RESET"/> state.
        /// </summary>
        protected virtual void OnReset()
        {
            mIsBeingDragged = false;
            mLayoutVisibilityChangesEnabled = true;

            // Always reset both layouts, just in case...
            mHeaderLayout.reset();
            mFooterLayout.reset();

            SmoothScrollTo(0);
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state is Bundle bundle)
            {
                Mode = (PullMode)bundle.GetInt(STATE_MODE, 0);
                mCurrentMode = (PullMode)bundle.GetInt(STATE_CURRENT_MODE, 0);

                mScrollingWhileRefreshingEnabled = bundle.GetBoolean(STATE_SCROLLING_REFRESHING_ENABLED, false);
                mShowViewWhileRefreshing = bundle.GetBoolean(STATE_SHOW_REFRESHING_VIEW, true);

                // Let super Restore Itself
                base.OnRestoreInstanceState((IParcelable)bundle.GetParcelable(STATE_SUPER));

                RefreshState viewState = (RefreshState)bundle.GetInt(STATE_STATE, 0);
                if (viewState == RefreshState.REFRESHING || viewState == RefreshState.MANUAL_REFRESHING)
                {
                    SetState(viewState, true);
                }

                // Now let derivative classes restore their state
                OnPtrRestoreInstanceState(bundle);
                return;
            }

            try
            {
                base.OnRestoreInstanceState(state);
            }
            catch (System.Exception e)
            {
                Log.Error("PullToRefreshBase", e.StackTrace);
                Log.Error("PullToRefreshBase", e.Message);
                state = null;
            }
                      
        }

        protected IParcelable OnSaveInstanceState()
        {
            Bundle bundle = new Bundle();

            // Let derivative classes get a chance to save state first, that way we
            // can make sure they don't overrite any of our values
            OnPtrSaveInstanceState(bundle);

            bundle.PutInt(STATE_STATE, (int)mState);
            bundle.PutInt(STATE_MODE, (int)mMode);
            bundle.PutInt(STATE_CURRENT_MODE, (int)mCurrentMode);
            bundle.PutBoolean(STATE_SCROLLING_REFRESHING_ENABLED, mScrollingWhileRefreshingEnabled);
            bundle.PutBoolean(STATE_SHOW_REFRESHING_VIEW, mShowViewWhileRefreshing);
            bundle.PutParcelable(STATE_SUPER, base.OnSaveInstanceState());

            return bundle;
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            // We need to update the header/footer when our size changes
            RefreshLoadingViewsSize();

            // Update the Refreshable View layout
            RefreshRefreshableViewSize(w, h);

            /// <summary>
            /// As we're currently in a Layout Pass, we need to schedule another one
            /// to layout any changes we've made here
            /// </summary>
            Post(() =>
            {
                RequestLayout();
            });
        }

        /// <summary>
        /// Re-measure the Loading Views height, and adjust internal padding as
        /// necessary
        /// </summary>
        protected void RefreshLoadingViewsSize()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int maximumPullScroll = (int)(getMaximumPullScroll() * 1.2f);
            int maximumPullScroll = (int)(MaximumPullScroll * 1.2f);

            int pLeft = PaddingLeft;
            int pTop = PaddingTop;
            int pRight = PaddingRight;
            int pBottom = PaddingBottom;

            switch (PullToRefreshScrollDirection)
            {
                case ScrollOrientation.HORIZONTAL:
                    if (mMode.showHeaderLoadingLayout())
                    {
                        mHeaderLayout.SetWidth(maximumPullScroll);
                        pLeft = -maximumPullScroll;
                    }
                    else
                    {
                        pLeft = 0;
                    }

                    if (mMode.showFooterLoadingLayout())
                    {
                        mFooterLayout.SetWidth(maximumPullScroll);
                        pRight = -maximumPullScroll;
                    }
                    else
                    {
                        pRight = 0;
                    }
                    break;

                case ScrollOrientation.VERTICAL:
                    if (mMode.showHeaderLoadingLayout())
                    {
                        mHeaderLayout.SetHeight(maximumPullScroll);
                        pTop = -maximumPullScroll;
                    }
                    else
                    {
                        pTop = 0;
                    }

                    if (mMode.showFooterLoadingLayout())
                    {
                        mFooterLayout.SetHeight(maximumPullScroll);
                        pBottom = -maximumPullScroll;
                    }
                    else
                    {
                        pBottom = 0;
                    }
                    break;
            }
            SetPadding(pLeft, pTop, pRight, pBottom);
        }

        protected void RefreshRefreshableViewSize(int width, int height)
        {
            // We need to set the Height of the Refreshable View to the same as
            // this layout
            LayoutParams lp = (LayoutParams)mRefreshableViewWrapper.LayoutParameters;

            switch (PullToRefreshScrollDirection)
            {
                case ScrollOrientation.HORIZONTAL:
                    if (lp.Width != width)
                    {
                        lp.Width = width;
                        mRefreshableViewWrapper.RequestLayout();
                    }
                    break;
                case ScrollOrientation.VERTICAL:
                    if (lp.Height != height)
                    {
                        lp.Height = height;
                        mRefreshableViewWrapper.RequestLayout();
                    }
                    break;
            }
        }

        /// <summary>
        /// Helper method which just calls scrollTo() in the correct scrolling
        /// direction.
        /// </summary>
        /// <param name="value"> - New Scroll value </param>
        public int HeaderScroll
        {
            set
            {
                // Clamp value to with pull scroll range
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int maximumPullScroll = getMaximumPullScroll();
                int maximumPullScroll = MaximumPullScroll;
                value = Math.Min(maximumPullScroll, Math.Max(-maximumPullScroll, value));

                if (mLayoutVisibilityChangesEnabled)
                {
                    if (value < 0)
                    {
                        mHeaderLayout.Visibility = ViewStates.Visible;
                    }
                    else if (value > 0)
                    {
                        mFooterLayout.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        mHeaderLayout.Visibility = ViewStates.Invisible;
                        mFooterLayout.Visibility = ViewStates.Invisible;
                    }
                }

                if (USE_HW_LAYERS)
                {
                    /// <summary>
                    /// Use a Hardware Layer on the Refreshable View if we've scrolled at
                    /// all. We don't use them on the Header/Footer Views as they change
                    /// often, which would negate any HW layer performance boost.
                    /// </summary>
                    ViewCompat.setLayerType(mRefreshableViewWrapper, value != 0 ? LayerType.Hardware : LayerType.None);
                }

                switch (PullToRefreshScrollDirection)
                {
                    case ScrollOrientation.VERTICAL:
                        ScrollTo(0, value);
                        break;
                    case ScrollOrientation.HORIZONTAL:
                        ScrollTo(value, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Smooth Scroll to position using the default duration of
        /// {@value #SMOOTH_SCROLL_DURATION_MS} ms.
        /// </summary>
        /// <param name="scrollValue"> - Position to scroll to </param>
        protected void SmoothScrollTo(int scrollValue)
        {
            SmoothScrollTo(scrollValue, PullToRefreshScrollDuration);
        }

        /// <summary>
        /// Smooth Scroll to position using the default duration of
        /// {@value #SMOOTH_SCROLL_DURATION_MS} ms.
        /// </summary>
        /// <param name="scrollValue"> - Position to scroll to </param>
        /// <param name="listener"> - Listener for scroll </param>
        protected void SmoothScrollTo(int scrollValue, IOnSmoothScrollFinishedListener listener)
        {
            SmoothScrollTo(scrollValue, PullToRefreshScrollDuration, 0, listener);
        }

        /// <summary>
        /// Smooth Scroll to position using the longer default duration of
        /// {@value #SMOOTH_SCROLL_LONG_DURATION_MS} ms.
        /// </summary>
        /// <param name="scrollValue"> - Position to scroll to </param>
        protected void SmoothScrollToLonger(int scrollValue)
        {
            SmoothScrollTo(scrollValue, PullToRefreshScrollDurationLonger);
        }

        /// <summary>
        /// Updates the View RefreshState when the mode has been set. This does not do any
        /// checking that the mode is different to current state so always updates.
        /// </summary>
        protected virtual void UpdateUIForMode()
        {
            // We need to use the correct LayoutParam values, based on scroll
            // direction
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final LayoutParams lp = getLoadingLayoutLayoutParams();
            LayoutParams lp = LoadingLayoutLayoutParams;

            // Remove Header, and then add Header Loading View again if needed
            if (this == mHeaderLayout.Parent)
            {
                RemoveView(mHeaderLayout);
            }
            if (mMode.showHeaderLoadingLayout())
            {
                AddViewInternal(mHeaderLayout, 0, lp);
            }

            // Remove Footer, and then add Footer Loading View again if needed
            if (this == mFooterLayout.Parent)
            {
                RemoveView(mFooterLayout);
            }
            if (mMode.showFooterLoadingLayout())
            {
                AddViewInternal(mFooterLayout, lp);
            }

            // Hide Loading Views
            RefreshLoadingViewsSize();

            // If we're not using PullMode.BOTH, set mCurrentMode to mMode, otherwise
            // set it to pull down
            mCurrentMode = (mMode != PullMode.BOTH) ? mMode : PullMode.PULL_FROM_START;
        }

        private void AddRefreshableView(Context context, T refreshableView)
        {
            mRefreshableViewWrapper = new FrameLayout(context);
            mRefreshableViewWrapper.AddView(refreshableView, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            AddViewInternal(mRefreshableViewWrapper, new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent));
        }

        private void CallRefreshListener()
        {
            if (null != mOnRefreshListener)
            {
                mOnRefreshListener.OnRefresh(this);
            }
            else if (null != mOnPullRefreshListener)
            {
                if (mCurrentMode == PullMode.PULL_FROM_START)
                {
                    mOnPullRefreshListener.OnPullDownToRefresh(this);
                }
                else if (mCurrentMode == PullMode.PULL_FROM_END)
                {
                    mOnPullRefreshListener.OnPullUpToRefresh(this);
                }
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("deprecation") private void init(android.content.Context context, android.util.IAttributeSet attrs)
        private void Init(Context context, IAttributeSet attrs)
        {
            switch (PullToRefreshScrollDirection)
            {
                case ScrollOrientation.HORIZONTAL:
                    Orientation = Android.Widget.Orientation.Horizontal;
                    break;
                case ScrollOrientation.VERTICAL:
                default:
                    Orientation = Android.Widget.Orientation.Vertical;
                    break;
            }

            SetGravity(GravityFlags.Center);

            ViewConfiguration config = ViewConfiguration.Get(context);
            mTouchSlop = config.ScaledTouchSlop;

            // Styleables from XML
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.PullToRefresh);

            if (a.HasValue(Resource.Styleable.PullToRefresh_ptrMode))
            {
                mMode = (PullMode)a.GetInteger(Resource.Styleable.PullToRefresh_ptrMode, 0);
            }

            if (a.HasValue(Resource.Styleable.PullToRefresh_ptrAnimationStyle))
            {
                mLoadingAnimationStyle = (AnimationStyle)a.GetInteger(Resource.Styleable.PullToRefresh_ptrAnimationStyle, 0);
            }

            // Refreshable View
            // By passing the attrs, we can add ListView/GridView params via XML
            mRefreshableView = CreateRefreshableView(context, attrs);
            AddRefreshableView(context, mRefreshableView);

            // We need to create now layouts now
            mHeaderLayout = CreateLoadingLayout(context, PullMode.PULL_FROM_START, a);
            mFooterLayout = CreateLoadingLayout(context, PullMode.PULL_FROM_END, a);

            /// <summary>
            /// Styleables from XML
            /// </summary>
            if (a.HasValue(Resource.Styleable.PullToRefresh_ptrRefreshableViewBackground))
            {
                Drawable background = a.GetDrawable(Resource.Styleable.PullToRefresh_ptrRefreshableViewBackground);
                if (null != background)
                {
                    mRefreshableView.SetBackgroundDrawable(background);
                }
            }
            else if (a.HasValue(Resource.Styleable.PullToRefresh_ptrAdapterViewBackground))
            {
                Utils.warnDeprecation("ptrAdapterViewBackground", "ptrRefreshableViewBackground");
                Drawable background = a.GetDrawable(Resource.Styleable.PullToRefresh_ptrAdapterViewBackground);
                if (null != background)
                {
                    mRefreshableView.SetBackgroundDrawable(background);
                }
            }

            if (a.HasValue(Resource.Styleable.PullToRefresh_ptrOverScroll))
            {
                mOverScrollEnabled = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrOverScroll, true);
            }

            if (a.HasValue(Resource.Styleable.PullToRefresh_ptrScrollingWhileRefreshingEnabled))
            {
                mScrollingWhileRefreshingEnabled = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrScrollingWhileRefreshingEnabled, false);
            }

            // Let the derivative classes have a go at handling attributes, then
            // recycle them...
            HandleStyledAttributes(a);
            a.Recycle();

            // Finally update the UI for the modes
            UpdateUIForMode();
        }

        private bool ReadyForPull
        {
            get
            {
                switch (mMode)
                {
                    case PullMode.PULL_FROM_START:
                        return ReadyForPullStart;
                    case PullMode.PULL_FROM_END:
                        return ReadyForPullEnd;
                    case PullMode.BOTH:
                        return ReadyForPullEnd || ReadyForPullStart;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Actions a Pull Event
        /// </summary>
        /// <returns> true if the Event has been handled, false if there has been no
        ///         change </returns>
        private void PullEvent()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int newScrollValue;
            int newScrollValue;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int itemDimension;
            int itemDimension;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final float initialMotionValue, lastMotionValue;
            float initialMotionValue, lastMotionValue;

            switch (PullToRefreshScrollDirection)
            {
                case ScrollOrientation.HORIZONTAL:
                    initialMotionValue = mInitialMotionX;
                    lastMotionValue = mLastMotionX;
                    break;
                case ScrollOrientation.VERTICAL:
                default:
                    initialMotionValue = mInitialMotionY;
                    lastMotionValue = mLastMotionY;
                    break;
            }

            switch (mCurrentMode)
            {
                case PullMode.PULL_FROM_END:
                    if (mHasPullUpFriction)
                    {
                        newScrollValue = (int)Math.Round(Math.Max(initialMotionValue - lastMotionValue, 0) / FRICTION);
                    }
                    else
                    {
                        newScrollValue = (int)Math.Round(Math.Max(initialMotionValue - lastMotionValue, 0));
                    }
                    itemDimension = FooterSize;
                    break;
                case PullMode.PULL_FROM_START:
                default:
                    if (mHasPullDownFriction)
                    {
                        newScrollValue = (int)Math.Round(Math.Min(initialMotionValue - lastMotionValue, 0) / FRICTION);
                    }
                    else
                    {
                        newScrollValue = (int)Math.Round(Math.Min(initialMotionValue - lastMotionValue, 0));
                    }
                    itemDimension = HeaderSize;
                    break;
            }

            HeaderScroll = newScrollValue;

            if (newScrollValue != 0 && !IsRefreshing)
            {
                float scale = Math.Abs(newScrollValue) / (float)itemDimension;
                switch (mCurrentMode)
                {
                    case PullMode.PULL_FROM_END:
                        mFooterLayout.onPull(scale);
                        break;
                    case PullMode.PULL_FROM_START:
                    default:
                        mHeaderLayout.onPull(scale);
                        break;
                }

                if (mState != RefreshState.PULL_TO_REFRESH && itemDimension >= Math.Abs(newScrollValue))
                {
                    SetState(RefreshState.PULL_TO_REFRESH);
                }
                else if (mState == RefreshState.PULL_TO_REFRESH && itemDimension < Math.Abs(newScrollValue))
                {
                    SetState(RefreshState.RELEASE_TO_REFRESH);
                }
            }
        }

        private LayoutParams LoadingLayoutLayoutParams
        {
            get
            {
                switch (PullToRefreshScrollDirection)
                {
                    case ScrollOrientation.HORIZONTAL:
                        return new LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent);
                    case ScrollOrientation.VERTICAL:
                    default:
                        return new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
                }
            }
        }

        private int MaximumPullScroll
        {
            get
            {
                switch (PullToRefreshScrollDirection)
                {
                    case ScrollOrientation.HORIZONTAL:
                        return (int)Math.Round(Width / FRICTION);
                    case ScrollOrientation.VERTICAL:
                    default:
                        return (int)Math.Round(Height / FRICTION);
                }
            }
        }

        /// <summary>
        /// Smooth Scroll to position using the specific duration
        /// </summary>
        /// <param name="scrollValue"> - Position to scroll to </param>
        /// <param name="duration"> - Duration of animation in milliseconds </param>
        private void SmoothScrollTo(int scrollValue, long duration)
        {
            SmoothScrollTo(scrollValue, duration, 0, null);
        }

        private void SmoothScrollTo(int newScrollValue, long duration, long delayMillis, IOnSmoothScrollFinishedListener listener)
        {
            if (null != mCurrentSmoothScrollRunnable)
            {
                mCurrentSmoothScrollRunnable.Stop();
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int oldScrollValue;
            int oldScrollValue;
            switch (PullToRefreshScrollDirection)
            {
                case ScrollOrientation.HORIZONTAL:
                    oldScrollValue = ScrollX;
                    break;
                case ScrollOrientation.VERTICAL:
                default:
                    oldScrollValue = ScrollY;
                    break;
            }

            if (oldScrollValue != newScrollValue)
            {
                if (null == mScrollAnimationInterpolator)
                {
                    // Default interpolator is a Decelerate Interpolator
                    mScrollAnimationInterpolator = new DecelerateInterpolator();
                }
                mCurrentSmoothScrollRunnable = new SmoothScrollRunnable(this, oldScrollValue, newScrollValue, duration, listener);

                if (delayMillis > 0)
                {
                    PostDelayed(mCurrentSmoothScrollRunnable, delayMillis);
                }
                else
                {
                    Post(mCurrentSmoothScrollRunnable);
                }
            }
        }

        private void SmoothScrollToAndBack(int y)
        {
            SmoothScrollTo(y, SMOOTH_SCROLL_DURATION_MS, 0, new IOnSmoothScrollFinishedListenerAnonymousInnerClass2(this));
        }

        public void FirstRefresh()
        {
            var task = new FirstRefreshAsyncTask(this);
            task.Execute();
        }

        private class FirstRefreshAsyncTask : AsyncTask<Integer, Void, Integer>
        {
            PullToRefreshBase<T> outerInstance;

            public FirstRefreshAsyncTask(PullToRefreshBase<T> outer) {
                outerInstance = outer;
            }
            protected override Integer RunInBackground(params Integer[] @params)
            {
                while (true)
                {
                    if (outerInstance.mHeaderLayout.Height > 0) //已测量，则跳回到主线程执行postExecute()
                        return null;
                    SystemClock.Sleep(200);//sleep一小段时间
                }
            }

            protected override void OnPostExecute(Integer i)
            {
                outerInstance.IsRefreshing = true;//刷新
                return;
            }
        }

        private class IOnSmoothScrollFinishedListenerAnonymousInnerClass2 : IOnSmoothScrollFinishedListener
        {
            private readonly PullToRefreshBase<T> outerInstance;

            public IOnSmoothScrollFinishedListenerAnonymousInnerClass2(PullToRefreshBase<T> outerInstance)
            {
                this.outerInstance = outerInstance;
            }


            public virtual void onSmoothScrollFinished()
            {
                outerInstance.SmoothScrollTo(0, SMOOTH_SCROLL_DURATION_MS, DEMO_SCROLL_INTERVAL, null);
            }
        }

        internal sealed class SmoothScrollRunnable : Java.Lang.Object, IRunnable
        {
            private readonly PullToRefreshBase<T> outerInstance;

            internal readonly IInterpolator mInterpolator;
            internal readonly int mScrollToY;
            internal readonly int mScrollFromY;
            internal readonly long mDuration;
            internal IOnSmoothScrollFinishedListener mListener;

            internal bool mContinueRunning = true;
            internal long mStartTime = -1;
            internal int mCurrentY = -1;

            public SmoothScrollRunnable(PullToRefreshBase<T> outerInstance, int fromY, int toY, long duration, IOnSmoothScrollFinishedListener listener)
            {
                this.outerInstance = outerInstance;
                mScrollFromY = fromY;
                mScrollToY = toY;
                mInterpolator = outerInstance.mScrollAnimationInterpolator;
                mDuration = duration;
                mListener = listener;
            }

            public void Run()
            {

                /// <summary>
                /// Only set mStartTime if this is the first time we're starting,
                /// else actually calculate the Y delta
                /// </summary>
                if (mStartTime == -1)
                {
                    mStartTime = DateTimeHelperClass.CurrentUnixTimeMillis();
                }
                else
                {

                    /// <summary>
                    /// We do do all calculations in long to reduce software float
                    /// calculations. We use 1000 as it gives us good accuracy and
                    /// small rounding errors
                    /// </summary>
                    long normalizedTime = (1000 * (DateTimeHelperClass.CurrentUnixTimeMillis() - mStartTime)) / mDuration;
                    normalizedTime = Math.Max(Math.Min(normalizedTime, 1000), 0);

                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int deltaY = Math.round((mScrollFromY - mScrollToY) * mInterpolator.getInterpolation(normalizedTime / 1000f));
                    int deltaY = (int)Math.Round((mScrollFromY - mScrollToY) * mInterpolator.GetInterpolation(normalizedTime / 1000f));
                    mCurrentY = mScrollFromY - deltaY;
                    outerInstance.HeaderScroll = mCurrentY;
                }

                // If we're not at the target Y, keep going...
                if (mContinueRunning && mScrollToY != mCurrentY)
                {
                    ViewCompat.postOnAnimation(outerInstance, this);
                }
                else
                {
                    if (null != mListener)
                    {
                        mListener.onSmoothScrollFinished();
                    }
                }
            }

            public void Stop()
            {
                mContinueRunning = false;
                outerInstance.RemoveCallbacks(this);
            }
        }

    }

}