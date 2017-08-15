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

using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh
{

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @TargetApi(9) public final class OverscrollHelper
    public sealed class OverscrollHelper
	{

		internal const string LOG_TAG = "OverscrollHelper";
		internal const float DEFAULT_OVERSCROLL_SCALE = 1f;

		/// <summary>
		/// Helper method for Overscrolling that encapsulates all of the necessary
		/// function.
		/// 
		/// This should only be used on AdapterView's such as ListView as it just
		/// calls through to overScrollBy() with the scrollRange = 0. AdapterView's
		/// do not have a scroll range (i.e. getScrollY() doesn't work).
		/// </summary>
		/// <param name="view"> - PullToRefreshView that is calling this. </param>
		/// <param name="deltaX"> - Change in X in pixels, passed through from from
		///            overScrollBy call </param>
		/// <param name="scrollX"> - Current X scroll value in pixels before applying deltaY,
		///            passed through from from overScrollBy call </param>
		/// <param name="deltaY"> - Change in Y in pixels, passed through from from
		///            overScrollBy call </param>
		/// <param name="scrollY"> - Current Y scroll value in pixels before applying deltaY,
		///            passed through from from overScrollBy call </param>
		/// <param name="isTouchEvent"> - true if this scroll operation is the result of a
		///            touch event, passed through from from overScrollBy call </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public static void overScrollBy(final PullToRefreshBase<?> view, final int deltaX, final int scrollX, final int deltaY, final int scrollY, final boolean isTouchEvent)
		public static void overScrollBy<T>(PullToRefreshBase<T> view, int deltaX, int scrollX, int deltaY, int scrollY, bool isTouchEvent) where T:View
		{
			overScrollBy(view, deltaX, scrollX, deltaY, scrollY, 0, isTouchEvent);
		}

		/// <summary>
		/// Helper method for Overscrolling that encapsulates all of the necessary
		/// function. This version of the call is used for Views that need to specify
		/// a Scroll Range but scroll back to it's edge correctly.
		/// </summary>
		/// <param name="view"> - PullToRefreshView that is calling this. </param>
		/// <param name="deltaX"> - Change in X in pixels, passed through from from
		///            overScrollBy call </param>
		/// <param name="scrollX"> - Current X scroll value in pixels before applying deltaY,
		///            passed through from from overScrollBy call </param>
		/// <param name="deltaY"> - Change in Y in pixels, passed through from from
		///            overScrollBy call </param>
		/// <param name="scrollY"> - Current Y scroll value in pixels before applying deltaY,
		///            passed through from from overScrollBy call </param>
		/// <param name="scrollRange"> - Scroll Range of the View, specifically needed for
		///            ScrollView </param>
		/// <param name="isTouchEvent"> - true if this scroll operation is the result of a
		///            touch event, passed through from from overScrollBy call </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static void overScrollBy(final PullToRefreshBase<?> view, final int deltaX, final int scrollX, final int deltaY, final int scrollY, final int scrollRange, final boolean isTouchEvent)
		public static void overScrollBy<T>(PullToRefreshBase<T> view, int deltaX, int scrollX, int deltaY, int scrollY, int scrollRange, bool isTouchEvent) where T: View
        {
			overScrollBy(view, deltaX, scrollX, deltaY, scrollY, scrollRange, 0, DEFAULT_OVERSCROLL_SCALE, isTouchEvent);
		}

		/// <summary>
		/// Helper method for Overscrolling that encapsulates all of the necessary
		/// function. This is the advanced version of the call.
		/// </summary>
		/// <param name="view"> - PullToRefreshView that is calling this. </param>
		/// <param name="deltaX"> - Change in X in pixels, passed through from from
		///            overScrollBy call </param>
		/// <param name="scrollX"> - Current X scroll value in pixels before applying deltaY,
		///            passed through from from overScrollBy call </param>
		/// <param name="deltaY"> - Change in Y in pixels, passed through from from
		///            overScrollBy call </param>
		/// <param name="scrollY"> - Current Y scroll value in pixels before applying deltaY,
		///            passed through from from overScrollBy call </param>
		/// <param name="scrollRange"> - Scroll Range of the View, specifically needed for
		///            ScrollView </param>
		/// <param name="fuzzyThreshold"> - Threshold for which the values how fuzzy we
		///            should treat the other values. Needed for WebView as it
		///            doesn't always scroll back to it's edge. 0 = no fuzziness. </param>
		/// <param name="scaleFactor"> - Scale Factor for overscroll amount </param>
		/// <param name="isTouchEvent"> - true if this scroll operation is the result of a
		///            touch event, passed through from from overScrollBy call </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public static void overScrollBy(final PullToRefreshBase<?> view, final int deltaX, final int scrollX, final int deltaY, final int scrollY, final int scrollRange, final int fuzzyThreshold, final float scaleFactor, final boolean isTouchEvent)
		public static void overScrollBy<T>(PullToRefreshBase<T> view, 
            int deltaX, int scrollX, int deltaY, int scrollY, int scrollRange,
            int fuzzyThreshold, float scaleFactor, bool isTouchEvent) where T : View
        {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int deltaValue, currentScrollValue, scrollValue;
			int deltaValue, currentScrollValue, scrollValue;
			switch (view.PullToRefreshScrollDirection)
			{
				case ScrollOrientation.HORIZONTAL:
					deltaValue = deltaX;
					scrollValue = scrollX;
					currentScrollValue = view.ScrollX;
					break;
				case ScrollOrientation.VERTICAL:
				default:
					deltaValue = deltaY;
					scrollValue = scrollY;
					currentScrollValue = view.ScrollY;
					break;
			}

			// Check that OverScroll is enabled and that we're not currently
			// refreshing.
			if (view.PullToRefreshOverScrollEnabled && !view.IsRefreshing)
			{
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final PullToRefresh.PullMode mode = view.getMode();
				PullMode mode = view.Mode;

				// Check that Pull-to-Refresh is enabled, and the event isn't from
				// touch
				if (mode.permitsPullToRefresh() && !isTouchEvent && deltaValue != 0)
				{
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int newScrollValue = (deltaValue + scrollValue);
					int newScrollValue = (deltaValue + scrollValue);

					if (newScrollValue < (0 - fuzzyThreshold))
					{
						// Check the mode supports the overscroll direction, and
						// then move scroll
						if (mode.showHeaderLoadingLayout())
						{
							// If we're currently at zero, we're about to start
							// overscrolling, so change the state
							if (currentScrollValue == 0)
							{
								view.SetState(RefreshState.OVERSCROLLING);
							}

							view.HeaderScroll = (int)(scaleFactor * (currentScrollValue + newScrollValue));
						}
					}
					else if (newScrollValue > (scrollRange + fuzzyThreshold))
					{
						// Check the mode supports the overscroll direction, and
						// then move scroll
						if (mode.showFooterLoadingLayout())
						{
							// If we're currently at zero, we're about to start
							// overscrolling, so change the state
							if (currentScrollValue == 0)
							{
								view.SetState(RefreshState.OVERSCROLLING);
							}

							view.HeaderScroll = (int)(scaleFactor * (currentScrollValue + newScrollValue - scrollRange));
						}
					}
					else if (Math.Abs(newScrollValue) <= fuzzyThreshold || Math.Abs(newScrollValue - scrollRange) <= fuzzyThreshold)
					{
						// Means we've stopped overscrolling, so scroll back to 0
						view.SetState(RefreshState.RESET);
					}
				}
				else if (isTouchEvent && RefreshState.OVERSCROLLING == view.State)
				{
					// This condition means that we were overscrolling from a fling,
					// but the user has touched the View and is now overscrolling
					// from touch instead. We need to just reset.
					view.SetState(RefreshState.RESET);
				}
			}
		}

		internal static bool isAndroidOverScrollEnabled(View view)
		{
			return view.OverScrollMode != OverScrollMode.Never;
		}
	}

}