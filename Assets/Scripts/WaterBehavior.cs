using UnityEngine;
using System.Collections;
using System;

public class WaterBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.ValueTo(gameObject, iTween.Hash("from", new Color(1,1,1,0.2f),"to", new Color(1,1,1,0.6f),"onupdate", "UpdateColor", "time", 1f));
	}

	public void UpdateColor(Color newValue)
	{
		GetComponent<SpriteRenderer> ().color = (Color)newValue;
	}


}
