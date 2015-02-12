using UnityEngine;
using System.Collections;

public class SnakeGameManager : MonoBehaviour {
	public Snake snakePrefab;
	public float tileSize = 1;

	private bool isMoving = false;
	private Snake snake;
	private BoardDirection currentMove = BoardDirection.NONE;

	void Awake() {
		snake = Instantiate(snakePrefab) as Snake;
	}

	public void OnSwipeBegan(BoardDirection swipeDirection) {
		if (!isMoving) StartMove(swipeDirection);
	}

	public void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / tileSize);

		if (isMoving) ContinueMove(normalizedVal);
	}

	public void OnSwipeEnded(float swipePixelDistance) {
		float swipeWorldDistance = swipePixelDistance * (Camera.main.orthographicSize / (Screen.height / 2.0f));
		float normalizedVal = Mathf.Clamp01(Mathf.Abs(swipeWorldDistance) / tileSize);

		if (normalizedVal < 0.5f) CancelMove();
		else CommitMove();
	}

	public void OnSwipeCanceled() {
		CancelMove();
	}
	
	void ContinueMove(float swipeDistance) {
		snake.ContinueMove(swipeDistance);
	}

	void StartMove(BoardDirection direction) {
		currentMove = direction;
		isMoving = true;
		snake.StartMove(direction);
	}
	
	void CommitMove() {
		currentMove = BoardDirection.NONE;
		isMoving = false;
		snake.CommitMove();
	}
	
	void CancelMove() {
		currentMove = BoardDirection.NONE;
		isMoving = false;
		snake.CancelMove();
	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}
