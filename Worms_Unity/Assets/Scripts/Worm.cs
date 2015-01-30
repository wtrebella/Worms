using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : MonoBehaviour {
	public WormBodyPart wormBodyPartPrefab;
	public WormHead wormHeadPartPrefab;
	public List<WormBodyPart> bodyParts;
	public WormHead head;

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

		Enemy enemy = currentTile.GetEntity(TileEntityType.Enemy) as Enemy;
		if (enemy != null) EatEnemy(enemy);
	}

	public void EatEnemy(Enemy enemy) {
		GameManager.instance.enemyManager.RemoveEnemy(enemy);
		RemoveBodyPart(0);
		RemoveBodyPart(0);
	}
}