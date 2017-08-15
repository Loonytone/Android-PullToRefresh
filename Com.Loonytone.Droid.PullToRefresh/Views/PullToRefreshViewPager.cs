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
using Android.Support.V4.View;
using Android.Util;

namespace Com.Loonytone.Droid.PullToRefresh.Views
{


	public class PullToRefreshViewPager : PullToRefreshBase<ViewPager>
	{

		public PullToRefreshViewPager(Context context) : base(context)
		{
		}

		public PullToRefreshViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public override sealed ScrollOrientation PullToRefreshScrollDirection
		{
			get
			{
				return ScrollOrientation.HORIZONTAL;
			}
		}

		protected override ViewPager CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			ViewPager viewPager = new ViewPager(context, attrs);
	//		viewPager.setId(Resource.Id.viewpager);
			return viewPager;
		}

		protected override bool ReadyForPullStart
		{
			get
			{
				ViewPager refreshableView = RefreshableView;
    
				PagerAdapter adapter = refreshableView.Adapter;
				if (null != adapter)
				{
					return refreshableView.CurrentItem == 0;
				}
    
				return false;
			}
		}

		protected override bool ReadyForPullEnd
		{
			get
			{
				ViewPager refreshableView = RefreshableView;
    
				PagerAdapter adapter = refreshableView.Adapter;
				if (null != adapter)
				{
					return refreshableView.CurrentItem == adapter.Count - 1;
				}
    
				return false;
			}
		}
	}

}