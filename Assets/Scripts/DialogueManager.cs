using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	//Textboxes for name and dialogue
	public Text nameText;
	public Text dialogueText;

	//Sprites for main character and osbornes
	public GameObject mc;
	public GameObject cybosborne;
	public GameObject osborne;

	//Reference to animator to create the textbox animation
	public Animator animator;

	//Queue of names and sentences that will be displayed
	private Queue<string> sentences;
	private Queue<string> names;

	//Reference to player script
	private Player player;

	//Initializes queues and finds player
	public void Init() {
		sentences = new Queue<string> ();
		names = new Queue<string> ();

		player = GameObject.Find ("Player").GetComponent<Player> ();
	}

	public void StartDialogue(Dialogue dialogue) {
		//Plays animation
		animator.SetBool ("isOpen", true);

		player.enabled = false;

		//Sets bool to true so that pause/journal can't be opened
        GameController.instance.cutscenePlaying = true;

		//Clear queues
		sentences.Clear ();
		names.Clear ();

		//add each sentence to queue
		foreach (string sentence in dialogue.sentences) {
			sentences.Enqueue (sentence);
		}

		//add each name to the queue
		foreach (string name in dialogue.names) {
			names.Enqueue (name);
		}

		//show the sprites of each character if they have dialogue
		if (names.Contains("You")) {
			mc.SetActive (true);
			mc.GetComponent<SpriteRenderer> ().color = Color.gray;
		}
		if (names.Contains ("Mr. Osborne")) {
			osborne.SetActive (true);
			osborne.GetComponent<SpriteRenderer> ().color = Color.gray;
		} else if (names.Contains ("???")) {
			cybosborne.SetActive (true);
			cybosborne.GetComponent<SpriteRenderer> ().color = Color.gray;
		}

		DisplayNextSentence ();
	}

	public void DisplayNextSentence() {
		//If there are no more sentences, end dialogue
		if (sentences.Count == 0) {
			EndDialogue ();
			return;
		}

		//prints the sentence and name
		string sentence = sentences.Dequeue ();
		string name = names.Dequeue ();

		//If the name is "You" change it to the player's name instead
		if (name.Equals ("You")) {
			name = GameController.instance.name;
		}

		//Stop all other coroutines and start a new one
		StopAllCoroutines ();
		StartCoroutine (TypeSentence (sentence));

		//just prints the name regularly
		nameText.text = name;

		//Changes if the sprits should have a gray or no overlay color
		//Based on if they're talking or not
		if (name.Equals("???")) {
			osborne.SetActive (false);
			cybosborne.SetActive (true);
			cybosborne.GetComponent<SpriteRenderer> ().color = Color.white;
		} else {
			if (cybosborne.activeInHierarchy) {
				cybosborne.GetComponent<SpriteRenderer> ().color = Color.gray;
			}
		}
		if (name.Equals ("You") || name.Equals(GameController.instance.name)) {
			mc.GetComponent<SpriteRenderer> ().color = Color.white;
		} else {
			mc.GetComponent<SpriteRenderer> ().color = Color.gray;
		}
		if (name.Equals ("Mr. Osborne")) {
			cybosborne.SetActive (false);
			osborne.SetActive (true);
			osborne.GetComponent<SpriteRenderer> ().color = Color.white;
		} else {
			if (osborne.activeInHierarchy) {
				osborne.GetComponent<SpriteRenderer> ().color = Color.gray;
			}
		}
	}

	//makes it so that the letters appear one by one
	IEnumerator TypeSentence(string sentence) {
		dialogueText.text = "";

		//Add each character to the textbox one by one
		foreach (char letter in sentence.ToCharArray()) {
			dialogueText.text += letter;
			yield return null;
		}
	}
		
	void EndDialogue() {
		//Hides each profile sprite
		cybosborne.SetActive (false);
		mc.SetActive (false);
		osborne.SetActive (false);

		//Play the closing animation
		animator.SetBool ("isOpen", false);

		//Allow the player to move/shoot again
		player.enabled = true;

		//Resume all gameObjects (bullets, moving enemies)
        GameController.instance.resumeGameobjects();

		//Calls different functions depending on the scenario
		if (GameController.instance.form2dead) {
			GameController.instance.winGame ();
		} else if (GameController.instance.form1dead) {
			GameController.instance.wumpusTime2 ();
		} else if (GameController.instance.endgame && GameController.instance.currentCave == 0) {
			GameController.instance.wumpusTime ();
		}

		//Allows player to open journal/pause again
		if (!GameController.instance.form2dead) {
			GameController.instance.cutscenePlaying = false;
		}
    }
}
