using UnityEngine;
using System.Collections;

public class SnakeGameManager : MonoBehaviour {
	public Snake snakePrefab;
	public SwipeEventSystem swipeEventSystem;
	public float tileSize = 1;
	public float lerpTime = 0.3f;

	private float curVal = 0;
	private bool isMoving = false;
	private bool isAutoMoving = false;
	private Snake snake;

	void Awake() {
		snake = Instantiate(snakePrefab) as Snake;
	}

	public void OnSwipeBegan(BoardDirection swipeDirection) {
		if (isAutoMoving) swipeEventSystem.CancelTouch();

		if (!isMoving) {
			if (swipeDirection.GetOpposite() != snake.previousDirection) StartMove(swipeDirection);
		}
	}

	public void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		if (!isMoving) return;
		
		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / tileSize);

		ContinueMove(normalizedVal);
		curVal = normalizedVal;
	}

	public void OnSwipeEnded(float swipePixelDistance) {
		if (!isMoving) return;

		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / tileSize);

		if (normalizedVal < 0.5f) StartCoroutine(AutoMoveCancel());
		else StartCoroutine(AutoMoveCommit());
	}

	public void OnSwipeCanceled() {
		if (!isMoving) return;

		CancelMove();
	}
	
	void ContinueMove(float swipeDistance) {
		snake.ContinueMove(swipeDistance);
	}

	void StartMove(BoardDirection direction) {
		curVal = 0;
		isMoving = true;
		snake.StartMove(direction);
	}
	
	void CommitMove() {
		isMoving = false;
		isAutoMoving = false;
		snake.CommitMove();
	}
	
	void CancelMove() {
		isMoving = false;
		isAutoMoving = false;
		snake.CancelMove();
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
		BoardDirection direction = BoardDirection.NONE;
		
		if (Input.GetKeyDown(KeyCode.UpArrow)) direction = BoardDirection.Up;
		else if (Input.GetKeyDown(KeyCode.RightArrow)) direction = BoardDirection.Right;
		else if (Input.GetKeyDown(KeyCode.DownArrow)) direction = BoardDirection.Down;
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = BoardDirection.Left;
		
		if (direction != BoardDirection.NONE && direction != snake.previousDirection.GetOpposite() && !isMoving && !isAutoMoving) AutoMove(direction);
	}
}
