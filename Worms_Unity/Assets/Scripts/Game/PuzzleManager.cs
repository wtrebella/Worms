using UnityEngine;
using System.Collections;
using System;

public class PuzzleManager : MonoBehaviour {
	public PuzzleData[] puzzles;

	void Awake() {
		puzzles = Resources.LoadAll<PuzzleData>("Puzzles") as PuzzleData[];
		Array.Sort(puzzles, (x,y) => String.Compare(x.name, y.name));
	}

	public PuzzleData GetPuzzle(int index) {
		if (index < 0 || index >= puzzles.Length) Debug.LogError("index out of range!");

		return puzzles[index];
	}

	public int GetIndex(PuzzleData puzzle) {
		for (int i = 0; i < puzzles.Length; i++) {
			if (puzzles[i] == puzzle) return i;
		}

		return -1;
	}
}
