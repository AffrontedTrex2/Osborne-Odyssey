using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

	public GameObject tutorialUI;

	public void showTutorial(){
		tutorialUI.SetActive (true);
	}

	public void hideTutorial() {
		tutorialUI.SetActive(false);
	}
}
