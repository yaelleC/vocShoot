using UnityEngine;
using System.Collections;

public class fireballScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnBecameInvisible() {  
		// Destroy the bullet 
		Destroy(gameObject);
	}
}
