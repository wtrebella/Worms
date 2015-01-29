using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHead wormHeadPartPrefab;

	private WormHead head;
	private List<WormBodyPart> bodyParts;

	public void Initialize(Board board, IntVector2 startCoordinates) {
		bodyParts = new List<WormBodyPart>();

		head = Instantiate(wormHeadPartPrefab) as WormHead;
		head.Initialize(this, startCoordinates, BoardDirection.Up); 
		head.transform.parent = transform;
		head.transform.position = Board.instance.GetTilePosition(startCoordinates);

		PlaceBodyPart(startCoordinates, BoardDirection.Down);
	}

	public void PlaceBodyPart(IntVector2 coordinates, BoardDirection direction) {
		WormBodyPart bodyPart = Instantiate(wormBodyPartPrefab) as WormBodyPart;
		bodyPart.Initialize(this, coordinates, direction);
		bodyPart.transform.parent = transform;
		bodyPart.transform.position = Board.instance.GetTilePosition(coordinates);
		bodyParts.Add(bodyPart);
	}

	public void Move(BoardDirection direction) {
		head.SetDirection(direction);

		TileEdge edge = head.currentTile.GetEdge(direction);
		Tile otherTile = null;
		if (edge is TilePassage) otherTile = edge.otherTile;

		if (otherTile != null) {
			bool occupied = Board.instance.GetTileIsOccupied(otherTile.coordinates);

			if (!occupied) {
				PlaceBodyPart(head.currentTile.coordinates, direction);
				head.SetPosition(otherTile.coordinates);
				PlaceBodyPart(head.currentTile.coordinates, direction.GetOpposite());
			}
		}
	}

	void Start () {
	
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.UpArrow)) Move(BoardDirection.Up);
		else if (Input.GetKeyDown(KeyCode.RightArrow)) Move(BoardDirection.Right);
		else if (Input.GetKeyDown(KeyCode.DownArrow)) Move(BoardDirection.Down);
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(BoardDirection.Left);
	}
}