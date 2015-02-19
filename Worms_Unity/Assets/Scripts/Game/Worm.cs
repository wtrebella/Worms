using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Worm : TileEntity {
	public static float wormWidth = 50;

	public Action SignalWormDied;
	public WormSprite wormSpritePrefab;
	public WormBodyPart blankBodyPartPrefab;

	public List<WormBodyPart> bodyParts {get; private set;}
	public BoardDirection direction {get; private set;}
	public WormType wormType {get; private set;}

	private BoardDirection currentMoveDirection = BoardDirection.NONE;
	private WormSprite wormSprite;

	public void Initialize(Tile tile, BoardDirection newDirection, Color color, WormType wormType) {
		this.wormType = wormType;

		wormSprite = Instantiate(wormSpritePrefab) as WormSprite;
		wormSprite.transform.parent = transform;
		wormSprite.transform.localPosition = Vector3.zero;
		wormSprite.SetColor(color);

		transform.parent = Board.instance.transform;
		transform.localPosition = Vector3.zero;
		
		bodyParts = new List<WormBodyPart>();

		tileEntityType = TileEntityType.Worm;
		direction = BoardDirection.NONE;
		SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
		PlaceBlankBodyPart(tile);
	}

	public void PlaceBlankBodyPart(Tile tile) {
		WormBodyPart bodyPart;
		
		bodyPart = Instantiate(blankBodyPartPrefab) as WormBodyPart;
		bodyPart.Initialize(this, tile);
		
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

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
	}

	protected override void StartMove(BoardDirection newDirection) {
		base.StartMove(newDirection);

		currentMoveDirection = newDirection;
		wormSprite.StartMove(newDirection);
	}
	
	protected override void ContinueMove(float normalizedPosition) {
		base.ContinueMove(normalizedPosition);

		wormSprite.ContinueMove(normalizedPosition);
	}
	
	protected override void CommitMove() {
		base.CommitMove();

		SetDirection(currentMoveDirection);
		PlaceBlankBodyPart(currentTile);
		wormSprite.CommitMove();

		DeathTrap deathTrap = currentTile.GetTileEntity(TileEntityType.DeathTrap) as DeathTrap;
		if (deathTrap != null) {
			if (SignalWormDied != null) SignalWormDied();
			Destroy(gameObject);
		}
	}
	
	protected override void CancelMove() {
		base.CancelMove();
		wormSprite.CancelMove();
	}
}