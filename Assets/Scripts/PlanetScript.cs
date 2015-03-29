using UnityEngine;
using System.Collections;

public class PlanetScript : MonoBehaviour {

	public GameObject fireball;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate () 
	{
	}

	public void FireBullet(float x, float y)
	{
		Vector2 target = new Vector2(x, y);
		Vector2 myPos = new Vector2(transform.position.x,transform.position.y);
		Vector2 direction = target - myPos;
		direction.Normalize();
		Quaternion rotation = Quaternion.Euler( 0, 0, Mathf.Atan2 ( direction.y, direction.x ) * Mathf.Rad2Deg + 135 );
		
		GameObject projectile = (GameObject) Instantiate( fireball, myPos, rotation);
		projectile.GetComponent<Rigidbody2D> ().velocity = direction * 6;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.CompareTag("Asteroid"))
			Destroy (collider.gameObject);
	}
}
