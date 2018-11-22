using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaveManager : MonoBehaviour {

	private int numOfCaves = 30; //Number of caves
	private int cavesPerRow = 5; //Number of caves per row (see map)

	private int cavesTraversed; //Used to prevent an infinite loop when traversing caves

	private Map map; //Reference to map code, used to generate portals and journals

	private Cave[] cave; //Array of cave objects
	public GameObject[] exitObjects; //Reference to locations of exits
	public GameObject[] enemyLocations; //Reference to locations of enemy spawns
    public GameObject[] dropLocations; //Reference to locations for item drops

    public GameObject[] drops; //Reference to all possible drops
	public GameObject page; //Reference to pages, used to spawn pages
	public GameObject osborne; //Reference to Osborne sprite
	public GameObject[] enemies; //Array of all types of enemies
	public GameObject sally; //Reference to Sally, a type of enemy
	public GameObject portal; //Reference to portal
	public GameObject ozzyRun; //Reference to the ozzyrun animation, which is sometimes played
	public GameObject classroomBG; //Reference to the CS classroom sprite

	private int[] pageLocations; //Keeps track of where the pages will be located

	//Called when the game begins
	public void Init() {
		cavesTraversed = 0;

		//Enable the classroom bg, because that's cave0
		classroomBG = GameObject.Find ("ClassroomBG");
		classroomBG.SetActive(true);

		//Assigned map to the script by name
		map = GameObject.Find("Map").GetComponent<Map>();

		//Initializes map, so that it will have all the object locations
		map.Init ();

		//Creates pageLocations and sets them to random cave numbers
		//Each page will be located in a "row" of caves
		//Ex: manuscript 1 will be located in cave 0, 5, 10, 15, or 25
		//manuscript 2 will be located in cave 1, 6, 11, 16, or 26
		pageLocations = new int[5];
		string pageStr = "pages located in caves: ";
		for (int i = 0; i < pageLocations.Length; i++) {
			int location = Random.Range (0, 6);

			//If it's generating page location for the first row, it can't be in cave 0, because that's the CS classroom
			if (i == 0) {
				location = Random.Range (1, 6);
			}

			//Add the new generated location to pageLocations
			pageLocations [i] = location * 5 + i;

			pageStr += pageLocations [i] + " ";
		}
		//prints where the pages are located
		//Debug.Log (pageStr);

//		//example page generation
//		for (int i = 0; i < pageLocations.Length; i++) {
//			pageLocations [i] = i + 1;
//		}
        

		//example cave
//		cave = new Cave[] {
//			new Cave(new int[] { 2 }),
//			new Cave(new int[] { 3 }),
//			new Cave(new int[] { 1 }),
//			new Cave(new int[] { 0, 1, 2, 3, 4, 5 }),
//			new Cave(new int[] { 4 }),
//			new Cave(new int[] { 0 }),
//			new Cave(new int[] { 5 })
//		};

		//2d array that shows the location of each exit, clockwise
		//0 = top right, 1 = right, etc
		cave = new Cave[numOfCaves];
		for (int i = 0; i < numOfCaves; i++) {
			//Create a new cave of that cave number and adds it to the array
			cave [i] = new Cave (i);
		}

		//Creates cave structure
		createCave();

		//Prints cave structure
//		for (int i = 0; i < numOfCaves; i++) {
//			string str = "cave " + i + ": ";
//			Cave tempCave = cave [i];
//			for (int j = 0; j < tempCave.exits.Length; j++) {
//				str += tempCave.exits [j] + " ";
//			}
//			Debug.Log (str);
//		}
	}

	//Returns the locations of each page, used in Journal
	public int[] getPageLocations() {
		return pageLocations;
	}

	//Creates the new cave based on the exit used and the cave you just left
	public int SetupScene(int caveExit, int prevCave) {
		//Find the cave you are about to enter based on the exit location
		int nextCave = findNextCave(caveExit, prevCave);

		SetupScene (nextCave);

		//Returns the new cave number
		return nextCave;
	}

	//Enables the corresponding exits to the cave num
	//currentcave = the one you just exited
	//cave exit = the exit you took
	//returns the new cave you're in
	public void SetupScene(int nextCave) {
		
		Cave tempCave = cave [nextCave];

		//If the cave is 0, enable the classroom bg. otherwise, hide it
		if (nextCave == 0) {
			//GameObject.Find ("ClassroomBG").GetComponent<SpriteRenderer> ().enabled = true;
			classroomBG.SetActive(true);
		} else {
			//GameObject.Find ("ClassroomBG").GetComponent<SpriteRenderer> ().enabled = false;
			classroomBG.SetActive(false);
		}

		if (GameController.instance.endgame && nextCave == 0) {
			//if you have all pages and are in the apcs room, the wumpus appears
			GameObject ozzy = GameController.instance.createObject(osborne, new Vector3 (3.77f, 6.73f, 0f));

			//wumpusTime is called, which will start the dialogue/fight
			GameController.instance.wumpusTime (ozzy);

			//don't create exits and other stuff if the boss fight is happening, so just return
			return;
		} else if (map.sallyInCave(nextCave) && !GameController.instance.endgame) {
			//Create sally if the nextcave should have a sally
			GameController.instance.createObject(sally, new Vector3 (3.5f, 3.5f, 0f));

			//update sally's location so that she spawns somewhere else next time
			map.updateSally();
		} else if (nextCave != 0) {
			//Create enemies if the next cave isn't the CS classroom
			createEnemies ();

			//Create the ozzyrun animation if conditions are met
			spawnOzzyRun ();
		}

		//For every exit in that room
		for (int i = 0; i < tempCave.exits.Length; i++) {
			if (tempCave.exits [i] != -1) {
				//Get a reference to the corresponding exit object
				GameObject tempExit = exitObjects [tempCave.exits [i]];

				//And make it visible + give it a collider
				tempExit.SetActive (true);
			}
		}

        //Create drops if the room is not room 0 
        if (nextCave != 0) {
            createDrops();
        }

		//If there's a portal in the room, create the portal
		for (int i = 0; i < map.portals.Length; i++) {
			//Checking every single portal in the map portal array
			Location tempLocation = map.portals [i];

			//And if a portal matches the next cave
			if (nextCave == tempLocation.a || nextCave == tempLocation.b) {
				//Create the portal
				GameObject tempPortal = GameController.instance.createObject(portal, new Vector3 (3.5f, 3.5f, 0f));

				//Set the destination based on the location object's info
				if (nextCave == tempLocation.a) {
					tempPortal.GetComponent<Portal> ().destination = tempLocation.b;
				} else {
					tempPortal.GetComponent<Portal> ().destination = tempLocation.a;
				}

				Debug.Log("Destination: " + tempPortal.GetComponent<Portal>().destination);
			}
		}

		//Create journal pages if the cave is correct
		createPages(nextCave);

		//Debug.Log ("Current cave: " + nextCave);
	}

	//Creates ozzyrun animation
	private void spawnOzzyRun() {
		//Random num to see if it should be spawned
		int spawnOzzy = Random.Range (0, 7);
		if (spawnOzzy == 0) {
			//If it should, create ozzyrun and play the spooky sound

			Instantiate (ozzyRun, new Vector3 (-8f, 4f, 0f), Quaternion.identity);

			GameController.instance.playSound ("ozzyRun");
		}
	}

    //Create random drops in teh room
    private void createDrops() {
		//Random num to see what should be dropped
        int drop = Random.Range(0, 2);
        if (drop == 0) {
			//Random num to see how much should be dropped
            int numOfDrops = Random.Range(1, 4);

            //Keeps track if that location has been used for a drop
            bool[] spawned = new bool[dropLocations.Length];

            //for loop to spawn all drops
            for (int i = 0; i < numOfDrops; i++) {
                int location = -1;

                //Keep finding a new random location until that location hasn't been spawned from
                do {
                    location = Random.Range(0, dropLocations.Length);
                } while (spawned[location]);

				//Set it to true so another drop isn't spawned at that location
                spawned[location] = true;

                //Create drop and add it to the list
                int dropType = Random.Range(0, drops.Length);
                GameController.instance.createObject(drops[dropType], dropLocations[location].transform.position);
            }
        }
    }

	//Create enemies
	private void createEnemies() {
		//A random number of enemies from 1 to 4
		int numOfEnemies = Random.Range (1, 5);

		//Boolean array to keep track of which enemy locations have already been used
		bool[] spawned = new bool[enemyLocations.Length];

		//for loop to spawn all enemies
		for (int i = 0; i < numOfEnemies; i++) {
			int location = -1;

			//Keep finding a new random location until that location hasn't been spawned from
			do {
				location = Random.Range (0, enemyLocations.Length);
			} while (spawned [location]);

			spawned [location] = true;

			//Create enemy and add it to the list
			int enemyType = Random.Range(0, enemies.Length);
			GameController.instance.createObject (enemies [enemyType], enemyLocations [location].transform.position);
		}
	}

	//Create journal pages
	private void createPages(int currentCave) {
		//Check pagelocations array for which caves should contain a page
		for (int i = 0; i < pageLocations.Length; i++) {
			//If the cave should contain a page, create the page and return
			if (pageLocations [i] == currentCave) {
				GameObject tempPage = GameController.instance.createObject(page, new Vector3 (2.5f, 2.5f, 0f));

				//Set the page's location to the index
				tempPage.GetComponent<Page>().location = i;

				return;
			}
		}

		//If the cave shouldn't contain a page, check if the location is -1
		//That would mean that the page has already been collected
		for (int i = 0; i < pageLocations.Length; i++) {
			if (pageLocations [i] != -1) {
				return;
			}
		}
	}

	//Find the cave that connects to the exit you took, given the cave you just left
	private int findNextCave(int caveExit, int prevCave) {
		int nextCave = 0;

		//If the previous cave is located on an even row or not
		bool evenRow = prevCave % 10 >= 0 && prevCave % 10 <= 4;

		//Manually setting nextCave depending on where the prevCave is
		if (caveExit == 1) {
			nextCave = prevCave + 1;
		}
		if (caveExit == 4) {
			nextCave = prevCave - 1;
		}
		if (caveExit == 2) {
			if (evenRow) {
				nextCave = prevCave + cavesPerRow;
			} else {
				nextCave = prevCave + 1 + cavesPerRow;
			}
		}
		if (caveExit == 3) {
			if (evenRow) {
				nextCave = prevCave + cavesPerRow - 1;
			} else {
				nextCave = prevCave + cavesPerRow;
			}
		}
		if (caveExit == 0) {
			if (evenRow) {
				nextCave = prevCave - cavesPerRow;
			} else {
				nextCave = prevCave - cavesPerRow + 1;
			}
		}
		if (caveExit == 5) {
			if (evenRow) {
				nextCave = prevCave - 1 - cavesPerRow;
			} else {
				nextCave = prevCave - cavesPerRow;
			}
		}

		//Return nextCave
		return nextCave;
	}

	//Called by the player to show that the page was picked up
	public void obtainedPage(int location) {
		//Sets it to -1 so that caveManager won't create a new page in that cave again
		pageLocations [location] = -1;
	}

	//Deletes all the exits in the cave so they can be recreated
	public void deleteScene() {
		for (int i = 0; i < exitObjects.Length; i++) {
			exitObjects [i].SetActive (false);
		}
	}

	//Creates the original cave (sets the exit locations)
	private void createCave() {
		//Cave 0 will have one defined exit
		cave[0].setExit(1);
		//also set cave1's exit back to cave0
		cave[1].setExit(4);

		//For every cave after the CS classroom (cave0)
		for (int i = 1; i < numOfCaves; i++) {
			//Get the cave object
			Cave tempCave = cave [i];

			//For every single possible exit in tempCave
			for (int j = tempCave.currentIndex; j < tempCave.numOfExits; j++) {
				int exitIndex = getPossibleExit (tempCave);

				//Add the exit to the tempCave exit list
				tempCave.setExit (tempCave.possibleExits[exitIndex]);

				//Also have to add the exit to the cave that the exit leads to, so that both caves lead to each other
				int nextCaveNum = findNextCave(tempCave.possibleExits[exitIndex], i);
				Cave nextCave = cave [nextCaveNum];
				nextCave.setExit (findExitNum (i, nextCaveNum));
			}
		}

		//keep adding caves until all caves are reachable
		do {
			//will set cave.visited to true if that cave is reachable
			//traverseCaves (cave[0]);
			traverseCaves ();

			//for every cave that is not reachable, fix it
			for (int i = 0; i < numOfCaves; i++) {
				if (!cave [i].visited) {
					//if not visited, add an exit
					for (int exitIndex = 0; exitIndex < cave [i].possibleExits.Length; exitIndex++) {
						//for every single possible exit, if that exit hasn't already been used in the cave
						if (!contains (cave [i].exits, cave [i].possibleExits [exitIndex])) {
							//Add that exit to the cave's exits
							cave [i].setExit (cave [i].possibleExits [exitIndex]);

							//And add the corresponding exit to the cave that it connects to
							int nextCaveNum = findNextCave (cave [i].possibleExits [exitIndex], i);
							Cave nextCave = cave [nextCaveNum];
							nextCave.setExit (findExitNum (i, nextCaveNum));

							//After we've added one exit, we can break out of the for loop
							//And keep traversing caves until all of them can be reachable
							break;
						}
					}
				}
			}
		} while (!allCavesVisited ());
	}

	//check if all caves are visited-able
	private bool allCavesVisited() {
		bool visited = true;

		//set visited to whether or not all caves are visited
		for (int i = 0; i < cave.Length; i++) {
			//if any cave hasn't been visited yet, set visited to false
			if (cave [i].visited == false) {
				visited = false;
				break;
			}
		}

		//reset all caves to unvisited so the process can start again
		for (int i = 0; i < cave.Length; i++) {
			cave [i].visited = false;
		}

		return visited;
	}

	//traverse caves with a queue
	private void traverseCaves() {
		Queue cavesToVisit = new Queue ();
		cavesToVisit.Enqueue (cave [0]);

		//while the queue still has caves to traverse
		while (cavesToVisit.Count != 0) {
			//get a cave from the queue, visit it
			Cave temp = (Cave)cavesToVisit.Dequeue ();

			//set visited to true, because the current cave can be visited
			temp.visited = true;

			//for every single exit in the tempCave
			for (int i = 0; i < temp.exits.Length; i++) {
				if (temp.exits [i] != -1) {
					//Find the cave that the exit connects to
					int nextCaveNum = findNextCave (temp.exits [i], temp.caveNum);

					//If that cave hasn't been visited yet, add it to the queue
					if (!cave [nextCaveNum].visited) {
						cavesToVisit.Enqueue (cave [nextCaveNum]);
					}
				}
			}
		}
	}

	//traverse caves recursively
//	private void traverseCaves(Cave currentCave) {
//		//if the number of caves we've gone through is more than the numOfCaves, break so there isn't an error
//		if (cavesTraversed > numOfCaves) {
//			return;
//		}
//
//		//set visited to true because this cave is reachable, and increment traversed
//		currentCave.visited = true;
//		cavesTraversed++;
//
//		//for every single exit in currentcave
//		for (int i = 0; i < currentCave.exits.Length; i++) {
//			//-1 means that there aren't any more exits
//			if (currentCave.exits [i] == -1) {
//				return;
//			}
//
//			int nextCaveNum = findNextCave(currentCave.exits[i], currentCave.caveNum);
//
//			//if nextcave hasn't been visited yet, visit it
//			if (!cave [nextCaveNum].visited) {
//				traverseCaves (cave [nextCaveNum]);
//			}
//		}
//	}

	//returns a possible exit for the currentCave
	private int getPossibleExit(Cave currentCave) {
		int exitIndex = -1;

		//Find an exitIndex at where the exit at that location possibleExits isn't already in exits[]
		do {
			exitIndex = Random.Range (0, currentCave.possibleExits.Length);
		} while (contains (currentCave.exits, currentCave.possibleExits [exitIndex]));

		return exitIndex;
	}

	//Finds the exit that connects prevCave and nextCave; this exit is relative the nextCave
	private int findExitNum(int prevCave, int nextCave) {
		//If nextCave is on an even row or not
		bool evenRow = nextCave % 10 >= 0 && nextCave % 10 <= 4;

		//Manually sets the exit number
		if (nextCave - prevCave == 1) {
			return 4;
		}
		if (nextCave - prevCave == -1) {
			return 1;
		}
		if (evenRow) {
			if (nextCave - prevCave == cavesPerRow + 1) {
				return 5;
			}
			if (nextCave - prevCave == cavesPerRow) {
				return 0;
			}
			if (nextCave - prevCave == -cavesPerRow + 1) {
				return 3;
			}
			if (nextCave - prevCave == -cavesPerRow) {
				return 2;
			}
		} else {
			if (nextCave - prevCave == cavesPerRow) {
				return 5;
			}
			if (nextCave - prevCave == cavesPerRow - 1) {
				return 0;
			}
			if (nextCave - prevCave == -cavesPerRow) {
				return 3;
			}
			if (nextCave - prevCave == -cavesPerRow - 1) {
				return 2;
			}
		}

		return -1;
	}

	//returns if an array contains a number
	//Used to check if exits[] and other arrays already contain a certain location
	private bool contains(int[] arr, int num) {
		for (int i = 0; i < arr.Length; i++) {
			if (arr[i] == num) {
				return true;
			}
		}
		return false;
	}
}


//Cave class
class Cave {

	public int caveNum; //Number of cave
	public int[] exits; //actual exits that this cave has
	public int[] possibleExits; //possible exits that this cave has
	public int numOfExits; //number of exits this cave will have
	public bool visited; //if the cave has been generated or not

	public int currentIndex; //the index where the next exit will be added

	public Cave(int caveNum) {
		visited = false;
		this.caveNum = caveNum;

		possibleExits = getPossibleExits (caveNum);

		//Sets the number of exits from 3 - the number of possible exits, and creates an array of that length
		numOfExits = Random.Range (0, possibleExits.Length);
		exits = new int[possibleExits.Length];

		//Sets each element of exits to -1 to prevent an infinite loop
		for (int i = 0; i < exits.Length; i++) {
			exits [i] = -1;
		}

		currentIndex = 0;
	}

	//used for example cave
	public Cave(int[] exits) {
		this.exits = exits;
	}

	//Adds exitNum to exits[]
	public void setExit(int exitNum) {
		if (!contains(exitNum) && currentIndex < exits.Length) {
			//Add the exit to the array in increase the currentindex
			exits [currentIndex] = exitNum;
			currentIndex++;
		}
	}

	//checks if exits already contains the number
	private bool contains(int num) {
		for (int i = 0; i < exits.Length; i++) {
			if (exits[i] == num) {
				return true;
			}
		}
		return false;
	}

	//Gets an int[] of possible exits based on where the cave is located
	private int[] getPossibleExits(int caveNum) {
		if (caveNum == 0)
			return new int[] {1, 2};
		if (caveNum == 5 || caveNum == 15)
			return new int[] { 0, 1, 2, 3, 5 };
		if (caveNum == 10 || caveNum == 20)
			return new int[] { 0, 1, 2 };
		if (caveNum == 25)
			return new int[] { 0, 1, 5 };
		if (caveNum == 1 || caveNum == 2 || caveNum == 3)
			return new int[] { 1, 2, 3, 4 };
		if (caveNum == 26 || caveNum == 27 || caveNum == 28)
			return new int[] { 0, 1, 4, 5 };
		if (caveNum == 4)
			return new int[] { 2, 3, 4 };
		if (caveNum == 9 || caveNum == 19)
			return new int[] { 3, 4, 5 };
		if (caveNum == 14 || caveNum == 24)
			return new int[] { 0, 2, 3, 4, 5 };
		if (caveNum == 29)
			return new int[] { 4, 5 };
		return new int[] { 0, 1, 2, 3, 4, 5 };
	}
}