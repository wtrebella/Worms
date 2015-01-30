using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHead wormHeadPartPrefab;

	private WormHead head;
	private List<WormBodyPart> bodyParts;

	public void Initialize(Tile tile, BoardDirection direction) {
		transform.parent = Board.instance.transform;
		transform.localPosition = Vector3.zero;

		bodyParts = new List<WormBodyPart>();

		head = Instantiate(wormHeadPartPrefab) as WormHead;
		head.Initialize(this, tile, direction); 
	}

	public void PlaceBodyPart(Tile tile, BoardDirection direction) {
		WormBodyPart bodyPart = Instantiate(wormBodyPartPrefab) as WormBodyPart;
		bodyPart.Initialize(this, tile, direction);
		bodyPart.transform.parent = transform;
		bodyPart.transform.position = Board.instance.GetTilePosition(tile.coordinates);
		bodyParts.Add(bodyPart);
	}
	
	public void HandleHeadMoved(Tile previousTile, Tile currentTile, BoardDirection direction) {
		if (previousTile != null) PlaceBodyPart(previousTile, direction);
		PlaceBodyPart(currentTile, direction.GetOpposite());
	}
}