using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class TileEntity : MonoBehaviour {
	public Tile currentTile;
	public TileEntityType tileEntityType = TileEntityType.NONE;
	public bool isMoving {get; protected set;}
	public bool isAutoMoving {get; protected set;}
	public Action SignalStartedMove;
	public Action SignalCanceledMove;
	public Action SignalCommitedMove;

	public abstract void SetTile(Tile tile);
	public abstract void OnSwipeBegan(BoardDirection swipeDirection);
	public abstract void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance);
	public abstract void OnSwipeEnded(float swipePixelDistance);
	public abstract void OnSwipeCanceled();
	public abstract void AutoMove(BoardDirection newDirection);
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
