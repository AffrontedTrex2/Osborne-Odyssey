using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {

	public GameObject movingEnemy; //reference to moving enemy which will spawn

	private float spawnRate; //how fast enemies will spawn
	private int health = 10; //how much health spawns will have

	// Use this for initialization
	void Start () {
		//Sets spawn rate to a random number
		spawnRate = Random.Range(1, 5);

		//Spawns an enemy repeatedly with a delay of spawnRate
		InvokeRepeating("SpawnEnemies", Random.Range(0f, 1f), spawnRate);
	}

	//Create new movingEnemy
	void SpawnEnemies() {
		GameController.instance.createObject (movingEnemy, transform.position);
	}

	//Subtract health from spawner
	public void loseHealth(int health) {
		this.health -= health;

		//if the spawner has no health left, destroy it
		if (this.health <= 0) {
			GameController.instance.destroyObject (this.gameObject);
		}
	}
}
