using System;

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
using Android.Views.Animations;
using Android.Widget;
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh.Inner
{

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @SuppressLint("ViewConstructor") public class FlipLoadingLayout extends LoadingLayout
    public class FlipLoadingLayout : LoadingLayout
	{

		internal const int FLIP_ANIMATION_DURATION = 150;

		private readonly Animation mRotateAnimation, mResetRotateAnimation;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public FlipLoadingLayout(android.content.Context context, final PullToRefresh.PullMode mode, final PullToRefresh.ScrollOrientation scrollDirection, android.content.res.TypedArray attrs)
		public FlipLoadingLayout(Context context, PullMode mode, ScrollOrientation scrollDirection, TypedArray attrs) : base(context, mode, scrollDirection, attrs)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rotateAngle = mode == PullToRefresh.PullMode.PULL_FROM_START ? -180 : 180;
			int rotateAngle = mode == PullMode.PULL_FROM_START ? -180 : 180;

			mRotateAnimation = new RotateAnimation(0, rotateAngle, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
			mRotateAnimation.Interpolator = ANIMATION_INTERPOLATOR;
			mRotateAnimation.Duration = FLIP_ANIMATION_DURATION;
			mRotateAnimation.FillAfter = true;

			mResetRotateAnimation = new RotateAnimation(rotateAngle, 0, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
			mResetRotateAnimation.Interpolator = (ANIMATION_INTERPOLATOR);
			mResetRotateAnimation.Duration = FLIP_ANIMATION_DURATION;
			mResetRotateAnimation.FillAfter = true;
		}

		protected override void onLoadingDrawableSet(Drawable imageDrawable)
		{
			if (null != imageDrawable)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dHeight = imageDrawable.getIntrinsicHeight();
				int dHeight = imageDrawable.IntrinsicHeight;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dWidth = imageDrawable.getIntrinsicWidth();
				int dWidth = imageDrawable.IntrinsicWidth;

				/// <summary>
				/// We need to set the width/height of the ImageView so that it is
				/// square with each side the size of the largest drawable dimension.
				/// This is so that it doesn't clip when rotated.
				/// </summary>
				ViewGroup.LayoutParams lp = mHeaderImage.LayoutParameters;
				lp.Width = lp.Height = Math.Max(dHeight, dWidth);
				mHeaderImage.RequestLayout();

				/// <summary>
				/// We now rotate the Drawable so that is at the correct rotation,
				/// and is centered.
				/// </summary>
				mHeaderImage.SetScaleType(ImageView.ScaleType.Matrix);
				Matrix matrix = new Matrix();
				matrix.PostTranslate((lp.Width - dWidth) / 2f, (lp.Height - dHeight) / 2f);
				matrix.PostRotate(DrawableRotationAngle, lp.Width / 2f, lp.Height / 2f);
				mHeaderImage.ImageMatrix = matrix;
			}
		}

		protected override void onPullImpl(float scaleOfLayout)
		{
			// NO-OP
		}

		protected override void pullToRefreshImpl()
		{
			// Only start reset Animation, we've previously show the rotate anim
			if (mRotateAnimation == mHeaderImage.Animation)
			{
				mHeaderImage.StartAnimation(mResetRotateAnimation);
			}
		}

		protected override void refreshingImpl()
		{
			mHeaderImage.ClearAnimation();
			mHeaderImage.Visibility = ViewStates.Invisible;
			mHeaderProgress.Visibility = ViewStates.Visible;
		}

		protected override void releaseToRefreshImpl()
		{
			mHeaderImage.StartAnimation(mRotateAnimation);
		}

		protected override void resetImpl()
		{
			mHeaderImage.ClearAnimation();
			mHeaderProgress.Visibility = ViewStates.Gone;
			mHeaderImage.Visibility = ViewStates.Visible;
		}

		protected override int DefaultDrawableResId
		{
			get
			{
				return Resource.Drawable.default_ptr_flip;
			}
		}

		private float DrawableRotationAngle
		{
			get
			{
				float angle = 0f;
				switch (mMode)
				{
					case PullMode.PULL_FROM_END:
						if (mScrollDirection == ScrollOrientation.HORIZONTAL)
						{
							angle = 90f;
						}
						else
						{
							angle = 180f;
						}
						break;
    
					case PullMode.PULL_FROM_START:
						if (mScrollDirection == ScrollOrientation.HORIZONTAL)
						{
							angle = 270f;
						}
						break;
    
					default:
						break;
				}
    
				return angle;
			}
		}

	}

}