using System;

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
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Loonytone.Droid.PullToRefresh.Inner;
using Java.Lang.Reflect;

namespace Com.Loonytone.Droid.PullToRefresh
{

    public class PullToRefreshListView : PullToRefreshAdapterViewBase<ListView>
	{

		private LoadingLayoutBase mHeaderLoadingView;
		private LoadingLayoutBase mFooterLoadingView;

		private FrameLayout mLvHeaderLoadingFrame;
		private FrameLayout mLvFooterLoadingFrame;
		private FrameLayout mLvSecondFooterLoadingFrame;

		private bool mListViewExtrasEnabled;

		public PullToRefreshListView(Context context) : base(context)
		{
		}

		public PullToRefreshListView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public PullToRefreshListView(Context context, PullMode mode) : base(context, mode)
		{
		}

		public PullToRefreshListView(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
		{
		}

		/// <summary>
		/// 获取刷新方向
		/// </summary>
		public override ScrollOrientation PullToRefreshScrollDirection
		{
			get
			{
				return ScrollOrientation.VERTICAL;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected void onRefreshing(final boolean doScroll)
		protected override void OnRefreshing(bool doScroll)
		{
            IListAdapter adapter = mRefreshableView.Adapter;
			if (!mListViewExtrasEnabled || !ShowViewWhileRefreshing || null == adapter || adapter.IsEmpty)
			{
				base.OnRefreshing(doScroll);
				return;
			}

			base.OnRefreshing(false);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoadingLayoutBase origLoadingView, listViewLoadingView, oppositeListViewLoadingView;
			LoadingLayoutBase origLoadingView, listViewLoadingView, oppositeListViewLoadingView;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int selection, scrollToY;
			int selection, scrollToY;

			switch (CurrentMode)
			{
				case PullMode.MANUAL_REFRESH_ONLY:
				case PullMode.PULL_FROM_END:
					origLoadingView = FooterLayout;
					listViewLoadingView = mFooterLoadingView;
					oppositeListViewLoadingView = mHeaderLoadingView;
					selection = mRefreshableView.Count - 1;
					scrollToY = ScrollY - FooterSize;
					break;
				case PullMode.PULL_FROM_START:
				default:
					origLoadingView = HeaderLayout;
					listViewLoadingView = mHeaderLoadingView;
					oppositeListViewLoadingView = mFooterLoadingView;
					selection = 0;
					scrollToY = ScrollY + HeaderSize;
					break;
			}

			// Hide our original Loading View
			origLoadingView.reset();
			origLoadingView.hideAllViews();

			// Make sure the opposite end is hidden too
			oppositeListViewLoadingView.Visibility = ViewStates.Gone;

			// Show the ListView Loading View and set it to refresh.
			listViewLoadingView.Visibility = ViewStates.Visible;
			listViewLoadingView.refreshing();

			if (doScroll)
			{
				// We need to disable the automatic visibility changes for now
				DisableLoadingLayoutVisibilityChanges();

				// We scroll slightly so that the ListView's header/footer is at the
				// same Y position as our normal header/footer
				HeaderScroll = scrollToY;

				// Make sure the ListView is scrolled to show the loading
				// header/footer
				mRefreshableView.SetSelection(selection);

				// Smooth scroll as normal
				SmoothScrollTo(0);
			}
		}

		protected override void OnReset()
		{
			/// <summary>
			/// If the extras are not enabled, just call up to super and return.
			/// </summary>
			if (!mListViewExtrasEnabled)
			{
				base.OnReset();
				return;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LoadingLayoutBase originalLoadingLayout, listViewLoadingLayout;
			LoadingLayoutBase originalLoadingLayout, listViewLoadingLayout;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int scrollToHeight, selection;
			int scrollToHeight, selection;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean scrollLvToEdge;
			bool scrollLvToEdge;

			switch (CurrentMode)
			{
				case PullMode.MANUAL_REFRESH_ONLY:
				case PullMode.PULL_FROM_END:
					originalLoadingLayout = FooterLayout;
					listViewLoadingLayout = mFooterLoadingView;
					selection = mRefreshableView.Count - 1;
					scrollToHeight = FooterSize;
					scrollLvToEdge = Math.Abs(mRefreshableView.LastVisiblePosition - selection) <= 1;
					break;
				case PullMode.PULL_FROM_START:
				default:
					originalLoadingLayout = HeaderLayout;
					listViewLoadingLayout = mHeaderLoadingView;
					scrollToHeight = -HeaderSize;
					selection = 0;
					scrollLvToEdge = Math.Abs(mRefreshableView.FirstVisiblePosition - selection) <= 1;
					break;
			}

			// If the ListView header loading layout is showing, then we need to
			// flip so that the original one is showing instead
			if (listViewLoadingLayout.Visibility == ViewStates.Visible)
			{

				// Set our Original View to Visible
				originalLoadingLayout.showInvisibleViews();

				// Hide the ListView Header/Footer
				listViewLoadingLayout.Visibility = ViewStates.Gone;

				/// <summary>
				/// Scroll so the View is at the same Y as the ListView
				/// header/footer, but only scroll if: we've pulled to refresh, it's
				/// positioned correctly
				/// </summary>
				if (scrollLvToEdge && State != RefreshState.MANUAL_REFRESHING)
				{
					mRefreshableView.SetSelection(selection);
					HeaderScroll = scrollToHeight;
				}
			}

			// Finally, call up to super
			base.OnReset();
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected LoadingLayoutProxy createLoadingLayoutProxy(final boolean includeStart, final boolean includeEnd)
		protected override LoadingLayoutProxy CreateLoadingLayoutProxy(bool includeStart, bool includeEnd)
		{
			LoadingLayoutProxy proxy = base.CreateLoadingLayoutProxy(includeStart, includeEnd);

			if (mListViewExtrasEnabled)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PullMode mode = getMode();
				PullMode mode = Mode;

				if (includeStart && mode.showHeaderLoadingLayout())
				{
					proxy.addLayout(mHeaderLoadingView);
				}
				if (includeEnd && mode.showFooterLoadingLayout())
				{
					proxy.addLayout(mFooterLoadingView);
				}
			}

			return proxy;
		}

		protected virtual ListView createListView(Context context, IAttributeSet attrs)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.widget.ListView lv;
			ListView lv;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
			{
				lv = new InternalListViewSDK9(this, context, attrs);
			}
			else
			{
				lv = new InternalListView(this, context, attrs);
			}
			return lv;
		}

		protected override ListView CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			ListView lv = createListView(context, attrs);

			// Set it to this so it can be used in ListActivity/ListFragment
			lv.Id = Android.Resource.Id.List;
			return lv;
		}

		protected override void HandleStyledAttributes(TypedArray a)
		{
			base.HandleStyledAttributes(a);

			mListViewExtrasEnabled = a.GetBoolean(Resource.Styleable.PullToRefresh_ptrListViewExtrasEnabled, true);

			if (mListViewExtrasEnabled)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.widget.FrameLayout.LayoutParams lp = new android.widget.FrameLayout.LayoutParams(android.widget.FrameLayout.LayoutParams.MATCH_PARENT, android.widget.FrameLayout.LayoutParams.WRAP_CONTENT, android.view.Gravity.CENTER_HORIZONTAL);
				FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent, FrameLayout.LayoutParams.WrapContent, GravityFlags.CenterHorizontal);

				// Create Loading Views ready for use later
				mLvHeaderLoadingFrame = new FrameLayout(Context);
				mHeaderLoadingView = CreateLoadingLayout(Context, PullMode.PULL_FROM_START, a);
				mHeaderLoadingView.Visibility = ViewStates.Gone;
				mLvHeaderLoadingFrame.AddView(mHeaderLoadingView, lp);
				mRefreshableView.AddHeaderView(mLvHeaderLoadingFrame, null, false);

				mLvFooterLoadingFrame = new FrameLayout(Context);
				mFooterLoadingView = CreateLoadingLayout(Context, PullMode.PULL_FROM_END, a);
				mFooterLoadingView.Visibility = ViewStates.Gone;
				mLvFooterLoadingFrame.AddView(mFooterLoadingView, lp);

				mLvSecondFooterLoadingFrame = new FrameLayout(Context);

				/// <summary>
				/// If the value for Scrolling While Refreshing hasn't been
				/// explicitly set via XML, enable Scrolling While Refreshing.
				/// </summary>
				if (!a.HasValue(Resource.Styleable.PullToRefresh_ptrScrollingWhileRefreshingEnabled))
				{
					ScrollingWhileRefreshingEnabled = true;
				}
			}
		}

		public override LoadingLayoutBase HeaderLayout
		{
			set
			{
				base.HeaderLayout = value;
    
				try
				{
					Constructor c = value.Class.GetDeclaredConstructor(new Java.Lang.Class[]{ Context.Class});
					LoadingLayoutBase mHeaderLayout = (LoadingLayoutBase)c.NewInstance(new Java.Lang.Object[]{Context});
					if (null != mHeaderLayout)
					{
						mRefreshableView.RemoveHeaderView(mLvHeaderLoadingFrame);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final android.widget.FrameLayout.LayoutParams lp = new android.widget.FrameLayout.LayoutParams(android.widget.FrameLayout.LayoutParams.MATCH_PARENT, android.widget.FrameLayout.LayoutParams.WRAP_CONTENT, android.view.Gravity.CENTER_HORIZONTAL);
						FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, GravityFlags.CenterHorizontal);
    
						mLvHeaderLoadingFrame = new FrameLayout(Context);
						mHeaderLoadingView = mHeaderLayout;
						mHeaderLoadingView.Visibility = ViewStates.Gone;
						mLvHeaderLoadingFrame.AddView(mHeaderLoadingView, lp);
						mRefreshableView.AddHeaderView(mLvHeaderLoadingFrame, null, false);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public override LoadingLayoutBase FooterLayout
		{
			set
			{
				base.FooterLayout = value;
    
				try
				{
					Constructor c = value.Class.GetDeclaredConstructor(new Java.Lang.Class[]{ Context.Class });
					LoadingLayoutBase mFooterLayout = (LoadingLayoutBase)c.NewInstance(new Java.Lang.Object[]{Context});
					if (null != mFooterLayout)
					{
						mRefreshableView.RemoveFooterView(mLvFooterLoadingFrame);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final android.widget.FrameLayout.LayoutParams lp = new android.widget.FrameLayout.LayoutParams(android.widget.FrameLayout.LayoutParams.MATCH_PARENT, android.widget.FrameLayout.LayoutParams.WRAP_CONTENT, android.view.Gravity.CENTER_HORIZONTAL);
						FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, GravityFlags.CenterHorizontal);
    
						mLvFooterLoadingFrame = new FrameLayout(Context);
						mFooterLoadingView = mFooterLayout;
						mFooterLoadingView.Visibility = ViewStates.Gone;
						mLvFooterLoadingFrame.AddView(mFooterLoadingView, lp);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public override View SecondFooterLayout
		{
			set
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final android.widget.FrameLayout.LayoutParams lp = new android.widget.FrameLayout.LayoutParams(android.widget.FrameLayout.LayoutParams.MATCH_PARENT, android.widget.FrameLayout.LayoutParams.WRAP_CONTENT, android.view.Gravity.CENTER_HORIZONTAL);
				FrameLayout.LayoutParams lp = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, GravityFlags.CenterHorizontal);
    
				mLvSecondFooterLoadingFrame.AddView(value, lp);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(9) final class InternalListViewSDK9 extends InternalListView
		internal sealed class InternalListViewSDK9 : InternalListView
		{
			private readonly PullToRefreshListView outerInstance;


			public InternalListViewSDK9(PullToRefreshListView outerInstance, Context context, IAttributeSet attrs) : base(outerInstance, context, attrs)
			{
				this.outerInstance = outerInstance;
			}

			protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean returnValue = super.overScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);
				bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

				// Does all of the hard work...
				OverscrollHelper.overScrollBy(outerInstance, deltaX, scrollX, deltaY, scrollY, isTouchEvent);

				return returnValue;
			}
		}

		internal class InternalListView : ListView, IEmptyViewMethodAccessor
		{
			private readonly PullToRefreshListView outerInstance;


			internal bool mAddedLvFooter = false;

			public InternalListView(PullToRefreshListView outerInstance, Context context, IAttributeSet attrs) : base(context, attrs)
			{
				this.outerInstance = outerInstance;
			}

			protected override void DispatchDraw(Canvas canvas)
			{
				/// <summary>
				/// This is a bit hacky, but Samsung's ListView has got a bug in it
				/// when using Header/Footer Views and the list is empty. This masks
				/// the issue so that it doesn't cause an FC. See Issue #66.
				/// </summary>
				try
				{
					base.DispatchDraw(canvas);
				}
				catch (System.IndexOutOfRangeException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}

			public override bool DispatchTouchEvent(MotionEvent ev)
			{
				/// <summary>
				/// This is a bit hacky, but Samsung's ListView has got a bug in it
				/// when using Header/Footer Views and the list is empty. This masks
				/// the issue so that it doesn't cause an FC. See Issue #66.
				/// </summary>
				try
				{
					return base.DispatchTouchEvent(ev);
				}
				catch (System.IndexOutOfRangeException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
					return false;
				}
			}

			public override IListAdapter Adapter
			{
				set
				{
					// Add the Footer View at the last possible moment
					if (null != outerInstance.mLvFooterLoadingFrame && !mAddedLvFooter)
					{
						AddFooterView(outerInstance.mLvSecondFooterLoadingFrame, null, false);
						AddFooterView(outerInstance.mLvFooterLoadingFrame, null, false);
						mAddedLvFooter = true;
					}
    
					base.Adapter = value;
				}
			}

			public override View EmptyView
			{
				set
				{
					outerInstance.EmptyView = value;
				}
			}

			public virtual View EmptyViewInternal
			{
				set
				{
					base.EmptyView = value;
				}
			}

		}

	}
}