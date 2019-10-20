using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using System;

public class RockBehavior : DestroyableBehavior {
	public float ShakeAmount;
	public float ShakeDuration;
	public AudioClip FlickSound;
	public AudioClip TapSound;

	public override void FlickedHandler (object sender, EventArgs e)
	{
		base.FlickedHandler (sender, e);

		FlickGesture gestureComponent = sender as FlickGesture;

		Move (gestureComponent.ScreenFlickVector.normalized);
	}

	public void PlayMoveSound()
	{
		GetComponent<AudioSource>().PlayOneShot (FlickSound);
	}

	public override void TappedHandler (object sender, EventArgs e)
	{
		base.TappedHandler (sender, e);
		iTween.ShakePosition(gameObject,new Vector3(ShakeAmount,ShakeAmount), ShakeDuration);
		GetComponent<AudioSource>().PlayOneShot (TapSound);
		TryToDestroy ();
	}
}
