using UnityEngine;
using System.Collections;

public class MarbleModel : MonoBehaviour
{
	private int type;			//0 for blank, 1 
	//private float clock;		// Keep track of time since creation for animation.
	private Marble owner;			// Pointer to the parent object.
	private Material mat;		// Material for setting/changing texture and color.
	private GameObject obj;
	private float r;
	private float g;
	private float b;

	public void init(Marble owner, GameObject obj) {

		this.owner = owner;
		this.obj = obj;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(0,0,0);		// Center the model on the parent.
		name = "Marble Model";									// Name the object.
		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 
		mat.mainTexture = Resources.Load<Texture2D>("Textures/marble");	// Set the texture.  Must be in Resources folder.

		if (owner.power == 3) {
			r = 1f;
			g = .6f;
			b = .3f;
		} else if (owner.power == 2) {
			r = .4f;
			g = 1f;
			b = 1f;
		} else if (owner.power == 1) {
			r = 1f;
			g = 1f;
			b = .5f;
		} else {
			r = 1f;
			g = 1f;
			b = 1f;
		}
		mat.color = new Color(r,g,b);											// Set the color (easy way to tint things).

	}

	//when marble is clicked, begin boost
	public void OnMouseUp(){
		if (!this.owner.boost && !this.owner.tired){
			this.owner.boost = true;
			this.owner.boostStart = this.owner.clock;
			this.owner.speed = this.owner.speed / 2;
		}
	}


	public void Update(){
		if (owner.power == 3) {
			r = 1f;
			g = .6f;
			b = .3f;
		} else if (owner.power == 2) {
			r = .4f;
			g = .9f;
			b = 1f;
		} else if (owner.power == 1) {
			r = 1f;
			g = 1f;
			b = .5f;
		} else {
			r = 1f;
			g = 1f;
			b = 1f;
		}
		float health = this.owner.health * .15f + .25f;
		if (this.owner.boost) {
			if (owner.speedingUp) {
				mat.color = new Color (((.5f*r) + owner.speed) * health, g * health, ((.5f*b) + owner.speed) * health);
			} else if (owner.slowingDown) {
				mat.color = new Color (((.5f*r) + owner.speed) * health, (g - owner.speed) * health , ((.5f*b)) * health);
			} else {
				mat.color = new Color ((.5f*r) * health, g * health, (.5f*b) * health);

			}
		} else if (this.owner.tired) {
			mat.color = new Color (r * health, (.5f * g) * health, (.5f*b) * health);
		}else {
			mat.color = new Color (health*r, health*g, health*b);
		}
	}



}

