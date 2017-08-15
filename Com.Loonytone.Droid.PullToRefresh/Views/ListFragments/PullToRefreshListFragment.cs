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
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh.Views.ListFragments
{

	/// <summary>
	/// A sample implementation of how to use <seealso cref="PullToRefreshListView"/> with
	/// {ListFragment}. This implementation simply replaces the ListView that
	/// {@code ListFragment} creates with a new PullToRefreshListView. This means
	/// that ListFragment still works 100% (e.g. <code>setListShown(...)</code> ).
	/// 
	/// The new PullToRefreshListView is created in the method
	/// <seealso cref="#onCreatePullToRefreshListView(LayoutInflater, Bundle)"/>. If you wish
	/// to customise the {@code PullToRefreshListView} then override this method and
	/// return your customised instance.
	/// 
	/// @author Chris Banes
	/// 
	/// </summary>
	public class PullToRefreshListFragment : PullToRefreshBaseListFragment<PullToRefreshListView>
	{

		protected override PullToRefreshListView onCreatePullToRefreshListView(LayoutInflater inflater, Bundle savedInstanceState)
		{
			return new PullToRefreshListView(Activity);
		}

	}
}