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

using Android.Views;
using Android.Views.Animations;

namespace Com.Loonytone.Droid.PullToRefresh
{
	public interface IPullToRefresh<T> where T : View
	{

		/// <summary>
		/// Demos the Pull-to-Refresh functionality to the user so that they are
		/// aware it is there. This could be useful when the user first opens your
		/// app, etc. The animation will only happen if the Refresh View (ListView,
		/// ScrollView, etc) is in a state where a Pull-to-Refresh could occur by a
		/// user's touch gesture (i.e. scrolled to the top/bottom).
		/// </summary>
		/// <returns> true - if the Demo has been started, false if not. </returns>
		bool Demo();

		/// <summary>
		/// Get the mode that this view is currently in. This is only really useful
		/// when using <code>PullMode.BOTH</code>.
		/// </summary>
		/// <returns> PullMode that the view is currently in </returns>
		PullMode CurrentMode {get;}

		/// <summary>
		/// Returns whether the Touch Events are filtered or not. If true is
		/// returned, then the View will only use touch events where the difference
		/// in the Y-axis is greater than the difference in the X-axis. This means
		/// that the View will not interfere when it is used in a horizontal
		/// scrolling View (such as a ViewPager).
		/// </summary>
		/// <returns> boolean - true if the View is filtering Touch Events </returns>
		bool FilterTouchEvents {get;set;}

		/// <summary>
		/// Returns a proxy object which allows you to call methods on all of the
		/// LoadingLayouts (the Views which show when Pulling/Refreshing).
		/// 
		/// You should not keep the result of this method any longer than you need
		/// it.
		/// </summary>
		/// <returns> Object which will proxy any calls you make on it, to all of the
		///         LoadingLayouts. </returns>
		ILoadingLayout LoadingLayoutProxy {get;}

		/// <summary>
		/// Returns a proxy object which allows you to call methods on the
		/// LoadingLayouts (the Views which show when Pulling/Refreshing). The actual
		/// LoadingLayout(s) which will be affected, are chosen by the parameters you
		/// give.
		/// 
		/// You should not keep the result of this method any longer than you need
		/// it.
		/// </summary>
		/// <param name="includeStart"> - Whether to include the Start/Header Views </param>
		/// <param name="includeEnd"> - Whether to include the End/Footer Views </param>
		/// <returns> Object which will proxy any calls you make on it, to the
		///         LoadingLayouts included. </returns>
		ILoadingLayout GetLoadingLayoutProxy(bool includeStart, bool includeEnd);

		/// <summary>
		/// Get the mode that this view has been set to. If this returns
		/// <code>PullMode.BOTH</code>, you can use <code>getCurrentMode()</code> to
		/// check which mode the view is currently in
		/// </summary>
		/// <returns> PullMode that the view has been set to </returns>
		PullMode Mode {get;set;}

		/// <summary>
		/// Get the Wrapped Refreshable View. Anything returned here has already been
		/// added to the content view.
		/// </summary>
		/// <returns> The View which is currently wrapped </returns>
		T RefreshableView {get;}

		/// <summary>
		/// Get whether the 'Refreshing' View should be automatically shown when
		/// refreshing. Returns true by default.
		/// </summary>
		/// <returns> - true if the Refreshing View will be show </returns>
		bool ShowViewWhileRefreshing {get;set;}

		/// <returns> - The state that the View is currently in. </returns>
		RefreshState State {get;}

		/// <summary>
		/// Whether Pull-to-Refresh is enabled
		/// </summary>
		/// <returns> enabled </returns>
		bool IsPullToRefreshEnabled {get;}

		/// <summary>
		/// Gets whether Overscroll support is enabled. This is different to
		/// Android's standard Overscroll support (the edge-glow) which is available
		/// from GINGERBREAD onwards
		/// </summary>
		/// <returns> true - if both PullToRefresh-OverScroll and Android's inbuilt
		///         OverScroll are enabled </returns>
		bool PullToRefreshOverScrollEnabled {get;set;}

		/// <summary>
		/// Returns whether the Widget is currently in the Refreshing mState
		/// </summary>
		/// <returns> true if the Widget is currently refreshing </returns>
		bool IsRefreshing {get;set;}

		/// <summary>
		/// Returns whether the widget has enabled scrolling on the Refreshable View
		/// while refreshing.
		/// </summary>
		/// <returns> true if the widget has enabled scrolling while refreshing </returns>
		bool ScrollingWhileRefreshingEnabled {get;set;}

		/// <summary>
		/// Mark the current Refresh as complete. Will Reset the UI and hide the
		/// Refreshing View
		/// </summary>
		void OnRefreshComplete();



		/// <summary>
		/// Set IOnPullEventListener for the Widget
		/// </summary>
		/// <param name="listener"> - Listener to be used when the Widget has a pull event to
		///            propogate. </param>
		IOnPullEventListener<T> OnPullEventListener {set;}

		/// <summary>
		/// Set IOnRefreshListener for the Widget
		/// </summary>
		/// <param name="listener"> - Listener to be used when the Widget is set to Refresh </param>
		IOnRefreshListener<T> OnRefreshListener {set;}

		/// <summary>
		/// Set IOnRefreshListener for the Widget
		/// </summary>
		/// <param name="listener"> - Listener to be used when the Widget is set to Refresh </param>
		IOnPullRefreshListener<T> OnPullRefreshListener {set;}

		/// <summary>
		/// Set a header LoadingLayout for the Widget
		/// </summary>
		/// <param name="headerLayout"> </param>
		LoadingLayoutBase HeaderLayout {set;}

		/// <summary>
		/// Set a footer LoadingLayout for the Widget
		/// </summary>
		/// <param name="footerLayout"> </param>
		LoadingLayoutBase FooterLayout {set;}

		/// <summary>
		/// Set a second footer LoadingLayout for the Widget，it used to the widget
		/// can add footer just like ListView or
		/// WrapRecyclerView(https://github.com/xuehuayous/WrapRecyclerView)
		/// </summary>
		/// <param name="view"> </param>
		View SecondFooterLayout {set;}


		/// <summary>
		/// Sets the Animation Interpolator that is used for animated scrolling.
		/// Defaults to a DecelerateInterpolator
		/// </summary>
		/// <param name="interpolator"> - Interpolator to use </param>
		IInterpolator ScrollAnimationInterpolator {set;}



		/// <summary>
		/// set weather has friction when pull down </summary>
		/// <param name="hasPullDownFriction"> </param>
		bool HasPullDownFriction {set;}

		/// <summary>
		/// set weather has friction when pull up </summary>
		/// <param name="hasPullUpFriction"> </param>
		bool HasPullUpFriction {set;}

	}


}