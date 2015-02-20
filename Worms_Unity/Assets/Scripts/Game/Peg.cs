using UnityEngine;
using System.Collections;

public class Peg : TileEntity {
	public void Initialize(Tile tile) {
		tileEntityType = TileEntityType.Peg;
		transform.parent = Board.instance.transform;
		SetTile(tile);
	}
	
	public override void SetTile(Tile tile) {
		base.SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}

	protected override void ContinueMove(float normalizedDistance) {
		base.ContinueMove(normalizedDistance);

		Vector3 curTilePos = Board.instance.GetTilePosition(currentTile.coordinates);
		Vector3 newTilePos = Board.instance.GetTilePosition(newTile.coordinates);

		transform.position = Vector3.Lerp(curTilePos, newTilePos, normalizedDistance);
	}

	protected override void CommitMove() {
		base.CommitMove();

//		DeathTrap deathTrap = currentTile.GetTileEntity(TileEntityType.DeathTrap) as DeathTrap;
//		if (deathTrap != null) {
//			RemoveFromTile();
//			Destroy(gameObject);
//		}
	}
}
