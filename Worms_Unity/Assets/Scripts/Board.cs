using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {
	public static Board instance;

	public int tileSize = 100;
	public IntVector2 size;

	public Tile tilePrefab;
	public TilePassage passagePrefab;
	public TileWall wallPrefab;
	public tk2dSprite enemyIndicatorPrefab;

	public Tile[,] tiles;

	private GameObject tileHolder;

	private IntVector2 nextEnemyCoordsLeftSide;
	private IntVector2 nextEnemyCoordsRightSide;
	private IntVector2 nextEnemyCoordsTopSide;
	private IntVector2 nextEnemyCoordsBottomSide;

	private List<Tile> enemyEntryTilesLeftSide;
	private List<Tile> enemyEntryTilesRightSide;
	private List<Tile> enemyEntryTilesTopSide;
	private List<Tile> enemyEntryTilesBottomSide;

	private tk2dSprite enemyIndicatorLeftSide;
	private tk2dSprite enemyIndicatorRightSide;
	private tk2dSprite enemyIndicatorTopSide;
	private tk2dSprite enemyIndicatorBottomSide;

	void Awake() {
		instance = this;
	}

	void Start () {

	}
	
	void Update () {
	
	}

	public void Generate() {
		tileHolder = new GameObject("Tiles");
		tileHolder.transform.parent = transform;
		tileHolder.transform.localPosition = Vector3.zero;

		tiles = new Tile[size.x, size.y];
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				CreateTile(new IntVector2(x, y));
			}
		}

		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				Tile tile = GetTile(new IntVector2(x, y));
				for (int i = 0; i < BoardDirections.Count; i++) {
					BoardDirection direction = (BoardDirection)i;
					if (tile.GetDirectionIsInitialized(direction)) continue;
						
					IntVector2 neighborCoordinates = tile.coordinates + direction.ToIntVector2();
					if (ContainsCoordinates(neighborCoordinates)) {
						Tile otherTile = GetTile(neighborCoordinates);
						if (Random.value > 0.2f) CreatePassage(tile, otherTile, direction);
						else CreateWall(tile, otherTile, direction);
					}
					else {
						CreateWall(tile, null, direction);
					}
				}
			}
		}

		UpdateNextEnemyPositions();
	}

	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.y));
		}
	}

	public Tile GetTile(IntVector2 coordinates) {
		if (!ContainsCoordinates(coordinates)) return null;
		else return tiles[coordinates.x, coordinates.y];
	}

	public Vector3 GetTilePosition(IntVector2 coordinates) {
		Vector3 pos = new Vector3();
		pos.x = (coordinates.x + 0.5f - size.x / 2f) * tileSize;
		pos.y = (coordinates.y + 0.5f - size.y / 2f) * tileSize;
		pos.z = 0;
		return pos;
	}

	public bool ContainsCoordinates(IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.y >= 0 && coordinate.y < size.y;
	}

	public void Move(BoardDirection direction) {
		Tile tile;
		
		if (direction == BoardDirection.Up) {
			for (int y = size.y - 2; y >= 0; y--) {
				for (int x = 0; x < size.x; x++) {
					tile = GetTile(new IntVector2(x, y));
					MoveTileEntities(tile, direction);
				}
			}
		}
		else if (direction == BoardDirection.Down) {
			for (int y = 1; y < size.y; y++) {
				for (int x = 0; x < size.x; x++) {
					tile = GetTile(new IntVector2(x, y));
					MoveTileEntities(tile, direction);
				}
			}
		}
		else if (direction == BoardDirection.Right) {
			for (int x = size.x - 2; x >= 0; x--) {
				for (int y = 0; y < size.y; y++) {
					tile = GetTile(new IntVector2(x, y));
					MoveTileEntities(tile, direction);
				}
			}
		}
		else if (direction == BoardDirection.Left) {
			for (int x = 1; x < size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					tile = GetTile(new IntVector2(x, y));
					MoveTileEntities(tile, direction);
				}
			}
		}
	}
	
	private void UpdateNextEnemyPositions() {
		if (enemyIndicatorLeftSide == null) enemyIndicatorLeftSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
		if (enemyIndicatorRightSide == null) enemyIndicatorRightSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
		if (enemyIndicatorTopSide == null) enemyIndicatorTopSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
		if (enemyIndicatorBottomSide == null) enemyIndicatorBottomSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;

		if (enemyEntryTilesLeftSide == null) enemyEntryTilesLeftSide = new List<Tile>();
		if (enemyEntryTilesRightSide == null) enemyEntryTilesRightSide = new List<Tile>();
		if (enemyEntryTilesTopSide == null) enemyEntryTilesTopSide = new List<Tile>();
		if (enemyEntryTilesBottomSide == null) enemyEntryTilesBottomSide = new List<Tile>();

		enemyEntryTilesLeftSide.Clear();
		enemyEntryTilesRightSide.Clear();
		enemyEntryTilesTopSide.Clear();
		enemyEntryTilesBottomSide.Clear();

		for (int y = 0; y < size.y; y++) {
			Tile tile = GetTile(new IntVector2(0, y));
			if (tile.IsEmpty()) enemyEntryTilesLeftSide.Add(tile);
		}

		for (int y = 0; y < size.y; y++) {
			Tile tile = GetTile(new IntVector2(size.x - 1, y));
			if (tile.IsEmpty()) enemyEntryTilesRightSide.Add(tile);
		}

		for (int x = 0; x < size.x; x++) {
			Tile tile = GetTile(new IntVector2(x, size.y - 1));
			if (tile.IsEmpty()) enemyEntryTilesTopSide.Add(tile);
		}
		
		for (int x = 0; x < size.x; x++) {
			Tile tile = GetTile(new IntVector2(x, 0));
			if (tile.IsEmpty()) enemyEntryTilesBottomSide.Add(tile);
		}

		if (enemyEntryTilesLeftSide.Count > 0) nextEnemyCoordsLeftSide = enemyEntryTilesLeftSide[Random.Range(0, enemyEntryTilesLeftSide.Count)].coordinates;
		else nextEnemyCoordsLeftSide = new IntVector2(-10, -10);

		if (enemyEntryTilesRightSide.Count > 0) nextEnemyCoordsRightSide = enemyEntryTilesRightSide[Random.Range(0, enemyEntryTilesRightSide.Count)].coordinates;
		else nextEnemyCoordsRightSide = new IntVector2(-10, -10);

		if (enemyEntryTilesTopSide.Count > 0) nextEnemyCoordsTopSide = enemyEntryTilesTopSide[Random.Range(0, enemyEntryTilesTopSide.Count)].coordinates;
		else nextEnemyCoordsTopSide = new IntVector2(-10, -10);

		if (enemyEntryTilesBottomSide.Count > 0) nextEnemyCoordsBottomSide = enemyEntryTilesBottomSide[Random.Range(0, enemyEntryTilesBottomSide.Count)].coordinates;
		else nextEnemyCoordsBottomSide = new IntVector2(-10, -10);

		enemyIndicatorLeftSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsLeftSide.x - 1, nextEnemyCoordsLeftSide.y));
		enemyIndicatorRightSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsRightSide.x + 1, nextEnemyCoordsRightSide.y));
		enemyIndicatorTopSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsTopSide.x, nextEnemyCoordsTopSide.y + 1));
		enemyIndicatorBottomSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsBottomSide.x, nextEnemyCoordsBottomSide.y - 1));
	}

	private void CreatePassage(Tile tile, Tile otherTile, BoardDirection direction) {
		TilePassage passage = Instantiate(passagePrefab) as TilePassage;
		passage.Initialize(tile, otherTile, direction);
		passage = Instantiate(passagePrefab) as TilePassage;
		passage.Initialize(otherTile, tile, direction.GetOpposite());
	}
	
	private void CreateWall(Tile tile, Tile otherTile, BoardDirection direction) {
		TileWall wall = Instantiate(wallPrefab) as TileWall;
		wall.Initialize(tile, otherTile, direction);
		if (otherTile != null) {
			wall = Instantiate(wallPrefab) as TileWall;
			wall.Initialize(otherTile, tile, direction.GetOpposite());
		}
	}
	
	private Tile CreateTile(IntVector2 coordinates) {
		Tile newTile = Instantiate(tilePrefab) as Tile;
		tiles[coordinates.x, coordinates.y] = newTile;
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.y;
		newTile.coordinates = coordinates;
		newTile.transform.parent = tileHolder.transform;
		newTile.transform.position = GetTilePosition(coordinates);
		
		return newTile;
	}

	public void AttemptToAddEnemy(BoardDirection direction) {
//		if (direction == BoardDirection.Up) AttemptToAddEnemyToRow(0);
//		else if (direction == BoardDirection.Down) AttemptToAddEnemyToRow(size.y - 1);
//		else if (direction == BoardDirection.Right) AttemptToAddEnemyToColumn(0);
//		else if (direction == BoardDirection.Left) AttemptToAddEnemyToColumn(size.x - 1);

		if (direction == BoardDirection.Up) AddEnemy(nextEnemyCoordsBottomSide);
		else if (direction == BoardDirection.Down) AddEnemy(nextEnemyCoordsTopSide);
		else if (direction == BoardDirection.Right) AddEnemy(nextEnemyCoordsLeftSide);
		else if (direction == BoardDirection.Left) AddEnemy(nextEnemyCoordsRightSide);

		UpdateNextEnemyPositions();
	}

	private void MoveTileEntities(Tile tile, BoardDirection direction) {
		TileEdge edge = tile.GetEdge(direction);
		if (edge is TileWall) return;
		
		List<TileEntity> tileEntitiesToMove = new List<TileEntity>();
		Tile newTile = edge.otherTile;
		
		foreach (TileEntity t in tile.tileEntities) {
			if (t.CanMoveToTile(newTile)) tileEntitiesToMove.Add(t);
		}
		
		foreach (TileEntity t in tileEntitiesToMove) {
			t.GoToTile(newTile, direction);
		}
	}

	private void AttemptToAddEnemyToRow(int row) {
		List<Tile> potentialTiles = new List<Tile>();

		for (int x = 0; x < size.x; x++) {
			Tile tile = GetTile(new IntVector2(x, row));
			if (tile.IsEmpty()) potentialTiles.Add(tile);
		}
		
		if (potentialTiles.Count > 0) {
			Tile tile = potentialTiles[Random.Range(0, potentialTiles.Count)];
			GameManager.instance.enemyManager.AddEnemy(tile);
		}
	}
	
	private void AttemptToAddEnemyToColumn(int column) {
		List<Tile> potentialTiles = new List<Tile>();

		for (int y = 0; y < size.y; y++) {
			Tile tile = GetTile(new IntVector2(column, y));
			if (tile.IsEmpty()) potentialTiles.Add(tile);
		}
		
		if (potentialTiles.Count > 0) {
			Tile tile = potentialTiles[Random.Range(0, potentialTiles.Count)];
			GameManager.instance.enemyManager.AddEnemy(tile);
		}
	}

	private void AddEnemy(IntVector2 coordinates) {
		if (!ContainsCoordinates(coordinates)) return;

		Tile tile = GetTile(coordinates);
		GameManager.instance.enemyManager.AddEnemy(tile);
	}
}
