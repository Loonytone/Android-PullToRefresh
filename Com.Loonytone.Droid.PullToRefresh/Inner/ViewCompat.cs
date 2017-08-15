using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using static Android.OS.Build;
using Java.Lang;

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

namespace Com.Loonytone.Droid.PullToRefresh.Inner
{

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @SuppressWarnings("deprecation") public class ViewCompat
    public class ViewCompat
	{

		public static void postOnAnimation(View view, IRunnable runnable)
		{
			if (VERSION.SdkInt >= BuildVersionCodes.JellyBean)
			{
				SDK16.postOnAnimation(view, runnable);
			}
			else
			{
				view.PostDelayed(runnable, 16);
			}
		}

		public static void setBackground(View view, Drawable background)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
			{
				SDK16.setBackground(view, background);
			}
			else
			{
				view.SetBackgroundDrawable(background);
			}
		}

		public static void setLayerType(View view, LayerType layerType)
		{
			if (VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
			{
				SDK11.setLayerType(view, layerType);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(11) static class SDK11
		internal class SDK11
		{

			public static void setLayerType(View view, LayerType layerType)
			{
				view.SetLayerType(layerType, null);
			}
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(16) static class SDK16
		internal class SDK16
		{

			public static void postOnAnimation(View view, IRunnable runnable)
			{
				view.PostOnAnimation(runnable);
			}

			public static void setBackground(View view, Drawable background)
			{
				view.Background = background;
			}

		}

	}

}