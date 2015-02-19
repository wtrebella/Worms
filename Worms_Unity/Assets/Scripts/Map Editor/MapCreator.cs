#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public enum State {
	Tile,
	Wall,
	Worm,
	Peg,
	DeathTrap,
	MAX
}

public class MapCreator : MonoBehaviour {
	public Camera cam;

	public PuzzleData puzzleToLoad;

	public Button switchStateButton;
	public InputField inputField;

	public MapEditorTile tilePrefab;
	public MapEditorWall wallPrefab;
	public MapEditorWorm wormPrefab;
	public MapEditorGenericTileEntity pegPrefab;
	public MapEditorGenericTileEntity deathTrapPrefab;

	public int tileSize = 100;
	public IntVector2 size;
	public MapEditorTile[,] tiles;

	private static MapCreator instance;
	private List<MapEditorWorm> worms;
	private List<MapEditorGenericTileEntity> tileEntities;
	private State state = State.Tile;

	void Start () {
		instance = this;
		Initialize();
	}
	
	void Initialize() {
		worms = new List<MapEditorWorm>();
		tileEntities = new List<MapEditorGenericTileEntity>();

		switchStateButton.GetComponentInChildren<Text>().text = state.ToString();

		if (puzzleToLoad != null) Load();
		else {
			tiles = new MapEditorTile[size.x, size.y];

			for (int x = 0; x < size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					MapEditorTile tile = Instantiate(tilePrefab) as MapEditorTile;
					tile.transform.parent = transform;
					tile.coordinates = new IntVector2(x, y);
					tile.transform.position = GetPosition(tile.coordinates);
					tiles[x, y] = tile;
				}
			}

			for (int x = 0; x < size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					SetTileType(GetTile(new IntVector2(x, y)), TileType.Tile);
				}
			}
		}

	}

	void Load() {
		size = puzzleToLoad.size;
		tiles = new MapEditorTile[size.x, size.y];

		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				MapEditorTile tile = Instantiate(tilePrefab) as MapEditorTile;
				tile.transform.parent = transform;
				tile.coordinates = new IntVector2(x, y);
				tile.transform.position = GetPosition(tile.coordinates);
				tiles[x, y] = tile;
			}
		}

		for (int x = 0; x < size.x; x++) {
			for (int y = 0; y < size.y; y++) {
				MapEditorTileData tileData = puzzleToLoad.tiles[y * size.x + x];
				MapEditorTile tile = GetTile(new IntVector2(x, y));

				tile.SetTileType(tileData.tileType);

				foreach (MapEditorWallData wallData in tileData.walls) CreateWall(tile, wallData.direction, false);
				if (tileData.worm != null && tileData.worm.wormType != WormType.NONE) CreateWorm(tile, tileData.worm.direction, tileData.worm.wormType);
				if (tileData.tileEntity != null && tileData.tileEntity.tileEntityType != TileEntityType.NONE) CreateTileEntity(tile, tileData.tileEntity.tileEntityType);
			}
		}
	}

	public static void ExportData() {
		PuzzleData asset;

		if (instance.puzzleToLoad != null) asset = instance.puzzleToLoad;
		else {
			asset = ScriptableObject.CreateInstance<PuzzleData>();
		
			string path = "Assets/Resources/Puzzles";

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(PuzzleData).ToString() + ".asset");

			AssetDatabase.CreateAsset (asset, assetPathAndName);
			AssetDatabase.RenameAsset(assetPathAndName, instance.inputField.text);
		}

		asset.size = instance.size;
		asset.tiles = new MapEditorTileData[asset.size.x * asset.size.y];
		for (int x = 0; x < asset.size.x; x++) {
			for (int y = 0; y < asset.size.y; y++) {
				asset.tiles[y * asset.size.x + x] = instance.GetTile(new IntVector2(x, y)).GetDataVersion();
			}
		}

		EditorUtility.SetDirty(asset);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	Vector3 GetPosition(IntVector2 coordinates) {
		Vector3 pos = new Vector3();
		pos.x = (coordinates.x + 0.5f - size.x / 2f) * tileSize;
		pos.y = (coordinates.y + 0.5f - size.y / 2f) * tileSize;
		pos.z = 0;
		return pos;
	}

	public void SwitchState() {
		state = (State)(((int)state + 1) % ((int)State.MAX));

		switchStateButton.GetComponentInChildren<Text>().text = state.ToString();
	}

	void UpdateWormTypes() {
		for (int i = 0; i < worms.Count; i++) {
			MapEditorWorm worm = worms[i];
			worm.SetWormType((WormType)i);
		}
	}

	private MapEditorTile GetTile(IntVector2 coordinates) {
		if (!ContainsCoordinates(coordinates)) return null;

		return tiles[coordinates.x, coordinates.y];
	}

	void CreateWall(MapEditorTile tile, BoardDirection direction, bool removeIfDuplicate) {
		MapEditorWall wall;

		IntVector2 neighborCoordinates = tile.coordinates + direction.ToIntVector2();
		MapEditorTile neighborTile = GetTile(neighborCoordinates);

		if (neighborTile != null && neighborTile.tileType != TileType.Blocked) {
			wall = neighborTile.GetWall(direction.GetOpposite());

			if (wall) {
				if (removeIfDuplicate) {
					neighborTile.walls.Remove(wall);
					Destroy(wall.gameObject);
				}
			}
			else {
				wall = Instantiate(wallPrefab) as MapEditorWall;
				wall.SetDirection(direction.GetOpposite());
				wall.transform.parent = neighborTile.transform;
				wall.transform.localPosition = Vector3.zero;
				neighborTile.walls.Add(wall);
			}
		}

		wall = tile.GetWall(direction);

		if (wall) {
			if (removeIfDuplicate) {
				tile.walls.Remove(wall);
				Destroy(wall.gameObject);
			}
		}
		else {
			wall = Instantiate(wallPrefab) as MapEditorWall;
			wall.SetDirection(direction);
			wall.transform.parent = tile.transform;
			wall.transform.localPosition = Vector3.zero;
			tile.walls.Add(wall);
		}
	}

	void RemoveWall(MapEditorTile tile, BoardDirection direction) {
		MapEditorWall wall;
		
		IntVector2 neighborCoordinates = tile.coordinates + direction.ToIntVector2();
		MapEditorTile neighborTile = GetTile(neighborCoordinates);
		
		if (neighborTile != null) {
			wall = neighborTile.GetWall(direction.GetOpposite());
			
			if (wall) {
				neighborTile.walls.Remove(wall);
				Destroy(wall.gameObject);
			}
		}
		
		wall = tile.GetWall(direction);
		
		if (wall) {
			tile.walls.Remove(wall);
			Destroy(wall.gameObject);
		}
	}

	void CreateWorm(MapEditorTile tile, BoardDirection direction, WormType type) {
		MapEditorWorm worm = Instantiate(wormPrefab) as MapEditorWorm;
		worm.SetDirection(direction);
		worm.SetWormType(type);
		worm.transform.parent = tile.transform;
		worm.transform.localPosition = Vector3.zero;
		tile.worm = worm;
		
		worms.Add(worm);
	}

	void CreateTileEntity(MapEditorTile tile, TileEntityType tileEntityType) {
		MapEditorGenericTileEntity prefab = null;
		if (tileEntityType == TileEntityType.Peg) prefab = pegPrefab;
		else if (tileEntityType == TileEntityType.DeathTrap) prefab = deathTrapPrefab;

		MapEditorGenericTileEntity tileEntity = Instantiate(prefab) as MapEditorGenericTileEntity;
		tileEntity.SetTileEntityType(tileEntityType);
		tileEntity.transform.parent = tile.transform;
		tileEntity.transform.localPosition = Vector3.zero;
		tile.tileEntity = tileEntity;
		
		tileEntities.Add(tileEntity);
	}

	public bool ContainsCoordinates(IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.y >= 0 && coordinate.y < size.y;
	}

	void SetTileType(MapEditorTile tile, TileType tileType) {
		TileType previousTileType = tile.tileType;

		tile.SetTileType(tileType);

		if (tileType == TileType.Blocked) {
			for (int i = 0; i < BoardDirections.Count; i++) {
				BoardDirection direction = (BoardDirection)i;

				RemoveWall(tile, direction);

				MapEditorTile neighborTile = GetTile(tile.coordinates + direction.ToIntVector2());
				if (neighborTile != null && neighborTile.tileType != TileType.Blocked) {
					CreateWall(neighborTile, direction.GetOpposite(), false);
				}
			}
		}
		else if (tileType == TileType.Tile) {
			for (int i = 0; i < BoardDirections.Count; i++) {
				BoardDirection direction = (BoardDirection)i;
					
				MapEditorTile neighborTile = GetTile(tile.coordinates + direction.ToIntVector2());
				if (neighborTile == null || neighborTile.tileType == TileType.Blocked) CreateWall(tile, direction, false);
				else if (previousTileType == TileType.Blocked) RemoveWall(neighborTile, direction.GetOpposite());
			}
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Q)) {
			SwitchState();
			return;
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			ExportData();
			return;
		}

		MapEditorTile clickedTile = null;

		if (Input.GetMouseButtonDown(0)) {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast(ray, out hit);
			if (hit.collider != null) {
				clickedTile = hit.collider.GetComponent<MapEditorTile>();
			}
		}

		if (state == State.Tile) {
			if (clickedTile) {
				TileType newTileType = (TileType)(((int)clickedTile.tileType + 1) % 2);
				SetTileType(clickedTile, newTileType);
			}
		}

		else if (state == State.Peg) {
			if (clickedTile) {
				MapEditorGenericTileEntity peg = clickedTile.tileEntity;
				if (peg != null && peg.tileEntityType == TileEntityType.Peg) {
					clickedTile.tileEntity = null;
					tileEntities.Remove(peg);
					Destroy(peg.gameObject);
				}
				else {
					CreateTileEntity(clickedTile, TileEntityType.Peg);
				}
			}
		}

		else if (state == State.DeathTrap) {
			if (clickedTile) {
				MapEditorGenericTileEntity deathTrap = clickedTile.tileEntity;
				if (deathTrap != null && deathTrap.tileEntityType == TileEntityType.DeathTrap) {
					clickedTile.tileEntity = null;
					tileEntities.Remove(deathTrap);
					Destroy(deathTrap.gameObject);
				}
				else {
					CreateTileEntity(clickedTile, TileEntityType.DeathTrap);
				}
			}
		}

		else if (state == State.Wall) {
			if (clickedTile) {
				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) CreateWall(clickedTile, BoardDirection.Up, true);
				if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) CreateWall(clickedTile, BoardDirection.Right, true);
				if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) CreateWall(clickedTile, BoardDirection.Down, true);
				if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) CreateWall(clickedTile, BoardDirection.Left, true);
			}
		}

		else if (state == State.Worm) {
			if (clickedTile) {
				MapEditorWorm worm = clickedTile.worm;
				if (worm) {
					BoardDirection newDirection = (BoardDirection)(((int)worm.direction + 1) % 5);
					if (newDirection == BoardDirection.NONE) {
						clickedTile.worm = null;
						worms.Remove(worm);
						Destroy(worm.gameObject);
						UpdateWormTypes();
					}
					else {
						worm.SetDirection(newDirection);
					}
				}
				else {
					if (worms.Count < 4) CreateWorm(clickedTile, BoardDirection.Up, (WormType)worms.Count);
				}
			}
		}
	}
}
#endif
