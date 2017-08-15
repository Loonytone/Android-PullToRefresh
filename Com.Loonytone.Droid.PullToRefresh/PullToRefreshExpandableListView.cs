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
using Com.Loonytone.Droid.PullToRefresh.Inner;

namespace Com.Loonytone.Droid.PullToRefresh
{

    public class PullToRefreshExpandableListView : PullToRefreshAdapterViewBase<ExpandableListView>
    {

        public PullToRefreshExpandableListView(Context context) : base(context)
        {
        }

        public PullToRefreshExpandableListView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public PullToRefreshExpandableListView(Context context, PullMode mode) : base(context, mode)
        {
        }

        public PullToRefreshExpandableListView(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
        {
        }

        public override ScrollOrientation PullToRefreshScrollDirection
        {
            get
            {
                return ScrollOrientation.VERTICAL;
            }
        }

        protected override ExpandableListView CreateRefreshableView(Context context, IAttributeSet attrs)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final android.widget.ExpandableListView lv;
            ExpandableListView lv;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
            {
                lv = new InternalExpandableListViewSDK9(this, context, attrs);
            }
            else
            {
                lv = new InternalExpandableListView(this, context, attrs);
            }

            // Set it to this so it can be used in ListActivity/ListFragment
            lv.Id = Android.Resource.Id.List;
            return lv;
        }

        internal class InternalExpandableListView : ExpandableListView, IEmptyViewMethodAccessor
        {
            private readonly PullToRefreshExpandableListView outerInstance;


            public InternalExpandableListView(PullToRefreshExpandableListView outerInstance, Context context, IAttributeSet attrs) : base(context, attrs)
            {
                this.outerInstance = outerInstance;
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

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @TargetApi(9) final class InternalExpandableListViewSDK9 extends InternalExpandableListView
        internal sealed class InternalExpandableListViewSDK9 : InternalExpandableListView
        {
            private readonly PullToRefreshExpandableListView outerInstance;


            public InternalExpandableListViewSDK9(PullToRefreshExpandableListView outerInstance, Context context, IAttributeSet attrs) : base(outerInstance, context, attrs)
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
    }

}