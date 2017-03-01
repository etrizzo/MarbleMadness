//using UnityEngine;
//using System.Collections;
//
//public class WallModel : MonoBehaviour
//{
//	private int type;			//0 for blank, 1 
//	private Material mat;		// Material for setting/changing texture and color.
//	private 
//
//	public void init() {
//		
//		this.owner = owner;
//		transform.parent = owner.transform;					// Set the model's parent to the gem.
//		transform.localPosition = new Vector3(0,0,0);		// Center the model on the parent.
//		name = "Wall Model";									// Name the object.
//		mat = GetComponent<Renderer>().material;								// Get the material component of this quad object.
//		mat.shader = Shader.Find("Sprites/Default"); 
//		mat.mainTexture = Resources.Load<Texture2D>("Textures/tileWall");	// Set the texture.  Must be in Resources folder.
//		mat.color = new Color(1,1,1);											// Set the color (easy way to tint things).
//	}
//		
//		
//}
//
