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
	public PuzzleManager puzzleManager;
	public Board boardPrefab;

	[Range(-1, 25)]
	public int puzzleToLoad = -1;

	public GameState gameState {get; private set;}
	public Board board {get; private set;}
	public int currentPuzzleIndex {get; private set;}

	public void LoadPuzzle(int puzzleIndex) {
		currentPuzzleIndex = puzzleIndex;
		StartOrRestartGame();
	}

	public void LoadNextPuzzle() {
		LoadPuzzle(currentPuzzleIndex + 1);
	}

	public void LoadPreviousPuzzle() {
		LoadPuzzle(currentPuzzleIndex - 1);
	}

	private void Awake() {
		instance = this;
		currentPuzzleIndex = puzzleToLoad;
		gameState = GameState.Playing;
	}

	private void Start () {
		if (currentPuzzleIndex < 0 || currentPuzzleIndex >= puzzleManager.puzzles.Length) currentPuzzleIndex = 0;

		LoadPuzzle(currentPuzzleIndex);
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) StartOrRestartGame();

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
	
	public void StartOrRestartGame () {
		swipeEventSystem.CancelTouch();
		if (board != null) Destroy(board.gameObject);
	
		PuzzleData puzzle = puzzleManager.GetPuzzle(currentPuzzleIndex);

		board = Instantiate(boardPrefab) as Board;
		board.Generate(puzzle);
		
		gameState = GameState.Playing;

		gameUIManager.HandlePuzzleLoaded(currentPuzzleIndex, puzzle);
	}
}