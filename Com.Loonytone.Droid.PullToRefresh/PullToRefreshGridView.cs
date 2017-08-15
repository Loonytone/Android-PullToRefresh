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

    using IEmptyViewMethodAccessor = PullToRefresh.Inner.IEmptyViewMethodAccessor;

    public class PullToRefreshGridView : PullToRefreshAdapterViewBase<GridView>
    {

        public PullToRefreshGridView(Context context) : base(context)
        {
        }

        public PullToRefreshGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public PullToRefreshGridView(Context context, PullMode mode) : base(context, mode)
        {
        }

        public PullToRefreshGridView(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
        {
        }

        public override ScrollOrientation PullToRefreshScrollDirection
        {
            get
            {
                return ScrollOrientation.VERTICAL;
            }
        }

        protected override GridView CreateRefreshableView(Context context, IAttributeSet attrs)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final android.widget.GridView gv;
            GridView gv;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
            {
                gv = new InternalGridViewSDK9(this, context, attrs);
            }
            else
            {
                gv = new InternalGridView(this, context, attrs);
            }

            // Use Generated ID (from res/values/ids.xml)
            gv.Id = Resource.Id.gridview;
            return gv;
        }

        internal class InternalGridView : GridView, IEmptyViewMethodAccessor
        {
            private readonly PullToRefreshGridView outerInstance;


            public InternalGridView(PullToRefreshGridView outerInstance, Context context, IAttributeSet attrs) : base(context, attrs)
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

            public View EmptyViewInternal
            {
                set
                {
                    base.EmptyView = value;
                }
            }
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @TargetApi(9) final class InternalGridViewSDK9 extends InternalGridView
        internal sealed class InternalGridViewSDK9 : InternalGridView
        {
            private readonly PullToRefreshGridView outerInstance;


            public InternalGridViewSDK9(PullToRefreshGridView outerInstance, Context context, IAttributeSet attrs) : base(outerInstance, context, attrs)
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