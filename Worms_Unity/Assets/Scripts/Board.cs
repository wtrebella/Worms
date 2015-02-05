﻿using UnityEngine;
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
	private GameObject enemyIndicatorHolder;

	private BoardDirection lastDirection = BoardDirection.NONE;

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

		enemyIndicatorHolder = new GameObject("Enemy Indicators");
		enemyIndicatorHolder.transform.parent = transform;
		enemyIndicatorHolder.transform.localPosition = Vector3.zero;

		tiles = new Tile[size.x, size.y];
		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				tiles[x, y] = CreateTile(new IntVector2(x, y), tileHolder.transform, true);
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
		List<TileEntity> tileEntities = null;

		if (direction == BoardDirection.Up) {
			for (int x = 0; x < size.x; x++) {
				tileEntities = GetMovableTileEntitiesInColumn(direction, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.Move(direction);
			}
		}
		else if (direction == BoardDirection.Down) {
			for (int x = 0; x < size.x; x++) {
				tileEntities = GetMovableTileEntitiesInColumn(direction, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.Move(direction);
			}
		}
		else if (direction == BoardDirection.Right) {
			for (int y = 0; y < size.y; y++) {
				tileEntities = GetMovableTileEntitiesInRow(direction, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.Move(direction);
			}
		}
		else if (direction == BoardDirection.Left) {
			for (int y = 0; y < size.y; y++) {
				tileEntities = GetMovableTileEntitiesInRow(direction, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.Move(direction);
			}
		}

		lastDirection = direction;
	}

	public List<TileEntity> GetMovableTileEntitiesInRow(BoardDirection direction, int row) {
		if (direction == BoardDirection.Down || direction == BoardDirection.Up || direction == BoardDirection.NONE) Debug.LogError("can't use direction " + direction.ToString() + " in row");

		List<TileEntity> movableEntities = new List<TileEntity>();
		List<TileEntity> previousTileEntities = new List<TileEntity>();
		List<TileEntity> tempPreviousTileEntities = new List<TileEntity>();

		if (direction == BoardDirection.Right) {
			for (int i = size.x - 1; i >= 0; i--) {
				Tile tile = GetTile(new IntVector2(i, row));
				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;

				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						if (!isWall && tileEntity.CanEnterTileWithTileEntities(previousTileEntities)) movableEntities.Add(tileEntity);
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
				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;
				
				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						if (!isWall && tileEntity.CanEnterTileWithTileEntities(previousTileEntities)) movableEntities.Add(tileEntity);
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
				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;
				
				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						if (!isWall && tileEntity.CanEnterTileWithTileEntities(previousTileEntities)) movableEntities.Add(tileEntity);
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
				TileEdge edge = tile.GetEdge(direction);
				bool isWall = edge is TileWall;
				
				if (tile.tileEntities.Count > 0) {
					foreach (TileEntity tileEntity in tile.tileEntities) {
						if (!isWall && tileEntity.CanEnterTileWithTileEntities(previousTileEntities)) movableEntities.Add(tileEntity);
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

	public void ValidateNextEnemyPositions() {
//		Tile leftTile = GetTile(nextEnemyCoordsLeftSide);
//		if (leftTile != null && !leftTile.IsEmpty()) UpdateNextEnemyPositions(BoardDirection.Right);
//
//		Tile rightTile = GetTile(nextEnemyCoordsRightSide);
//		if (rightTile != null && !rightTile.IsEmpty()) UpdateNextEnemyPositions(BoardDirection.Left);
//
//		Tile topTile = GetTile(nextEnemyCoordsTopSide);
//		if (topTile != null && !topTile.IsEmpty()) UpdateNextEnemyPositions(BoardDirection.Down);
//
//		Tile bottomTile = GetTile(nextEnemyCoordsBottomSide);
//		if (bottomTile != null && !bottomTile.IsEmpty()) UpdateNextEnemyPositions(BoardDirection.Up);
	}
	
	public void UpdateNextEnemyPositions(BoardDirection direction) {
		if (direction == BoardDirection.Right) {
			if (enemyIndicatorLeftSide == null) {
				enemyIndicatorLeftSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
				enemyIndicatorLeftSide.transform.parent = enemyIndicatorHolder.transform;
			}
			if (enemyEntryTilesLeftSide == null) enemyEntryTilesLeftSide = new List<Tile>();

			enemyEntryTilesLeftSide.Clear();

			for (int y = 0; y < size.y; y++) {
				Tile tile = GetTile(new IntVector2(0, y));
//				if (tile.IsEmpty()) enemyEntryTilesLeftSide.Add(tile);
			}

			if (enemyEntryTilesLeftSide.Count > 0) nextEnemyCoordsLeftSide = enemyEntryTilesLeftSide[Random.Range(0, enemyEntryTilesLeftSide.Count)].coordinates;
			else nextEnemyCoordsLeftSide = new IntVector2(-10, -10);

			enemyIndicatorLeftSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsLeftSide.x - 1, nextEnemyCoordsLeftSide.y));
		}

		else if (direction == BoardDirection.Left) {
			if (enemyIndicatorRightSide == null) {
				enemyIndicatorRightSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
				enemyIndicatorRightSide.transform.parent = enemyIndicatorHolder.transform;
			}
			if (enemyEntryTilesRightSide == null) enemyEntryTilesRightSide = new List<Tile>();

			enemyEntryTilesRightSide.Clear();

			for (int y = 0; y < size.y; y++) {
				Tile tile = GetTile(new IntVector2(size.x - 1, y));
//				if (tile.IsEmpty()) enemyEntryTilesRightSide.Add(tile);
			}

			if (enemyEntryTilesRightSide.Count > 0) nextEnemyCoordsRightSide = enemyEntryTilesRightSide[Random.Range(0, enemyEntryTilesRightSide.Count)].coordinates;
			else nextEnemyCoordsRightSide = new IntVector2(-10, -10);

			enemyIndicatorRightSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsRightSide.x + 1, nextEnemyCoordsRightSide.y));
		}

		else if (direction == BoardDirection.Down) {
			if (enemyIndicatorTopSide == null) {
				enemyIndicatorTopSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
				enemyIndicatorTopSide.transform.parent = enemyIndicatorHolder.transform;
			}
			if (enemyEntryTilesTopSide == null) enemyEntryTilesTopSide = new List<Tile>();

			enemyEntryTilesTopSide.Clear();

			for (int x = 0; x < size.x; x++) {
				Tile tile = GetTile(new IntVector2(x, size.y - 1));
//				if (tile.IsEmpty()) enemyEntryTilesTopSide.Add(tile);
			}

			if (enemyEntryTilesTopSide.Count > 0) nextEnemyCoordsTopSide = enemyEntryTilesTopSide[Random.Range(0, enemyEntryTilesTopSide.Count)].coordinates;
			else nextEnemyCoordsTopSide = new IntVector2(-10, -10);

			enemyIndicatorTopSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsTopSide.x, nextEnemyCoordsTopSide.y + 1));
		}

		else if (direction == BoardDirection.Up) {
			if (enemyIndicatorBottomSide == null) {
				enemyIndicatorBottomSide = Instantiate(enemyIndicatorPrefab) as tk2dSprite;
				enemyIndicatorBottomSide.transform.parent = enemyIndicatorHolder.transform;
			}
			if (enemyEntryTilesBottomSide == null) enemyEntryTilesBottomSide = new List<Tile>();
			
			enemyEntryTilesBottomSide.Clear();

			for (int x = 0; x < size.x; x++) {
				Tile tile = GetTile(new IntVector2(x, 0));
//				if (tile.IsEmpty()) enemyEntryTilesBottomSide.Add(tile);
			}

			if (enemyEntryTilesBottomSide.Count > 0) nextEnemyCoordsBottomSide = enemyEntryTilesBottomSide[Random.Range(0, enemyEntryTilesBottomSide.Count)].coordinates;
			else nextEnemyCoordsBottomSide = new IntVector2(-10, -10);
			
			enemyIndicatorBottomSide.transform.position = GetTilePosition(new IntVector2(nextEnemyCoordsBottomSide.x, nextEnemyCoordsBottomSide.y - 1));
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
	
	private Tile CreateTile(IntVector2 coordinates, Transform newParent, bool withSprite) {
		Tile newTile = Instantiate(tilePrefab) as Tile;
		newTile.Initialize();
		newTile.name = "Tile " + coordinates.x + ", " + coordinates.y;
		newTile.coordinates = coordinates;
		newTile.transform.parent = newParent;
		newTile.transform.position = GetTilePosition(coordinates);
		if (withSprite) newTile.InitializeSprite();
		
		return newTile;
	}

	public void AttemptToAddEnemy(BoardDirection direction) {
		if (direction == BoardDirection.Up) AddEnemy(nextEnemyCoordsBottomSide);
		else if (direction == BoardDirection.Down) AddEnemy(nextEnemyCoordsTopSide);
		else if (direction == BoardDirection.Right) AddEnemy(nextEnemyCoordsLeftSide);
		else if (direction == BoardDirection.Left) AddEnemy(nextEnemyCoordsRightSide);

		UpdateNextEnemyPositions(lastDirection);
	}

	private void OnDestroy() {
		if (enemyIndicatorLeftSide) Destroy(enemyIndicatorLeftSide.gameObject);
		if (enemyIndicatorRightSide) Destroy(enemyIndicatorRightSide.gameObject);
		if (enemyIndicatorTopSide) Destroy(enemyIndicatorTopSide.gameObject);
		if (enemyIndicatorBottomSide) Destroy(enemyIndicatorBottomSide.gameObject);
	}

	public void AddEnemy(IntVector2 coordinates) {
		if (!ContainsCoordinates(coordinates)) return;

		Tile tile = GetTile(coordinates);
		GameManager.instance.enemyManager.AddEnemy(tile);
	}
}
