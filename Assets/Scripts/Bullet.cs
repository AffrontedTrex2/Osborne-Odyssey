using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public Vector3 target; //The location of the mouse when it is clicked
	public Vector3 trueTarget; //The extended vector that the bullet will move towards
	public float moveSpeed; //How fast the bullet will move

	//Extends the vector3 of target so that the bullet will continue moving past the mouse click
	public void updateTarget() {
		//Finds the slope of the line of player position to bullet position
		float slope = (target.y - transform.position.y) / (target.x - transform.position.x);

		//If the mouse's x is larger than the bullet, at 100, else subtract 100
		trueTarget = target;
		if (target.x > transform.position.x) {
			trueTarget.x += 100;
		} else {
			trueTarget.x -= 100;
		}
		//Calculate the correct y position based on the x position
		trueTarget.y = trueTarget.x * slope;
	}

	
	// Update is called once per frame
	void Update () {
		//Move towards the target
		transform.position = Vector2.MoveTowards (transform.position, trueTarget, moveSpeed * Time.deltaTime);
	}
}
