using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wumpus1 : Enemy {

    //TODO add drops during fight

	public GameObject enemyBullet; //Reference to what it will shoot
	public GameObject wumpusBullet; //reference to a special wumpus bullet
	public GameObject ozzySprite; //just a plain sprite that will be used during the dialogue

	private float fireRate = 1.15f; //how long to wait between each bullet volley
	private int health = 30; //wumpus form 1 health
	private Vector3 dashTarget;

	private bool goingLeft; //handles if the wumpus should change direction when moving
	private Renderer renderer;

	// Use this for initialization
	void Start () {
		enabled = false;

		renderer = GetComponent<SpriteRenderer> ();

		speed = 10f;

		//goingLeft = true;

		score = 500;

		target = GameObject.FindGameObjectWithTag ("Player").transform;

		//Invokes its two attacks repeately
		InvokeRepeating ("BulletAttack", 1f, fireRate);

		InvokeRepeating ("DashAttack", 2f, 2f);
	}

	public void loseHealth(int health) {
		this.health -= health;

		//if the wumpus has no health left, destroy it
		if (this.health <= 0) {
			GameObject ozzy = Instantiate (ozzySprite, transform.position, Quaternion.identity);

			//play the dialogue
			GameController.instance.defeatFormOneDialogue (ozzy, this.gameObject);

            //Add to score
            GameController.instance.changeScore(score);
        }
	}

	void DashAttack() {
		StartCoroutine(Dash());
	}

	//flash sprite 5 times and dash towards player
	IEnumerator Dash() {
		for(int i = 0; i < 5; i++) {
			renderer.enabled = true;
			yield return new WaitForSeconds(.1f);
			renderer.enabled = false;
			yield return new WaitForSeconds(.1f);
		}
		renderer.enabled = true;

		//enables starts Update() and moves osborne to the player location
		dashTarget = GameObject.FindGameObjectWithTag ("Player").transform.position;
		enabled = true;

        //		while (transform.position != dashTarget) {
        //			transform.position = Vector2.MoveTowards (transform.position, dashTarget, speed * Time.deltaTime);
        //		}

        createDropsWumpus();

		yield return new WaitForSeconds (2f);
	}

    void createDropsWumpus() {
        int dropType = Random.Range(0, 2);

        //If dropType is a 0, drop medicine
        if (dropType == 0) {
            GameController.instance.createObject(medicine, transform.position);
        }
        //If dropType is a 1 or 2, drop bullets
        if (dropType == 1) {
            GameController.instance.createObject(bulletDrop, transform.position);
        }
    }

	//Will move towards the player
	void Update() {
		transform.position = Vector2.MoveTowards (transform.position, dashTarget, speed * Time.deltaTime);

		if (transform.position == dashTarget) {
			enabled = false;
		}
	}

	// Update is called once per frame
//	void Update () {
//		Vector3 newPos = transform.position;
//		if (newPos.x <= 1.3f) {
//			goingLeft = false;
//		}
//		if (newPos.x >= 6f) {
//			goingLeft = true;
//		}
//
//		if (goingLeft) {
//			newPos.x -= speed;
//		} else {
//			newPos.x += speed;
//		}
//		transform.position = newPos;
//	}

	void BulletAttack() {
		//if the player is far, use a spread attack. else, use a straight attack
		Vector3 targetDist = target.position;
		targetDist.z = 0;

		if (Vector3.Distance (targetDist, transform.position) > 5) {
			SpreadAttack ();
		} else {
			StraightAttack ();
		}
	}

	//mimics genji's right click
	void SpreadAttack() {
		//shoots a bullet straight down
		GameObject straight = GameController.instance.createObject (wumpusBullet, transform.position);
		straight.GetComponent<WumpusBullet> ().setTarget (new Vector3 (0, this.transform.position.y - 100, 0));

		//bullet to the left
		GameObject left = GameController.instance.createObject (wumpusBullet, transform.position);
		left.GetComponent<WumpusBullet> ().setTarget (new Vector3 (this.transform.position.x - 20, this.transform.position.y - 10, 0));

		//left bullet, smaller angle
		GameObject left2 = GameController.instance.createObject (wumpusBullet, transform.position);
		left2.GetComponent<WumpusBullet> ().setTarget (new Vector3 (this.transform.position.x - 10, this.transform.position.y - 10, 0));

		//bullet to the right
		GameObject right = GameController.instance.createObject (wumpusBullet, transform.position);
		right.GetComponent<WumpusBullet> ().setTarget (new Vector3 (this.transform.position.x + 20, this.transform.position.y - 10, 0));

		//bullet to the right, smaller angle
		GameObject right2 = GameController.instance.createObject (wumpusBullet, transform.position);
		right2.GetComponent<WumpusBullet> ().setTarget (new Vector3 (this.transform.position.x + 10, this.transform.position.y - 10, 0));
	}

	//shoots three bullets in a line
	void StraightAttack() {
		for (int i = 0; i < 3; i++) {
			Invoke ("ShootBullet", .2f);
		}
	}

	//shoots a bullet at the player
	void ShootBullet() {
		//shoot 3 bullets with .2 seconds in between each bullet
		for (int i = 0; i < 3; i++) {
			Invoke ("ShootStraight", i * 0.2f);
		}
	}

	void ShootStraight() {
		//Create a bullet at enemy's location and add it to the list
		GameController.instance.createObject (enemyBullet, transform.position);
	}
}
