
namespace Com.Loonytone.Droid.PullToRefresh
{

    public enum RefreshState
    {

        /**
         * When the UI is in a state which means that user is not interacting
         * with the Pull-to-Refresh function.
         */
        RESET = (0x0),

        /**
         * When the UI is being pulled by the user, but has not been pulled far
         * enough so that it refreshes when released.
         */
        PULL_TO_REFRESH = (0x1),

        /**
         * When the UI is being pulled by the user, and <strong>has</strong>
         * been pulled far enough so that it will refresh when released.
         */
        RELEASE_TO_REFRESH = (0x2),

        /**
         * When the UI is currently refreshing, caused by a pull gesture.
         */
        REFRESHING = (0x8),

        /**
         * When the UI is currently refreshing, caused by a call to
         * {@link PullToRefreshBase#setRefreshing= () setRefreshing= ()}.
         */
        MANUAL_REFRESHING = (0x9),

        /**
         * When the UI is currently overscrolling, caused by a fling on the
         * Refreshable View.
         */
        OVERSCROLLING = (0x10)
    }
}