using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour {

	//Loads the game
	public void PlayGame() {
		SceneManager.LoadScene (1);
	}

	//Loads the menu
	public void MainMenu() {
		SceneManager.LoadScene (0);
	}

	//Exits the game
	public void Quit() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}
