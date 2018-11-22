using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wumpus2 : Enemy {

    public GameObject wumpusBullet; //reference to a special wumpus bullet
    private float fireRate = 6f; //how long to wait between each bullet volley
    public GameObject spawner; //spawners that will create enemies 

    private int health = 30;

	//Used to calculate where it will shoot
    private int x = -20;
    private int y = -10;

    // Use this for initialization
    void Start () {
        score = 1000;

        InvokeRepeating("whipAttack", 1f, fireRate);

		//Creates two spawners on each side
        GameController.instance.createObject(spawner, new Vector3(-4.41f, 4.41f, 0));
        GameController.instance.createObject(spawner, new Vector3(11.51f, 4.41f, 0));
    }

	public void loseHealth(int health) {
		this.health -= health;

		//if the wumpus has no health left, destroy it
		if (this.health <= 0) {
			//play the dialogue
			GameController.instance.defeatFormTwoDialogue (this.gameObject);

            //Add to score
            GameController.instance.changeScore(score);
		}
	}

	//Shoot 20 bullets in a whiplike pattern
    void whipAttack() {
        for (int i = 0; i < 20; i++) {
            Invoke("shoot", .15f * i);
        }
        x = -25;
    }

    void SpreadAttack() {
        //shoots a bullet straight down
        GameObject straight = GameController.instance.createObject(wumpusBullet, transform.position);
        straight.GetComponent<WumpusBullet>().setTarget(new Vector3(0, this.transform.position.y - 100, 0));

        //bullet to the left
        GameObject left = GameController.instance.createObject(wumpusBullet, transform.position);
        left.GetComponent<WumpusBullet>().setTarget(new Vector3(this.transform.position.x - 20, this.transform.position.y - 10, 0));

        //left bullet, smaller angle
        GameObject left2 = GameController.instance.createObject(wumpusBullet, transform.position);
        left2.GetComponent<WumpusBullet>().setTarget(new Vector3(this.transform.position.x - 10, this.transform.position.y - 10, 0));

        //bullet to the right
        GameObject right = GameController.instance.createObject(wumpusBullet, transform.position);
        right.GetComponent<WumpusBullet>().setTarget(new Vector3(this.transform.position.x + 20, this.transform.position.y - 10, 0));

        //bullet to the right, smaller angle
        GameObject right2 = GameController.instance.createObject(wumpusBullet, transform.position);
        right2.GetComponent<WumpusBullet>().setTarget(new Vector3(this.transform.position.x + 10, this.transform.position.y - 10, 0));
    }

    void shoot() {
        //Create a bullet at enemy's location and add it to the list
        GameObject bullet = GameController.instance.createObject(wumpusBullet, transform.position);
        bullet.GetComponent<WumpusBullet>().setTarget(new Vector3(this.transform.position.x + x, this.transform.position.y - 10, 0));
        //Debug.Log("(" + (this.transform.position.x + x) + ", " + (this.transform.position.y + y) + ")");
        x += 3;
    }
}
