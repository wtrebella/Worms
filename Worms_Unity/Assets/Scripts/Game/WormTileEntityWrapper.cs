using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormTileEntityWrapper : TileEntity {
	public BoardDirection direction {get; private set;}
	public WormController wormControllerPrefab;

	private BoardDirection currentMoveDirection = BoardDirection.NONE;
	private WormController wormController;
	private Worm worm;

	public void Initialize(Worm worm, Tile tile, BoardDirection newDirection, Color color) {
		this.worm = worm;
		wormController = Instantiate(wormControllerPrefab) as WormController;
		wormController.transform.parent = transform;
		wormController.transform.localPosition = Vector3.zero;
		wormController.snakeSprite.SetColor(color);
		wormController.SignalCommitMove += HandleSnakeCommitedMove;
		wormController.SignalCancelMove += HandleSnakeCanceledMove;
		wormController.SignalStartMove += HandleSnakeStartedMove;
		tileEntityType = TileEntityType.WormHead;
		transform.parent = worm.transform;
		direction = BoardDirection.NONE;
		SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
		worm.HandleHeadMoved(null, tile, BoardDirection.NONE, BoardDirection.NONE);
	}

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
	}

	public override void OnSwipeBegan(BoardDirection swipeDirection) {
		wormController.OnSwipeBegan(swipeDirection);
	}
	
	public override void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		wormController.OnSwipeContinue(swipeDirection, swipePixelDistance);
	}
	
	public override void OnSwipeEnded(float swipePixelDistance, bool ignoreDistance) {
		wormController.OnSwipeEnded(swipePixelDistance, ignoreDistance);
	}
	
	public override void OnSwipeCanceled() {
		wormController.OnSwipeCanceled();
	}

	void HandleSnakeStartedMove(BoardDirection moveDirection) {
		currentMoveDirection = moveDirection;
		isMoving = true;
		if (SignalStartedMove != null) SignalStartedMove();
	}

	void HandleSnakeCommitedMove() {
		Tile previousTile = currentTile;
		BoardDirection previousDirection = direction;
		SetDirection(currentMoveDirection);
		SetTile(Board.instance.GetTile(currentTile.coordinates + currentMoveDirection.ToIntVector2()));
		worm.HandleHeadMoved(previousTile, currentTile, previousDirection, currentMoveDirection);
		isAutoMoving = false;
		isMoving = false;
		if (SignalCommitedMove != null) SignalCommitedMove();
	}

	void HandleSnakeCanceledMove() {
		isAutoMoving = false;
		isMoving = false;
		if (SignalCanceledMove != null) SignalCanceledMove();
	}

	public override void SetTile(Tile tile) {
		RemoveFromTile();
		currentTile = tile;
		if (!tile.tileEntities.Contains(this)) tile.tileEntities.Add(this);
	}
	
	public override void AutoMove(BoardDirection newDirection) {
		if (currentTile == null) Debug.LogError("can't move an entity before it has a tile");
		isAutoMoving = true;
		isMoving = true;
		wormController.AutoMove(newDirection);
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;

		if (currentTile.tileEntities.Contains(this)) currentTile.tileEntities.Remove(this);
		currentTile = null;
	}
}