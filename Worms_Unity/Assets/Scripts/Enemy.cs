using UnityEngine;
using System.Collections;

public class Enemy : TileEntity {
	public void Initialize(EnemyManager enemyManager, Tile tile) {
		tileEntityType = TileEntityType.Enemy;
		transform.parent = enemyManager.transform;
		GoToTile(tile, BoardDirection.NONE);
	}

	public override void GoToTile(Tile tile, BoardDirection newDirection) {
		RemoveFromTile();
		currentTile = tile;
		currentTile.AddObject(this);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;
		
		currentTile.RemoveObject(this);
		currentTile = null;
	}

	public override bool CanMoveToTile(Tile tile) {
		return !(tile.Contains(TileEntityType.Enemy) || tile.Contains(TileEntityType.WormHead) || tile.Contains(TileEntityType.WormBodyPart));
	}
}
