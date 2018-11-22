using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SallyEnemy : Enemy {

	// Use this for initialization
	void Start () {
		score = 50; //sets score

		speed = .4f; //sets speed

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	void Update() {
		//if the player is to the left of sally, flip her sprite
		if (target.transform.position.x - transform.position.x > 0) {
			GetComponent<SpriteRenderer> ().flipX = true;
		} else {
			GetComponent<SpriteRenderer> ().flipX = false;
		}

		MoveEnemy ();
	}

	//Moves enemy towards the target
	void MoveEnemy () {
		transform.position = Vector3.MoveTowards(transform.position, target.position, speed / 3);
	}
}
