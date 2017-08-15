
namespace Com.Loonytone.Droid.PullToRefresh
{

	public enum PullMode
	{
        /**
  * Disable all Pull-to-Refresh gesture and Refreshing handling
  */
        DISABLED= (0x0),

        /**
         * Only allow the user to Pull from the start of the Refreshable View to
         * refresh. The start is either the Top or Left, depending on the
         * scrolling direction.
         */
        PULL_FROM_START= (0x1),

        /**
         * Only allow the user to Pull from the end of the Refreshable View to
         * refresh. The start is either the Bottom or Right, depending on the
         * scrolling direction.
         */
        PULL_FROM_END= (0x2),

        /**
         * Allow the user to both Pull from the start, from the end to refresh.
         */
        BOTH= (0x3),

        /**
         * Disables Pull-to-Refresh gesture handling, but allows manually
         * setting the Refresh state via
         * {@link PullToRefreshBase#setRefreshing= () setRefreshing= ()}.
         */
        MANUAL_REFRESH_ONLY= (0x4)

    }

}