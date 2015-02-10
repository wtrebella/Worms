﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormType wormType {get; private set;}
	public WormBodyPart roadSpokePrefab;
	public WormBodyPart roadStraightPrefab;
	public WormBodyPart roadCurvedPrefab;
	public WormBodyPart roadBlankPrefab;
	public WormHead wormHead1Prefab;
	public WormHead wormHead2Prefab;
	public WormHead wormHead3Prefab;
	public WormHead wormHead4Prefab;
	public List<WormBodyPart> bodyParts;
	public WormHead head;

	private bool hasMoved = false;

	private Color color;
	
	public void Initialize(Tile tile, BoardDirection direction, Color color, WormType wormType) {
		this.color = color;
		this.wormType = wormType;
		transform.parent = Board.instance.transform;
		transform.localPosition = Vector3.zero;

		bodyParts = new List<WormBodyPart>();

		if (wormType == WormType.Worm1) head = Instantiate(wormHead1Prefab) as WormHead;
		else if (wormType == WormType.Worm2) head = Instantiate(wormHead2Prefab) as WormHead;
		else if (wormType == WormType.Worm3) head = Instantiate(wormHead3Prefab) as WormHead;
		else if (wormType == WormType.Worm4) head = Instantiate(wormHead4Prefab) as WormHead;

		if (head != null) head.Initialize(this, tile, direction); 
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
			if (!hasMoved) {
				PlaceBodyPartSpoke(previousTile, newDirection.GetOpposite());
				hasMoved = true;
			}
			PlaceBodyPartSpoke(previousTile, newDirection);
			PlaceBodyPartCenter(previousTile, previousDirection, newDirection);
//			PlaceBodyPartCenter(currentTile, BoardDirection.NONE, BoardDirection.NONE);
			PlaceBodyPartSpoke(currentTile, newDirection.GetOpposite());
		}
		else PlaceBodyPartCenter(currentTile, BoardDirection.NONE, BoardDirection.NONE);
	}
}