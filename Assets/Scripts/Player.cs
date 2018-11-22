using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	
	public float restartLevelDelay = 1f; //Delay creating this object so that it doesn't look funky
	public float speed = .05f; //Speed modifier

	public Animator animator; //Reference to the animation
	private Rigidbody2D rb2D; //The rigidbody attached to the object

	public GameObject bullet; //object used for bullets
	public GameObject[] playerLocations; //array of locations that the player will be located after entering a room

    private bool movedToRoom; //will be used to start new music
	private bool encounteredSally; //will be used to trigger a dialogue

	void Start() {
		//Get reference to animator
		animator = GetComponent<Animator>();

		//Gets a reference to the components
		rb2D = GetComponent<Rigidbody2D> ();

        movedToRoom = false;
		encounteredSally = false;
	}

	private void FixedUpdate() {
		//If lmb is pressed
		if (Input.GetButtonDown ("Fire1") && GameController.instance.bullets > 0) {
			Shoot ();
		}
	}

	private void Update() {
		//Gets player input
		float x = Input.GetAxisRaw ("Horizontal");
		float y = Input.GetAxisRaw ("Vertical");

		//Moves player
		Move (x, y, speed);
	}

	//Creates a bullet
	private void Shoot() {
		//Lose a bullet
		GameController.instance.changeBullets (-1);

		//Create the bullet
		GameController.instance.createObject (bullet, transform.position);

		//Play shooting sound
		GameController.instance.playSound ("shoot");
	}

	//Moves player
	private void Move(float x, float y, float speed) {
		//Starting position is the current position
		Vector2 start = transform.position;

		//Calculate the ending position
		Vector2 end = start + new Vector2(x * speed, y * speed);

		//Move player to new position
		rb2D.MovePosition(end);

		//Set animation depending on how the player moved
		if (y > 0) {
			backwards ();
		} else if (y < 0) {
			forwards ();
		}
		if (x > 0) {
			right ();
		} else if (x < 0) {
			left ();
		}

		if (x == 0 && y == 0) {
			resetAnim ();
		}
	}

	//Walking left anim
	private void left() {
		animator.SetBool ("left", true);
		animator.SetBool ("right", false);
		animator.SetBool ("forwards", false);
		animator.SetBool ("backwards", false);
	}

	//Walking right anim
	private void right() {
		animator.SetBool ("left", false);
		animator.SetBool ("right", true);
		animator.SetBool ("forwards", false);
		animator.SetBool ("backwards", false);
	}

	//Walking forwards anim
	private void forwards() {
		animator.SetBool ("left", false);
		animator.SetBool ("right", false);
		animator.SetBool ("forwards", true);
		animator.SetBool ("backwards", false);
	}

	//Walking backwards anim
	private void backwards() {
		animator.SetBool ("left", false);
		animator.SetBool ("right", false);
		animator.SetBool ("forwards", false);
		animator.SetBool ("backwards", true);
	}

	//No movement anim
	private void resetAnim() {
		animator.SetBool ("left", false);
		animator.SetBool ("right", false);
		animator.SetBool ("forwards", false);
		animator.SetBool ("backwards", false);
	}

	//Called when another object collides with the player
	private void OnTriggerEnter2D(Collider2D other) {
		//If the collision was with an exit
		if (other.gameObject.tag == "Exit") {
			//Restart the level
			//Invoke("Restart", restartLevelDelay);

			//Get which exit the player exited through
			int exitDir = getExitDir (other.gameObject);

			//Creates the new cave
			GameController.instance.createCave (exitDir, true);

			//Sets new player position that reflects which exit was just taken
			this.transform.position = playerLocations [exitDir].transform.position;

			//if this is the first room you've moved to, change the music
			if (!movedToRoom) {
				movedToRoom = true;
				GameController.instance.playBGM ();
			}

		} else if (other.gameObject.tag == "Food") {
			//Add life
			changeLife (1);

			//Destroy other object
			GameController.instance.destroyObject (other.gameObject);

			//Play sound
			GameController.instance.playSound ("pickup");

		} else if (other.gameObject.tag == "SallyEnemy") {
            //acts as hole for sending player back to the beginning 
			//Creates the new cave
			GameController.instance.createCave (0, false);

			//Moves the player to the main CS room
			this.transform.position = playerLocations [1].transform.position;

            //Displays sally dialogue if it's the first time you met the dog
			if (!encounteredSally) {
				encounteredSally = true;
				GameController.instance.sallyDialogue ();
			}

		}else if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "ShootingEnemy") {
			//Lose life
			changeLife (-1);

			//Destroy other object
			GameController.instance.destroyObject (other.gameObject);

			//Play sound
			GameController.instance.playSound ("hurt");

		} else if (other.gameObject.tag == "EnemyBullet") {
			//Play sound
			GameController.instance.playSound ("hurt");

		} else if (other.gameObject.tag == "Page") {
			//Get the page
			addPage ();

			//So that the cave won't instantiate another page in the specific location
			GameController.instance.obtainedPage (other.gameObject.GetComponent<Page> ().location);

			//Destroy page
			GameController.instance.destroyObject (other.gameObject);

			//Play sound
			GameController.instance.playSound ("pickup");

		} else if (other.gameObject.tag == "BulletDrop") {
			//Add bullets
			GameController.instance.changeBullets (10);

			//Destroy bullet
			GameController.instance.destroyObject (other.gameObject);

			//Play sound
			GameController.instance.playSound ("pickup");
		} else if (other.gameObject.tag == "Portal") {
			//Teleport the player to the new location
			Portal portalScript = other.gameObject.GetComponent<Portal> ();

			//Delete the portal
			GameController.instance.destroyObject (other.gameObject);

            //Change player location so you don't get stuck in an infinite loop
            //			Vector3 newLocation = new Vector3(5f, 3.5f, 0);
            //			if (transform.position.x - other.gameObject.transform.position.x < -.5f) {
            //				newLocation.x = 5f;
            //			} else if (transform.position.x - other.gameObject.transform.position.x > 1f) {
            //				newLocation.x = 2f;
            //			} else if (transform.position.y - other.gameObject.transform.position.y < -1f) {
            //				newLocation.y = 5f;
            //			} else if (transform.position.y - other.gameObject.transform.position.y > 1f) {
            //				newLocation.y = 2f;
            //			}
            //			this.transform.position = newLocation;

            //Plays portal sound effect
            GameController.instance.playSound("portal");

			//Gets player input
			float x = Input.GetAxisRaw ("Horizontal");
			float y = Input.GetAxisRaw ("Vertical");

			//Move player to new position so you don't infinitely teleport back and forth
			transform.position = (Vector2)transform.position + new Vector2(2f * x, 2f * y);

			//Creates the new cave
			GameController.instance.createCave (portalScript.destination, false);
		}
	}

	//Add page to the journal
	private void addPage() {
		GameController.instance.addPage ();
	}

	//Get the number of the exit based on the exit's name
	private int getExitDir(GameObject exit) {
		if (exit.name == "Exit_Top_Right") {
			return 0;
		} else if (exit.name == "Exit_Right") {
			return 1;
		} else if (exit.name == "Exit_Bottom_Right") {
			return 2;
		} else if (exit.name == "Exit_Bottom_Left") {
			return 3;
		} else if (exit.name == "Exit_Left") {
			return 4;
		} else if (exit.name == "Exit_Top_Left") {
			return 5;
		}
		return -1;
	}

	private void Restart() {
		//Reload the scene
		SceneManager.LoadScene (0);
	}

	public void changeLife(int health) {
		//Change health
		GameController.instance.changeHealth (health);

		//Check if game is over
		CheckIfGameOver();
	}

	//If you have no health, then call gameover
	private void CheckIfGameOver() {
		if (GameController.instance.health < 0) {
			GameController.instance.GameOver();
		}
	}
}

