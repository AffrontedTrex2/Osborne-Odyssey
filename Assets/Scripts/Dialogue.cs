using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue : MonoBehaviour {

	//Creates a place in the inspector to add the dialogue
	public string[] names;

	[TextArea(3, 10)]
	public string[] sentences;

	//Calls the dialogueManager to begin dialogue
	public void StartDialogue() {
		FindObjectOfType<DialogueManager> ().Init ();
		FindObjectOfType<DialogueManager> ().StartDialogue (this);
	}
}
