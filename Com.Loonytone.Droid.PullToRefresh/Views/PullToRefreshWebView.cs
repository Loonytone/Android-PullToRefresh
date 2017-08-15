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
using Android.Util;
using Android.Webkit;
using Java.Util.Concurrent.Atomic;

namespace Com.Loonytone.Droid.PullToRefresh.Views
{
	/// <summary>
	/// An advanced version of <seealso cref="PullToRefreshWebViewBase"/> which delegates the
	/// triggering of the PullToRefresh gesture to the Javascript running within the
	/// WebView. This means that you should only use this class if:
	/// 
	/// <ul>
	/// <li><seealso cref="PullToRefreshWebViewBase"/> doesn't work correctly because you're using
	/// <code>overflow:scroll</code> or something else which means
	/// <seealso cref="WebView#getScrollY()"/> doesn't return correct values.</li>
	/// <li>You control the web content being displayed, as you need to write some
	/// Javascript callbacks.</li>
	/// </ul>
	/// 
	/// The way this call works is that when a PullToRefresh gesture is in action,
	/// the following Javascript methods will be called:
	/// <code>isReadyForPullDown()</code> and <code>isReadyForPullUp()</code>, it is
	/// your job to calculate whether the view is in a state where a PullToRefresh
	/// can happen, and return the result via the callback mechanism. An example can
	/// be seen below:
	/// 
	/// 
	/// <pre>
	/// function isReadyForPullDown() {
	///   var result = ...  // Probably using the .scrollTop DOM attribute
	///   ptr.isReadyForPullDownResponse(result);
	/// }
	/// 
	/// function isReadyForPullUp() {
	///   var result = ...  // Probably using the .scrollBottom DOM attribute
	///   ptr.isReadyForPullUpResponse(result);
	/// }
	/// </pre>
	/// 
	/// @author Chris Banes
	/// </summary>
	public class PullToRefreshWebView : PullToRefreshWebViewBase
	{

		internal const string JS_INTERFACE_PKG = "ptr";
		internal const string DEF_JS_READY_PULL_DOWN_CALL = "javascript:isReadyForPullDown();";
		internal const string DEF_JS_READY_PULL_UP_CALL = "javascript:isReadyForPullUp();";

		public PullToRefreshWebView(Context context) : base(context)
		{
		}

		public PullToRefreshWebView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public PullToRefreshWebView(Context context, PullMode mode) : base(context, mode)
		{
		}

		private JsValueCallback mJsCallback;
		private readonly AtomicBoolean mIsReadyForPullDown = new AtomicBoolean(false);
		private readonly AtomicBoolean mIsReadyForPullUp = new AtomicBoolean(false);

		protected override WebView CreateRefreshableView(Context context, IAttributeSet attrs)
		{
			WebView webView = base.CreateRefreshableView(context, attrs);

			// Need to add JS Interface so we can get the response back
			mJsCallback = new JsValueCallback(this);
			webView.AddJavascriptInterface(mJsCallback, JS_INTERFACE_PKG);

			return webView;
		}

		protected override bool ReadyForPullStart
		{
			get
			{
				// Call Javascript...
				RefreshableView.LoadUrl(DEF_JS_READY_PULL_DOWN_CALL);
    
				// Response will be given to JsValueCallback, which will update
				// mIsReadyForPullDown
    
				return mIsReadyForPullDown.Get();
			}
		}

		protected override bool ReadyForPullEnd
		{
			get
			{
				// Call Javascript...
				RefreshableView.LoadUrl(DEF_JS_READY_PULL_UP_CALL);
    
				// Response will be given to JsValueCallback, which will update
				// mIsReadyForPullUp
    
				return mIsReadyForPullUp.Get();
			}
		}

        /// <summary>
        /// Used for response from Javascript
        /// 
        /// @author Chris Banes
        /// </summary>
        internal sealed class JsValueCallback : Java.Lang.Object
        {
			private readonly PullToRefreshWebView outerInstance;

			public JsValueCallback(PullToRefreshWebView outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public void isReadyForPullUpResponse(bool response)
			{
				outerInstance.mIsReadyForPullUp.Set(response);
			}

			public void isReadyForPullDownResponse(bool response)
			{
				outerInstance.mIsReadyForPullDown.Set(response);
			}
		}
	}

}