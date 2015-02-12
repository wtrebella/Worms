using UnityEngine;
using System.Collections;

public class SnakeGameManager : MonoBehaviour {
	public Snake snakePrefab;
	public SwipeEventSystem swipeEventSystem;
	public float tileSize = 1;

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
			curVal = 0;

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

		while (curVal > 0) {
			curVal = Mathf.Clamp01(curVal - Time.deltaTime * 4);
			ContinueMove(curVal);
			yield return null;
		}

		CancelMove();
	}
	
	IEnumerator AutoMoveCommit() {
		isMoving = true;
		isAutoMoving = true;

		while (curVal < 1) {
			curVal = Mathf.Clamp01(curVal + Time.deltaTime * 4);
			ContinueMove(curVal);
			yield return null;
		}

		CommitMove();
	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}
