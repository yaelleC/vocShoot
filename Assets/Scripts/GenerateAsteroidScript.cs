using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GenerateAsteroidScript : MonoBehaviour {
	
	public int speed = 1;
	public GameObject asteroid;  
	public Sprite[] availableAsteroids;  

	public GameObject panelWord;
	public Text textWord;

	public InputField textTranslation;
	public PlanetScript planet;
	public EngAGe engage;

	public UIManagerScript uiScript;

	private List<GameObject> asteroids;  
	
	private float worldMinX = 0.0f;
	private float worldMaxX = 0.0f;
	
	private float worldMinY = 0.0f;
	private float worldMaxY = 0.0f;
	
	private int wait = 0;
	private int waitGoal = 1000;

	// Use this for initialization
	void Start () {
		asteroids = new List<GameObject>();		

		float screenHeightInPoints = 2.0f * Camera.main.orthographicSize;
		float screenWidthInPoints = screenHeightInPoints * Camera.main.aspect;

		worldMaxX = screenWidthInPoints / 2;
		worldMinX = -1 * screenWidthInPoints / 2;
		worldMaxY = screenHeightInPoints / 2;
		worldMinY = -1 * screenHeightInPoints / 2;

		if (uiScript.getDifficulty() == 3)
		{
			waitGoal = 500;
		}
		else if (uiScript.getDifficulty() == 2)
		{
			waitGoal = 800;
		}
		else
		{
			waitGoal = 1000;
		}
		wait = waitGoal;
	}
	
	// Update is called once per frame
	void Update () {
		if (wait > waitGoal) {
			wait = 0;
			AddAsteroid ();
		}
		wait++;

		if(textTranslation.text != "" && Input.GetKey(KeyCode.Return)) 
		{			
			// create a JSON with one value, "country" (only parameter of countryReSelected)
			//JSONNode values = JSON.Parse("{ \"frenchWord\" : \"" + spr_word.name + "\" }");
			//StartCoroutine(engage.assess("translateFRtoEN", values, UpdateFeedbackAndScore));
			string wordTyped = textTranslation.text;
			foreach (GameObject ast in asteroids)
			{
				if (ast != null && ast.name == wordTyped)
				{
					planet.FireBullet(ast.transform.position.x, ast.transform.position.y);
				}
			}
			textTranslation.text = "";
			EventSystem.current.SetSelectedGameObject(textTranslation.gameObject, null);
			textTranslation.OnPointerClick (new PointerEventData(EventSystem.current));
		}
	}

	void GenerateObjectsIfRequired()
	{
		if (asteroids.ToArray().Length < 5)
			AddAsteroid();
	}

	void AddAsteroid()
	{
		JSONNode gameJSON = engage.getSG ();
		JSONArray words = new JSONArray ();
		try
		{
			print(gameJSON.ToString());
			JSONArray reactions = gameJSON["evidenceModel"]["translation"]["reactions"].AsArray;
			foreach(JSONNode reaction in reactions)
			{
				if (reaction["values"] != null)
				{
					words.Add(reaction["values"].AsArray);
				}
			}
		}
		catch (UnassignedReferenceException e)
		{
			print (e);
		}
		print (words);
		string wordToTranslate = "123";
		GameObject obj = (GameObject)Instantiate(asteroid);
		obj.name = wordToTranslate;

		GameObject panel = (GameObject)Instantiate (panelWord);
		Text text = (Text)Instantiate (textWord);
		text.text = wordToTranslate;

		float objectX = worldMaxX;
		float objectY = worldMaxY;

		// asteroid from the top
		if (Random.value > 0.7f) 
		{
			objectY = Random.Range (worldMinY, worldMaxY);
		}
		// asteroid from the right
		else if (Random.value > 0.4f) 
		{
			objectX = Random.Range (worldMinX, worldMaxX);
		}
		// asteroid from the left
		else if (Random.value > 0.2f) 
		{
			objectY = worldMinY;
			objectX = Random.Range (worldMinX, worldMaxX);
		}
		// asteroid from the bottom
		else
		{
			objectX = worldMinX;
			objectY = Random.Range (worldMinY, worldMaxY);
		}
		obj.transform.position = new Vector3(objectX,objectY,0); 
		
		float rotation = Random.Range(-45, 45);
		obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
		
		int randomSprite = Random.Range (0, availableAsteroids.Length);
		obj.GetComponent<SpriteRenderer> ().sprite = (Sprite) availableAsteroids [randomSprite];
		
		Vector2 target = Vector2.zero;
		Vector2 myPos = new Vector2(objectX,objectY);
		Vector2 direction = target - myPos;
		direction.Normalize();

				
		GameObject canvasGO = new GameObject ();			
		canvasGO.name = "canvasTextWord";

		canvasGO.AddComponent<Canvas> ();
		Canvas canvas = canvasGO.GetComponent<Canvas> ();
		canvas.name = "AsteroidCanvas";
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				
		RectTransform transform = canvas.transform as RectTransform;   
		transform.sizeDelta = new Vector2 (Screen.width, Screen.height);
		transform.transform.rotation = Quaternion.Euler (new Vector3(0, 0, 0));

		panel.transform.SetParent(canvas.transform);
		RectTransform transform2 = panel.transform as RectTransform;  
		Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3 (objectX, objectY, 0));
		transform2.anchoredPosition = new Vector3 (pos.x - Screen.width/2, pos.y - Screen.height/2, 0);
		panel.GetComponent<ObjectLabel> ().target = obj.transform;

		text.transform.SetParent(panel.transform);
		RectTransform transform3 = text.transform as RectTransform;   
		transform3.anchoredPosition = new Vector2 (0,0);

		
		obj.GetComponent<Rigidbody2D> ().velocity = direction * 0.5f;
		obj.GetComponent<AsteroidsScript> ().wordPanel = panel;

		asteroids.Add(obj);            
	}
}
