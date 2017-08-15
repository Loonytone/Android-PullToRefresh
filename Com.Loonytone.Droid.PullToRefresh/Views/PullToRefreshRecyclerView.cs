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
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh.Views
{
    /// <summary>
    /// 版权所有：XXX有限公司
    /// 
    /// PullToRefreshRecyclerView
    /// 
    /// @author zhou.wenkai  zwenkai@foxmail.com ,Created on 2015-9-23 09:07:33
    /// Major Function：对PullToRefresh的扩展,增加支持RecyclerView
    /// 
    /// 注:如果您修改了本类请填写以下内容作为记录，如非本人操作劳烦通知，谢谢！！！
    /// @author mender，Modified Date Modify Content:
    /// </summary>
    public class PullToRefreshRecyclerView : PullToRefreshBase<RecyclerView>
	{

		public PullToRefreshRecyclerView(Context context) : base(context)
		{
		}

		public PullToRefreshRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public PullToRefreshRecyclerView(Context context, PullMode mode) : base(context, mode)
		{
		}

		public PullToRefreshRecyclerView(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
		{
		}

		public override sealed ScrollOrientation PullToRefreshScrollDirection
		{
			get
			{
				return ScrollOrientation.VERTICAL;
			}
		}

		protected override RecyclerView CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			RecyclerView recyclerView = new RecyclerView(context, attrs);
			return recyclerView;
		}

		protected override bool ReadyForPullStart
		{
			get
			{
				return IsFirstItemVisible;
			}
		}

		protected override bool ReadyForPullEnd
		{
			get
			{
				return LastItemVisible;
			}
		}

		/// <summary>
		/// @Description: 判断第一个条目是否完全可见
		/// </summary>
		/// <returns> boolean:
		/// @version 1.0
		/// @date 2015-9-23
		/// @Author zhou.wenkai </returns>
		private bool IsFirstItemVisible
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final android.support.v7.widget.RecyclerView.Adapter<?> adapter = getRefreshableView().getAdapter();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
				RecyclerView.Adapter adapter = RefreshableView.GetAdapter();
    
				// 如果未设置Adapter或者Adapter没有数据可以下拉刷新
				if (null == adapter || adapter.ItemCount == 0)
				{
					return true;
    
				}
				else
				{
					// 第一个条目完全展示,可以刷新
					if (FirstVisiblePosition == 0)
					{
						return mRefreshableView.GetChildAt(0).Top >= mRefreshableView.Top;
					}
				}
    
				return false;
			}
		}

		/// <summary>
		/// @Description: 获取第一个可见子View的位置下标
		/// </summary>
		/// <returns> int: 位置
		/// @version 1.0
		/// @date 2015-9-23
		/// @Author zhou.wenkai </returns>
		private int FirstVisiblePosition
		{
			get
			{
				View firstVisibleChild = mRefreshableView.GetChildAt(0);
				return firstVisibleChild != null ? mRefreshableView.GetChildAdapterPosition(firstVisibleChild) : -1;
			}
		}

		/// <summary>
		/// @Description: 判断最后一个条目是否完全可见
		/// </summary>
		/// <returns> boolean:
		/// @version 1.0
		/// @date 2015-9-23
		/// @Author zhou.wenkai </returns>
		private bool LastItemVisible
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final android.support.v7.widget.RecyclerView.Adapter<?> adapter = getRefreshableView().getAdapter();
	//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
				RecyclerView.Adapter adapter = RefreshableView.GetAdapter();
    
				// 如果未设置Adapter或者Adapter没有数据可以上拉刷新
				if (null == adapter || adapter.ItemCount == 0)
				{
					return true;    
				}
				else
				{
					// 最后一个条目View完全展示,可以刷新
					int lastVisiblePosition = LastVisiblePosition;
					if (lastVisiblePosition >= mRefreshableView.GetAdapter().ItemCount - 1)
					{
						return mRefreshableView.GetChildAt(mRefreshableView.ChildCount - 1).Bottom <= mRefreshableView.Bottom;
					}
				}
    
				return false;
			}
		}

		/// <summary>
		/// @Description: 获取最后一个可见子View的位置下标
		/// </summary>
		/// <returns> int: 位置
		/// @version 1.0
		/// @date 2015-9-23
		/// @Author zhou.wenkai </returns>
		private int LastVisiblePosition
		{
			get
			{
				View lastVisibleChild = mRefreshableView.GetChildAt(mRefreshableView.ChildCount - 1);
				return lastVisibleChild != null ? mRefreshableView.GetChildAdapterPosition(lastVisibleChild) : -1;
			}
		}

	}
}