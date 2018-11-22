using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public Transform target; //The player
	public int score; //Number of points to add to score

	//References to possible item drops
	public GameObject bulletDrop; 
	public GameObject medicine;

	public float speed; //How fast the enemy moves

	public Animator animator; //Reference to animator

	//Drops a random item on enemy position, called after enemy dies
	public void createDrops() {
		int dropType = Random.Range (0, 4);

		//If dropType is a 0, drop medicine
		if (dropType == 0) {
			GameController.instance.createObject(medicine, transform.position);
		}
		//If dropType is a 1 or 2, drop bullets
		if (dropType == 1) {
			GameController.instance.createObject(bulletDrop, transform.position);
		}
	}
}
