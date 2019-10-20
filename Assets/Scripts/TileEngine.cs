using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TileEngine : MonoBehaviour {
	public int Width, Height;

	Layer floorLayer, mainLayer, collectiblesLayer;
	List<Layer> layers;

	public Text CoinsScore, GemstonesScore;
	private int coinsScore = 0, gemstonesScore = 0, totalGems=0, totalCoins=0, waterTilesCount=0;

	int maxWaterTiles;

	private GameObject waterPrefab;
	private Vector2 fountainPosition;

	public Text EogLine1, EogLine2;

	private struct AvailableTiles{
		private const string TileResourcesPath = "Tiles/";
		public const string Floor = TileResourcesPath + "Floor";
		public const string Fountain = TileResourcesPath + "Fountain";
		public const string Wall = TileResourcesPath + "Wall";
		public const string Rock = TileResourcesPath + "Rock";
		public const string Dirt = TileResourcesPath + "Dirt";
		public const string Coin = TileResourcesPath + "Coin";
		public const string Gemstone = TileResourcesPath + "Gemstone";
		public const string Water = TileResourcesPath + "Water";
	}
	
	public AudioClip GameOverSound, VictorySound, CoinSound, GemSound, WaterSound;

	void Start(){
		CleanScene ();
		EogLine1.gameObject.SetActive(false);
		EogLine2.gameObject.SetActive(false);

		GenerateFloorLayer ();
		GenerateMainLayer ();

		layers = new List<Layer> ();
		layers.Add (floorLayer);
		layers.Add (collectiblesLayer);
		layers.Add (mainLayer);

		waterPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Water);
		maxWaterTiles = (int)((Width - 2) * (Height - 2) * 0.7);
		UpdateFountain ();

		UpdateScores ();

		float goX = -Width / 2 + (Width % 2 == 0 ? 0.5f : 0);
		float goY = -Height / 2 + (Height % 2 == 0 ? 0.5f : 0);
		gameObject.transform.position = new Vector3 (goX, goY, gameObject.transform.position.z);
	}

	void CleanScene ()
	{
		coinsScore = totalCoins = gemstonesScore = totalGems = waterTilesCount = maxWaterTiles = 0;
		gameObject.transform.position = Vector3.zero;

		if (floorLayer != null) {
			Destroy (floorLayer.LayerGameObject);
			floorLayer = null;
		}
		if (mainLayer != null) {
			Destroy (mainLayer.LayerGameObject);
			mainLayer = null;
		}

		if (collectiblesLayer != null) {
			Destroy (collectiblesLayer.LayerGameObject);
			collectiblesLayer = null;
		}
	}

	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			//Application.Quit();	
			Application.LoadLevel("MenuScene");
		}

		if (Input.GetKey(KeyCode.W)) {
			coinsScore= totalCoins;
			gemstonesScore= totalGems;
			UpdateScores();
		}

		if (Input.GetKey(KeyCode.Q)) {
			waterTilesCount = maxWaterTiles;
			UpdateFountain();		
		}

		if (EogLine1.gameObject.activeSelf && EogLine2.gameObject.activeSelf) {
			if (Input.touchCount > 0 || Input.GetKey(KeyCode.R)) {
				//CleanScene();
				Start ();
			}
		}
	}

	void UpdateFountain ()
	{
		int waterCountBefore = waterTilesCount;

		Stack<Vector2> cells = new Stack<Vector2> ();
		List<Vector2> processedCells = new List<Vector2> ();

		cells.Push (fountainPosition);

		while (cells.Count > 0) {
			Vector2 cell = cells.Pop();

			int cellPositionX = (int)cell.x;
			int cellPositionY = (int)cell.y;

			GameObject tile = mainLayer.LayerArray[cellPositionX, cellPositionY];

			if (processedCells.Contains(cell)) {
				continue;
			}

			if (tile == null || tile.name.ToLower().Contains("water")) {
				processedCells.Add(cell);

				if (tile == null) {
					mainLayer.AddTile(waterPrefab, cellPositionX, cellPositionY);
					waterTilesCount++;
				}

				Vector2 tileNorth = cell + new Vector2(0,1);
				if (!processedCells.Contains(tileNorth)) {
					cells.Push(tileNorth);
				}
				Vector2 tileSouth = cell + new Vector2(0,-1);
				if (!processedCells.Contains(tileSouth)) {
					cells.Push(tileSouth);
				}

				Vector2 tileEast = cell + new Vector2(1,0);
				if (!processedCells.Contains(tileEast)) {
					cells.Push(tileEast);
				}
				Vector2 tileWest = cell + new Vector2(-1,0);
				if (!processedCells.Contains(tileWest)) {
					cells.Push(tileWest);
				}

			}
		}

		if (waterCountBefore != waterTilesCount) {
			GetComponent<AudioSource>().PlayOneShot(WaterSound);
		}

		if (HasLost()) {
			GetComponent<AudioSource>().PlayOneShot (GameOverSound);
			if (EogLine1 != null && EogLine2 != null) {
				EogLine1.gameObject.SetActive(true);
				EogLine1.text = "Você perdeu!";
				EogLine2.gameObject.SetActive(true);
				return;
			}
		}
	}

	public bool HasWon()
	{
		return coinsScore == totalCoins && gemstonesScore == totalGems;
	}

	public bool HasLost()
	{
		return waterTilesCount >= maxWaterTiles;
	}

	public Layer FindTileLayer(GameObject tileGameObject)
	{
		if (tileGameObject != null) {
				foreach (Layer layer in layers) {
					if (layer.ContainsTile(tileGameObject)) {
						return layer;
					}
				}
		}

		return null;
	}

	public void MoveTile(GameObject tileGameObject, Vector2 direction)
	{
		Layer tileLayer = FindTileLayer (tileGameObject);

		if (tileLayer != null) {
			if(tileLayer.MoveTile(tileGameObject, direction))
			{
				RockBehavior rockBehavior = tileGameObject.GetComponent<RockBehavior>();

				if (rockBehavior != null) {
					rockBehavior.PlayMoveSound();
				}
			}
		}
		UpdateFountain ();
	}
	
	public void DestroyTile (GameObject tileGameObject)
	{
		Layer tileLayer = FindTileLayer (tileGameObject);
		
		if (tileLayer != null) {
			if (tileLayer.RemoveTile(tileGameObject)) {
				Destroy(tileGameObject);
			}
		}
		UpdateFountain ();
	}
	
	public void TappedCollectible (GameObject collectibleGameObject, Collectibles type)
	{
		DestroyTile (collectibleGameObject);
		
		switch (type) {
		case Collectibles.Coin:
			coinsScore++;
			GetComponent<AudioSource>().PlayOneShot(CoinSound);
			break;
		case Collectibles.Gemstone:
			gemstonesScore++;
			GetComponent<AudioSource>().PlayOneShot(GemSound);
			break;
		default:
			break;
		}
		
		UpdateScores ();
	}

	private void UpdateScores()
	{
		if (CoinsScore != null) {
			CoinsScore.text = coinsScore.ToString().PadLeft(2,'0') + "/" + totalCoins.ToString().PadLeft(2,'0');
		}
		if (GemstonesScore != null) {
			GemstonesScore.text = gemstonesScore.ToString().PadLeft(2,'0') + "/" + totalGems.ToString().PadLeft(2,'0');;
		}

		if (HasWon()) {
			UpdateFountain();

			if (HasLost()) {
				return;
			}

			GetComponent<AudioSource>().PlayOneShot (VictorySound);
			if (EogLine1 != null && EogLine2 != null) {
				EogLine1.gameObject.SetActive(true);
				EogLine1.text = "Você venceu!";
				EogLine2.gameObject.SetActive(true);
			}	
		}
	}

	void GenerateFloorLayer ()
	{
		floorLayer = new Layer ("FloorLayer", gameObject.transform.position, Width, Height);
		GameObjectUtils.AppendChild (gameObject, floorLayer.LayerGameObject);
		floorLayer.LayerGameObject.transform.position = Vector3.zero;

		GameObject floorPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Floor);
		GameObject fountainPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Fountain);

		if (floorPrefab != null && fountainPrefab != null) {
			// Generating fountain
			int fountainX = Random.Range(1, Width - 1);
			int fountainY = Random.Range(1, Height - 1);
			
			for (int i = 0; i < Width; i++) {
				for (int j = 0; j < Height; j++) {
					GameObject tilePrefab = floorPrefab;
					
					if (fountainX == i && fountainY == j) {
						tilePrefab = fountainPrefab;
					}

					floorLayer.AddTile(tilePrefab, i, j);

					if (tilePrefab.Equals(fountainPrefab)) {
						fountainPosition = new Vector2(i, j);
					}
				}
			}
		}
		//GameObjectUtils.CenterOnChildren (floorLayer.LayerGameObject);
	}

	void GenerateMainLayer ()
	{
		mainLayer = new Layer ("MainLayer", gameObject.transform.position, Width, Height);
		GameObjectUtils.AppendChild (gameObject, mainLayer.LayerGameObject);
		mainLayer.LayerGameObject.transform.position = Vector3.zero;

		collectiblesLayer = new Layer ("CollectiblesLayer", gameObject.transform.position, Width, Height);
		GameObjectUtils.AppendChild (gameObject, collectiblesLayer.LayerGameObject);
		collectiblesLayer.LayerGameObject.transform.position = Vector3.zero;

		GameObject wallPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Wall);
		GameObject rockPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Rock);
		GameObject dirtPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Dirt);
		GameObject coinPrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Coin);
		GameObject gemStonePrefab = GameObjectUtils.GetPrefabFromResources (AvailableTiles.Gemstone);

		if (wallPrefab != null && rockPrefab != null) {
			GameObject[] tilesOnMainLayer = {dirtPrefab, dirtPrefab, rockPrefab};

			for (int i = 0; i < Width; i++) {
				for (int j = 0; j < Height; j++) {
					GameObject prefabMain = null, prefabCollectible = null;

					if (i == 0 || j == 0 || i == Width-1 || j == Height-1) {
						prefabMain = wallPrefab;
					}else{
						int randomTileIndex = Random.Range(0, tilesOnMainLayer.Length + 1);

						if (randomTileIndex != tilesOnMainLayer.Length) {
							prefabMain = tilesOnMainLayer[randomTileIndex];

							if (prefabMain.Equals(dirtPrefab)) {
								int randomNumber = Random.Range(1,100);
								
								if (randomNumber > 90) {
									prefabCollectible = gemStonePrefab;
									totalGems++;

								}else if (randomNumber > 50){
									prefabCollectible = coinPrefab;
									totalCoins++;
								}
							}
						}
					}

					if (prefabMain != null) {
						mainLayer.AddTile(prefabMain, i, j);
					}

					if (prefabCollectible != null) {
						collectiblesLayer.AddTile(prefabCollectible, i, j);
					}
				}
			}
		}
		//GameObjectUtils.CenterOnChildren (mainLayer.LayerGameObject);
		//GameObjectUtils.CenterOnChildren (collectiblesLayer.LayerGameObject);
	}
}
