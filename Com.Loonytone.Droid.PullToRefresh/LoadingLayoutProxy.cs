using System.Collections.Generic;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Lang;

namespace Com.Loonytone.Droid.PullToRefresh
{

	public class LoadingLayoutProxy : ILoadingLayout
	{

		private readonly HashSet<LoadingLayoutBase> mLoadingLayouts;

		internal LoadingLayoutProxy()
		{
			mLoadingLayouts = new HashSet<LoadingLayoutBase>();
		}

		/// <summary>
		/// This allows you to add extra LoadingLayout instances to this proxy. This
		/// is only necessary if you keep your own instances, and want to have them
		/// included in any
		/// {@link PullToRefreshBase#createLoadingLayoutProxy(boolean, boolean)
		/// createLoadingLayoutProxy(...)} calls.
		/// </summary>
		/// <param name="layout"> - LoadingLayout to have included. </param>
		public virtual void addLayout(LoadingLayoutBase layout)
		{
			if (null != layout)
			{
				mLoadingLayouts.Add(layout);
			}
		}

		public virtual string LastUpdatedLabel
		{
			set
			{
				foreach (LoadingLayoutBase layout in mLoadingLayouts)
				{
					layout.LastUpdatedLabel = value;
				}
			}
		}

		public virtual Drawable LoadingDrawable
		{
			set
			{
				foreach (LoadingLayoutBase layout in mLoadingLayouts)
				{
					layout.LoadingDrawable = value;
				}
			}
		}

		public virtual string RefreshingLabel
		{
			set
			{
				foreach (LoadingLayoutBase layout in mLoadingLayouts)
				{
					layout.RefreshingLabel = value;
				}
			}
		}

		public virtual string PullLabel
		{
			set
			{
				foreach (LoadingLayoutBase layout in mLoadingLayouts)
				{
					layout.PullLabel = value;
				}
			}
		}

		public virtual string ReleaseLabel
		{
			set
			{
				foreach (LoadingLayoutBase layout in mLoadingLayouts)
				{
					layout.ReleaseLabel = value;
				}
			}
		}

		public virtual Typeface TextTypeface
		{
			set
			{
				foreach (LoadingLayoutBase layout in mLoadingLayouts)
				{
					layout.TextTypeface = value;
				}
			}
		}
	}

}