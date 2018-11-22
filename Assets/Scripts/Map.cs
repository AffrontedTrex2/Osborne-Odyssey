using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour {

	public int[] sally; //Cavenum where good dog sally will be
	public Location[] portals; //Array of rooms where the portals are located
	//portals[0] is connected to portals[1], and so on
	public int numOfPortals; //The size of portals[], the number of portals there will be

	public void Init() {
		numOfPortals = 2;

        //Initialize sally[] and sally[0] to be a random cave from 1-29
        sally = new int[2];
        updateSally();

		//Creates portal[] with portals that have -1 as values
		portals = new Location[numOfPortals];
		for (int i = 0; i < 2; i++) {
			portals [i] = new Location(-1, -1);
		}

		//Fills portal[] with portal objects
		for (int i = 0; i < numOfPortals; i++) {
			int a = -1;
			int b = -1;

			//Find a location that hasn't already been set
			do {
				a = Random.Range(1, 30);
				b = Random.Range(1, 30);
			} while (contains(a, b) ||  a == b);

			portals[i] = new Location(a, b);
		}
	}

    //Returns if the caveNum is where sally is located
    public bool sallyInCave(int caveNum) {
        return sally[0] == caveNum || sally[1] == caveNum;
    }

	//updates sally's location, location changes every time you meet her
	public void updateSally() {
        sally[0] = Random.Range(1, 30);
        sally[1] = sally[0];

        //Makes sure that sally[1] is different from sally[0]
        while (sally[1] == sally[0]) {
            sally[1] = Random.Range(1, 30);
        }

        Debug.Log(sally[0] + " " + sally[1]);
    }

	//returns if portals[] contains a number
	bool contains(int a, int b) {
		for (int i = 0; i < portals.Length; i++) {
			if (portals [i].a == a || portals [i].a == b ||
			    portals [i].b == a || portals [i].b == b) {
				return true;
			}
		}
		return false;
	}
}	

//Used to keep track of where the portals are connected
public class Location {
	public int a;
	public int b;

	public Location (int a, int b) {
		this.a = a;
		this.b = b;
	}
}