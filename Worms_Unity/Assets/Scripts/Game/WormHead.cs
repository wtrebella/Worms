using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormHead : TileEntity {
	public BoardDirection direction {get; private set;}
	public SnakeController snakeControllerPrefab;

	private BoardDirection currentMoveDirection = BoardDirection.NONE;
	private SnakeController snakeController;
	private Worm worm;

	public void Initialize(Worm worm, Tile tile, BoardDirection newDirection, Color color) {
		this.worm = worm;
		snakeController = Instantiate(snakeControllerPrefab) as SnakeController;
		snakeController.transform.parent = transform;
		snakeController.transform.localPosition = Vector3.zero;
		snakeController.snakeSprite.SetColor(color);
		snakeController.SignalCommitMove += HandleSnakeCommitedMove;
		snakeController.SignalCancelMove += HandleSnakeCanceledMove;
		snakeController.SignalStartMove += HandleSnakeStartedMove;
		tileEntityType = TileEntityType.WormHead;
		transform.parent = worm.transform;
		direction = BoardDirection.NONE;
		SetTile(tile);
		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
//		if (newDirection != BoardDirection.NONE) transform.localRotation = newDirection.ToRotation();
		worm.HandleHeadMoved(null, tile, BoardDirection.NONE, BoardDirection.NONE);
	}

	private void SetDirection(BoardDirection newDirection) {
		direction = newDirection;
//		transform.localRotation = direction.ToRotation();
	}

	public override void OnSwipeBegan(BoardDirection swipeDirection) {
		snakeController.OnSwipeBegan(swipeDirection);
	}
	
	public override void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		snakeController.OnSwipeContinue(swipeDirection, swipePixelDistance);
	}
	
	public override void OnSwipeEnded(float swipePixelDistance) {
		snakeController.OnSwipeEnded(swipePixelDistance);
	}
	
	public override void OnSwipeCanceled() {
		snakeController.OnSwipeCanceled();
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
//		transform.position = Board.instance.GetTilePosition(currentTile.coordinates);
	}
	
	public override void AutoMove(BoardDirection newDirection) {
		if (currentTile == null) Debug.LogError("can't move an entity before it has a tile");
		isAutoMoving = true;
		isMoving = true;
		snakeController.AutoMove(newDirection);
	}

	public override void RemoveFromTile() {
		if (currentTile == null) return;

		if (currentTile.tileEntities.Contains(this)) currentTile.tileEntities.Remove(this);
		currentTile = null;
	}
}