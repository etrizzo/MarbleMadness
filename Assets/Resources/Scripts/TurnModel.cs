using UnityEngine;
using System.Collections;

public class TurnModel : MonoBehaviour
{
	private int type;			//0 for blank, 1 
	private Tile owner;			// Pointer to the parent object.
	private Material mat;		// Material for setting/changing texture and color.

	public void init(Tile owner) {
		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(0,0,-.01f);		// Center the model on the parent.
		name = "Turn Model";									// Name the object.
		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 
		//mat.renderQueue = 1000;
		mat.mainTexture = Resources.Load<Texture2D>("Textures/turnLights");	// Set the texture.  Must be in Resources folder.
		mat.color = new Color(1,1,1);											// Set the color (easy way to tint things).
		//mat.shader = Shader.Find ("Transparent/Diffuse");						// Tell the renderer that our textures have transparency. 
		for (int i = 0; i < (int)Random.Range (0, 4); i++) {
			this.owner.rotate();
		}
	}
		

	void rotate(){
		this.owner.rotate();
	}

	void OnMouseDown(){
		//print ("You Clicked on tile: " + this.owner);
		if (!this.owner.marble) {
			this.rotate();
		}
	}
}

