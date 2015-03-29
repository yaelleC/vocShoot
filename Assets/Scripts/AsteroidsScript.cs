using UnityEngine;
using System.Collections;
using SimpleJSON;

public class AsteroidsScript : MonoBehaviour {

	public GameObject wordPanel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
		
	void OnBecameInvisible() {  
		// Destroy the bullet 
		Destroy(gameObject);
		Destroy (wordPanel);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.CompareTag ("Fireball")) 
		{			
			Destroy (gameObject);
			Destroy (collider.gameObject);
		}
	}
}
