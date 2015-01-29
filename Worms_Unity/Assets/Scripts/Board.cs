using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {
	public static Board instance;

	public int tileSize = 100;
	public IntVector2 size;

	public Tile tilePrefab;
	public TilePassage passagePrefab;
	public TileWall wallPrefab;

	public Tile[,] tiles;
	public int[,] tileBitmasks;
	public int[,] tempTileBitmasks;

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

		tileBitmasks = new int[size.x, size.y];
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				tileBitmasks[x,y] = 0;
			}
		}

		tempTileBitmasks = new int[size.x, size.y];
		ResetTempTileBitmasks();
	}

	public void ResetTempTileBitmasks() {
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				tempTileBitmasks[x,y] = tileBitmasks[x,y];
			}
		}
	}

	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.y));
		}
	}

	public void AddObject(int[,] bitmaskArray, IntVector2 coordinates, ObjectType objectType) {
		if (!ContainsCoordinates(coordinates)) return;

		int bitmask = bitmaskArray[coordinates.x, coordinates.y];
		bitmask |= (int)objectType;
		bitmaskArray[coordinates.x, coordinates.y] = bitmask;
	}

	public void RemoveObject(int[,] bitmaskArray, IntVector2 coordinates, ObjectType objectType) {
		if (!ContainsCoordinates(coordinates)) return;

		int bitmask = bitmaskArray[coordinates.x, coordinates.y];
		bitmask &= ~((int)objectType);
		bitmaskArray[coordinates.x, coordinates.y] = bitmask;
	}

	public int GetTileBitmask(int[,] bitmaskArray, IntVector2 coordinates) {
		if (!ContainsCoordinates(coordinates)) return 0;

		return bitmaskArray[coordinates.x, coordinates.y];
	}

	public bool GetTileIsOccupied(int[,] bitmaskArray, IntVector2 coordinates) {
		return GetTileBitmask(bitmaskArray, coordinates) != 0;
	}

	public bool GetTileContains(int[,] bitmaskArray, IntVector2 coordinates, ObjectType objectType) {
		int bitmask = GetTileBitmask(bitmaskArray, coordinates);
		return (bitmask & (int)objectType) == (int)objectType;
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
}
