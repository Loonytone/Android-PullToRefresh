
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

using System;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Webkit;

namespace Com.Loonytone.Droid.PullToRefresh
{
    public class PullToRefreshWebViewBase : PullToRefreshBase<WebView>
	{
        private class RefreshListener : IOnRefreshListener<WebView>
        {

            public virtual void OnRefresh(PullToRefreshBase<WebView> refreshView)
            {
                refreshView.RefreshableView.Reload();
            }
        }


        private static readonly IOnRefreshListener<WebView> defaultOnRefreshListener = new RefreshListener();

        private readonly WebChromeClient defaultWebChromeClient;

		private class WebChromeClientPullRefreshClass : WebChromeClient
		{
            PullToRefreshWebViewBase outerInstance;
            public WebChromeClientPullRefreshClass(PullToRefreshWebViewBase outer)
			{
                outerInstance = outer;
            }

			public override void OnProgressChanged(WebView view, int newProgress)
			{
				if (newProgress == 100)
				{
					outerInstance.OnRefreshComplete();
				}
			}

		}

		public PullToRefreshWebViewBase(Context context) : base(context)
		{

            /// <summary>
            /// Added so that by default, Pull-to-Refresh refreshes the page
            /// </summary>            
            base.OnRefreshListener = defaultOnRefreshListener;

            defaultWebChromeClient = new WebChromeClientPullRefreshClass(this);
            mRefreshableView.SetWebChromeClient( defaultWebChromeClient);
		}

		public PullToRefreshWebViewBase(Context context, IAttributeSet attrs) : base(context, attrs)
		{

            /// <summary>
            /// Added so that by default, Pull-to-Refresh refreshes the page
            /// </summary>            
            base.OnRefreshListener = defaultOnRefreshListener;

            defaultWebChromeClient = new WebChromeClientPullRefreshClass(this);
            mRefreshableView.SetWebChromeClient(defaultWebChromeClient);
		}

		public PullToRefreshWebViewBase(Context context, PullMode mode) : base(context, mode)
		{

			/// <summary>
			/// Added so that by default, Pull-to-Refresh refreshes the page
			/// </summary>
			base.OnRefreshListener = defaultOnRefreshListener;
            defaultWebChromeClient = new WebChromeClientPullRefreshClass(this);
            mRefreshableView.SetWebChromeClient(defaultWebChromeClient);
        }

		public PullToRefreshWebViewBase(Context context, PullMode mode, AnimationStyle style) : base(context, mode, style)
		{

			/// <summary>
			/// Added so that by default, Pull-to-Refresh refreshes the page
			/// </summary>
			base.OnRefreshListener = defaultOnRefreshListener;
            defaultWebChromeClient = new WebChromeClientPullRefreshClass(this);
            mRefreshableView.SetWebChromeClient(defaultWebChromeClient);
        }

		public override sealed ScrollOrientation PullToRefreshScrollDirection
		{
			get
			{
				return ScrollOrientation.VERTICAL;
			}
		}

		protected override WebView CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			WebView webView;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
			{
				webView = new InternalWebViewSDK9(this, context, attrs);
			}
			else
			{
				webView = new WebView(context, attrs);
			}

			webView.Id = Resource.Id.webview;
			return webView;
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
				float exactContentHeight = (float) Math.Floor(mRefreshableView.ContentHeight * mRefreshableView.Scale);
				return mRefreshableView.ScrollY >= (exactContentHeight - mRefreshableView.Height);
			}
		}

		protected override void OnPtrRestoreInstanceState(Bundle savedInstanceState)
		{
			base.OnPtrRestoreInstanceState(savedInstanceState);
			mRefreshableView.RestoreState(savedInstanceState);
		}

		protected override void OnPtrSaveInstanceState(Bundle saveState)
		{
			base.OnPtrSaveInstanceState(saveState);
			mRefreshableView.SaveState(saveState);
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(9) final class InternalWebViewSDK9 extends android.webkit.WebView
		internal sealed class InternalWebViewSDK9 : WebView
		{
			private readonly PullToRefreshWebViewBase outerInstance;


			// WebView doesn't always scroll back to it's edge so we add some
			// fuzziness
			internal const int OVERSCROLL_FUZZY_THRESHOLD = 2;

			// WebView seems quite reluctant to overscroll so we use the scale
			// factor to scale it's value
			internal const float OVERSCROLL_SCALE_FACTOR = 1.5f;

			public InternalWebViewSDK9(PullToRefreshWebViewBase outerInstance, Context context, IAttributeSet attrs) : base(context, attrs)
			{
				this.outerInstance = outerInstance;
			}

			protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean returnValue = super.overScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);
				bool returnValue = base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);

				// Does all of the hard work...
				OverscrollHelper.overScrollBy(outerInstance, deltaX, scrollX, deltaY, scrollY, ScrollRange, OVERSCROLL_FUZZY_THRESHOLD, OVERSCROLL_SCALE_FACTOR, isTouchEvent);

				return returnValue;
			}

			internal int ScrollRange
			{
				get
				{
					return (int) Math.Max(0, Math.Floor(outerInstance.mRefreshableView.ContentHeight * outerInstance.mRefreshableView.Scale) - (Height - PaddingBottom - PaddingTop));
				}
			}
		}
	}
}