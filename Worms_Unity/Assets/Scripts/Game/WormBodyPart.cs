﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormBodyPart : TileEntity {
	public Worm worm;
	public BoardDirection direction {get; private set;}
	
	public void Initialize(Worm worm, Tile tile, BoardDirection newDirection, Color color) {
		this.worm = worm;
//		GetComponentInChildren<tk2dSprite>().color = color;
		tileEntityType = TileEntityType.WormBodyPart;
		transform.parent = worm.transform;
		SetTile(tile);
		SetDirection(newDirection);
	}

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
		if (direction == BoardDirection.NONE) return;
		transform.localRotation = direction.ToRotation();
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;
		
		if (currentTile.tileEntities.Contains(this)) currentTile.tileEntities.Remove(this);
		currentTile = null;
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
}