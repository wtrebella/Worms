using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHead wormHeadPartPrefab;

	private WormHead head;
	private List<WormBodyPart> bodyParts;

	public void Initialize(IntVector2 startCoordinates, BoardDirection startDirection) {
		transform.parent = Board.instance.transform;
		transform.localPosition = Vector3.zero;

		bodyParts = new List<WormBodyPart>();

		head = Instantiate(wormHeadPartPrefab) as WormHead;
		head.Initialize(this, startCoordinates, startDirection); 
		head.transform.parent = transform;
		head.transform.position = Board.instance.GetTilePosition(startCoordinates);

		PlaceBodyPart(startCoordinates, startDirection.GetOpposite());
	}

	public void PlaceBodyPart(IntVector2 coordinates, BoardDirection direction) {
		WormBodyPart bodyPart = Instantiate(wormBodyPartPrefab) as WormBodyPart;
		bodyPart.Initialize(this, coordinates, direction);
		bodyPart.transform.parent = transform;
		bodyPart.transform.position = Board.instance.GetTilePosition(coordinates);
		bodyParts.Add(bodyPart);
	}
	
	public void ProposeMove(BoardDirection direction) {
		head.SetDirection(direction);

		TileEdge edge = head.currentTile.GetEdge(direction);
		Tile otherTile = null;
		if (edge is TilePassage) otherTile = edge.otherTile;
		
		if (otherTile != null) {
			head.ProposePosition(otherTile.coordinates);
		}
	}

	public void CommitMove() {
		IntVector2 prevCoords = head.coordinates;

		if (head.CommitPosition()) {
			PlaceBodyPart(prevCoords, head.direction);
			PlaceBodyPart(head.coordinates, head.direction.GetOpposite());
		}
	}
}