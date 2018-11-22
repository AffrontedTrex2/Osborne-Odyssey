using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	private Text scoreText; //Reference to the gold textbox
	private Image healthImage; //Reference to hearts
	private Text bulletText; //Reference to bullet textbox
	private Text timer; //Reference to timer textbox
	private Text caveText; //Reference to cave number textbox

    public Text creditsScoreText; //reference to the score that will be displayed in the credits

	public Sprite[] hearts; //Reference to the array of heart anims that will be used

	public void Init() {
		//Get a reference to the text by finding it by name
		scoreText = GameObject.Find ("scoreText").GetComponent<Text>();
		updateScore ();

		//Get a reference to the health image by name
		healthImage = GameObject.Find("Health").GetComponent<Image>();
		updateHealth ();

		//Get a reference to the bullet text by name
		bulletText = GameObject.Find("bulletText").GetComponent<Text>();
		updateBullets ();

		//Get a reference to the caveText by name, and set it to "AP CS Classroom" in the beginning
		caveText = GameObject.Find("caveText").GetComponent<Text>();
		caveText.text = "AP CS Classroom";

		//Get a reference to the timer text by name
		timer = GameObject.Find("Timer").GetComponent<Text>();

		//Calls the change timer function once per second
		InvokeRepeating ("changeTimer", .01f, 1.0f);
	}

	//Updates cave number that is displayed
	public void updateCave(int caveNum) {
		if (caveNum == 0) {
			caveText.text = "AP CS Classroom";
		} else {
			caveText.text = "Room " + caveNum;
		}
	}

	public void updateHealth() {
		//Get health from gameController
		int health = GameController.instance.health;

		//If you have more hearts than possible
		if (health > hearts.Length - 1) {
			return;
		}

		//Change the image to reflect your new health
		if (health >= 0) {
			healthImage.sprite = hearts [health];
		}
	}

	//update bullet text
	public void updateBullets() {
		bulletText.text = "Bullets: " + GameController.instance.bullets;
	}

	//update score text
	public void updateScore() {
		scoreText.text = "Score: " + GameController.instance.score;
        creditsScoreText.text = "" + GameController.instance.score;
	}

	private void changeTimer() {
		//Get the timer text and split it into hours and minutes and seconds
		string[] timeStr = timer.text.Split (":" [0]);

		//Convert the strings into ints
		int[] time = new int[3];
		time [0] = int.Parse (timeStr [0]);
		time [1] = int.Parse (timeStr [1]);
		time [2] = int.Parse (timeStr [2]);

		//Add one second
		time [2]++;

		//If it has been 60 seconds, change seconds back to zero and change minutes
		if (time [2] >= 60) {
			time [2] = 00;
			time [1]++;
		}

		//If it has been 60 minutes, change minutes back to zero and change hours
		if (time [1] >= 60) {
			time [1] = 00;
			time [0]++;
		}

		//Creates the new string for timer, makes it look pretty
		string newText = time [0] + ":";
		if (time [1] < 10) {
			newText += "0";
		}
		newText += time [1] + ":";
		if (time [2] < 10) {
			newText += "0";
		}
		newText += time [2];

		//Sets the timer text to the new text
		timer.text = newText;
	}
		
}
