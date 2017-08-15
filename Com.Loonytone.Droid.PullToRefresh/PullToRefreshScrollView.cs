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
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Com.Loonytone.Droid.PullToRefresh
{

    public class PullToRefreshScrollView : PullToRefreshBase<ScrollView>
	{

		public PullToRefreshScrollView(Context context) : base(context)
		{
		}

		public PullToRefreshScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public PullToRefreshScrollView(Context context, PullMode mode) : base(context, mode)
		{
		}

		public PullToRefreshScrollView(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
		{
		}

		public override sealed ScrollOrientation PullToRefreshScrollDirection
		{
			get
			{
				return ScrollOrientation.VERTICAL;
			}
		}

		protected override ScrollView CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			ScrollView scrollView;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
			{
				scrollView = new InternalScrollViewSDK9(this, context, attrs);
			}
			else
			{
				scrollView = new ScrollView(context, attrs);
			}

			scrollView.Id = Resource.Id.scrollview;
			return scrollView;
		}

		protected override bool ReadyForPullStart
		{
			get
			{
				return mRefreshableView.ScrollY == 0;
			}
		}

		protected override bool ReadyForPullEnd
		{
			get
			{
				View scrollViewChild = mRefreshableView.GetChildAt(0);
				if (null != scrollViewChild)
				{
					return mRefreshableView.ScrollY >= (scrollViewChild.Height - Height);
				}
				return false;
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(9) final class InternalScrollViewSDK9 extends android.widget.ScrollView
		internal sealed class InternalScrollViewSDK9 : ScrollView
		{
			private readonly PullToRefreshScrollView outerInstance;


			public InternalScrollViewSDK9(PullToRefreshScrollView outerInstance, Context context, IAttributeSet attrs) : base(context, attrs)
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
						scrollRange = Math.Max(0, child.Height - (Height - PaddingBottom - PaddingTop));
					}
					return scrollRange;
				}
			}
		}
	}

}