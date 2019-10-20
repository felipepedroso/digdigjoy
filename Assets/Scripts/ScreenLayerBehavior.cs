using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using TouchScript.Gestures.Simple;

public class ScreenLayerBehavior : MonoBehaviour {

	public bool CanPress
	{
		get
		{
			return GetComponents<PressGesture>().Length > 0 && GetComponents<ReleaseGesture>().Length > 0 ;
		}
	}

	public bool IsPannable
	{
		get
		{
			return GetComponents<SimplePanGesture>().Length > 0;
		}
	}
	
	// Use this for initialization
	void OnEnable () {
		if (CanPress) {
			GetComponent<PressGesture>().Pressed += HandlePressed;
			GetComponent<ReleaseGesture>().Released += HandleReleased;
		}

		if (IsPannable) {
			SimplePanGesture panGesture = GetComponent<SimplePanGesture>();

			panGesture.Panned += HandlePanned;
			panGesture.PanCompleted += HandlePanCompleted;
			panGesture.PanStarted += HandlePanStarted;
		}

	}
	void OnDisable () {
		if (CanPress) {
			GetComponent<PressGesture>().Pressed -= HandlePressed;
			GetComponent<ReleaseGesture>().Released -= HandleReleased;
		}

		if (IsPannable) {
			SimplePanGesture panGesture = GetComponent<SimplePanGesture>();
			
			panGesture.Panned -= HandlePanned;
			panGesture.PanCompleted -= HandlePanCompleted;
			panGesture.PanStarted -= HandlePanStarted;
		}
	}
	void HandlePanStarted (object sender, System.EventArgs e)
	{
		Debug.Log ("HandlePanStarted");
	}

	void HandlePanCompleted (object sender, System.EventArgs e)
	{
		Debug.Log ("HandlePanCompleted");
	}

	void HandlePanned (object sender, System.EventArgs e)
	{
		SimplePanGesture panGesture = sender as SimplePanGesture;
		Debug.Log ("HandlePanned: " + panGesture.LocalDeltaPosition);

		GameObject.Find ("Main Camera").transform.position -= panGesture.LocalDeltaPosition;


		if (panGesture.ActiveTouches.Count > 2) {
			//
		}
	}



	void HandleReleased (object sender, System.EventArgs e)
	{
		Debug.Log("Released");
	}

	void HandlePressed (object sender, System.EventArgs e)
	{
		Debug.Log("Pressed");
	}
}
