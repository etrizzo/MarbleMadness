// Emily Rizzo
// Game Manager script
// Creates and maintains list of marbles, initializes and stores the board of tiles.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Random;

public class GameManager : MonoBehaviour {
	public static int score = 0;	//players score
	public static int GEMSPAWN = 4;		//how often gems spawn
	public int LastAdded = 0;
	public static int NUMMARBLES = 4;
	public static int MINMARBLES = 2;

	public static int BOARDSIZE = 12;
	public static int offset = 0; //(int) BOARDSIZE / 2;
	GameObject marbleFolder;	// This will be an empty game object used for organizing objects in the Hierarchy pane.
	GameObject tileFolder;
	GameObject gemFolder;
	GameObject powerupFolder;
	List<Tile> tiles;			// This list will hold the tiles objects that are created
	public static List<Marble> marbles;
	List<Gem> gems;
	List<Powerup> powerups;
	public Tile[,] board;
	int gemType; 			// The next gem type to be created.
	int powerType;
	public float clock;
	public static bool go;
	public Camera cam;
	//public int numMarbles = 4;

	// Start is called once when the script is created.
	void Start () {
		go = false;
		this.clock = 0f;

		marbleFolder = new GameObject(); 
		marbleFolder.layer = 1;
		tileFolder = new GameObject();
		tileFolder.layer = 2;
		gemFolder = new GameObject ();
		powerupFolder = new GameObject ();
		gemFolder.layer = 1;
		tileFolder.name = "Tiles";
		marbleFolder.name = "Marbles";		// The name of a game object is visible in the hHerarchy pane.
		gemFolder.name = "Gems";
		powerupFolder.name = "Powerups";
		tiles = new List<Tile>();
		marbles = new List<Marble>();
		gems = new List<Gem> ();
		powerups = new List<Powerup> ();
		gemType = 1;
		powerType = 2;
		makeBoard();
	}

	// Update is called every frame.
	void Update () {
		if (NUMMARBLES < MINMARBLES) {
			go = false;
		}
		if (go) {
			this.clock = this.clock + Time.deltaTime;

			foreach (Marble m in marbles){			///move each marble
				m.Move();
			}

			if (this.clock > (LastAdded + GEMSPAWN)){		//check to add another gem/powerup
				LastAdded = (int) this.clock;
				int x = (int)Random.Range(0, BOARDSIZE);
				int y = (int) Random.Range(0,BOARDSIZE);
				while (board[x,y].marble || board[x,y].item){
					x = (int) Random.Range(0, BOARDSIZE);
					y = (int) Random.Range(0,BOARDSIZE);
				}
				int type = Random.Range (0, 2);
				//int type =1;
				if (type == 0) {
					addGem (x, y, board [x, y]);
				} else {
					addPowerup (x, y, board [x, y]);
				}
				board [x, y].item = true;
				//print ("added the gem");
			}
		}


	}

	void makeBoard () {
		this.cam = Camera.main;
		int height = (int)(BOARDSIZE / 2);
		int width = (int)(BOARDSIZE / 2);
		cam.orthographicSize = height + 2;
		cam.transform.position = new Vector3 (width , height , -12);


		board = new Tile[BOARDSIZE,BOARDSIZE];
		for (int row = 0; row < BOARDSIZE; row++) {
			//get a random int between 0 and BOARDSIZE for random turn tile
			int turn = (int) Random.Range(0, BOARDSIZE);
			for (int col = 0; col < BOARDSIZE; col++) {
				Tile tile = addTile(row - offset, col - offset);
				board [row, col] = tile;
				if (col == turn) {
					tile.makeTurn ();
				}
			}
		}
		//making sure each col has a turn. some rows may have 2 turns, which is chill
		for (int col = 0; col < BOARDSIZE; col++) {
//			if (!checkforturn (col, board)) {
//				int turn = (int)Random.Range (0, BOARDSIZE);
//				board [turn, col].makeTurn ();
//				//print ("tile: " + turn + " " + col + " is now a turn!");
//				//addGem (turn - offset, col - offset);
//			}
			int turn = (int) Random.Range(0, BOARDSIZE);
			while (board[turn,col].isTurn()){
				turn = (int) Random.Range(0, BOARDSIZE);
			}
			board [turn, col].makeTurn ();
		}
		addMarbles ();

	}

	void addMarbles(){
		int x;
		int y;
		Tile t;
		for (int i = 0; i < NUMMARBLES; i++) {
			x = Random.Range(0,BOARDSIZE);
			y = Random.Range(0,BOARDSIZE);
			while (board [x, y].marble) {
				x = Random.Range (0, BOARDSIZE);
				y = Random.Range (0, BOARDSIZE);
			}
			t = board [x, y];
			addMarble (t.getX (), t.getY (), t);
		}
	}

	bool checkforturn(int col, Tile[,] board){
		for (int i = 0; i < BOARDSIZE; i++){
			if (board[i, col].isTurn()){
				return true;
			}
		}
		return false;
	}

	Tile addTile(int x, int y) {
		//print ("Tile added at " + x + " " + y);
		GameObject tileObject = new GameObject();			// Create a new empty game object that will hold a gem.
		tileObject.name = "Tile Object";
		Tile tile = tileObject.AddComponent<Tile>();			// Add the Gem.cs script to the object.
		// We can now refer to the object via this script.
		tile.transform.parent = tileFolder.transform;			// Set the gem's parent object to be the gem folder.
		tile.transform.position = new Vector3(x,y,0);		// Position the gem at x,y.								

		//MAY need to add offset back?
		tile.init((int) x, (int) y);							// Initialize the gem script.

		tiles.Add(tile);										// Add the gem to the Gems list for future access.
		tile.name = "Tile "+tiles.Count;						// Give the gem object a name in the Hierarchy pane.

		return tile;							
	}

	Marble addMarble(float x, float y, Tile t){
		GameObject marbleObject = new GameObject();
		Marble marble = marbleObject.AddComponent<Marble> ();
		marble.transform.parent = marbleFolder.transform;
		print ("marble at: " + x + ", " + y);
		marble.transform.position = new Vector3 (x, y, -.15f);

		marble.init (t, this);
		marbles.Add (marble);
		marble.name = "Marble " + marbles.Count;
		for (int i = 0; i < Random.Range (0, 4); i++) {
			marble.turnRight ();
		}
		return marble;
	}

	void addGem(float x, float y, Tile t) {
		//print ("Added Gem at " + x + " " + y);
		GameObject gemObject = new GameObject();			// Create a new empty game object that will hold a gem.
		//gemObject.layer = 1;
		Gem gem = gemObject.AddComponent<Gem>();			// Add the Gem.cs script to the object.
		// We can now refer to the object via this script.
		gem.transform.parent = gemFolder.transform;			// Set the gem's parent object to be the gem folder.
		gem.transform.position = new Vector3(x,y,-.15f);		// Position the gem at x,y.								

		gem.init(gemType, t, this);							// Initialize the gem script.

		gems.Add(gem);										// Add the gem to the Gems list for future access.
		gem.name = "Gem "+gems.Count;						// Give the gem object a name in the Hierarchy pane.

		gemType = (int)Random.Range (1, 5);
		//int gemType = 1;							
	}


	void addPowerup(float x, float y, Tile t) {
		//powerType = 2;
		powerType = (int)Random.Range (1, 4);
		//print ("Added Gem at " + x + " " + y);
		GameObject gemObject = new GameObject();			// Create a new empty game object that will hold a gem.
		//gemObject.layer = 1;
		Powerup pow = gemObject.AddComponent<Powerup>();			// Add the Powerup.cs script to the object.
		// We can now refer to the object via this script.
		pow.transform.parent = powerupFolder.transform;			// Set the gem's parent object to be the gem folder.
		pow.transform.position = new Vector3(x,y,-.15f);		// Position the gem at x,y.								

		pow.init(powerType, t, this);							// Initialize the gem script.

		powerups.Add(pow);										// Add the gem to the Gems list for future access.
		pow.name = "Powerup "+powerups.Count;						// Give the gem object a name in the Hierarchy pane.



		//int gemType = 1;							
	}

	public void toggleStart(){
		if (go) {
			go = false;

		} else {
			go = true;

		}
	}

	// This function defines the buttons, and dictates what happens when they're pressed.
	void OnGUI () {
		
		if (NUMMARBLES < MINMARBLES) {
			GUI.Box (new Rect (Screen.width/2 - 50, Screen.height/2 - 25, 100, 50), "Game Over!");
		}

		if (GUI.Button (new Rect (-1, 30, 160, 30), "Exit Game")) {
			Application.Quit ();
		}
		GUI.Box (new Rect (Screen.width - 100, -1, 100, 30), "Score: " + score);
		GUI.Box (new Rect (-1, Screen.height - 200, 160, 30), "Marbles Remaining: " + NUMMARBLES);
		GUI.Box (new Rect (-1, Screen.height - 170, 160, 170), "Collect as many \ngems as \npossible.\n \n Powerups can \n restore a marble's \nhealth, \n grant a shield, \n or give you a 1-time \nturbo boost.");
		if (GUI.Button (new Rect (-1, -1, 160, 30), "Start"))
			this.toggleStart ();
		// Printing goes to the Console pane.  
		// If an object doesn't extend monobehavior, calling print won't do anything.  
		// Make sure "Collapse" isn't selected in the Console pane if you want to see duplicate messages.
	}

}
