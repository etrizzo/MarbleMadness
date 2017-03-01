// Emily Rizzo
// Marble Class

// Script for the marble objects in the game. Manages health, speed, power-ups, and behavior for a marble.
// This script primarily controls movement for marbles by keeping track of their position, the tile that the marble is currently on, and how long it has been on the tile.
// at default speed, the marble spends .6 seconds in a tile. Using this and the time spent in a tile, percentage through the tile is determined. This is used to calculate position 
// and triggers turns/collisions.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Marble : MonoBehaviour
{

	private MarbleModel model;		// The model object.
	private GameManager m;			// A pointer to the manager

	public float DEFAULTSPEED = .6f;		//default speed for the game (measured in seconds spent in a tile)
	public float DEFAULTBOOST = .6f;		//default boost ratio for the game
	public float turboBoost = .2f;			//difference between a regular boost and turbo boost
	public float boostratio;

	public int x;
	public int y;
	public int power;		//0 - none, 1- shield, 2- health(irr), 3-turbo boost
	public Tile tile;
	public float percThrough;
	public float percent;


	public int dir;		//0 north, 1 east, 2 south, 3 west
	public float clock;
	public float tileclock;

	public int health = 5;

	public Vector3 direction;


	public float speed;		// seconds/tile
	public bool boost;
	public float boostStart;
	public bool tired;
	public float tiredStart;
	public bool speedingUp;
	public bool slowingDown;
	public bool pause;
	public bool turbo = false;

	public bool gotNext = false;
	public bool canMoveForward = true;
	public Tile next = null;

	public BoxCollider2D col;


	public void init (Tile t, GameManager m)
	{
		this.m = m;
		this.x = t.getX ();
		this.y = t.getY ();
		this.tile = t;
		this.tile.addMarble (this);
		this.percThrough = .5f;		//to start in middle of tile
		this.percent = .5f;

		this.speed = DEFAULTSPEED;
		this.boostratio = DEFAULTBOOST;
		this.clock = 0f;
		this.tileclock = this.speed / 2;	//start at middle of tile

		this.dir = 0;
		this.direction = new Vector3 (0f, 1f, 0f);
		this.boost = false;
		this.tired = false;
		this.speedingUp = false;
		this.slowingDown = false;
		this.power = 0;
		this.pause = false;

		var modelObject = GameObject.CreatePrimitive (PrimitiveType.Quad);	// Create a quad object for holding the gem texture.
		this.model = modelObject.AddComponent<MarbleModel> ();						// Add a gemModel script to control visuals of the gem.
		this.model.init (this, modelObject);	

		MeshCollider mcol = modelObject.GetComponent<MeshCollider> ();			//remove the mesh collider from themodel, and replace with rigidbody and box collider (for picking up gems/powerups)
		if (mcol != null) {
			DestroyImmediate (mcol);
		}
		Rigidbody2D rb = modelObject.AddComponent<Rigidbody2D> ();
		rb.isKinematic = true;
		this.col = modelObject.AddComponent<BoxCollider2D> ();
		this.col.name = "Marble Collider";
		this.col.size = new Vector2 (.5f, .5f);
		this.col.isTrigger = true;

	}

	//gets the next tile the marble will enter, and updates this.next.
	public Tile getNextTile ()
	{
		print ("STARTING: " + this.percent + "/" + this.percThrough);
		int x = this.tile.getX ();
		int y = this.tile.getY ();
		int newx;
		int newy;
		//get next tile based on the direction the marble is currently going, and the current position.
		if (this.dir % 2 == 0) {	//get the tile to either the north or the south
			newy = (y - (this.dir - 1)) % GameManager.BOARDSIZE;
			if (newy < 0) {
				newy = newy + GameManager.BOARDSIZE;
			}
			newx = x;
		} else {	// get the tile to the east or west
			newx = (x - (this.dir - 2)) % GameManager.BOARDSIZE;
			if (newx < 0) {
				newx = newx + GameManager.BOARDSIZE;
			}
			newy = y;
		}
		this.next = this.m.board [newx, newy];
		return this.next;


	}

	//Checks if the marble currently has a powerup, and checks if the marble is currently in boost mode.
	public void checkPower ()
	{
		if (this.power == 3) {		//if the power is a boost, update current boost ratio
			this.boostratio = this.DEFAULTBOOST - this.turboBoost;
			if (this.boost) {
				this.power = 0;
				this.turbo = true;
			}
		} else if (this.power == 1) {
			this.health += 2;
			if (this.health > 5) {
				this.health = 5;
			}
			this.power = 0;
		}


		//if in boost mode, the marble will speed up to a maximum speed for .4 secs, travel at that speed for 1.1 seconds, then slow down until it reaches default speed again.
		//After finishing a boost, the marble will be tired for 4 seconds, and will not be able to boost.
		if (this.boost) {
			if (this.clock < this.boostStart + .4) {		//the marble is in the initial speed-up part of the boost
				this.speedingUp = true;
				if (this.speed > this.DEFAULTSPEED * this.boostratio) {
					this.speed = this.speed - (Time.deltaTime);
				} else {
					this.speed = this.DEFAULTSPEED * this.boostratio;
				}
			}
			if ((this.clock > this.boostStart + .4) && (this.clock < this.boostStart + 1.5)) {		//the marble is travelling at top speed
				this.speedingUp = false;
				this.speed = this.DEFAULTSPEED * this.boostratio;
			}
			if (this.clock > this.boostStart + 1.5) {			//the marble is slowing down
				if (this.speed < this.DEFAULTSPEED) {
					this.slowingDown = true;
					if (this.speed + (Time.deltaTime / 2) < this.DEFAULTSPEED) {		//decrement speed until it reaches default speed
						this.speed = this.speed + (Time.deltaTime / 2);
					} else {
						this.speed = this.DEFAULTSPEED;
					}
				} else {					//after the boost has ended, set the marble to be tired.
					if (this.turbo) {				//if the boost WAS a turbo boost, return boost ratio to normal.
						this.boostratio = DEFAULTBOOST;
						this.turbo = false;
					}
					this.slowingDown = false;
					this.speed = this.DEFAULTSPEED;
					this.boost = false;
					this.tired = true;
					this.tiredStart = this.clock;
				}
			}
		}

		if (this.tired) {			//check how long the marble has been tired since finishing last boost.
			if (this.clock > this.tiredStart + 4) {
				this.tired = false;
			}
		}
			
	}



	public void Move ()
	{
		if (GameManager.go && !(this.pause && this.canMoveForward)) {			//if the game is not paused and the marble can move forward

			//find the percentage of the tile that the marble has traveled through based on POSITION
			if (this.dir % 2 == 0) {
				this.percent = Mathf.Abs (((this.gameObject.transform.position.y + .5f) % 1));
				if (this.dir == 2) {
					this.percent = 1 - this.percent;
				}
			} else {
				this.percent = Mathf.Abs (((this.gameObject.transform.position.x + .5f) % 1));
				if (this.dir == 3) {
					this.percent = 1 - this.percent;
				}
			}

			this.checkPower ();		//applies powers, and checks for boost usage.

			this.clock = this.clock + Time.deltaTime;		//game time
			if (!this.pause) {
				this.tileclock = this.tileclock + Time.deltaTime;		//time spent on the tile
			
			}
			this.percThrough = (this.tileclock % this.speed) / this.speed;		//finds percentage of the tile that the marble has traveled through based on TIME SPENT IN TILE


			if (this.percent > .3 || !this.canMoveForward) {
				
				if (next != null && this.canMoveForward) {		//if you have already gotten next tile and can move forward, check to see if the marble is moving to a new tile
					if (this.percent > .9) {				//the marble's position is almost off of the tile, the tile should be read as vacant.
						this.tile.removeMarble ();
					}

					if ((this.tileclock / this.speed) > 1) {		//the marble is switching tiles
						this.tile.removeMarble ();						//remove marble from old tile
						this.tile = next;								//update current tile to be the "next" tile
						this.tile.marble = this;						//set new tile to have this marble
						this.tileclock = 0f;							//update tileclock and percThrough
						this.percThrough = 0f;
						this.next = null;								//update no next tile
					}
				} else {								//either no next tile, or can't move forward
					this.next = this.getNextTile ();
					if (this.next.marble == null || (this.next.marble == this)) {		//if there's no marble in front of you, unpause, this means an old obstruction has moved
						if (!this.canMoveForward) {
							this.pause = false;				
						}
						this.next.marble = this;
						this.canMoveForward = true;			
					} else {							//otherwise there is a marble in front of you. collide and pause marble.
						if (this.canMoveForward) {			//the first time a marble is encountered, decrement both marble's health
							this.Hit ();
							this.next.marble.Hit ();
						}
						this.canMoveForward = false;		//cannot move forward, pause
						this.pause = true;
					}
				}
			}
					

				
			Vector3 update = this.translation (this.dir, this.percThrough, this.tile);		//this finds the marbles new position based on TIME SPENT IN TILE

			if (update.x > GameManager.BOARDSIZE - .5f) {				//if the marble travels off the board, rotate back around pac-man style
				update.x = update.x - GameManager.BOARDSIZE;
			}
			if (update.y > GameManager.BOARDSIZE - .5f) {
				update.y = update.y - GameManager.BOARDSIZE;
			}
			this.transform.position = update;
		}

	}



	//determines where on the tile the marble is based on percentage through the tile. also accounts for turns. returns a vector3 of the new position for the marble.
	public Vector3 translation(int entry, float percentage, Tile tile){
		Vector3 t;
		if (tile.turnsHere(entry)) {			//eezy turn
			if ((entry % 2 == 0)) {																//if moving north or south
				if (percentage < .05f) {
					t = new Vector3 (tile.getX(), ((entry - 1) * percentage) + tile.getY(), -.5f);		//move straight until 50% through
				} else { 
					if (entry == tile.type()) {												//when entry and turnType match (0 or 2), turn right
						this.turnRight ();														
					} else {
						this.turnLeft ();															//otherwise, turn left
					}
					t = new Vector3 (tile.getX(), tile.getY(), -.5f);
				}
			} else {																			//moving east or west
				if (percentage < .05f) {
					t = new Vector3 (tile.getX() + ((entry - 2) * percentage * -1), tile.getY(), -.5f);	//move straight until 50% through
				} else {
					if (entry == tile.type()) {												//when entry and turnType match (1 or 3), turn right
						this.turnRight ();
					} else {
						this.turnLeft ();															//otherwise, turn left
					}
					t = new Vector3 (tile.getX(), tile.getY(), -.5f);
				}
			}

		} else {							//moves straight
			if (entry % 2 == 0) {					//entering from north or south
				t = new Vector3 (tile.getX(), tile.getY() - ((entry - 1) * percentage), -.5f);
			} else {								//entering from east or west
				t = new Vector3 (tile.getX()+ ((entry - 2) * percentage * -1), tile.getY(), -.5f);
			}
		}
		return t;
	}


	//decrements health of the marble, or consumes a shield powerup. If health hits 0, the marble is destroyed.
	public void Hit ()
	{
		if (this.power == 2) {
			this.power = 0;
		} else {
			this.health--;
		}

		if (this.health == 0) {
			marbleDestroy ();
		}
	}


	//removes the marble object and model, and decrements the number of marbles in the game.
	public void marbleDestroy ()
	{
		print (this + " has been destroyed");
		GameManager.marbles.Remove (this);
		GameManager.NUMMARBLES--;
		this.tile.marble = null;
		Destroy (this.model);
		Destroy (this.gameObject);

	}





	public void turnRight ()
	{
		this.transform.Rotate (0, 0, -90);
		this.dir = (this.dir + 1) % 4;
		if (this.next != null) {
			this.next.marble = null;
			this.next = getNextTile ();
		}
	}

	public void turnLeft ()
	{
		this.transform.Rotate (0, 0, 90);
		this.dir = (this.dir + 3) % 4;
		if (this.next != null) {
			this.next.marble = null;
			this.next = getNextTile ();
		}
	}






}

