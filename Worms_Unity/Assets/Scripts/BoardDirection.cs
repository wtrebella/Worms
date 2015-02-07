using UnityEngine;

public enum BoardDirection {
	Up,
	Right,
	Down,
	Left,
	NONE
}

public static class BoardDirections {
	
	public const int Count = 4;
	
	public static BoardDirection RandomValue {
		get {
			return (BoardDirection)Random.Range(0, Count);
		}
	}

	private static IntVector2[] vectors = {
		new IntVector2(0, 1),
		new IntVector2(1, 0),
		new IntVector2(0, -1),
		new IntVector2(-1, 0)
	};

	public static IntVector2 ToIntVector2 (this BoardDirection direction) {
		return vectors[(int)direction];
	}

	private static BoardDirection[] opposites = {
		BoardDirection.Down,
		BoardDirection.Left,
		BoardDirection.Up,
		BoardDirection.Right
	};
	
	public static BoardDirection GetOpposite (this BoardDirection direction) {
		if (direction == BoardDirection.NONE) return BoardDirection.NONE;
		return opposites[(int)direction];
	}

	private static Quaternion[] rotations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, -180f),
		Quaternion.Euler(0f, 0f, -270f)
	};
	
	public static Quaternion ToRotation (this BoardDirection direction) {
		return rotations[(int)direction];
	}
}