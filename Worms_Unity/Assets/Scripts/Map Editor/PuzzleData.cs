using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PuzzleData : ScriptableObject {
	public IntVector2 size;

	public MapEditorTileData[] tiles;
}