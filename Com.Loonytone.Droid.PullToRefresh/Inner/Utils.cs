
using Android.Util;

namespace Com.Loonytone.Droid.PullToRefresh.Inner
{

	public class Utils
	{

		internal const string LOG_TAG = "PullToRefresh";

		public static void warnDeprecation(string depreacted, string replacement)
		{
			Log.Warn(LOG_TAG, "You're using the deprecated " + depreacted + " attr, please switch over to " + replacement);
		}

	}

}