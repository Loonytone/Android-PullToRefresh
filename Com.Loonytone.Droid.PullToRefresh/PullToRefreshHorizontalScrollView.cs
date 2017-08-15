
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
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Math = System.Math;

namespace Com.Loonytone.Droid.PullToRefresh
{
    public class PullToRefreshHorizontalScrollView : PullToRefreshBase<HorizontalScrollView>
	{

		public PullToRefreshHorizontalScrollView(Context context) : base(context)
		{
		}

		public PullToRefreshHorizontalScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public PullToRefreshHorizontalScrollView(Context context, PullMode mode) : base(context, mode)
		{
		}

		public PullToRefreshHorizontalScrollView(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
		{
		}

		public override sealed ScrollOrientation PullToRefreshScrollDirection
		{
			get
			{
				return ScrollOrientation.HORIZONTAL;
			}
		}

		protected override HorizontalScrollView CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			HorizontalScrollView scrollView;

			if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
			{
				scrollView = new InternalHorizontalScrollViewSDK9(this, context, attrs);
			}
			else
			{
				scrollView = new HorizontalScrollView(context, attrs);
			}

			scrollView.Id = Resource.Id.scrollview;
			return scrollView;
		}

		protected override bool ReadyForPullStart
		{
			get
			{
				return mRefreshableView.ScrollX == 0;
			}
		}

		protected override bool ReadyForPullEnd
		{
			get
			{
				View scrollViewChild = mRefreshableView.GetChildAt(0);
				if (null != scrollViewChild)
				{
					return mRefreshableView.ScrollX >= (scrollViewChild.Width - Width);
				}
				return false;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(9) final class InternalHorizontalScrollViewSDK9 extends android.widget.HorizontalScrollView
		internal sealed class InternalHorizontalScrollViewSDK9 : HorizontalScrollView
		{
			private readonly PullToRefreshHorizontalScrollView outerInstance;


			public InternalHorizontalScrollViewSDK9(PullToRefreshHorizontalScrollView outerInstance, Context context, IAttributeSet attrs) : base(context, attrs)
			{
				this.outerInstance = outerInstance;
			}

			protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean returnValue = super.overScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);
				bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

				// Does all of the hard work...
				OverscrollHelper.overScrollBy(outerInstance, deltaX, scrollX, deltaY, scrollY, ScrollRange, isTouchEvent);

				return returnValue;
			}

			/// <summary>
			/// Taken from the AOSP ScrollView source
			/// </summary>
			internal int ScrollRange
			{
				get
				{
					int scrollRange = 0;
					if (ChildCount > 0)
					{
						View child = GetChildAt(0);
						scrollRange = Math.Max(0, child.Width - (Width - PaddingLeft - PaddingRight));
					}
					return scrollRange;
				}
			}
		}
	}

}