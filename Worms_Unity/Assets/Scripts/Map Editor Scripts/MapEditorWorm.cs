using UnityEngine;
using System.Collections;

public enum WormType {
	Worm1,
	Worm2,
	Worm3,
	Worm4
}

public class MapEditorWorm : MonoBehaviour {
	public MapEditorDirection direction {get; private set;}
	public WormType wormType {get; private set;}

	public void SetDirection(MapEditorDirection direction) {
		this.direction = direction;
		
		transform.eulerAngles = new Vector3(0, 0, (int)direction * -90);
	}

	public void SetWormType(WormType wormType) {
		this.wormType = wormType;

		tk2dSprite sprite = GetComponent<tk2dSprite>();
		if (wormType == WormType.Worm1) sprite.color = Color.blue;
		else if (wormType == WormType.Worm2) sprite.color = Color.red;
		else if (wormType == WormType.Worm3) sprite.color = Color.green;
		else if (wormType == WormType.Worm4) sprite.color = Color.magenta;
	}

	public MapEditorWormData GetDataVersion() {
		MapEditorWormData data = new MapEditorWormData();
		data.direction = direction;
		data.wormType = wormType;
		return data;
	}
}
