// Emily Rizzo
// Tile Class

// Script for a tile object.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	private TileModel tileModel;		// The model object.
	private TurnModel turnModel;
	public Marble marble;
	private bool turn;
	private bool active;
	public bool item;
	private GameObject models;
	private BoxCollider2D boxcollider;
	private int turnType;	// -1 noturn, 0 SE, 1 SW, 2 NW, 3 NE
	private int x;
	private int y;

	public void init(int x, int y) {
		this.turn = false;
		this.active = false;
		this.marble = null;
		this.turnType = -1;
		this.x = x;
		this.y = y;

		this.models = new GameObject();
		this.models.name = "Models";
		this.models.transform.parent = this.transform;
		var modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);	// Create a quad object for holding the tile texture.
		modelObject.name = "Tile Quad";
		modelObject.transform.parent = models.transform;
		modelObject.layer = 0;
		this.tileModel = modelObject.AddComponent<TileModel>();						// Add a tileModel script to control visuals of the tile.
		this.tileModel.init(this);					
	}

	public void makeTurn(){
		this.turn = true;
		this.active = true;
		this.turnType = 0;		//default SE turn, then rotated randomly in GameManager
		var modelObject = GameObject.CreatePrimitive(PrimitiveType.Quad);	// add a second model for the turn
		modelObject.transform.parent = models.transform;
		modelObject.layer = 1;
		this.turnModel = modelObject.AddComponent<TurnModel> ();
		this.turnModel.init(this);
	}



	public bool isTurn(){
		return this.turn;
	}

	public bool isActive(){
		return this.active;
	}


	public void rotate(){
		//rotate the turn model
		this.turnModel.transform.Rotate (0, 0, -90);
		//maybe update the orientation of the ports
		this.turnType = (turnType + 1) % 4;
	}

	public int getX(){
		return this.x;
	}
	public int getY(){
		return this.y;
	}

	public void addMarble(Marble m){
		//this.marbles.Add (m);
		this.marble = m;
	}

	public void removeMarble(){
		this.marble = null;
//		if (this.marbles.Contains (m)) {
//			this.marbles.Remove (m);
//		}
//		if (this.marbles.Count == 0) {
//			this.marble = false;
//		}
	}


	public int type(){
		return this.turnType;
	}

	//returns true if entry from a certain direction will turn on the tile.
	public bool turnsHere(int entry){
		if (entry == 0)	{ //enters from south
			if (this.turnType == 0 || this.turnType == 1){
				return true;
			}
		} else if (entry == 1){	//enters from west
			if (this.turnType == 1 || this.turnType == 2){
				return true;
			}
		} else if (entry == 2){ //enters from north
			if (this.turnType == 2 || this.turnType == 3){
				return true;
			}
		} else if (entry == 3) {	//enters from east
			if (this.turnType == 3 || this.turnType == 0){
				return true;
			}
		}
		return false;
	}
		



}

