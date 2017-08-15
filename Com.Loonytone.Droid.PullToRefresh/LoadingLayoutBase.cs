
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Com.Loonytone.Droid.PullToRefresh
{

	/// <summary>
	/// Created by zwenkai on 2015/12/19.
	/// </summary>
	public abstract class LoadingLayoutBase : FrameLayout, ILoadingLayout
	{
		public abstract string ReleaseLabel {set;}
		public abstract string RefreshingLabel {set;}
		public abstract string PullLabel {set;}

		public LoadingLayoutBase(Context context) : base(context)
		{
		}

		public LoadingLayoutBase(Context context, IAttributeSet attrs) : base(context, attrs)
		{
		}

		public LoadingLayoutBase(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}


		public void SetHeight(int value)
		{
            ViewGroup.LayoutParams lp = LayoutParameters;
            lp.Height = value;
            RequestLayout();
        }

		public void  SetWidth(int value)
		{
            ViewGroup.LayoutParams lp = LayoutParameters;
            lp.Width = value;
            RequestLayout();
        }

		public virtual string LastUpdatedLabel
		{
			set
			{
    
			}
		}

		public virtual Drawable LoadingDrawable
		{
			set
			{
    
			}
		}

		public virtual Typeface TextTypeface
		{
			set
			{
    
			}
		}

		/// <summary>
		/// get the LoadingLayout height or width
		/// </summary>
		/// <returns> size </returns>
		public abstract int ContentSize {get;}

		/// <summary>
		/// Call when the widget begins to slide
		/// </summary>
		public abstract void pullToRefresh();

		/// <summary>
		/// Call when the LoadingLayout is fully displayed
		/// </summary>
		public abstract void releaseToRefresh();

		/// <summary>
		/// Call when the LoadingLayout is sliding
		/// </summary>
		/// <param name="scaleOfLayout"> scaleOfLayout </param>
		public abstract void onPull(float scaleOfLayout);

		/// <summary>
		/// Call when the LoadingLayout is fully displayed and the widget has released.
		/// Used to prompt the user loading data
		/// </summary>
		public abstract void refreshing();

		/// <summary>
		/// Call when the data has loaded
		/// </summary>
		public abstract void reset();

		public virtual void hideAllViews()
		{
			hideAllViews(this);
		}

		public virtual void showInvisibleViews()
		{
			showAllViews(this);
		}

		private void hideAllViews(View view)
		{
			if (view is ViewGroup)
			{
				for (int i = 0; i < ((ViewGroup)view).ChildCount; i++)
				{
					hideAllViews(((ViewGroup)view).GetChildAt(i));
				}
			}
			else
			{
				if (ViewStates.Visible == view.Visibility)
				{
					view.Visibility = ViewStates.Invisible;
				}
			}
		}

		private void showAllViews(View view)
		{
			if (view is ViewGroup)
			{
				for (int i = 0; i < ((ViewGroup)view).ChildCount; i++)
				{
					showAllViews(((ViewGroup) view).GetChildAt(i));
				}
			}
			else
			{
				if (ViewStates.Invisible == view.Visibility)
				{
					view.Visibility = ViewStates.Visible;
				}
			}
		}

	}

}