using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	public void OnClickPlay()
	{
		Application.LoadLevel ("GameScene");
	}

	public void OnClickTutorial()
	{
		Application.LoadLevel ("TutorialScene");
	}
	
	public void OnClickExit()
	{
		Application.Quit ();
	}

	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();	
		}
	}
}
