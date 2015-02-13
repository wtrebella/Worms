using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(SnakeSprite))]
public class SnakeController : MonoBehaviour {
	public Action SignalCommitMove;
	public Action SignalCancelMove;
	public Action<BoardDirection> SignalStartMove;
	public float tileSize = 1;
	public float lerpTime = 0.3f;
	public SnakeSprite snakeSprite;

	private float curVal = 0;
	public bool isMoving {get; private set;}
	public bool isAutoMoving {get; private set;}

	void Awake() {
		isMoving = false;
		isAutoMoving = false;
		snakeSprite = GetComponentInChildren<SnakeSprite>();
	}

	public void OnSwipeBegan(BoardDirection swipeDirection) {
		if (!isMoving) {
			if (swipeDirection.GetOpposite() != snakeSprite.previousDirection) StartMove(swipeDirection);
		}
	}

	public void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		if (!isMoving) return;
		
		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / tileSize);

		ContinueMove(normalizedVal);
		curVal = normalizedVal;
	}

	public void OnSwipeEnded(float swipePixelDistance, bool ignoreDistance) {
		if (!isMoving) return;

		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / tileSize);

		if (normalizedVal >= 0.5f || ignoreDistance) StartCoroutine(AutoMoveCommit());
		else StartCoroutine(AutoMoveCancel());
	}

	public void OnSwipeCanceled() {
		if (!isMoving) return;

		CancelMove();
	}
	
	void ContinueMove(float swipeDistance) {
		snakeSprite.ContinueMove(swipeDistance);
	}

	void StartMove(BoardDirection direction) {
		curVal = 0;
		isMoving = true;
		snakeSprite.StartMove(direction);
		if (SignalStartMove != null) SignalStartMove(direction);
	}
	
	void CommitMove() {
		isMoving = false;
		isAutoMoving = false;
		snakeSprite.CommitMove();
		if (SignalCommitMove != null) SignalCommitMove();
	}
	
	void CancelMove() {
		isMoving = false;
		isAutoMoving = false;
		snakeSprite.CancelMove();
		if (SignalCancelMove != null) SignalCancelMove();
	}
	
	IEnumerator AutoMoveCancel() {
		isMoving = true;
		isAutoMoving = true;

		float currentLerpTime = 0;
		float t;

		while (curVal > 0) {
			currentLerpTime += Time.deltaTime;
			t = currentLerpTime / lerpTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			curVal -= t;
			curVal = Mathf.Clamp01(curVal);
			ContinueMove(curVal);
			yield return null;
		}

		CancelMove();
	}
	
	IEnumerator AutoMoveCommit() {
		isMoving = true;
		isAutoMoving = true;

		float currentLerpTime = 0;
		float t;

		while (curVal < 1) {
			currentLerpTime += Time.deltaTime;
			t = currentLerpTime / lerpTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			curVal += t;
			curVal = Mathf.Clamp01(curVal);
			ContinueMove(curVal);
			yield return null;
		}

		CommitMove();
	}

	public void AutoMove(BoardDirection direction) {
		if (isMoving || isAutoMoving) return;

		StartMove(direction);
		StartCoroutine(AutoMoveCommit());
	}

	void Start () {
	
	}
	
	void Update () {
//		BoardDirection direction = BoardDirection.NONE;
//		
//		if (Input.GetKeyDown(KeyCode.UpArrow)) direction = BoardDirection.Up;
//		else if (Input.GetKeyDown(KeyCode.RightArrow)) direction = BoardDirection.Right;
//		else if (Input.GetKeyDown(KeyCode.DownArrow)) direction = BoardDirection.Down;
//		else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = BoardDirection.Left;
//		
//		if (direction != BoardDirection.NONE && direction != snakeSprite.previousDirection.GetOpposite() && !isMoving && !isAutoMoving) AutoMove(direction);
	}
}
