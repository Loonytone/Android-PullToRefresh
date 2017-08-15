using Android.Content;
using Android.Content.Res;
using Com.Loonytone.Droid.PullToRefresh.Inner;

namespace Com.Loonytone.Droid.PullToRefresh
{
    public static class EnumExtensions
    {
        public static bool permitsPullToRefresh(this PullMode value)
        {
            return !(value == PullMode.DISABLED || value == PullMode.MANUAL_REFRESH_ONLY);
        }

        public static bool showHeaderLoadingLayout(this PullMode value)
        {
            return value == PullMode.PULL_FROM_START || value == PullMode.BOTH;
        }

        /**
         * @return true if this mode wants the Loading Layout Footer to be shown
         */
        public static bool showFooterLoadingLayout(this PullMode value)
        {
            return value == PullMode.PULL_FROM_END || value == PullMode.BOTH || value == PullMode.MANUAL_REFRESH_ONLY;
        }

        public static LoadingLayoutBase createLoadingLayout(this AnimationStyle value, Context context, PullMode mode, ScrollOrientation scrollDirection, TypedArray attrs)
        {
            switch (value)
            {
                case AnimationStyle.ROTATE:
                default:
                    return new RotateLoadingLayout(context, mode, scrollDirection, attrs);
                case AnimationStyle.FLIP:
                    return new FlipLoadingLayout(context, mode, scrollDirection, attrs);
            }
        }

    }
}