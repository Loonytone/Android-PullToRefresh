using System.Collections.Generic;

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
using Android.Media;
using Android.Views;

namespace Com.Loonytone.Droid.PullToRefresh.Views
{

    public class SoundPullEventListener<V> : IOnPullEventListener<V> where V : View
	{

		private readonly Context mContext;
		private readonly Dictionary<RefreshState, int?> mSoundMap;

		private MediaPlayer mCurrentMediaPlayer;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"> - Context </param>
		public SoundPullEventListener(Context context)
		{
			mContext = context;
			mSoundMap = new Dictionary<RefreshState, int?>();
		}

		public void onPullEvent(PullToRefreshBase<V> refreshView, RefreshState ev, PullMode direction)
		{
			int? soundResIdObj = mSoundMap[ev];
			if (null != soundResIdObj)
			{
				playSound(soundResIdObj.Value);
			}
		}

		/// <summary>
		/// Set the Sounds to be played when a Pull Event happens. You specify which
		/// sound plays for which events by calling this method multiple times for
		/// each event.
		/// 
		/// If you've already set a sound for a certain event, and add another sound
		/// for that event, only the new sound will be played.
		/// </summary>
		/// <param name="event"> - The event for which the sound will be played. </param>
		/// <param name="resId"> - Resource Id of the sound file to be played (e.g.
		///            <var>R.raw.pull_sound</var>) </param>
		public virtual void addSoundEvent(RefreshState ev, int resId)
		{
			mSoundMap[ev] = resId;
		}

		/// <summary>
		/// Clears all of the previously set sounds and events.
		/// </summary>
		public virtual void clearSounds()
		{
			mSoundMap.Clear();
		}

		/// <summary>
		/// Gets the current (or last) MediaPlayer instance.
		/// </summary>
		public virtual MediaPlayer CurrentMediaPlayer
		{
			get
			{
				return mCurrentMediaPlayer;
			}
		}

		private void playSound(int resId)
		{
			// Stop current player, if there's one playing
			if (null != mCurrentMediaPlayer)
			{
				mCurrentMediaPlayer.Stop();
				mCurrentMediaPlayer.Release();
			}

			mCurrentMediaPlayer = MediaPlayer.Create(mContext, resId);
			if (null != mCurrentMediaPlayer)
			{
				mCurrentMediaPlayer.Start();
			}
		}

	}

}