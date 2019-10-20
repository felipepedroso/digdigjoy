using UnityEngine;
using System.Collections;

public class DonationMenuScript : MonoBehaviour {
	public void OnClickDonate10()
	{
		Debug.Log ("Not implemented yet!");
		// TODO: implement the routine to donate 10
	}

	public void OnClickDonate20()
	{
		Debug.Log ("Not implemented yet!");
		// TODO: implement the routine to donate 20
	}
	
	public void OnClickDonate30()
	{
		Debug.Log ("Not implemented yet!");
		// TODO: implement the routine to donate 30
	}

	public void OnClickSubscribe()
	{
		Debug.Log ("Not implemented yet!");
	}

	public void OnClickBack()
	{
		Destroy (gameObject);
		Application.LoadLevel(0);
	}

	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			//Application.Quit();
			OnClickBack();
		}
	}

	void Start () { 
		//SoomlaStore.Initialize (new DigDigJoyAssets());
	} 
}
