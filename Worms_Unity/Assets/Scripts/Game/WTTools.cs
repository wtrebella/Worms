using UnityEngine;
using System.Collections;

public static class WTTools {

	public static Vector2 RotatePoint(Vector2 p, Vector2 o, float angle)
	{
		float s = Mathf.Sin(angle);
		float c = Mathf.Cos(angle);
		
		// translate point back to origin:
		p.x -= o.x;
		p.y -= o.y;
		
		// Which One Is Correct:
		// This?
		float xnew = p.x * c - p.y * s;
		float ynew = p.x * s + p.y * c;
//		// Or This?
//		float xnew = p.x * c + p.y * s;
//		float ynew = -p.x * s + p.y * c;
		
		// translate point back:
		p.x = xnew + o.x;
		p.y = ynew + o.y;

		return p;
	}
}
