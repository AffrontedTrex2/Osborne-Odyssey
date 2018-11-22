using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public static bool GameIsPaused = false; // Variable to pause the game
	public GameObject pauseMenuUI; // Panel for the game panel
	public GameObject topScoresUI; // Panel to display HighScores

	// Update is called once per frame
	void Update () {
		// Pause or resume when escape key is pressed
		if (Input.GetKeyDown (KeyCode.Escape) && (!GameController.instance.cutscenePlaying || GameIsPaused)) {
			if (GameIsPaused) {
				Resume ();
			} else {
				Pause ();
			}
		}

	}

	// Resume the game 
	public void Resume(){
		pauseMenuUI.SetActive (false);
		//Time.timeScale = 1f;
		GameIsPaused = false;
		GameController.instance.resume ();
	}

	// Pause the game
	void Pause(){
		pauseMenuUI.SetActive (true);
		//Time.timeScale = 0f;
		GameIsPaused = true;
		GameController.instance.pause ();
	}

	// Go to the menu scene
	public void LoadMenu(){
		Time.timeScale = 1f;
		SceneManager.LoadScene (0);
	}

	// Display highscores
	public void GetHighScore(){
		topScoresUI.SetActive (true);
	}

	// Return to the game 
	public void Return(){
		topScoresUI.SetActive (false);
		pauseMenuUI.SetActive (true);
		GameIsPaused = false;
		GameController.instance.resume ();
	}
}
