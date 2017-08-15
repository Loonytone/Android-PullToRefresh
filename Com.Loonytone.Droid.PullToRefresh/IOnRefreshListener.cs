
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh
{
    /// <summary>
    /// Simple Listener to listen for any callbacks to Refresh.
    /// 
    /// @author Chris Banes
    /// </summary>
    public interface IOnRefreshListener<V> where V : View
    {

        /// <summary>
        /// onRefresh will be called for both a Pull from start, and Pull from
        /// end
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void onRefresh(final PullToRefreshBase<V> refreshView);
        void OnRefresh(PullToRefreshBase<V> refreshView);

    }
}