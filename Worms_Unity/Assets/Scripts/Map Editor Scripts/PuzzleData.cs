using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class PuzzleData : ScriptableObject {
	[SerializeField]
	public string testString;

	[SerializeField]
	public IntVector2 size;

	[SerializeField]
	public MapEditorTileData[,] tiles;
}