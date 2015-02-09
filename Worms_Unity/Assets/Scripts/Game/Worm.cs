using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart roadSpokePrefab;
	public WormBodyPart roadStraightPrefab;
	public WormBodyPart roadCurvedPrefab;
	public WormBodyPart roadBlankPrefab;
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

	public void PlaceBodyPartSpoke(Tile tile, BoardDirection direction) {
		if (direction == BoardDirection.NONE) return;

		WormBodyPart bodyPart;
		bodyPart = Instantiate(roadSpokePrefab) as WormBodyPart;
		bodyPart.Initialize(this, tile, direction, color);
		bodyPart.transform.parent = transform;
		bodyPart.transform.position = Board.instance.GetTilePosition(tile.coordinates);
		bodyParts.Add(bodyPart);
	}

	public void PlaceBodyPartCenter(Tile tile, BoardDirection fromDirection, BoardDirection toDirection) {
		WormBodyPart bodyPart;

		if (fromDirection == BoardDirection.NONE && toDirection == BoardDirection.NONE) {
			// start point
			bodyPart = Instantiate(roadBlankPrefab) as WormBodyPart;
			bodyPart.Initialize(this, tile, toDirection, color);
		}

		else if (fromDirection == toDirection || fromDirection == BoardDirection.NONE) {
			// straight
			bodyPart = Instantiate(roadStraightPrefab) as WormBodyPart;
			bodyPart.Initialize(this, tile, toDirection, color);
		}

		else {
			// curved
			BoardDirection rotateDirection = BoardDirection.NONE;

			bodyPart = Instantiate(roadCurvedPrefab) as WormBodyPart;
			if (fromDirection == BoardDirection.Down && toDirection == BoardDirection.Right || fromDirection == BoardDirection.Left && toDirection == BoardDirection.Up) rotateDirection = BoardDirection.Up;
			else if (fromDirection == BoardDirection.Left && toDirection == BoardDirection.Down || fromDirection == BoardDirection.Up && toDirection == BoardDirection.Right) rotateDirection = BoardDirection.Right;
			else if (fromDirection == BoardDirection.Up && toDirection == BoardDirection.Left || fromDirection == BoardDirection.Right && toDirection == BoardDirection.Down) rotateDirection = BoardDirection.Down;
			else if (fromDirection == BoardDirection.Right && toDirection == BoardDirection.Up || fromDirection == BoardDirection.Down && toDirection == BoardDirection.Left) rotateDirection = BoardDirection.Left;

			bodyPart.Initialize(this, tile, rotateDirection, color);
		}
		
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
	
	public void HandleHeadMoved(Tile previousTile, Tile currentTile, BoardDirection previousDirection, BoardDirection newDirection) {
		if (previousTile != null) {
			PlaceBodyPartSpoke(previousTile, newDirection);
			PlaceBodyPartCenter(previousTile, previousDirection, newDirection);
			PlaceBodyPartCenter(currentTile, BoardDirection.NONE, BoardDirection.NONE);
			PlaceBodyPartSpoke(currentTile, newDirection.GetOpposite());
		}
		else PlaceBodyPartCenter(currentTile, BoardDirection.NONE, BoardDirection.NONE);
	}
}