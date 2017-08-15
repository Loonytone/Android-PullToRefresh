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

using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Com.Loonytone.Droid.PullToRefresh.Views.ListFragments
{

    public abstract class PullToRefreshBaseListFragment<T> : ListFragment where T : View
    {

		private T mPullToRefreshListView;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View layout = base.OnCreateView(inflater, container, savedInstanceState);

			ListView lv = (ListView) layout.FindViewById(Android.Resource.Id.List);
			ViewGroup parent = (ViewGroup) lv.Parent;

			// Remove ListView and add PullToRefreshListView in its place
			int lvIndex = parent.IndexOfChild(lv);
			parent.RemoveViewAt(lvIndex);
			mPullToRefreshListView = onCreatePullToRefreshListView(inflater, savedInstanceState);
			parent.AddView(mPullToRefreshListView, lvIndex, lv.LayoutParameters);

			return layout;
		}

		/// <returns> The <seealso cref="PullToRefreshBase"/> attached to this ListFragment. </returns>
		public T PullToRefreshListView
		{
			get
			{
				return mPullToRefreshListView;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="PullToRefreshBase"/> which will replace the ListView
		/// created from ListFragment. You should override this method if you wish to
		/// customise the <seealso cref="PullToRefreshBase"/> from the default.
		/// </summary>
		/// <param name="inflater"> - LayoutInflater which can be used to inflate from XML. </param>
		/// <param name="savedInstanceState"> - Bundle passed through from
		///            {ListFragment#onCreateView(LayoutInflater, ViewGroup, Bundle)
		///            onCreateView(...)} </param>
		/// <returns> The <seealso cref="PullToRefreshBase"/> which will replace the ListView. </returns>
		protected abstract T onCreatePullToRefreshListView(LayoutInflater inflater, Bundle savedInstanceState);

	}
}