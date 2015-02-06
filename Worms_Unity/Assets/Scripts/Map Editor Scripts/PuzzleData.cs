using UnityEngine;
using System.Collections;

public class PuzzleData : ScriptableObject {
	public int tileSize;
	public IntVector2 size;
	public MapEditorTileData[,] tiles;
}
