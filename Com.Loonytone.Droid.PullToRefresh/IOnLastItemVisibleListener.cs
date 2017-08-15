namespace Com.Loonytone.Droid.PullToRefresh
{
    /// <summary>
    /// Simple Listener that allows you to be notified when the user has scrolled
    /// to the end of the AdapterView. See (
    /// <seealso cref="PullToRefreshAdapterViewBase#setOnLastItemVisibleListener"/>.
    /// 
    /// @author Chris Banes
    /// </summary>
    public interface IOnLastItemVisibleListener
	{

		/// <summary>
		/// Called when the user has scrolled to the end of the list
		/// </summary>
		void onLastItemVisible();

	}
}