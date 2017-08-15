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
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Loonytone.Droid.PullToRefresh.Inner;


namespace Com.Loonytone.Droid.PullToRefresh
{

    public abstract class PullToRefreshAdapterViewBase<T> : PullToRefreshBase<T>, AbsListView.IOnScrollListener where T : AbsListView
    {

        private static FrameLayout.LayoutParams convertEmptyViewLayoutParams(ViewGroup.LayoutParams lp)
        {
            FrameLayout.LayoutParams newLp = null;

            if (null != lp)
            {
                newLp = new FrameLayout.LayoutParams(lp);

                if (lp is LinearLayout.LayoutParams)
                {
                    newLp.Gravity = ((LinearLayout.LayoutParams)lp).Gravity;
                }
                else
                {
                    newLp.Gravity = GravityFlags.Center;
                }
            }

            return newLp;
        }

        private bool mLastItemVisible;
        private AbsListView.IOnScrollListener mOnScrollListener;
        private IOnLastItemVisibleListener mOnLastItemVisibleListener;
        private View mEmptyView;

        private IndicatorLayout mIndicatorIvTop;
        private IndicatorLayout mIndicatorIvBottom;

        private bool mShowIndicator;
        private bool mScrollEmptyView = true;

        public PullToRefreshAdapterViewBase(Context context) : base(context)
        {
            mRefreshableView.SetOnScrollListener(this);
        }

        public PullToRefreshAdapterViewBase(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mRefreshableView.SetOnScrollListener(this);
        }

        public PullToRefreshAdapterViewBase(Context context, PullMode mode) : base(context, mode)
        {
            mRefreshableView.SetOnScrollListener(this);
        }

        public PullToRefreshAdapterViewBase(Context context, PullMode mode, AnimationStyle animStyle) : base(context, mode, animStyle)
        {
            mRefreshableView.SetOnScrollListener(this);
        }

        /// <summary>
        /// Gets whether an indicator graphic should be displayed when the View is in
        /// a state where a Pull-to-Refresh can happen. An example of this state is
        /// when the Adapter View is scrolled to the top and the mode is set to
        /// <seealso cref="PullMode#PULL_FROM_START"/>. The default value is <var>true</var> if
        /// {@link PullToRefreshBase#isPullToRefreshOverScrollEnabled()
        /// isPullToRefreshOverScrollEnabled()} returns false.
        /// </summary>
        /// <returns> true if the indicators will be shown </returns>
        public virtual bool ShowIndicator
        {
            get
            {
                return mShowIndicator;
            }
            set
            {
                mShowIndicator = value;

                if (ShowIndicatorInternal)
                {
                    // If we're set to Show Indicator, add/update them
                    addIndicatorViews();
                }
                else
                {
                    // If not, then remove then
                    removeIndicatorViews();
                }
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public final void onScroll(final android.widget.AbsListView view, final int firstVisibleItem, final int visibleItemCount, final int totalItemCount)
        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            /// <summary>
            /// Set whether the Last Item is Visible. lastVisibleItemIndex is a
            /// zero-based index, so we minus one totalItemCount to check
            /// </summary>
            if (null != mOnLastItemVisibleListener)
            {
                mLastItemVisible = (totalItemCount > 0) && (firstVisibleItem + visibleItemCount >= totalItemCount - 1);
            }

            // If we're showing the indicator, check positions...
            if (ShowIndicatorInternal)
            {
                updateIndicatorViewsVisibility();
            }

            // Finally call OnScrollListener if we have one
            if (null != mOnScrollListener)
            {
                mOnScrollListener.OnScroll(view, firstVisibleItem, visibleItemCount, totalItemCount);
            }
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public final void onScrollStateChanged(final android.widget.AbsListView view, final int state)
        public void OnScrollStateChanged(AbsListView view, ScrollState state)
        {
            /// <summary>
            /// Check that the scrolling has stopped, and that the last item is
            /// visible.
            /// </summary>
            if (state == ScrollState.Idle && null != mOnLastItemVisibleListener && mLastItemVisible)
            {
                mOnLastItemVisibleListener.onLastItemVisible();
            }

            if (null != mOnScrollListener)
            {
                mOnScrollListener.OnScrollStateChanged(view, state);
            }
        }

        /// <summary>
        /// Pass-through method for {@link PullToRefreshBase#getRefreshableView()
        /// getRefreshableView()}.
        /// <seealso cref="AdapterView#setAdapter(Adapter)"/>
        /// setAdapter(adapter)}. This is just for convenience!
        /// </summary>
        /// <param name="adapter"> - Adapter to set </param>
        public virtual IListAdapter Adapter
        {
            set
            {
                mRefreshableView.Adapter = value;
            }
        }

        /// <summary>
        /// Sets the Empty View to be used by the Adapter View.
        /// 
        /// We need it handle it ourselves so that we can Pull-to-Refresh when the
        /// Empty View is shown.
        /// 
        /// Please note, you do <strong>not</strong> usually need to call this method
        /// yourself. Calling setEmptyView on the AdapterView will automatically call
        /// this method and set everything up. This includes when the Android
        /// Framework automatically sets the Empty View based on it's ID.
        /// </summary>
        /// <param name="newEmptyView"> - Empty View to be used </param>
        public View EmptyView
        {
            set
            {
                FrameLayout refreshableViewWrapper = RefreshableViewWrapper;

                if (null != value)
                {
                    // New view needs to be clickable so that Android recognizes it as a
                    // target for Touch Events
                    value.Clickable = true;

                    IViewParent newEmptyViewParent = value.Parent;
                    if (null != newEmptyViewParent && newEmptyViewParent is ViewGroup)
                    {
                        ((ViewGroup)newEmptyViewParent).RemoveView(value);
                    }

                    // We need to convert any LayoutParams so that it works in our
                    // FrameLayout
                    FrameLayout.LayoutParams lp = convertEmptyViewLayoutParams(value.LayoutParameters);
                    if (null != lp)
                    {
                        refreshableViewWrapper.AddView(value, lp);
                    }
                    else
                    {
                        refreshableViewWrapper.AddView(value);
                    }
                }

                if (mRefreshableView is IEmptyViewMethodAccessor)
                {
                    ((IEmptyViewMethodAccessor)mRefreshableView).EmptyViewInternal = value;
                }
                else
                {
                    mRefreshableView.EmptyView = value;
                }
                mEmptyView = value;
            }
        }

        /// <summary>
        /// Pass-through method for {@link PullToRefreshBase#getRefreshableView()
        /// getRefreshableView()}.
        /// {@link AdapterView#setOnItemClickListener(OnItemClickListener)
        /// setOnItemClickListener(listener)}. This is just for convenience!
        /// </summary>
        /// <param name="listener"> - OnItemClickListener to use </param>
        public virtual AdapterView.IOnItemClickListener OnItemClickListener
        {
            set
            {
                mRefreshableView.OnItemClickListener = value;
            }
        }

        public IOnLastItemVisibleListener OnLastItemVisibleListener
        {
            set
            {
                mOnLastItemVisibleListener = value;
            }
        }

        public AbsListView.IOnScrollListener OnScrollListener
        {
            set
            {
                mOnScrollListener = value;
            }
        }

        public bool ScrollEmptyView
        {
            set
            {
                mScrollEmptyView = value;
            }
        }


        protected override void OnPullToRefresh()
        {
            base.OnPullToRefresh();

            if (ShowIndicatorInternal)
            {
                switch (CurrentMode)
                {
                    case PullMode.PULL_FROM_END:
                        mIndicatorIvBottom.pullToRefresh();
                        break;
                    case PullMode.PULL_FROM_START:
                        mIndicatorIvTop.pullToRefresh();
                        break;
                    default:
                        // NO-OP
                        break;
                }
            }
        }

        protected override void OnRefreshing(bool doScroll)
        {
            base.OnRefreshing(doScroll);

            if (ShowIndicatorInternal)
            {
                updateIndicatorViewsVisibility();
            }
        }

        protected override void OnReleaseToRefresh()
        {
            base.OnReleaseToRefresh();

            if (ShowIndicatorInternal)
            {
                switch (CurrentMode)
                {
                    case PullMode.PULL_FROM_END:
                        mIndicatorIvBottom.releaseToRefresh();
                        break;
                    case PullMode.PULL_FROM_START:
                        mIndicatorIvTop.releaseToRefresh();
                        break;
                    default:
                        // NO-OP
                        break;
                }
            }
        }

        protected override void OnReset()
        {
            base.OnReset();

            if (ShowIndicatorInternal)
            {
                updateIndicatorViewsVisibility();
            }
        }

        protected override void HandleStyledAttributes(TypedArray a)
        {
            // Set Show Indicator to the XML value, or default value
            mShowIndicator = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrShowIndicator, !PullToRefreshOverScrollEnabled);
        }

        protected override bool ReadyForPullStart
        {
            get
            {
                return FirstItemVisible;
            }
        }

        protected override bool ReadyForPullEnd
        {
            get
            {
                return LastItemVisible;
            }
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            base.OnScrollChanged(l, t, oldl, oldt);
            if (null != mEmptyView && !mScrollEmptyView)
            {
                mEmptyView.ScrollTo(-l, -t);
            }
        }

        protected override void UpdateUIForMode()
        {
            base.UpdateUIForMode();

            // Check Indicator Views consistent with new PullMode
            if (ShowIndicatorInternal)
            {
                addIndicatorViews();
            }
            else
            {
                removeIndicatorViews();
            }
        }

        private void addIndicatorViews()
        {
            PullMode mode = Mode;
            FrameLayout refreshableViewWrapper = RefreshableViewWrapper;

            if (mode.showHeaderLoadingLayout() && null == mIndicatorIvTop)
            {
                // If the mode can pull down, and we don't have one set already
                mIndicatorIvTop = new IndicatorLayout(Context, PullMode.PULL_FROM_START);
                FrameLayout.LayoutParams @params = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                @params.RightMargin = Resources.GetDimensionPixelSize(Resource.Dimension.indicator_right_padding);
                @params.Gravity = GravityFlags.Top | GravityFlags.Right;
                refreshableViewWrapper.AddView(mIndicatorIvTop, @params);

            }
            else if (!mode.showHeaderLoadingLayout() && null != mIndicatorIvTop)
            {
                // If we can't pull down, but have a View then remove it
                refreshableViewWrapper.RemoveView(mIndicatorIvTop);
                mIndicatorIvTop = null;
            }

            if (mode.showFooterLoadingLayout() && null == mIndicatorIvBottom)
            {
                // If the mode can pull down, and we don't have one set already
                mIndicatorIvBottom = new IndicatorLayout(Context, PullMode.PULL_FROM_END);
                FrameLayout.LayoutParams @params = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                @params.RightMargin = Resources.GetDimensionPixelSize(Resource.Dimension.indicator_right_padding);
                @params.Gravity = GravityFlags.Bottom | GravityFlags.Right;
                refreshableViewWrapper.AddView(mIndicatorIvBottom, @params);

            }
            else if (!mode.showFooterLoadingLayout() && null != mIndicatorIvBottom)
            {
                // If we can't pull down, but have a View then remove it
                refreshableViewWrapper.RemoveView(mIndicatorIvBottom);
                mIndicatorIvBottom = null;
            }
        }

        private bool ShowIndicatorInternal
        {
            get
            {
                return mShowIndicator && IsPullToRefreshEnabled;
            }
        }

        private bool FirstItemVisible
        {
            get
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final android.widget.Adapter adapter = mRefreshableView.getAdapter();
                IAdapter adapter = mRefreshableView.Adapter;

                if (null == adapter || adapter.IsEmpty)
                {

                    return true;

                }
                else
                {

                    /// <summary>
                    /// This check should really just be:
                    /// mRefreshableView.getFirstVisiblePosition() == 0, but PtRListView
                    /// internally use a HeaderView which messes the positions up. For
                    /// now we'll just add one to account for it and rely on the inner
                    /// condition which checks getTop().
                    /// </summary>
                    if (mRefreshableView.FirstVisiblePosition <= 1)
                    {
                        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                        //ORIGINAL LINE: final View firstVisibleChild = mRefreshableView.GetChildAt(0);
                        View firstVisibleChild = mRefreshableView.GetChildAt(0);
                        if (firstVisibleChild != null)
                        {
                            return firstVisibleChild.Top >= mRefreshableView.Top;
                        }
                    }
                }

                return false;
            }
        }

        private bool LastItemVisible
        {
            get
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final android.widget.Adapter adapter = mRefreshableView.getAdapter();
                IAdapter adapter = mRefreshableView.Adapter;

                if (null == adapter || adapter.IsEmpty)
                {
                    return true;
                }
                else
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int lastItemPosition = mRefreshableView.getCount() - 1;
                    int lastItemPosition = mRefreshableView.Count - 1;
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int lastVisiblePosition = mRefreshableView.getLastVisiblePosition();
                    int lastVisiblePosition = mRefreshableView.LastVisiblePosition;

                    /// <summary>
                    /// This check should really just be: lastVisiblePosition ==
                    /// lastItemPosition, but PtRListView internally uses a FooterView
                    /// which messes the positions up. For me we'll just subtract one to
                    /// account for it and rely on the inner condition which checks
                    /// getBottom().
                    /// </summary>
                    if (lastVisiblePosition >= lastItemPosition - 1)
                    {
                        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                        //ORIGINAL LINE: final int childIndex = lastVisiblePosition - mRefreshableView.getFirstVisiblePosition();
                        int childIndex = lastVisiblePosition - mRefreshableView.FirstVisiblePosition;
                        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                        //ORIGINAL LINE: final View lastVisibleChild = mRefreshableView.GetChildAt(childIndex);
                        View lastVisibleChild = mRefreshableView.GetChildAt(childIndex);
                        if (lastVisibleChild != null)
                        {
                            return lastVisibleChild.Bottom <= mRefreshableView.Bottom;
                        }
                    }
                }

                return false;
            }
        }

        private void removeIndicatorViews()
        {
            if (null != mIndicatorIvTop)
            {
                RefreshableViewWrapper.RemoveView(mIndicatorIvTop);
                mIndicatorIvTop = null;
            }

            if (null != mIndicatorIvBottom)
            {
                RefreshableViewWrapper.RemoveView(mIndicatorIvBottom);
                mIndicatorIvBottom = null;
            }
        }

        private void updateIndicatorViewsVisibility()
        {
            if (null != mIndicatorIvTop)
            {
                if (!IsRefreshing && ReadyForPullStart)
                {
                    if (!mIndicatorIvTop.IsVisible)
                    {
                        mIndicatorIvTop.show();
                    }
                }
                else
                {
                    if (mIndicatorIvTop.IsVisible)
                    {
                        mIndicatorIvTop.hide();
                    }
                }
            }

            if (null != mIndicatorIvBottom)
            {
                if (!IsRefreshing && ReadyForPullEnd)
                {
                    if (!mIndicatorIvBottom.IsVisible)
                    {
                        mIndicatorIvBottom.show();
                    }
                }
                else
                {
                    if (mIndicatorIvBottom.IsVisible)
                    {
                        mIndicatorIvBottom.hide();
                    }
                }
            }
        }
    }

}