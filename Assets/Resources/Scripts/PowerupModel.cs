using UnityEngine;
using System.Collections;

public class PowerupModel : MonoBehaviour
{
	private int power;		
	private float clock;		// Keep track of time since creation for animation.
	private Powerup owner;			// Pointer to the parent object.
	private Material mat;		// Material for setting/changing texture and color.

	public void init(int powerType, Powerup owner) {
		this.owner = owner;
		this.power = powerType;	//1 - health, 2 - shield, 3- turbo

		transform.parent = owner.transform;					
		transform.localPosition = new Vector3(0,0,0);		// Center the model on the parent.
		name = "GemCollider";									// Name the object.

		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 
		mat.mainTexture = Resources.Load<Texture2D>("Textures/powerup");	// Set the texture.  Must be in Resources folder.
		if (this.power == 1) {
			mat.color = new Color (1, .4f, .4f);											// Set the color (easy way to tint things).
		} else if (this.power == 2) {
			mat.color = new Color (.4f, .8f, 1f);
		} else {
			mat.color = new Color (1f, .6f, .3f);
		}
	}

	void Start () {
		clock = 0f;
	}

	void Update () {

		// Incrememnt the clock based on how much time has elapsed since the previous update.
		clock = clock + Time.deltaTime;
		if (clock > this.owner.lifespan - 1) {
			Color col;
			if (this.power == 1) {
				 col = new Color (1, .4f, .4f);										
			} else if (this.power == 2) {
				 col = new Color (.4f, .8f, 1f);
			} else {
				 col = new Color (1f, .6f, .3f);
			}
			col.a = this.owner.lifespan - clock;
			mat.color = col;
			//print (col.a + "!");
		}
			
					transform.localScale = new Vector3(1+Mathf.Sin(3*clock)/4,1+Mathf.Sin(3*clock)/4,1);
	}

	public void OnTriggerEnter2D(Collider2D col){
		this.owner.tile.item = false;
		Marble m = col.transform.parent.gameObject.GetComponent<Marble>();
		m.power = this.power;
		Destroy (this.owner.modelObject);
	}
}

