using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MapEditorWormData {
	public BoardDirection direction = BoardDirection.NONE;
	public WormType wormType = WormType.NONE;

	public MapEditorWormData() {

	}
}
