
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh
{

    /// <summary>
    /// An advanced version of the Listener to listen for callbacks to Refresh.
    /// This listener is different as it allows you to differentiate between Pull
    /// Ups, and Pull Downs.
    /// 
    /// @author Chris Banes
    /// </summary>
    public interface IOnPullRefreshListener<V> where V : View
    {
        // TODO These methods need renaming to START/END rather than DOWN/UP

        /// <summary>
        /// onPullDownToRefresh will be called only when the user has Pulled from
        /// the start, and released.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void onPullDownToRefresh(final PullToRefreshBase<V> refreshView);
        void OnPullDownToRefresh(PullToRefreshBase<V> refreshView);

        /// <summary>
        /// onPullUpToRefresh will be called only when the user has Pulled from
        /// the end, and released.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void onPullUpToRefresh(final PullToRefreshBase<V> refreshView);
        void OnPullUpToRefresh(PullToRefreshBase<V> refreshView);

    }

}