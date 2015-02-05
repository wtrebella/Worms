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

	public override bool CanEnterTileWithTileEntities(List<TileEntity> tileEntities) {
		foreach (TileEntity t in tileEntities) {
			if (t.tileEntityType == TileEntityType.Enemy || t.tileEntityType == TileEntityType.WormBodyPart || t.tileEntityType == TileEntityType.WormHead) return false;
		}

		return true;
	}
}
