using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ObjectType {
	NONE = 0,
	WormHead = 1,
	WormBodyPart = 2,
	Enemy = 4,
}

public static class ObjectTypes {
	public const int Count = 3;

	public static ObjectType[] GetObjectTypes(int objectBitmask) {
		var objectTypes = new List<ObjectType>();

		for (int i = 0; i < Count; i++) {
			int singularObjectBitmask = 1 << i;
			if ((singularObjectBitmask & objectBitmask) == singularObjectBitmask) objectTypes.Add((ObjectType)singularObjectBitmask);
		}

		return objectTypes.ToArray();
	}

	public static int GetBitmask(params ObjectType[] objectTypes) {
		int bitmask = 0;

		foreach (ObjectType o in objectTypes) {
			bitmask |= (int)o;
		}

		return bitmask;
	}
}