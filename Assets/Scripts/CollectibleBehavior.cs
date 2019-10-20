using UnityEngine;
using System.Collections;

public class CollectibleBehavior : BaseTileBehavior {
	public Collectibles Type;

	public override void TappedHandler (object sender, System.EventArgs e)
	{
		TileEngine.TappedCollectible (gameObject, Type);
	}
}
