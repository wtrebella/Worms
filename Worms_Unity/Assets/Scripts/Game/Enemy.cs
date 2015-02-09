using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : TileEntity {
	public void Initialize(EnemyManager enemyManager, Tile tile) {
		tileEntityType = TileEntityType.Enemy;
		transform.parent = enemyManager.transform;
		SetTile(tile);
	}

	public override void SetTile(Tile tile) {
		RemoveFromTile();
		currentTile = tile;
		if (!tile.tileEntities.Contains(this)) tile.tileEntities.Add(this);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);

//		var wormBodyParts = currentTile.GetTileEntities(TileEntityType.WormBodyPart);
//		if (wormBodyParts.Count > 0) {
//			Worm worm = (wormBodyParts[0] as WormBodyPart).worm;
//			int highestIndex = 0;
//			for (int i = 0; i < worm.bodyParts.Count; i++) {
//				WormBodyPart w = worm.bodyParts[i];
//				if (wormBodyParts.Contains(w)) highestIndex = Mathf.Max(highestIndex, i);
//			}
//			for (int i = 0; i < highestIndex + 1; i++) {
//				EatWormBodyPart(worm.bodyParts[0]);
//			}
//		}
	}

	public override void Move(BoardDirection newDirection) {
		if (currentTile == null) Debug.LogError("can't move an entity before it has a tile");

		SetTile(Board.instance.GetTile(currentTile.coordinates + newDirection.ToIntVector2()));
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;
		
		if (currentTile.tileEntities.Contains(this)) currentTile.tileEntities.Remove(this);
		currentTile = null;
	}

	public void EatWormBodyPart(WormBodyPart wormBodyPart) {
		wormBodyPart.worm.RemoveBodyPart(wormBodyPart);
	}
}
