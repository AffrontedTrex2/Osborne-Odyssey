using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour {

	public static GameController instance = null; //Static instance of this object
	private CaveManager caveScript; //Reference to cave, which will set up the map
	private UI ui; //Reference to UI
	private SoundManager sound; //Reference to sound object
	private Player player; //Reference to player
	private Journal journal; //Reference to journal
	private HighScore upScore; //Reference to highscore
    //private Button forName;

    private Animator introAnim; //Intro animation
	public AnimationClip exitClip; //Clip of mc exiting cs classroom

	private Animator credits; //Reference to credits animator

	private GameObject ozzy; //Reference to the objects that will spawn in the final battle
	public GameObject wumpus1; //Reference to wumpus form1
	public GameObject wumpus2; //Reference to wumpus form2

	//Reference for UI when asking for player name
	public GameObject menuUI; 
	public GameObject getNameUI;
	public InputField input;

	public bool endgame; //if the final scene has started
	public bool battleStart; //if the battle has started: used to tell the journal not to open journal at that time
	public bool form1dead; //used by dialogue to see if it should call wumpustime2()
	public bool form2dead; //used by dialogue; if wumpus2 is dead
	private bool firstcave; //used to trigger the exit animation

	private ArrayList objectsOnScreen; //Keeps track of which objects are on the screen

	public int score = 0;
	public int health = 10;
	public int bullets = 100;

    public string name; //Player name, asked for in the beginning of the game
    public bool cutscenePlaying; //Will be used to prevent player from opening journal/pause during cutscenes

	public int currentCave = 0; //current cave that the player is in

	//Called before Start
	void Awake() {
		//Check if instance exists
		if (instance == null) {
			//If not, set it up
			instance = this;
			//If it's already set up and it's not this, destroy it
		} else if (instance != this) {
			Destroy (gameObject);
		}

        //Get the intro animation object
        introAnim = GameObject.Find("IntroAnim").GetComponent<Animator>();

        //Instantiate the arraylist
        objectsOnScreen = new ArrayList();

		//Don't destroy this when reloading a scene
		//DontDestroyOnLoad(gameObject);

		//Get caveScript, which is attached to the Cave object
		caveScript = GameObject.Find("Cave").GetComponent<CaveManager>();

		//Find sound by name
		sound = GameObject.Find("Sound").GetComponent<SoundManager>();
        sound.Init();

        //Find ui by name
        ui = GameObject.Find("UI").GetComponent<UI>();
		ui.Init ();

		//Find player by name
		player = GameObject.Find("Player").GetComponent<Player>();

		//Find journal by name
		journal = GameObject.Find("Journal").GetComponent<Journal>();

		//Update highscores
		upScore = GameObject.Find ("HighScore").GetComponent<HighScore> ();

		//Get credits animator
		credits = GameObject.Find ("Credits").GetComponent<Animator> ();
        
		//Show the UI for getting the player's name, and pauses the game
        menuUI.SetActive (false);
		getNameUI.SetActive (true);
		Time.timeScale = 0f;
		input.ActivateInputField();

		//Initialize the game by creating cave 0
		caveScript.Init();
		createCave(0, false);

		//Start the intro conversation
		GameObject.Find("IntroDialogue").GetComponent<Dialogue>().StartDialogue();

		//Set all bools to false, don't allow the player to move/shoot yet
		player.enabled = false;
		battleStart = false;
		form1dead = false;
		form2dead = false;
		endgame = false;
        cutscenePlaying = true;

		//Firstcave = if the player has not yet gone to a different cave yet
		firstcave = true;
	}

    //Starts wumpus form1 dialogue
    public void wumpusTime(GameObject ozzy) {
        //plays the audio for the final battle
        sound.playWumpus();

		battleStart = true;

		//First start the dialogue
		GameObject.Find ("EndgameDialogue").GetComponent<Dialogue> ().StartDialogue ();
		player.enabled = false;
		this.ozzy = ozzy;
	}

	//Sets name to the player input
	public void getString(){
		name = input.text;
	}

	//Close name UI after the user presses enter
	public void nameEnd(){
		//Hides UI and resumes game
		getNameUI.SetActive (false);
		menuUI.SetActive (true);
		Time.timeScale = 1f;
		input.DeactivateInputField();

		//Plays intro animation and helloWorld audio
        introAnim.SetTrigger("start");
        sound.playHello();
	}

	//Starts the wumpus form1 fight
	public void wumpusTime() {
		//Delete the regular ozzy....
		objectsOnScreen.Remove(ozzy);
		Destroy (ozzy);

		//And create the wumpus! (form 1)
		GameObject wumpusEnemy = Instantiate (wumpus1, new Vector3 (3.8f, 6.73f, 0f), Quaternion.identity);
		objectsOnScreen.Add (wumpusEnemy);
	}

	//called after wumpus1 dies, shows the dialogue
	public void defeatFormOneDialogue(GameObject ozzy, GameObject wumpus1) {
        //Delete form1 cause it's dead
		GameController.instance.objectsOnScreen.Remove(wumpus1);
        Destroy(wumpus1);

		//Play new dialogue
		this.ozzy = ozzy;
		form1dead = true;
		GameObject.Find ("DefeatFormOneDialogue").GetComponent<Dialogue> ().StartDialogue ();
		player.enabled = false;
	}

	//Spawns form 2, called by dialogue after it ends
	public void wumpusTime2() {
		//Destroy the placeholder sprite
		Destroy (ozzy);

		//Spawn form2
		GameObject wumpusEnemy = Instantiate (wumpus2, new Vector3 (3.91f, 6.45f, 0f), Quaternion.identity);
		objectsOnScreen.Add (wumpusEnemy);
	}

	//called after you kill form 2
	public void defeatFormTwoDialogue(GameObject wumpus2) {
		//Delete wumpus2 cause it's dead
		GameController.instance.objectsOnScreen.Remove(wumpus2);
		Destroy(wumpus2);

		//plays "goodbye world"
		sound.playGoodbye();

		//Plays new dialogue
		form2dead = true;
		GameObject.Find ("DefeatFormTwoDialogue").GetComponent<Dialogue> ().StartDialogue ();
		player.enabled = false;

        //Delete everything on screen
        destroyAll();
	}

	//Called when the player picks up a page; location is the cave number
	public void obtainedPage(int location) {
		caveScript.obtainedPage (location);
	}

	//Adds a page to the journal
	public void addPage() {
		journal.availableIndex++;

        //Pause stuff if a journal dialogue is about to start
        if (journal.availableIndex == 1 || journal.availableIndex == 2 || journal.availableIndex == 5) {
            player.enabled = false;
            pauseGameobjects();
        }
		//show tutorial for opening the journal if this is the first page found
		if (journal.availableIndex == 1) {
			GameObject.Find ("JournalDialogue").GetComponent<Dialogue> ().StartDialogue ();
        }
		//show tutorial for turning pages if this is the second page found
		if (journal.availableIndex == 2) {
			GameObject.Find ("JournalDialogue2").GetComponent<Dialogue> ().StartDialogue ();
		}
		//show last dialogue tutorial if this is the last page
		if (journal.availableIndex == 5) {
			GameObject.Find ("JournalDialogue3").GetComponent<Dialogue> ().StartDialogue ();
		}
	}

	public void sallyDialogue() {
		GameObject.Find ("SallyDialogue").GetComponent<Dialogue> ().StartDialogue ();
	}

	//Change health
	public void changeHealth(int change) {
		//If you already have max health, you get more points
		if (change > 0 && health >= 10) {
			changeScore (5);
		} else {
			//Else update health and the UI
			health += change;
			ui.updateHealth ();
		}
	}

	//Changes score and updates UI
	public void changeScore(int change) {
		score += change;
		ui.updateScore ();
		upScore.getScores ();

	}

	//Changes bullet count and calls UI
	public void changeBullets(int change) {
		bullets += change;
		ui.updateBullets ();
	}

	//Changes the scene to the "game over" scene
	//Also plays "goodbye world" and stops BGM music
	public void GameOver() {
        sound.playGoodbye();
		sound.GetComponent<AudioSource> ().Stop ();
		SceneManager.LoadScene (2);
	}

	//Destroys all objects on the screen
	public void destroyAll() {
		for (int i = 0; i < objectsOnScreen.Count; i++) {
			Destroy ((GameObject)objectsOnScreen [i]);
		}
	}

	//Plays the exit scene animation, and waits until the animation is done
	//Before setting up the cave
	private IEnumerator playExitAnim(int cave, bool isExit) {
		introAnim.SetTrigger ("exit");
        cutscenePlaying = true;
        player.enabled = false;

		yield return new WaitForSeconds (exitClip.length / .1f - 1.5f);

		createCaveHelper (cave, isExit);
        player.enabled = true;
        cutscenePlaying = false;
    }

	//Plays certain sound from the soundManager
	public void playSound(string soundStr) {
		sound.playSound (soundStr);
	}

	//Creates the map
	public void createCave(int cave, bool isExit) {
		//If it's the first cave, then play the anim first
		if (firstcave) {
			firstcave = false;
			StartCoroutine (playExitAnim (cave, isExit));
		} else {
			//Else just create the cave as usual
			createCaveHelper (cave, isExit);
		}
	}

	//Creates the next cave
	private void createCaveHelper(int cave, bool isExit) {
        //Deletes the current cave exits
        caveScript.deleteScene();

        //Deletes all the objects on the screen
        destroyAll();

        //If cave is the exit number
        if (isExit) {
			//Sets the new cave number and sets up the new cave
			currentCave = caveScript.SetupScene (cave, currentCave);
		} else {
			//If cave is the actual cave number, set new cave number and set up new cave
			currentCave = cave;
			caveScript.SetupScene (cave);
		}

		//If the cave you are in is adjacent to the APCS class (room 1 or 5), then play the monologue
		if (journal.availableIndex == 5) {
			float volume = 0f;
			if (currentCave == 1 || currentCave == 5) {
				volume = 0.9f;
			}
			if (currentCave == 2 || currentCave == 6 || currentCave == 10 || currentCave == 11) {
				volume = 0.6f;
			}
			if (currentCave == 3 || currentCave == 7) {
				volume = 0.3f;
			}
			//Change volume depending on proximity to APCS class
			sound.changeMonologueVolume(volume);
		} else {
			//If you don't have all journal pages, the volume is 0
			sound.changeMonologueVolume(0f);
		}

		//Update the ui to show the currentCave
		ui.updateCave (currentCave);
	}

	//Creates new object
	public GameObject createObject(GameObject obj, Vector3 location) {
		//Creates object at the location
		GameObject temp = Instantiate (obj, location, Quaternion.identity);

		//Add the new object to the list
		objectsOnScreen.Add (temp);

		//Returns reference to the created object
		return temp;
	}

	//Destroy certain object and remove it from the list
	public void destroyObject(GameObject obj) {
		Destroy (obj);
		objectsOnScreen.Remove (obj);
	}
		
	//Called after you defeat wumpus2 and see the dialogue/animation
	public void winGame() {
		cutscenePlaying = true;

        //play clapping sound effect
        sound.playClap();

        //plays win game music
        sound.playWinMusic();

		//Play the credits animation
		credits.SetTrigger("StartCredits");
	}

	//Returns page locations
	public int[] getPageLocations() {
		return caveScript.getPageLocations();
	}

    //pause all gameobjects (used for journal dialogue)
    public void pauseGameobjects() {
        cutscenePlaying = true;

		//For every single object on screen
        for (int i = 0; i < objectsOnScreen.Count; i++) {
            GameObject obj = (GameObject)objectsOnScreen[i];

			//Prevents an error if the obj is null
            if (obj == null) {
                continue;
            }

			//Stop the obj from updating
            if (obj.tag.Equals("EnemyBullet")) {
                //obj.GetComponent<EnemyBullet>().enabled = false;
                //obj.GetComponent<Bullet>().enabled = false;
                //obj.GetComponent<EnemyBullet>().GetComponent<Bullet>().enabled = true;
            } 
            if (obj.tag.Equals("Enemy")) {
                obj.GetComponent<MovingEnemy>().enabled = false;
            }
            if (obj.tag.Equals("ShootingEnemy")) {
                obj.GetComponent<ShootingEnemy>().CancelInvoke();
            }
			if (obj.tag.Equals ("SallyEnemy")) {
				obj.GetComponent<SallyEnemy> ().enabled = false;
			}
        }
    }

    //resume all gameobjects (used for journal dialogue)
    public void resumeGameobjects() {
        cutscenePlaying = false;

		//For every single object
        for (int i = 0; i < objectsOnScreen.Count; i++) {
            GameObject obj = (GameObject)objectsOnScreen[i];

			//Make sure it's not null
            if (obj == null) {
                continue;
            }

			//Resume the updating and enable them
            if (obj.tag.Equals("EnemyBullet")) {
                //obj.GetComponent<EnemyBullet>().enabled = true;
                //obj.GetComponent<Bullet>().enabled = true;
                //obj.GetComponent<EnemyBullet>().GetComponent<Bullet>().enabled = false;
            }
            if (obj.tag.Equals("Enemy")) {
                obj.GetComponent<MovingEnemy>().enabled = true;
            }
            if (obj.tag.Equals("ShootingEnemy")) {
                obj.GetComponent<ShootingEnemy>().startAttacking();
            }
			if (obj.tag.Equals ("SallyEnemy")) {
				obj.GetComponent<SallyEnemy> ().enabled = true;
			}
        }
    }

	//Pause time and stop the player from moving/shooting
	public void pause(){
		Time.timeScale = 0f;
		player.enabled = false;
		cutscenePlaying = true;
		pauseGameobjects ();
	}

	//Resume
	public void resume(){
		Time.timeScale = 1f;
		player.enabled = true;
		cutscenePlaying = false;
		resumeGameobjects ();
	}

	//Plays BGM
    public void playBGM() {
        sound.playBGM();
    }

}
