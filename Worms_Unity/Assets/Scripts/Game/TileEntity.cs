using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class TileEntity : MonoBehaviour {
	[HideInInspector]
	public Tile currentTile;

	[HideInInspector]
	public TileEntityType tileEntityType = TileEntityType.NONE;

	public bool isMoving {get; protected set;}
	public bool isAutoMoving {get; protected set;}
	public Action SignalCommitMove;
	public Action SignalCancelMove;
	public Action<float> SignalContinueMove;
	public Action<BoardDirection> SignalStartMove;

	protected Tile newTile;

	private float curVal = 0;

	virtual protected void StartMove(BoardDirection direction) {
		curVal = 0;
		isMoving = true;
		newTile = Board.instance.GetTile(currentTile.coordinates + direction.ToIntVector2());

		if (SignalStartMove != null) SignalStartMove(direction);
	}

	virtual protected void ContinueMove(float normalizedPosition) {
		if (SignalContinueMove != null) SignalContinueMove(normalizedPosition);
	}

	virtual protected void CommitMove() {
		isMoving = false;
		isAutoMoving = false;
		SetTile(newTile);
		if (SignalCommitMove != null) SignalCommitMove();
	}
	
	virtual protected void CancelMove() {
		isMoving = false;
		isAutoMoving = false;
		if (SignalCancelMove != null) SignalCancelMove();
	}

	virtual public void SetTile(Tile tile) {
		RemoveFromTile();
		currentTile = tile;

		if (!tile.tileEntities.Contains(this)) tile.tileEntities.Add(this);
	}

	virtual public void AutoMove(BoardDirection direction) {
		if (currentTile == null) Debug.LogError("can't move an entity before it has a tile");

		if (isMoving || isAutoMoving) return;
		
		StartMove(direction);
		StartCoroutine(AutoMoveCommit());
	}
	
	public void RemoveFromTile() {
		if (currentTile == null) return;
		
		if (currentTile.tileEntities.Contains(this)) currentTile.tileEntities.Remove(this);
		currentTile = null;
	}

	public void OnSwipeBegan(BoardDirection swipeDirection) {
		if (!isMoving) StartMove(swipeDirection);
	}
	
	public void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		if (!isMoving) return;
		
		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / Board.tileSize);
		
		ContinueMove(normalizedVal);
		curVal = normalizedVal;
	}
	
	public void OnSwipeEnded(float swipePixelDistance, bool ignoreDistance) {
		if (!isMoving) return;
		
		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / Board.tileSize);
		
		if (normalizedVal >= 0.5f || ignoreDistance) StartCoroutine(AutoMoveCommit());
		else StartCoroutine(AutoMoveCancel());
	}
	
	public void OnSwipeCanceled() {
		if (!isMoving) return;
		
		CancelMove();
	}

	public IEnumerator AutoMoveCancel() {
		isMoving = true;
		isAutoMoving = true;
		
		float currentLerpTime = 0;
		float t;
		
		while (curVal > 0) {
			currentLerpTime += Time.deltaTime;
			t = currentLerpTime / Board.lerpTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			curVal -= t;
			curVal = Mathf.Clamp01(curVal);
			ContinueMove(curVal);
			yield return null;
		}
		
		CancelMove();
	}
	
	public IEnumerator AutoMoveCommit() {
		isMoving = true;
		isAutoMoving = true;
		
		float currentLerpTime = 0;
		float t;
		
		while (curVal < 1) {
			currentLerpTime += Time.deltaTime;
			t = currentLerpTime / Board.lerpTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			curVal += t;
			curVal = Mathf.Clamp01(curVal);
			ContinueMove(curVal);
			yield return null;
		}
		
		CommitMove();
	}

	public static bool TileEntityCanMoveToTile(TileEntityType tileEntityType, List<TileEntity> tileEntities) {
		if (tileEntityType == TileEntityType.Worm) {
			foreach (TileEntity t in tileEntities) {
				if (t.tileEntityType == TileEntityType.WormBodyPart ||
				    t.tileEntityType == TileEntityType.Peg) return false;
			}
		}
		else if (tileEntityType == TileEntityType.Peg) {
			foreach (TileEntity t in tileEntities) {
				if (t.tileEntityType == TileEntityType.WormBodyPart ||
				    t.tileEntityType == TileEntityType.Worm ||
				    t.tileEntityType == TileEntityType.Peg) return false;
			}
		}
		else if (tileEntityType == TileEntityType.DeathTrap) return false;
		else if (tileEntityType == TileEntityType.WormBodyPart) return false;
		
		return true;
	}
}
