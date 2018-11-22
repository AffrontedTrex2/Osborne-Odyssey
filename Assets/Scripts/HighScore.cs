using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.IO;

public class HighScore: MonoBehaviour{

	// An array to store all of the top scores with the names of the players.
	// Accesses the HighScores text file.
	private List<String> lines = new List<String>();
	private StreamReader hs;
	private string path;

	// Get the path of the HighScore file 
	void Start() {
		path = "Assets/Resources/HighScores.txt";
	}


	// Get the top five highscores and update it to the file.
	public void getScores(){
		// Create reader to read the file.
		hs = new StreamReader(path);

		// Transfers all of the highscores into the ArrayList along with the new score.
		String line;
		int num = 0;
		int index = 0;

		// Add the new score to the ArrayList
		lines.Add(GameController.instance.score + "  " + GameController.instance.name);

		// Add all of the lines to the ArrayList
		while ((line = hs.ReadLine()) != null) {

			// Check if the name is the same.
				if ((line.ToLower ().IndexOf ((GameController.instance.name).ToLower ())) == -1) {
					lines.Add (line);

				} else {

				// Check if there is a num in the string.
				if (line.IndexOf ("  ") != -1) {
					// Get the num from the string 
					index = line.IndexOf ("  ");
					num = int.Parse(line.Substring(0, index));

					// If the new score is smaller than the already existing code in the file
					// then delete the new score and add the line from the file to the ArrayList
					if (GameController.instance.score < num) {
						lines.Remove ((GameController.instance.score + "  " + GameController.instance.name));
						lines.Add (line);
					}
				}
			
			}

		}

		// Variables to store the scores and indexes of the scores in the String.
		int num1 = 0;
		int num2 = 0;
		int index1 = 0;
		int index2 = 0;

		// Use selection sort to sort the HighScores.
		// Check if there is an int in the string and find the num to sort.
		for(int t = 0; t < lines.Count-1; t++){
			if (lines [t].IndexOf ("  ") != -1) {
				index1 = lines[t].IndexOf ("  ");
				num1  = int.Parse(lines[t].Substring(0, index1));
				for (int j = t+1; j < lines.Count; j++) {
					if (lines [j].IndexOf ("  ") != -1) {
						index2 = lines[j].IndexOf ("  ");
						num2 = int.Parse(lines[j].Substring(0, index2));
						if (num1 < num2) {
							String temp = lines [j];
							lines [j] = lines [t];
							lines [t] = temp;
						}
					}
				}
			}
		}


		// Close the reader for the file.
		hs.Close();

		// Writes the highscore in the file.
		System.IO.File.WriteAllText (path, lines[0] + "\n");

		// Adds rest of the four highscores in the file
		for (int n = 1; n < Math.Min(5, lines.Count); n++) {
			System.IO.File.AppendAllText (path, lines[n] + "\n");
		}

		// Clear the ArrayList
		lines.Clear ();


	}


}
