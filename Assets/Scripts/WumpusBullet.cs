using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WumpusBullet : Bullet {

	private GameObject player;

	// Use this for initialization
	void Start () {
		moveSpeed = 6f; //move speed

		player = GameObject.FindGameObjectWithTag("Player");
	}

	//Finds a new target
	public void setTarget(Vector3 target) {
		this.target = target;
		updateTarget ();
	}
	
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			//Take health away
			player.GetComponent<Player>().changeLife(-1);

			//Destroy the bullet
			GameController.instance.destroyObject (this.gameObject);
		}

		//Remove both this bullet and the player's bullet
		if (other.gameObject.tag == "PlayerBullet") {
			GameController.instance.destroyObject (other.gameObject);
			GameController.instance.destroyObject (this.gameObject);
		}

		//Remove the bullet if it hits the wall or exit
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Exit") {
			GameController.instance.destroyObject (this.gameObject);
		}
	}
}
