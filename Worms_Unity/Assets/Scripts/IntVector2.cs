using System;
using UnityEngine;

[Serializable]
public struct IntVector2 {
	public static IntVector2 zero {
		get {
			return new IntVector2(0, 0);
		}
	}

	[SerializeField]
	public int x, y;

	[SerializeField]
	public IntVector2 (int x, int z) {
		this.x = x;
		this.y = z;
	}

	public static IntVector2 operator + (IntVector2 a, IntVector2 b) {
		a.x += b.x;
		a.y += b.y;
		return a;
	}
}