using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PlanetScript : MonoBehaviour {

	public GameObject fireball;
	public ParticleSystem winningExplosion;
	public EngAGe engage;
	public UIManagerScript uiScript;
	
	public AudioClip fireBulletSound;
	public AudioClip planetHitSound;
	
	private bool endWin = false;
	private bool endLose = false;
	Animator animator;

	// Use this for initialization
	void Start () {
		
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate () 
	{
		if (endWin) 
		{
			Time.timeScale = 0;
			animator.SetBool("win", true);
		}
		else if (endLose) 
		{
			Time.timeScale = 0;
			animator.SetBool ("lose", true);
		}
		AdjustWinningExplosion ();
	}
	
	void AdjustWinningExplosion ()
	{
		winningExplosion.enableEmission = endWin;
	}

	public void winGame()
	{
		endWin = true;
	}
	public void loseGame()
	{
		endLose = true;
	}

	public void FireBullet(float x, float y)
	{
		AudioSource.PlayClipAtPoint(fireBulletSound, transform.position);

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
		if (collider.gameObject.CompareTag ("Asteroid")) 
		{
			string wordToTranslate = collider.gameObject.name.Split('_')[0];
			string correctTranslation = collider.gameObject.name.Split('_')[1];

			JSONNode values = JSON.Parse("{ \"toTranslate\" : \"" + wordToTranslate + "\", " +
			                             "\"correctWord\" : \"" + correctTranslation + "\" }");
			StartCoroutine(engage.assess("planetHit", values, uiScript.UpdateFeedbackAndScore));

			AudioSource.PlayClipAtPoint(planetHitSound, transform.position);
			animator.SetBool ("hit", true);
			animator.SetBool ("hitOnceTrigger", true);
			Destroy (collider.gameObject);
		}
	}
}
