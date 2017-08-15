
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Lang;

namespace Com.Loonytone.Droid.PullToRefresh
{

	public interface ILoadingLayout
	{

		/// <summary>
		/// Set the Last Updated Text. This displayed under the main label when
		/// Pulling
		/// </summary>
		/// <param name="label"> - Label to set </param>
		string LastUpdatedLabel {set;}

		/// <summary>
		/// Set the drawable used in the loading layout. This is the same as calling
		/// <code>setLoadingDrawable(drawable, PullMode.BOTH)</code>
		/// </summary>
		/// <param name="drawable"> - Drawable to display </param>
		Drawable LoadingDrawable {set;}

		/// <summary>
		/// Set Text to show when the Widget is being Pulled
		/// <code>setPullLabel(releaseLabel, PullMode.BOTH)</code>
		/// </summary>
		/// <param name="pullLabel"> - ICharSequence to display </param>
		string PullLabel {set;}

		/// <summary>
		/// Set Text to show when the Widget is refreshing
		/// <code>setRefreshingLabel(releaseLabel, PullMode.BOTH)</code>
		/// </summary>
		/// <param name="refreshingLabel"> - ICharSequence to display </param>
		string RefreshingLabel {set;}

		/// <summary>
		/// Set Text to show when the Widget is being pulled, and will refresh when
		/// released. This is the same as calling
		/// <code>setReleaseLabel(releaseLabel, PullMode.BOTH)</code>
		/// </summary>
		/// <param name="releaseLabel"> - ICharSequence to display </param>
		string ReleaseLabel {set;}

		/// <summary>
		/// Set's the Sets the typeface and style in which the text should be
		/// displayed. Please see
		/// {@link android.widget.TextView#setTypeface(Typeface)
		/// TextView#setTypeface(Typeface)}. </summary>
		/// <param name="tf"> </param>
		Typeface TextTypeface {set;}

	}

}