﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHead wormHeadPartPrefab;
	public List<WormBodyPart> bodyParts;
	public WormHead head;

	private Color color;

//	private int wormLength = 0;

	public void Initialize(Tile tile, BoardDirection direction, Color color) {
		this.color = color;
		transform.parent = Board.instance.transform;
		transform.localPosition = Vector3.zero;

		bodyParts = new List<WormBodyPart>();

		head = Instantiate(wormHeadPartPrefab) as WormHead;
		head.Initialize(this, tile, direction, color); 
	}

	public void PlaceBodyPart(Tile tile, BoardDirection direction) {
		WormBodyPart bodyPart = Instantiate(wormBodyPartPrefab) as WormBodyPart;
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

//		if (wormLength > 0) {
//			if (previousTile != null) PlaceBodyPart(previousTile, direction);
//			PlaceBodyPart(currentTile, direction.GetOpposite());
//
//			int curWormLength = bodyParts.Count;
//
//			for (int i = wormLength * 2; i < curWormLength; i++) {
//				RemoveBodyPart(0);
//			}
//		}

		Enemy enemy = currentTile.GetTileEntity(TileEntityType.Enemy) as Enemy;
		if (enemy != null) EatEnemy(enemy);
	}

	public void EatEnemy(Enemy enemy) {
		GameManager.instance.enemyManager.RemoveEnemy(enemy);
		int bodyPartCount = bodyParts.Count;
		for (int i = 0; i < bodyPartCount; i++) RemoveBodyPart(0);
//		wormLength++;
//		RemoveBodyPart(0);
//		RemoveBodyPart(0);
	}
}