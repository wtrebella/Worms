using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {
	public static Board instance;

	public static int tileSize = 1;
	public static float lerpTime = 0.3f;

	public Tile tilePrefab;
	public Tile blockedTilePrefab;
	public TilePassage passagePrefab;
	public TileWall wallPrefab;
	public Worm wormPrefab;

	public IntVector2 size {get; private set;}
	public Tile[,] tiles {get; private set;}

	private GameObject tileHolder;


	void Awake() {
		instance = this;
	}

	void Start () {

	}
	
	void Update () {
	
	}

	public void Generate(PuzzleData puzzleData) {
		size = puzzleData.size;

		tileHolder = new GameObject("Tiles");
		tileHolder.transform.parent = transform;
		tileHolder.transform.localPosition = Vector3.zero;

		tiles = new Tile[size.x, size.y];
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				MapEditorTileData tileData = puzzleData.tiles[y * size.x + x];

				if (tileData.tileType == TileType.Blocked) tiles[x, y] = CreateBlockedTile(new IntVector2(x, y), tileHolder.transform);
				else if (tileData.tileType == TileType.Tile) tiles[x, y] = CreateTile(new IntVector2(x, y), tileHolder.transform);
			}
		}
		
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				Tile tile = GetTile(new IntVector2(x, y));
				if (tile.tileType == TileType.Blocked) continue;

				MapEditorTileData tileData = puzzleData.tiles[y * size.x + x];

				for (int i = 0; i < BoardDirections.Count; i++) {
					BoardDirection direction = (BoardDirection)i;

					if (tile.GetDirectionIsInitialized(direction)) continue;

					IntVector2 neighborCoordinates = tile.coordinates + direction.ToIntVector2();
					Tile neighborTile = null;
					
					if (ContainsCoordinates(neighborCoordinates)) neighborTile = GetTile(neighborCoordinates);
					
					MapEditorWallData wallData = null;
					foreach (MapEditorWallData w in tileData.walls) {
						if (w.direction == direction) {
							wallData = w;
							break;
						}
					}

					if (wallData != null) CreateWall(tile, neighborTile, direction);
					else if (neighborTile != null) CreatePassage(tile, neighborTile, direction);
				}

				if (tileData.worm != null && tileData.worm.wormType != WormType.NONE) {
					Color color = Color.white;

					if (tileData.worm.wormType == WormType.Worm1) color = new Color(30f / 255f, 167f / 255f, 225f / 255f);
					else if (tileData.worm.wormType == WormType.Worm2) color = new Color(188f / 255f, 67f / 255f, 89f / 255f);
					else if (tileData.worm.wormType == WormType.Worm3) color = new Color(69f / 255f, 186f / 255f, 106f / 255f);
					else if (tileData.worm.wormType == WormType.Worm4) color = new Color(123f / 255f, 69f / 255f, 186f / 255f);

					Worm worm = Instantiate(wormPrefab) as Worm;
					worm.Initialize(tile, tileData.worm.direction, color, tileData.worm.wormType);
				}
			}
		}
	}

	public void GenerateRandom() {
		tileHolder = new GameObject("Tiles");
		tileHolder.transform.parent = transform;
		tileHolder.transform.localPosition = Vector3.zero;

		tiles = new Tile[size.x, size.y];
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				if (x == 0 && y == size.y - 1) tiles[x, y] = CreateBlockedTile(new IntVector2(x, y), tileHolder.transform);
				else tiles[x, y] = CreateTile(new IntVector2(x, y), tileHolder.transform);
			}
		}

		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				Tile tile = GetTile(new IntVector2(x, y));
				if (tile.tileType == TileType.Blocked) continue;
				for (int i = 0; i < BoardDirections.Count; i++) {
					BoardDirection direction = (BoardDirection)i;
					if (tile.GetDirectionIsInitialized(direction)) continue;
						
					IntVector2 neighborCoordinates = tile.coordinates + direction.ToIntVector2();
					if (ContainsCoordinates(neighborCoordinates)) {
						Tile otherTile = GetTile(neighborCoordinates);
						if (otherTile.tileType == TileType.Blocked) CreateWall(tile, null, direction);
						else if (Random.value < 0.2f) CreateWall(tile, otherTile, direction);
						else CreatePassage(tile, otherTile, direction);
					}
					else {
						CreateWall(tile, null, direction);
					}
				}
			}
		}
	}

	public bool EntitiesAreMoving() {
		foreach (Tile tile in tiles) {
			foreach (TileEntity tileEntity in tile.tileEntities) {
				if (tileEntity.isAutoMoving || tileEntity.isMoving) return true;
			}
		}

		return false;
	}

	public void OnSwipeBegan(BoardDirection swipeDirection) {
		List<TileEntity> tileEntities = null;

		if (swipeDirection == BoardDirection.Up || swipeDirection == BoardDirection.Down) {
			for (int x = 0; x < size.x; x++) {
				tileEntities = GetMovableTileEntitiesInColumn(swipeDirection, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeBegan(swipeDirection);
			}
		}
		else if (swipeDirection == BoardDirection.Right || swipeDirection == BoardDirection.Left) {
			for (int y = 0; y < size.y; y++) {
				tileEntities = GetMovableTileEntitiesInRow(swipeDirection, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeBegan(swipeDirection);
			}
		}
	}
	
	public void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		List<TileEntity> tileEntities = null;
		
		if (swipeDirection == BoardDirection.Up || swipeDirection == BoardDirection.Down) {
			for (int x = 0; x < size.x; x++) {
				tileEntities = GetMovableTileEntitiesInColumn(swipeDirection, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeContinue(swipeDirection, swipePixelDistance);
			}
		}
		else if (swipeDirection == BoardDirection.Right || swipeDirection == BoardDirection.Left) {
			for (int y = 0; y < size.y; y++) {
				tileEntities = GetMovableTileEntitiesInRow(swipeDirection, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeContinue(swipeDirection, swipePixelDistance);
			}
		}
	}
	
	public void OnSwipeEnded(BoardDirection swipeDirection, float swipePixelDistance, bool ignoreDistance) {
		List<TileEntity> tileEntities = null;
		
		if (swipeDirection == BoardDirection.Up || swipeDirection == BoardDirection.Down) {
			for (int x = 0; x < size.x; x++) {
				tileEntities = GetMovableTileEntitiesInColumn(swipeDirection, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeEnded(swipePixelDistance, ignoreDistance);
			}
		}
		else if (swipeDirection == BoardDirection.Right || swipeDirection == BoardDirection.Left) {
			for (int y = 0; y < size.y; y++) {
				tileEntities = GetMovableTileEntitiesInRow(swipeDirection, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeEnded(swipePixelDistance, ignoreDistance);
			}
		}
	}
	
	public void OnSwipeCanceled(BoardDirection swipeDirection) {
		List<TileEntity> tileEntities = null;
		
		if (swipeDirection == BoardDirection.Up || swipeDirection == BoardDirection.Down) {
			for (int x = 0; x < size.x; x++) {
				tileEntities = GetMovableTileEntitiesInColumn(swipeDirection, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeCanceled();
			}
		}
		else if (swipeDirection == BoardDirection.Right || swipeDirection == BoardDirection.Left) {
			for (int y = 0; y < size.y; y++) {
				tileEntities = GetMovableTileEntitiesInRow(swipeDirection, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.OnSwipeCanceled();
			}
		}
	}

	public bool CheckWinConditions() {
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				Tile tile = GetTile(new IntVector2(x, y));
				if (!tile.IsCompleted()) return false;
			}
		}

		return true;
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

	public List<TileEntity> GetMovableTileEntitiesInRow(BoardDirection direction, int row) {
		if (direction == BoardDirection.Down || direction == BoardDirection.Up || direction == BoardDirection.NONE) Debug.LogError("can't use direction " + direction.ToString() + " in row");

		List<TileEntity> movableEntities = new List<TileEntity>();
		List<TileEntity> previousTileEntities = new List<TileEntity>();
		List<TileEntity> tempPreviousTileEntities = new List<TileEntity>();

		if (direction == BoardDirection.Right) {
			for (int i = size.x - 1; i >= 0; i--) {
				Tile tile = GetTile(new IntVector2(i, row));
				Tile otherTile = GetTile(tile.coordinates + direction.ToIntVector2());

				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;

				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						bool canMove = !isWall && otherTile != null && otherTile.tileType != TileType.Blocked && TileEntity.TileEntityTypeCanEnterTileWithTileEntities(tileEntity.tileEntityType, previousTileEntities);
						if (canMove) movableEntities.Add(tileEntity);
						else tempPreviousTileEntities.Add(tileEntity);
					}
				}

				previousTileEntities.Clear();
				foreach (TileEntity t in tempPreviousTileEntities) previousTileEntities.Add(t);
				tempPreviousTileEntities.Clear();
			}
		}

		else if (direction == BoardDirection.Left) {
			for (int i = 0; i < size.x; i++) {
				Tile tile = GetTile(new IntVector2(i, row));
				Tile otherTile = GetTile(tile.coordinates + direction.ToIntVector2());

				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;
				
				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						bool canMove = !isWall && otherTile != null && otherTile.tileType != TileType.Blocked && TileEntity.TileEntityTypeCanEnterTileWithTileEntities(tileEntity.tileEntityType, previousTileEntities);
						if (canMove) movableEntities.Add(tileEntity);
						else tempPreviousTileEntities.Add(tileEntity);
					}
				}
				
				previousTileEntities.Clear();
				foreach (TileEntity t in tempPreviousTileEntities) previousTileEntities.Add(t);
				tempPreviousTileEntities.Clear();
			}
		}

		return movableEntities;
	}

	public List<TileEntity> GetMovableTileEntitiesInColumn(BoardDirection direction, int column) {
		if (direction == BoardDirection.Right || direction == BoardDirection.Left || direction == BoardDirection.NONE) Debug.LogError("can't use direction " + direction.ToString() + " in column");

		List<TileEntity> movableEntities = new List<TileEntity>();
		List<TileEntity> previousTileEntities = new List<TileEntity>();
		List<TileEntity> tempPreviousTileEntities = new List<TileEntity>();

		if (direction == BoardDirection.Up) {
			for (int i = size.y - 1; i >= 0; i--) {
				Tile tile = GetTile(new IntVector2(column, i));
				Tile otherTile = GetTile(tile.coordinates + direction.ToIntVector2());

				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;
				
				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						bool canMove = !isWall && otherTile != null && otherTile.tileType != TileType.Blocked && TileEntity.TileEntityTypeCanEnterTileWithTileEntities(tileEntity.tileEntityType, previousTileEntities);
						if (canMove) movableEntities.Add(tileEntity);
						else tempPreviousTileEntities.Add(tileEntity);
					}
				}
				
				previousTileEntities.Clear();
				foreach (TileEntity t in tempPreviousTileEntities) previousTileEntities.Add(t);
				tempPreviousTileEntities.Clear();
			}
		}
		
		else if (direction == BoardDirection.Down) {
			for (int i = 0; i < size.y; i++) {
				Tile tile = GetTile(new IntVector2(column, i));
				Tile otherTile = GetTile(tile.coordinates + direction.ToIntVector2());

				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;
				
				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						bool canMove = !isWall && otherTile != null && otherTile.tileType != TileType.Blocked && TileEntity.TileEntityTypeCanEnterTileWithTileEntities(tileEntity.tileEntityType, previousTileEntities);
						if (canMove) movableEntities.Add(tileEntity);
						else tempPreviousTileEntities.Add(tileEntity);
					}
				}
				
				previousTileEntities.Clear();
				foreach (TileEntity t in tempPreviousTileEntities) previousTileEntities.Add(t);
				tempPreviousTileEntities.Clear();
			}
		}
		
		return movableEntities;
	}

	public bool TileEntityCanMove(TileEntity entity, BoardDirection direction) {
		List<TileEntity> movables = null;

		if (direction == BoardDirection.Left || direction == BoardDirection.Right) movables = GetMovableTileEntitiesInRow(direction, entity.currentTile.coordinates.y);
		else if (direction == BoardDirection.Up || direction == BoardDirection.Down) movables = GetMovableTileEntitiesInColumn(direction, entity.currentTile.coordinates.x);
		else Debug.LogError("no direction!");

		return movables.Contains(entity);
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
		if (otherTile != null && otherTile.tileType != TileType.Blocked) {
			wall = Instantiate(wallPrefab) as TileWall;
			wall.Initialize(otherTile, tile, direction.GetOpposite());
		}
	}
	
	private Tile CreateTile(IntVector2 coordinates, Transform newParent) {
		Tile newTile = Instantiate(tilePrefab) as Tile;
		newTile.Initialize();
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.y;
		newTile.coordinates = coordinates;
		newTile.transform.parent = newParent;
		Vector3 pos = GetTilePosition(coordinates);
		newTile.transform.position = pos;
		newTile.InitializeSprite();
		
		return newTile;
	}

	private Tile CreateBlockedTile(IntVector2 coordinates, Transform newParent) {
		Tile newTile = Instantiate(blockedTilePrefab) as Tile;
		newTile.Initialize();
		newTile.name = "Blocked Tile " + coordinates.x + ", " + coordinates.y;
		newTile.coordinates = coordinates;
		newTile.transform.parent = newParent;
		Vector3 pos = GetTilePosition(coordinates);
		newTile.transform.position = pos;
		newTile.InitializeSprite();
		
		return newTile;
	}
}
