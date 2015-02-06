using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public enum State {
	Tile,
	Wall,
	Worm
}

public class MapCreator : MonoBehaviour {
	public Camera cam;

	public Button switchStateButton;

	public MapEditorTile tilePrefab;
	public MapEditorWall wallPrefab;
	public MapEditorWorm wormPrefab;

	public int tileSize = 100;
	public IntVector2 size;
	public MapEditorTile[,] tiles;

	private static MapCreator instance;
	private List<MapEditorWorm> worms;
	private State state = State.Tile;

	void Start () {
		instance = this;
		Initialize();
	}
	
	void Initialize() {
		worms = new List<MapEditorWorm>();
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

		switchStateButton.GetComponentInChildren<Text>().text = state.ToString();
	}

	[UnityEditor.MenuItem( "Puzzle/Create New Puzzle", false, 10 )]
	public static void ExportData() {
		PuzzleData asset = ScriptableObject.CreateInstance<PuzzleData> ();
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets/Puzzles";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(PuzzleData).ToString() + ".asset");

		AssetDatabase.CreateAsset (asset, assetPathAndName);
		AssetDatabase.RenameAsset(assetPathAndName, "Puzzle");

		asset.size = instance.size;
		asset.tiles = new MapEditorTileData[asset.size.x * asset.size.y];
		for (int x = 0; x < asset.size.x; x++) {
			for (int y = 0; y < asset.size.y; y++) {
				asset.tiles[y * asset.size.x + x] = instance.tiles[x, y].GetDataVersion();
			}
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
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
		state = (State)(((int)state + 1) % 3);

		switchStateButton.GetComponentInChildren<Text>().text = state.ToString();
	}

	void UpdateWormTypes() {
		for (int i = 0; i < worms.Count; i++) {
			MapEditorWorm worm = worms[i];
			worm.SetWormType((WormType)i);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Q)) {
			SwitchState();
			return;
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
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
				MapEditorTileType newTileType = (MapEditorTileType)(((int)clickedTile.tileType + 1) % 2);
				clickedTile.SetTileType(newTileType);
			}
		}

		else if (state == State.Wall) {
			if (clickedTile) {
				MapEditorDirection direction = MapEditorDirection.NONE;

				if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direction = MapEditorDirection.Up;
				else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) direction = MapEditorDirection.Right;
				else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direction = MapEditorDirection.Down;
				else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direction = MapEditorDirection.Left;

				if (direction != MapEditorDirection.NONE) {
					MapEditorWall wall = clickedTile.GetWall(direction);
					if (wall) {
						clickedTile.walls.Remove(wall);
						Destroy(wall.gameObject);
					}
					else {
						wall = Instantiate(wallPrefab) as MapEditorWall;
						wall.SetDirection(direction);
						wall.transform.parent = clickedTile.transform;
						wall.transform.localPosition = Vector3.zero;
						clickedTile.walls.Add(wall);
					}
				}
			}
		}

		else if (state == State.Worm) {
			if (clickedTile) {
				MapEditorWorm worm = clickedTile.worm;
				if (worm) {
					MapEditorDirection newDirection = (MapEditorDirection)(((int)worm.direction + 1) % 5);
					if (newDirection == MapEditorDirection.NONE) {
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
					if (worms.Count < 4) {
						worm = Instantiate(wormPrefab) as MapEditorWorm;
						worm.SetDirection(MapEditorDirection.Up);
						worm.SetWormType((WormType)worms.Count);
						worm.transform.parent = clickedTile.transform;
						worm.transform.localPosition = Vector3.zero;
						clickedTile.worm = worm;

						worms.Add(worm);
					}
				}
			}
		}
	}
}
