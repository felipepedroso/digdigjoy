using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public abstract class BaseTileBehavior : MonoBehaviour {
	private TileEngine tileEngine;

	protected TileEngine TileEngine
	{
		get
		{
			return tileEngine;
		}
	}

	public bool IsTappable
	{
		get
		{
			return GetComponents<TapGesture>().Length > 0;
		}
	}

	public bool IsFlickable 
	{
		get
		{
			return GetComponents<FlickGesture>().Length > 0;
		}
	}

	private void OnEnable()
	{    
		//Debug.Log ("OnEnable");
		//Debug.Log ("IsTappable: " + IsTappable);
		if (IsTappable) {

			GetComponent<TapGesture>().Tapped += TappedHandler;
		}

		//Debug.Log ("IsFlickable: " + IsFlickable);
		if (IsFlickable) 
		{
			foreach (var flick in GetComponents<FlickGesture>())
			{
				flick.Flicked += FlickedHandler;
			}
		}

		tileEngine = (TileEngine)FindObjectOfType(typeof(TileEngine));
		//Debug.Log ("TileEngine was found: " + (tileEngine != null));
	}
	
	private void OnDisable()
	{
		//Debug.Log ("OnDisable");
		if (IsTappable) {
			GetComponent<TapGesture>().Tapped -= TappedHandler;
		}

		foreach (var flick in GetComponents<FlickGesture>())
		{
			flick.Flicked -= FlickedHandler;
		}
	}

	public virtual void FlickedHandler (object sender, System.EventArgs e)
	{
		//Debug.Log("Flicked!"); // Disabled if there is no FlickedGesture component
	}

	public virtual void TappedHandler (object sender, System.EventArgs e)
	{
		//Vector2 tilePosition = tileEngine.GetTilePosition (gameObject);

		//Debug.Log ("Tapped");
		//Debug.Log (string.Format("Tapped tile position: {0}", tilePosition));
	}


	public void Move(Vector2 direction)
	{
		if (tileEngine != null) {
			tileEngine.MoveTile(gameObject, direction);
		}
	}

	public void DestroyTile()
	{
		if (tileEngine != null) {
			tileEngine.DestroyTile(gameObject);
		}
	}
}
