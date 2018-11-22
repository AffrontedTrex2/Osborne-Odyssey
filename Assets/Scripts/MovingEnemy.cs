using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class MovingEnemy : Enemy {

	//Start overrides the virtual Start function of the base class.
	void Start () {
		score = 10; //Sets scpre

		speed = 0.2f; //Sets speed

		//Find the Player GameObject using it's tag and store a reference to its transform component.
		target = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	//Moves enemy every frame
	void Update() {
		MoveEnemy ();
	}

	//Moves enemy towards the target
	void MoveEnemy () {
		transform.position = Vector3.MoveTowards(transform.position, target.position, speed / 3);
	}
}