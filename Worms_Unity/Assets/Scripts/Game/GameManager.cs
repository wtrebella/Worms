using UnityEngine;
using System.Collections;

public enum GameState {
	Playing,
	Win
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	public SwipeEventSystem swipeEventSystem;
	public GameUIManager gameUIManager;

	public PuzzleData puzzleToLoad;

	public Board boardPrefab;
	public Worm wormPrefab;

	public Board board;

	public GameState gameState = GameState.Playing;

	private void Awake() {
		instance = this;
	}

	private void Start () {
		BeginGame();
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) RestartGame();

		if (gameState == GameState.Playing) {
			if (Input.GetKeyDown(KeyCode.UpArrow)) MoveUp();
			else if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
			else if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
		}
	}

	public void MoveUp() {
		board.Move(BoardDirection.Up);
		if (board.CheckWinConditions()) WinGame();
	}
	
	public void MoveRight() {
		board.Move(BoardDirection.Right);
		if (board.CheckWinConditions()) WinGame();
	}
	
	public void MoveDown() {
		board.Move(BoardDirection.Down);
		if (board.CheckWinConditions()) WinGame();
	}
	
	public void MoveLeft() {
		board.Move(BoardDirection.Left);
		if (board.CheckWinConditions()) WinGame();
	}

	private void WinGame() {
		gameState = GameState.Win;
		gameUIManager.HandleWin();
	}

	private void BeginGame () {
		gameUIManager.HandleBeginGame();

		board = Instantiate(boardPrefab) as Board;
		board.Generate(puzzleToLoad);

		gameState = GameState.Playing;
	}
	
	public void RestartGame () {
		swipeEventSystem.CancelSwipe();
		Destroy(board.gameObject);
		BeginGame();
	}
}