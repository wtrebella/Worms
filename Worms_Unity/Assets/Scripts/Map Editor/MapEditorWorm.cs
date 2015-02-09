using UnityEngine;
using System.Collections;

public enum WormType {
	Worm1,
	Worm2,
	Worm3,
	Worm4,
	NONE
}

public class MapEditorWorm : MonoBehaviour {
	public BoardDirection direction {get; private set;}
	public WormType wormType {get; private set;}

	public void SetDirection(BoardDirection direction) {
		this.direction = direction;
		
		transform.eulerAngles = new Vector3(0, 0, (int)direction * -90);
	}

	public void SetWormType(WormType wormType) {
		this.wormType = wormType;

		tk2dSprite sprite = GetComponent<tk2dSprite>();
		if (wormType == WormType.Worm1) sprite.color = new Color(83f / 255f, 148f / 255f, 255f / 255f);
		else if (wormType == WormType.Worm2) sprite.color = new Color(234f / 255f, 35f / 255f, 63f / 255f);
		else if (wormType == WormType.Worm3) sprite.color = new Color(58f / 255f, 194f / 255f, 93f / 255f);
		else if (wormType == WormType.Worm4) sprite.color = new Color(141f / 255f, 90f / 255f, 194f / 255f);
	}

	public MapEditorWormData GetDataVersion() {
		MapEditorWormData data = new MapEditorWormData();
		data.direction = direction;
		data.wormType = wormType;
		return data;
	}
}
