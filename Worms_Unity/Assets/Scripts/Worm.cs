using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormBodyPart wormBodyPartNubPrefab;
	public WormHead wormHeadPartPrefab;
	public List<WormBodyPart> bodyParts;
	public WormHead head;

	private Color color;
	
	public void Initialize(Tile tile, BoardDirection direction, Color color) {
		this.color = color;
		transform.parent = Board.instance.transform;
		transform.localPosition = Vector3.zero;

		bodyParts = new List<WormBodyPart>();

		head = Instantiate(wormHeadPartPrefab) as WormHead;
		head.Initialize(this, tile, direction, color); 
	}

	public void PlaceBodyPart(Tile tile, BoardDirection direction) {
		WormBodyPart bodyPart;
		if (direction == BoardDirection.NONE) bodyPart = Instantiate(wormBodyPartNubPrefab) as WormBodyPart;
		else bodyPart = Instantiate(wormBodyPartPrefab) as WormBodyPart;
		bodyPart.Initialize(this, tile, direction, color);
		bodyPart.transform.parent = transform;
		bodyPart.transform.position = Board.instance.GetTilePosition(tile.coordinates);
		bodyParts.Add(bodyPart);
	}

	public void RemoveBodyPart(WormBodyPart bodyPart) {
		for (int i = 0; i < bodyParts.Count; i++) {
			if (bodyPart == bodyParts[i]) {
				RemoveBodyPart(i);
				break;
			}
		}
	}

	public void RemoveBodyPart(int index) {
		if (bodyParts.Count == 0 || index < 0 || index >= bodyParts.Count) return;

		WormBodyPart bodyPart = bodyParts[index];
		bodyPart.RemoveFromTile();
		bodyParts.RemoveAt(index);
		Destroy(bodyPart.gameObject);
	}
	
	public void HandleHeadMoved(Tile previousTile, Tile currentTile, BoardDirection direction) {
		if (previousTile != null) PlaceBodyPart(previousTile, direction);
		PlaceBodyPart(currentTile, direction.GetOpposite());
	}
}