using UnityEngine;
using System.Collections;

public class TileModel : MonoBehaviour
{
	private int type;			//0 for blank, 1 
	//private float clock;		// Keep track of time since creation for animation.
	private Tile owner;			// Pointer to the parent object.
	private Material mat;		// Material for setting/changing texture and color.

	public void init(Tile owner) {
		
		this.owner = owner;
		transform.parent = owner.transform;					// Set the model's parent to the gem.
		transform.localPosition = new Vector3(0,0,0);		// Center the model on the parent.
		name = "Tile Model";									// Name the object.
		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
		mat.shader = Shader.Find("Sprites/Default"); 
		mat.renderQueue = 2000;
		mat.mainTexture = Resources.Load<Texture2D>("Textures/tileBlank");	// Set the texture.  Must be in Resources folder.
		mat.color = new Color(1,1,1);											// Set the color (easy way to tint things).
		//mat.shader = Shader.Find ("Transparent/Diffuse");						// Tell the renderer that our textures have transparency. 

	}
		
		
}

