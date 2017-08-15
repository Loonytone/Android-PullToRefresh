
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh
{
    /// <summary>
    /// Listener that allows you to be notified when the user has started or
    /// finished a touch event. Useful when you want to append extra UI events
    /// (such as sounds). See (
    /// <seealso cref="PullToRefreshAdapterViewBase#setOnPullEventListener"/>.
    /// 
    /// @author Chris Banes
    /// </summary>
    public interface IOnPullEventListener<V> where V : View
    {

        /// <summary>
        /// Called when the internal state has been changed, usually by the user
        /// pulling.
        /// </summary>
        /// <param name="refreshView"> - View which has had it's state change. </param>
        /// <param name="state"> - The new state of View. </param>
        /// <param name="direction"> - One of <seealso cref="PullMode#PULL_FROM_START"/> or
        ///            <seealso cref="PullMode#PULL_FROM_END"/> depending on which direction
        ///            the user is pulling. Only useful when <var>state</var> is
        ///            <seealso cref="RefreshState#PULL_TO_REFRESH"/> or
        ///            <seealso cref="RefreshState#RELEASE_TO_REFRESH"/>. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void onPullEvent(final PullToRefreshBase<V> refreshView, RefreshState state, PullMode direction);
        void onPullEvent(PullToRefreshBase<V> refreshView, RefreshState state, PullMode direction);

    }
}