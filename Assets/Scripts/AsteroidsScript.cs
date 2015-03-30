using UnityEngine;
using System.Collections;
using SimpleJSON;

public class AsteroidsScript : MonoBehaviour {

	public GameObject wordPanel;
	public AudioClip hitSound;

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
			AudioSource.PlayClipAtPoint(hitSound, transform.position);	
			Destroy (gameObject);
			Destroy (collider.gameObject);
		}
	}
}
