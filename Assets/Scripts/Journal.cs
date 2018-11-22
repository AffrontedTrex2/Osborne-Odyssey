using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Journal : MonoBehaviour {

	public Sprite[] pages; //Reference to the journal page images
	private int currentPage = 0; //Current page that you are viewing
	public int availableIndex = 0; //Available page indexes that you can view
	public bool paused; //If the game is paused

	public Text username; //Reference to the textbox where the username will be displayed
	public Text locationText; //Reference to textbox of page locations

	// Use this for initialization
	void Start () {
		paused = false;
		//The journal will be invisible at first
		this.GetComponent<Image> ().enabled = false;

		//Finds the username object and sets it to the user's computer name
		//username = this.GetComponentInChildren<Text>();
		username.enabled = false;

		//set the location text
		setLocationText();
	}


	//sets location text based on information from gamecontroller
	void setLocationText() {
		//Get locations of each page
		int[] pageLocations = GameController.instance.getPageLocations();

		//fenceposting the str so it looks pretty
		String str = "Page locations: ";

		//Put each location into the queue
		Queue<int> q = new Queue<int>();
		for (int i = 0; i < pageLocations.Length; i++) {
			//if the page hasn't been collected yet, at its location to the queue
			if (pageLocations [i] != -1) {
				q.Enqueue (pageLocations [i]);
			}
		}

		//add text to the str from the queue
		if (q.Count > 0) {
			str += "" + q.Dequeue();
		}
		while (q.Count != 0) {
			str += ", " + q.Dequeue();
		}

		//Set the text to the new str
		locationText.text = str;
	}
	
	// Update is called once per frame
	void Update () {
		//if you have all 5 pages, then you can meet osborne
		if (availableIndex == 5) {
			GameController.instance.endgame = true;
		}

		//If the space is pressed, then it will be visible
		//If there are no pages, then nothing happens
		//availableIndex != -1
		if (!GameController.instance.battleStart && Input.GetKey(KeyCode.Space) && (!GameController.instance.cutscenePlaying || paused)) {
			enabled = false;

			//If the journal is invisible, pause the game, because it's about to be visible
			if (!paused)
				GameController.instance.pause ();
			else
				GameController.instance.resume ();
			paused = !paused;

			//update journal location text
			setLocationText();

			//toggled visibility with a .1s delay
			StartCoroutine (toggleSprite (.2f));
		}

		//Change page (q = back, w = forwards) with a delay of .2 sec
		//the enabled = false prevents journal from updating so that you can't spam q and break the game
		if (this.GetComponent<Image>().enabled && Input.GetKey(KeyCode.Q)) {
			enabled = false;
			StartCoroutine (turnPage (.2f, -1));
		}

		if (this.GetComponent<Image>().enabled && Input.GetKey(KeyCode.W)) {
			enabled = false;
			StartCoroutine (turnPage (.2f, 1));
		}
	}

	//toggle sprite with a delay
	IEnumerator toggleSprite(float time) {
		yield return new WaitForSecondsRealtime(time);

		//If you have a page, set the sprite to currentpage
		GetComponent<Image> ().sprite = pages [currentPage];
		GetComponent<Image>().enabled = !GetComponent<Image>().enabled;

		//Show the username if the image is enabled and the page is 5
		if (GetComponent<Image> ().enabled && currentPage == 5) {
			username.enabled = true;
		} else {
			username.enabled = false;
		}

		//show location text if the image is enabled and page is 0
		if (GetComponent<Image> ().enabled && currentPage == 0) {
			locationText.enabled = true;
		} else {
			locationText.enabled = false;
		}

		enabled = true;
	}

	//This will delay the turnPage function by specified time
	IEnumerator turnPage(float time, int change) {
		//Turn the page, then wait
		turnPage (change);
		yield return new WaitForSecondsRealtime (time);
		enabled = true;
	}

	void turnPage(int change) {
		//Change the current page
		currentPage += change;

		//If the index is out of bounds, nothing happens
		if (currentPage < 0) {
			currentPage = 0;
		}
		if (currentPage > availableIndex) {
			currentPage = availableIndex;
		}

		//If the current page is the last page, display the user's name
		if (currentPage == 5) {
			username.text = GameController.instance.name;
			username.enabled = true;
		} else {
			username.enabled = false;
		}

		//If the current page is the map page, display the page locations
		if (currentPage == 0) {
			locationText.enabled = true;
		} else {
			locationText.enabled = false;
		}

		GetComponent<Image> ().sprite = pages [currentPage];
	}
}
