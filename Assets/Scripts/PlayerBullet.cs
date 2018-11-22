using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBullet : Bullet {

	// Use this for initialization
	void Start () {
		moveSpeed = 6f; //sets movespeed

		//Target is where the mouse is located
		target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		updateTarget ();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "ShootingEnemy" || other.gameObject.tag == "SallyEnemy") {
			//Add to player score depending on which type of enemy it is
			if (other.gameObject.tag == "Enemy") {
				GameController.instance.changeScore(other.gameObject.GetComponent<Enemy>().score);
			} else if (other.gameObject.tag == "ShootingEnemy") {
				GameController.instance.changeScore(other.gameObject.GetComponent<ShootingEnemy>().score);
			}

			//Create a random dropped object
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().createDrops ();
			} else if (other.gameObject.tag == "ShootingEnemy") {
				other.gameObject.GetComponent<ShootingEnemy> ().createDrops ();
			}

			//Destroy the bullet and the enemy
			GameController.instance.destroyObject (other.gameObject);
			GameController.instance.destroyObject (this.gameObject);
		}

		//Destroy this if it hits a wall/exit
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Exit") {
			GameController.instance.destroyObject (this.gameObject);
		}

		if (other.gameObject.tag == "Wumpus") {
			//subtract health from the wumpus
			Wumpus1 wumpusScript = other.gameObject.GetComponent<Wumpus1>();
			wumpusScript.loseHealth (1);

			//destroy this
			GameController.instance.destroyObject (this.gameObject);
		}

		if (other.gameObject.tag == "Spawner") {
			//subtract health from spawner
			Spawner spawnScript = other.gameObject.GetComponent<Spawner> ();
			spawnScript.loseHealth (1);

			//destroy this
			GameController.instance.destroyObject (this.gameObject);
		}

		if (other.gameObject.tag == "Wumpus2") {
			//subtract health from the wumpus
			Wumpus2 wumpusScript = other.gameObject.GetComponent<Wumpus2>();
			wumpusScript.loseHealth (1);

			//destroy this
			GameController.instance.destroyObject (this.gameObject);
		}
	}
}
