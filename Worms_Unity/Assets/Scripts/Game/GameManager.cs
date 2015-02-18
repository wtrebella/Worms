using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	private int currentlyMovingEntities = 0;

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
		Application.targetFrameRate = 60;
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
			if (Input.GetKeyDown(KeyCode.UpArrow)) AutoMove(BoardDirection.Up);
			else if (Input.GetKeyDown(KeyCode.RightArrow)) AutoMove(BoardDirection.Right);
			else if (Input.GetKeyDown(KeyCode.DownArrow)) AutoMove(BoardDirection.Down);
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) AutoMove(BoardDirection.Left);
		}
	}

	public void AutoMove(BoardDirection direction) {
		if (currentlyMovingEntities > 0) return;
	
		List<TileEntity> tileEntities = null;
		
		if (direction == BoardDirection.Up || direction == BoardDirection.Down) {
			for (int x = 0; x < board.size.x; x++) {
				tileEntities = board.GetMovableTileEntitiesInColumn(direction, x);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.AutoMove(direction);
			}
		}
		else if (direction == BoardDirection.Right || direction == BoardDirection.Left) {
			for (int y = 0; y < board.size.y; y++) {
				tileEntities = board.GetMovableTileEntitiesInRow(direction, y);
				foreach (TileEntity tileEntity in tileEntities) tileEntity.AutoMove(direction);
			}
		}
	}
	
	public void OnSwipeBegan(BoardDirection swipeDirection) {
		if (currentlyMovingEntities > 0) swipeEventSystem.CancelTouch(false);
		else board.OnSwipeBegan(swipeDirection);
	}
	
	public void OnSwipeContinue(BoardDirection swipeDirection, float swipePixelDistance) {
		board.OnSwipeContinue(swipeDirection, swipePixelDistance);
	}
	
	public void OnSwipeEnded(BoardDirection swipeDirection, float swipePixelDistance, float swipeTime) {
		board.OnSwipeEnded(swipeDirection, swipePixelDistance, swipeTime < swipeEventSystem.quickSwipeMaxLength);
	}
	
	public void OnSwipeCanceled(BoardDirection swipeDirection) {
		board.OnSwipeCanceled(swipeDirection);
	}

	private void WinGame() {
		gameState = GameState.Win;
		gameUIManager.HandleWin();
	}

	void HandleTileEntityStartedMove(BoardDirection direction) {
		currentlyMovingEntities++;
	}

	void HandleTileEntityCanceledMove() {
		currentlyMovingEntities--;
	}

	void HandleTileEntityCommitedMove() {
		currentlyMovingEntities--;
		if (currentlyMovingEntities == 0) {
			if (board.CheckWinConditions()) WinGame();
		}
	}
	
	public void StartOrRestartGame () {
		swipeEventSystem.CancelTouch();
		if (board != null) Destroy(board.gameObject);
	
		PuzzleData puzzle = puzzleManager.GetPuzzle(currentPuzzleIndex);

		board = Instantiate(boardPrefab) as Board;
		board.Generate(puzzle);

		foreach (Tile tile in board.tiles) {
			foreach (TileEntity tileEntity in tile.tileEntities) {
				tileEntity.SignalStartMove += HandleTileEntityStartedMove;
				tileEntity.SignalCancelMove += HandleTileEntityCanceledMove;
				tileEntity.SignalCommitMove += HandleTileEntityCommitedMove;
			}
		}

		gameState = GameState.Playing;

		gameUIManager.HandlePuzzleLoaded(currentPuzzleIndex, puzzle);
	}
}