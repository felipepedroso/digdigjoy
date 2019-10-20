using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using System;

public class DirtBehavior : DestroyableBehavior {

	public AudioClip FlickSound;

	public override void FlickedHandler (object sender, EventArgs e)
	{
		base.FlickedHandler (sender, e);
		GetComponent<AudioSource>().PlayOneShot (FlickSound);
		TryToDestroy ();
	}
}
