using UnityEngine;
using System.IO;
using System;
using System.Collections;

public class PrintHighScores : MonoBehaviour {



	void Start () {
	}

	// Update is called once per frame
	// Centers the highscores when displayed 
	void OnGUI () {
		GUI.skin.label.fontSize = Screen.width / 25;
		GUI.skin.label.alignment = TextAnchor.UpperCenter;
		GUI.skin.label.padding.top = Screen.width / 15;
		GUILayout.Label (System.IO.File.ReadAllText("Assets/Resources/HighScores.txt"),  GUILayout.MinWidth(Screen.width), GUILayout.MinHeight(Screen.height));
	} 
}
