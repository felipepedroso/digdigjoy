using UnityEngine;
using System.Collections;

public class DestroyableBehavior : BaseTileBehavior {

	public int Life;
	private int CurrentLife;
	public Sprite[] StateSprite;

	public void Start()
	{
		CurrentLife = Life;
	}

	public void TryToDestroy()
	{
		CurrentLife--;

		if (StateSprite != null && StateSprite.Length > 0) {
			float lifePercentage = (float) CurrentLife / (float) Life;

			int spriteIndex = (int)(lifePercentage * StateSprite.Length);
			GetComponent<SpriteRenderer>().sprite = StateSprite[spriteIndex];
		}

		if (CurrentLife <= 0) {

			DestroyTile();
		}
	}

}
