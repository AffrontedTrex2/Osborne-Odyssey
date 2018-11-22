using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy {

	public GameObject enemyBullet; //Reference to what it will shoot

	private float fireRate = 1.3f; //how fast it will shoot
	public GameObject turret; //reference to the turret part, which will rotate

	// Use this for initialization
	void Start () {
		//find player
		target = GameObject.Find ("Player").transform;

		//sets score
		score = 20;

        startAttacking();
	}

	//rotates the turret so that it tracks the player
	void Update() {
		Vector3 diff = target.position - turret.transform.position;
		diff.Normalize();

		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		turret.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
	}

    public void startAttacking() {
        //attacks depending on firing rate
        InvokeRepeating("Attack", .5f, fireRate);
    }

	void Attack() {
		//Create a bullet at enemy's location and add it to the list
		GameController.instance.createObject (enemyBullet, transform.position);
	}
}
