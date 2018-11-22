using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzzyRun : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		//change position so that it moves continuously to right
		Vector3 newPos = transform.position;
		newPos.x += .5f;
		transform.position = newPos;

		//if it goes off the screen, delete it
		if (transform.position.x > 15f) {
			Destroy (this.gameObject);
		}
	}
}
