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
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Com.Loonytone.Droid.PullToRefresh.Inner
{

    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @SuppressLint("ViewConstructor") public class IndicatorLayout extends android.widget.FrameLayout implements android.view.animation.Animation.AnimationListener
    public class IndicatorLayout : FrameLayout, Animation.IAnimationListener
    {

        internal const int DEFAULT_ROTATION_ANIMATION_DURATION = 150;

        private Animation mInAnim, mOutAnim;
        private ImageView mArrowImageView;

        private readonly Animation mRotateAnimation, mResetRotateAnimation;

        public IndicatorLayout(Context context, PullMode mode) : base(context)
        {
            mArrowImageView = new ImageView(context);
            
            Drawable arrowD = ContextCompat.GetDrawable(Context, Resource.Drawable.indicator_arrow); /*Resources.GetDrawable(Resource.Drawable.indicator_arrow);*/
            mArrowImageView.SetImageDrawable(arrowD);

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int padding = getResources().getDimensionPixelSize(PullToRefresh.Resource.Dimension.indicator_internal_padding);
            int padding = Resources.GetDimensionPixelSize(Resource.Dimension.indicator_internal_padding);
            mArrowImageView.SetPadding(padding, padding, padding, padding);
            AddView(mArrowImageView);

            int inAnimResId, outAnimResId;
            switch (mode)
            {
                case PullMode.PULL_FROM_END:
                    inAnimResId = Resource.Animation.slide_in_from_bottom;
                    outAnimResId = Resource.Animation.slide_out_to_bottom;
                    SetBackgroundResource(Resource.Drawable.indicator_bg_bottom);

                    // Rotate Arrow so it's pointing the correct way
                    mArrowImageView.SetScaleType(ImageView.ScaleType.Matrix);
                    Matrix matrix = new Matrix();
                    matrix.SetRotate(180f, arrowD.IntrinsicWidth / 2f, arrowD.IntrinsicHeight / 2f);
                    mArrowImageView.ImageMatrix = matrix;
                    break;
                case PullMode.PULL_FROM_START:
                default:
                    inAnimResId = Resource.Animation.slide_in_from_top;
                    outAnimResId = Resource.Animation.slide_out_to_top;
                    SetBackgroundResource(Resource.Drawable.indicator_bg_top);
                    break;
            }

            mInAnim = AnimationUtils.LoadAnimation(context, inAnimResId);
            mInAnim.SetAnimationListener(this);

            mOutAnim = AnimationUtils.LoadAnimation(context, outAnimResId);
            mOutAnim.SetAnimationListener(this);

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final android.view.animation.Interpolator interpolator = new android.view.animation.LinearInterpolator();
            IInterpolator interpolator = new LinearInterpolator();
            mRotateAnimation = new RotateAnimation(0, -180, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            mRotateAnimation.Interpolator = interpolator;
            mRotateAnimation.Duration = DEFAULT_ROTATION_ANIMATION_DURATION;
            mRotateAnimation.FillAfter = true;

            mResetRotateAnimation = new RotateAnimation(-180, 0, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            mResetRotateAnimation.Interpolator = (interpolator);
            mResetRotateAnimation.Duration = (DEFAULT_ROTATION_ANIMATION_DURATION);
            mResetRotateAnimation.FillAfter = (true);

        }

        public bool IsVisible
        {
            get
            {
                Animation currentAnim = Animation;
                if (null != currentAnim)
                {
                    return mInAnim == currentAnim;
                }

                return Visibility == ViewStates.Visible;
            }
        }

        public virtual void hide()
        {
            StartAnimation(mOutAnim);
        }

        public virtual void show()
        {
            mArrowImageView.ClearAnimation();
            StartAnimation(mInAnim);
        }

        public void OnAnimationEnd(Animation animation)
        {
            if (animation == mOutAnim)
            {
                mArrowImageView.ClearAnimation();
                Visibility = ViewStates.Gone;
            }
            else if (animation == mInAnim)
            {
                Visibility = ViewStates.Visible;
            }

            ClearAnimation();
        }

        public void OnAnimationRepeat(Animation animation)
        {
            // NO-OP
        }

        public void OnAnimationStart(Animation animation)
        {
            Visibility = ViewStates.Visible;
        }

        public virtual void releaseToRefresh()
        {
            mArrowImageView.StartAnimation(mRotateAnimation);
        }

        public virtual void pullToRefresh()
        {
            mArrowImageView.StartAnimation(mResetRotateAnimation);
        }

    }

}