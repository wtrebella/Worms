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

	public Tile[,] tiles;

	private GameObject tileHolder;

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
						if (Random.value > 0.035f) CreatePassage(tile, otherTile, direction);
						else CreateWall(tile, otherTile, direction);
					}
					else {
						CreateWall(tile, null, direction);
					}
				}
			}
		}
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

	public void AttemptToAddEnemy(EnemyManager enemyManager, BoardDirection direction) {
		if (direction == BoardDirection.Up) AttemptToAddEnemyToRow(enemyManager, 0);
		else if (direction == BoardDirection.Down) AttemptToAddEnemyToRow(enemyManager, size.y - 1);
		else if (direction == BoardDirection.Right) AttemptToAddEnemyToColumn(enemyManager, 0);
		else if (direction == BoardDirection.Left) AttemptToAddEnemyToColumn(enemyManager, size.x - 1);
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

	private void AttemptToAddEnemyToRow(EnemyManager enemyManager, int row) {
		List<Tile> potentialTiles = new List<Tile>();

		for (int x = 0; x < size.x; x++) {
			Tile tile = GetTile(new IntVector2(x, row));
			if (tile.IsEmpty()) potentialTiles.Add(tile);
		}
		
		if (potentialTiles.Count > 0) {
			Tile tile = potentialTiles[Random.Range(0, potentialTiles.Count)];
			enemyManager.AddEnemy(tile);
		}
	}
	
	private void AttemptToAddEnemyToColumn(EnemyManager enemyManager, int column) {
		List<Tile> potentialTiles = new List<Tile>();

		for (int y = 0; y < size.y; y++) {
			Tile tile = GetTile(new IntVector2(column, y));
			if (tile.IsEmpty()) potentialTiles.Add(tile);
		}
		
		if (potentialTiles.Count > 0) {
			Tile tile = potentialTiles[Random.Range(0, potentialTiles.Count)];
			enemyManager.AddEnemy(tile);
		}
	}
}
