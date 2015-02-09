using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TileEntity : MonoBehaviour {
	public Tile currentTile;
	public TileEntityType tileEntityType = TileEntityType.NONE;

	public abstract void SetTile(Tile tile);
	public abstract void Move(BoardDirection newDirection);
	public abstract void RemoveFromTile();

	public static bool TileEntityTypeCanEnterTileWithTileEntities(TileEntityType tileEntityType, List<TileEntity> tileEntities) {
		if (tileEntityType == TileEntityType.Enemy) {
			foreach (TileEntity t in tileEntities) {
				if (t.tileEntityType == TileEntityType.Enemy || t.tileEntityType == TileEntityType.WormHead || t.tileEntityType == TileEntityType.WormBodyPart) return false;
			}
		}
		else if (tileEntityType == TileEntityType.WormHead) {
			foreach (TileEntity t in tileEntities) {
				if (t.tileEntityType == TileEntityType.WormBodyPart) return false;
			}
		}
		else if (tileEntityType == TileEntityType.WormBodyPart) {
			return false;
		}
		
		return true;
	}
}
