using UnityEngine;
using System.Collections;

public class Enemy : TileEntity {
	public void Initialize(EnemyManager enemyManager, Tile tile) {
		tileEntityType = TileEntityType.Enemy;
		transform.parent = enemyManager.transform;
		GoToTile(tile, BoardDirection.NONE);
	}

	public override void GoToTile(Tile tile, BoardDirection newDirection) {
		if (currentTile != null) currentTile.RemoveObject(this);
		currentTile = tile;
		currentTile.AddObject(this);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}

	public override bool CanMoveToTile(Tile tile) {
		return !(tile.Contains(TileEntityType.Enemy) || tile.Contains(TileEntityType.WormHead) || tile.Contains(TileEntityType.WormBodyPart));
	}
}
