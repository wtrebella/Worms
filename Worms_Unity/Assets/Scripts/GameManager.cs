using UnityEngine;
using System.Collections;

public enum GameState {
	Playing,
	Win
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;

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
			BoardDirection direction = BoardDirection.NONE;

			if (Input.GetKeyDown(KeyCode.UpArrow)) direction = BoardDirection.Up;
			else if (Input.GetKeyDown(KeyCode.RightArrow)) direction = BoardDirection.Right;
			else if (Input.GetKeyDown(KeyCode.DownArrow)) direction = BoardDirection.Down;
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) direction = BoardDirection.Left;

			if (direction != BoardDirection.NONE) {
				board.Move(direction);
				if (board.CheckWinConditions()) WinGame();
			}
		}
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
	
	private void RestartGame () {
		Destroy(board.gameObject);
		BeginGame();
	}
}