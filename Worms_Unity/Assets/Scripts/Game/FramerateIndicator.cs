using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FramerateIndicator : MonoBehaviour {
	Text text;

	void Start () {
		text = GetComponentInChildren<Text>();
	}
	
	void Update () {
		text.text = (1 / Time.deltaTime).ToString("0.0") + " fps";
	}
}
