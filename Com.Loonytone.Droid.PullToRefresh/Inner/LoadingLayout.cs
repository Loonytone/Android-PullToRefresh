/// <summary>
///*****************************************************************************
/// Copyright 2011, 2012 Chris Banes.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
/// http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// ******************************************************************************
/// </summary>

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;

namespace Com.Loonytone.Droid.PullToRefresh.Inner
{

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @SuppressLint("ViewConstructor") abstract class LoadingLayout extends PullToRefresh.LoadingLayoutBase
    public abstract class LoadingLayout : LoadingLayoutBase
	{

		internal const string LOG_TAG = "PullToRefresh-LoadingLayout";

		internal static readonly IInterpolator ANIMATION_INTERPOLATOR = new LinearInterpolator();

		private FrameLayout mInnerLayout;

		protected readonly ImageView mHeaderImage;
		protected readonly ProgressBar mHeaderProgress;

		private bool mUseIntrinsicAnimation;

		private readonly TextView mHeaderText;
		private readonly TextView mSubHeaderText;

		protected readonly PullMode mMode;
		protected readonly ScrollOrientation mScrollDirection;

		private string mPullLabel;
		private string mRefreshingLabel;
		private string mReleaseLabel;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public LoadingLayout(android.content.Context context, final PullToRefresh.PullMode mode, final PullToRefresh.ScrollOrientation scrollDirection, android.content.res.TypedArray attrs)
		public LoadingLayout(Context context, PullMode mode, ScrollOrientation scrollDirection, TypedArray attrs) : base(context)
		{
			mMode = mode;
			mScrollDirection = scrollDirection;

			switch (scrollDirection)
			{
				case ScrollOrientation.HORIZONTAL:
					LayoutInflater.From(context).Inflate(Resource.Layout.pull_to_refresh_header_horizontal, this);
					break;
				case ScrollOrientation.VERTICAL:
				default:
					LayoutInflater.From(context).Inflate(Resource.Layout.pull_to_refresh_header_vertical, this);
					break;
			}

			mInnerLayout = (FrameLayout) FindViewById(Resource.Id.fl_inner);
			mHeaderText = (TextView) mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_text);
			mHeaderProgress = (ProgressBar) mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_progress);
			mSubHeaderText = (TextView) mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_sub_text);
			mHeaderImage = (ImageView) mInnerLayout.FindViewById(Resource.Id.pull_to_refresh_image);

			LayoutParams lp = (LayoutParams) mInnerLayout.LayoutParameters;

			switch (mode)
			{
				case PullMode.PULL_FROM_END:
					lp.Gravity = scrollDirection == ScrollOrientation.VERTICAL ? GravityFlags.Top : GravityFlags.Left;

                    // Load in labels
                    mPullLabel = context.GetString(Resource.String.pull_to_refresh_pull_label);
					mRefreshingLabel = context.GetString(Resource.String.pull_to_refresh_refreshing_label);
					mReleaseLabel = context.GetString(Resource.String.pull_to_refresh_release_label);
					break;

				case PullMode.PULL_FROM_START:
				default:
					lp.Gravity = scrollDirection == ScrollOrientation.VERTICAL ? GravityFlags.Bottom : GravityFlags.Right;

					// Load in labels
					mPullLabel = context.GetString(Resource.String.pull_to_refresh_pull_label);
					mRefreshingLabel = context.GetString(Resource.String.pull_to_refresh_refreshing_label);
					mReleaseLabel = context.GetString(Resource.String.pull_to_refresh_release_label);
					break;
			}

			if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderBackground))
			{
				Drawable background = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrHeaderBackground);
				if (null != background)
				{
					ViewCompat.setBackground(this, background);
				}
			}

			if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderTextAppearance))
			{
				TypedValue styleID = new TypedValue();
				attrs.GetValue(Resource.Styleable.PullToRefresh_ptrHeaderTextAppearance, styleID);
				TextAppearance = styleID.Data;
			}
			if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrSubHeaderTextAppearance))
			{
				TypedValue styleID = new TypedValue();
				attrs.GetValue(Resource.Styleable.PullToRefresh_ptrSubHeaderTextAppearance, styleID);
				SubTextAppearance = styleID.Data;
			}

			// Text Color attrs need to be set after TextAppearance attrs
			if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderTextColor))
			{
				ColorStateList colors = attrs.GetColorStateList(Resource.Styleable.PullToRefresh_ptrHeaderTextColor);
				if (null != colors)
				{
					TextColor = colors;
				}
			}
			if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrHeaderSubTextColor))
			{
				ColorStateList colors = attrs.GetColorStateList(Resource.Styleable.PullToRefresh_ptrHeaderSubTextColor);
				if (null != colors)
				{
					SubTextColor = colors;
				}
			}

			// Try and get defined drawable from Attrs
			Drawable imageDrawable = null;
			if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawable))
			{
				imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawable);
			}

			// Check Specific Drawable from Attrs, these overrite the generic
			// drawable attr above
			switch (mode)
			{
				case PullMode.PULL_FROM_START:
				default:
					if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableStart))
					{
						imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableStart);
					}
					else if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableTop))
					{
						Utils.warnDeprecation("ptrDrawableTop", "ptrDrawableStart");
						imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableTop);
					}
					break;

				case PullMode.PULL_FROM_END:
					if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableEnd))
					{
						imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableEnd);
					}
					else if (attrs.HasValue(Resource.Styleable.PullToRefresh_ptrDrawableBottom))
					{
						Utils.warnDeprecation("ptrDrawableBottom", "ptrDrawableEnd");
						imageDrawable = attrs.GetDrawable(Resource.Styleable.PullToRefresh_ptrDrawableBottom);
					}
					break;
			}

			// If we don't have a user defined drawable, load the default
			if (null == imageDrawable)
			{
				imageDrawable = context.Resources.GetDrawable(DefaultDrawableResId);
			}

			// Set Drawable, and save width/height
			LoadingDrawable = imageDrawable;

			reset();
		}

		public override sealed int ContentSize
		{
			get
			{
				switch (mScrollDirection)
				{
					case ScrollOrientation.HORIZONTAL:
						return mInnerLayout.Width;
					case ScrollOrientation.VERTICAL:
					default:
						return mInnerLayout.Height;
				}
			}
		}

		public override sealed void onPull(float scaleOfLayout)
		{
			if (!mUseIntrinsicAnimation)
			{
				onPullImpl(scaleOfLayout);
			}
		}

		public override sealed void pullToRefresh()
		{
			if (null != mHeaderText)
			{
				mHeaderText.Text = mPullLabel;
			}

			// Now call the callback
			pullToRefreshImpl();
		}

		public override sealed void refreshing()
		{
			if (null != mHeaderText)
			{
				mHeaderText.Text = mRefreshingLabel;
			}

			if (mUseIntrinsicAnimation)
			{
				((AnimationDrawable) mHeaderImage.Drawable).Start();
			}
			else
			{
				// Now call the callback
				refreshingImpl();
			}

			if (null != mSubHeaderText)
			{
				mSubHeaderText.Visibility = ViewStates.Gone;
			}
		}

		public override sealed void releaseToRefresh()
		{
			if (null != mHeaderText)
			{
				mHeaderText.Text = mReleaseLabel;
			}

			// Now call the callback
			releaseToRefreshImpl();
		}

		public override sealed void reset()
		{
			if (null != mHeaderText)
			{
				mHeaderText.Text = mPullLabel;
			}
			mHeaderImage.Visibility = ViewStates.Visible;

			if (mUseIntrinsicAnimation)
			{
				((AnimationDrawable) mHeaderImage.Drawable).Stop();
			}
			else
			{
				// Now call the callback
				resetImpl();
			}

			if (null != mSubHeaderText)
			{
				if (TextUtils.IsEmpty(mSubHeaderText.Text))
				{
					mSubHeaderText.Visibility = ViewStates.Gone;
				}
				else
				{
					mSubHeaderText.Visibility = ViewStates.Visible;
				}
			}
		}

		public override string LastUpdatedLabel
		{
			set
			{
				SubHeaderText = value.ToString();
			}
		}

		public sealed override Drawable LoadingDrawable
		{
			set
			{
				// Set Drawable
				mHeaderImage.SetImageDrawable(value);
				mUseIntrinsicAnimation = (value is AnimationDrawable);
    
				// Now call the callback
				onLoadingDrawableSet(value);
			}
		}

		public override string PullLabel
		{
			set
			{
				mPullLabel = value.ToString();
			}
		}

		public override string RefreshingLabel
		{
			set
			{
				mRefreshingLabel = value.ToString();
			}
		}

		public override string ReleaseLabel
		{
			set
			{
				mReleaseLabel = value.ToString();
			}
		}

		public override Typeface TextTypeface
		{
			set
			{
				mHeaderText.Typeface = value;
			}
		}

		/// <summary>
		/// Callbacks for derivative Layouts
		/// </summary>

		protected abstract int DefaultDrawableResId {get;}

		protected abstract void onLoadingDrawableSet(Drawable imageDrawable);

		protected abstract void onPullImpl(float scaleOfLayout);

		protected abstract void pullToRefreshImpl();

		protected abstract void refreshingImpl();

		protected abstract void releaseToRefreshImpl();

		protected abstract void resetImpl();

		private string SubHeaderText
		{
			set
			{
				if (null != mSubHeaderText)
				{
					if (TextUtils.IsEmpty(value))
					{
						mSubHeaderText.Visibility = ViewStates.Gone;
					}
					else
					{
						mSubHeaderText.Text = value;
    
						// Only set it to Visible if we're GONE, otherwise VISIBLE will
						// be set soon
						if (ViewStates.Gone == mSubHeaderText.Visibility)
						{
							mSubHeaderText.Visibility = ViewStates.Visible;
						}
					}
				}
			}
		}

		private int SubTextAppearance
		{
			set
			{
				if (null != mSubHeaderText)
				{
					mSubHeaderText.SetTextAppearance(Context, value);
				}
			}
		}

		private ColorStateList SubTextColor
		{
			set
			{
				if (null != mSubHeaderText)
				{
					mSubHeaderText.SetTextColor(value);
				}
			}
		}

		private int TextAppearance
		{
			set
			{
				if (null != mHeaderText)
				{
					mHeaderText.SetTextAppearance(Context, value);
				}
				if (null != mSubHeaderText)
				{
					mSubHeaderText.SetTextAppearance(Context, value);
				}
			}
		}

		private ColorStateList TextColor
		{
			set
			{
				if (null != mHeaderText)
				{
					mHeaderText.SetTextColor(value);
				}
				if (null != mSubHeaderText)
				{
					mSubHeaderText.SetTextColor(value);
				}
			}
		}

	}

}