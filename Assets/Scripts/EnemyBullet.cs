using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet {

	private GameObject player; //Reference to player

	void Start () {
		moveSpeed = 3f; //Set movespeed

		//Target is where the player is
		player = GameObject.FindGameObjectWithTag("Player");
		target = player.transform.position;

		//Will cause the vector3 of the player to extend offscreen
		updateTarget ();
	}

	//What happens if the bullet collides
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			//Take health away
			player.GetComponent<Player>().changeLife(-1);

			//Destroy the bullet
			GameController.instance.destroyObject(this.gameObject);
		}

		//Remove both this bullet and the player's bullet
		if (other.gameObject.tag == "PlayerBullet") {
			GameController.instance.destroyObject(other.gameObject);
			GameController.instance.destroyObject(this.gameObject);
		}

		//Remove the bullet if it hits the wall or exit
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Exit") {
			GameController.instance.destroyObject(this.gameObject);
		}
	}
}
