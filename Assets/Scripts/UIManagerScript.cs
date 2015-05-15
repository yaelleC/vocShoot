using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using SimpleJSON;
using System;
using System.Text.RegularExpressions;

public class UIManagerScript : MonoBehaviour {

	public EngAGe engage;
	public const int idSG = 128;

	// MenuScene
	public Animator startButton;
	public Animator settingsButton;
	public Animator dialog;
	public Animator imgLevel;
	public Animator contentPanel;
	public Animator gearImage;
	public Slider sdr_level;
	public GameObject badgeDialog;
	public GameObject infoDialog;
	public Text txt_title;
	public Text txt_description;
	public GameObject leaderboardDialog;
	public Text txt_listBestPlayers;

	// LoginScene
	public Text txtUsername;
	public InputField txtPassword;
	public Text txtLoginParagraph;
	
	private static string username;
	private static string password;

	// parameter scene
	public Text txtWelcome;
	public InputField inputPrefab;
	public GameObject inputParent;	
	private List<InputField> inputFields = new List<InputField>();

	// game scene
	public Text txtFeedback;
	public PlanetScript planet;
	public GenerateAsteroidScript astScript;
	
	public Text pointsFRtoENLabel;
	public Text pointsENtoFRLabel;
	public GameObject restartWinDialog;
	public GameObject restartLoseDialog;
	
	public GameObject feedbackDialog;
	
	public Image life1;
	public Image life2;
	public Image life3;

	public Image health;
	private int healthSize;

	private static int difficulty = 2;

	public int getDifficulty()
	{
		return difficulty;
	}

	void Start()
	{
		if (Application.loadedLevelName.Equals("LoginScene"))
		{
			// txtLoginParagraph.enabled = false;
			txtLoginParagraph.enabled = (engage.getErrorCode() > 0);
			txtLoginParagraph.text = engage.getError();
		}
		else if (Application.loadedLevelName.Equals("ParametersScene"))
		{
			txtWelcome.text = "Welcome " + username ;			
			int i = 0;
			// loop on all the player's characteristics needed
			foreach (JSONNode param in engage.getParameters())
			{
				// creates a text field in the panel parameters of the scene
				InputField inputParam = (InputField)Instantiate(inputPrefab);
				inputParam.name = "input_" + param["name"];
				inputParam.transform.SetParent(inputParent.transform);
				inputParam.text = "Enter your " + param["name"] + " ("+param["type"]+")";

				// position them, aligned vertically
				RectTransform transform = inputParam.transform as RectTransform;   
				transform.anchoredPosition = new Vector2(0, 20 - i*50 );
				// save the input in the input array 
				inputFields.Add(inputParam);				
				i++;
			}
		}
		else if (Application.loadedLevelName.Equals("MenuScene"))
		{
			RectTransform transform = contentPanel.gameObject.transform as RectTransform;        
			Vector2 position = transform.anchoredPosition;
			position.y -= transform.rect.height;
			transform.anchoredPosition = position;

			CloseSettings();

			// close all three windows
			badgeDialog.SetActive (false);
			infoDialog.SetActive (false);
			leaderboardDialog.SetActive (false);

			// retrieve EngAGe data about the game, the badges won and the leaderboard
			StartCoroutine(engage.getGameDesc(idSG));
			StartCoroutine(engage.getBadgesWon(idSG));
			StartCoroutine(engage.getLeaderboard(idSG));
		}
		else if (Application.loadedLevelName.Equals("GameScene"))
		{
			healthSize = Mathf.RoundToInt(health.rectTransform.sizeDelta.x);

			StartCoroutine(engage.getGameDesc(idSG));
			
			restartWinDialog.SetActive(false);
			restartLoseDialog.SetActive(false);
			feedbackDialog.SetActive (false);

			// Initialise the scores and lives
			UpdateScores ();

			Time.timeScale = 1;

		}
	}

	public void GoToMenu()
	{
		Time.timeScale = 1;
		// for each parameter required
		foreach (JSONNode param in engage.getParameters())
		{
			// find the corresponding input field
			foreach (InputField inputField in inputFields)
			{
				if (inputField.name == "input_" + param["name"])
				{
					// and store the value in the JSON
					string value = inputField.text;					
					param.Add("value", value);
				}
			}
		}
		Application.LoadLevel("MenuScene");
	}

	public void RestartGame()
	{
		Time.timeScale = 1;
		//Application.LoadLevel("GameScene");
		StartCoroutine (engage.startGameplay(idSG, "GameScene"));
	}
	
	public void StartGame()
	{
		//Application.LoadLevel("GameScene");
		StartCoroutine (engage.startGameplay(idSG, "GameScene"));
	}
	
	public void GetStarted()
	{
		username = txtUsername.text;
		password = txtPassword.text;
		
		//Application.LoadLevel("ParametersScene");
		StartCoroutine(engage.loginStudent(idSG, username, password, "LoginScene", "MenuScene", "ParametersScene"));
	}

	public void GetStartedGuest()
	{
		//Application.LoadLevel("ParametersScene");
		StartCoroutine(engage.guestLogin(idSG, "LoginScene", "ParametersScene"));
	}

	public void OpenSettings()
	{
		startButton.SetBool("isHidden", true);
		settingsButton.SetBool("isHidden", true);

		dialog.enabled = true;
		dialog.SetBool("isHidden", false);
	}

	public void CloseSettings()
	{
		startButton.SetBool("isHidden", false);
		settingsButton.SetBool("isHidden", false);
		dialog.SetBool("isHidden", true);
	}

	public void SetDifficulty()
	{
		difficulty = (int)sdr_level.value;
		imgLevel.SetInteger("difficulty", difficulty);
	}

	public void ToggleMenu()
	{
		contentPanel.enabled = true;
		
		bool isHidden = contentPanel.GetBool("isHidden");
		contentPanel.SetBool("isHidden", !isHidden);

		gearImage.enabled = true;
		gearImage.SetBool("isHidden", !isHidden);
	}

	public void OpenBadges()
	{
		badgeDialog.SetActive (!badgeDialog.activeSelf);
		infoDialog.SetActive (false);
		leaderboardDialog.SetActive (false);
	}
	
	public void CloseBadges()
	{
		badgeDialog.SetActive (false);
	}
	
	public void OpenInfo()
	{
		// get the seriousGame object from engage
		JSONNode SGdesc = engage.getSG () ["seriousGame"];

		// display the title and description
		txt_title.text = SGdesc["name"];
		txt_description.text = SGdesc["description"];
			
		// open the window
		infoDialog.SetActive (!infoDialog.activeSelf);
		badgeDialog.SetActive (false);
		leaderboardDialog.SetActive (false);
	}
	
	public void CloseInfo()
	{
		infoDialog.SetActive (false);
	}
	public void OpenLeaderboard()
	{
		// get the leaderboard object from engage
		JSONNode leaderboard = engage.getLeaderboardList ();
		
		// look only at the eu_score 
		JSONArray overallScorePerf = leaderboard ["overallScore"].AsArray;
		
		// display up to 10 best gameplays
		int max = 10;
		txt_listBestPlayers.text = "";
		foreach (JSONNode gameplay in overallScorePerf)
		{
			if (max-- > 0)
			{
				// each gameplay has a "name" and a "score"
				float score = gameplay["score"].AsFloat ;
				txt_listBestPlayers.text += score + " - " + gameplay["name"] + "\n";
			}
		}
		// open the window
		leaderboardDialog.SetActive (!leaderboardDialog.activeSelf);
		infoDialog.SetActive (false);
		badgeDialog.SetActive (false);
	}
	
	public void CloseLeaderboard()
	{
		leaderboardDialog.SetActive (false);
	}
	
	public void OpenFeedback()
	{
		feedbackDialog.SetActive (!feedbackDialog.activeSelf);
		if (feedbackDialog.activeSelf)
		{
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}
	
	public void CloseFeedback()
	{
		feedbackDialog.SetActive (false);
		Time.timeScale = 1;
	}

	public void ExitToMenu()
	{
		Application.LoadLevel ("MenuScene");
	}

	public void UpdateScores()
	{		
		foreach (JSONNode score in engage.getScores())
		{
			string scoreName = score["name"];
			string scoreValue = score["value"];

			// TODO : have only one label for overall score
			
			if (string.Equals(scoreName, "englishToFrench"))
			{
				pointsENtoFRLabel.text = float.Parse(scoreValue).ToString();
			}
			else if (string.Equals(scoreName, "frenchToEnglish"))
			{
				pointsFRtoENLabel.text = float.Parse(scoreValue).ToString();
			}
			else if (string.Equals(scoreName, "health"))
			{
				float startingValue = float.Parse(score["startingValue"]);
				float currentValue = float.Parse(scoreValue);

				if (startingValue != 0f)
				{
					float newSize = healthSize*(currentValue/startingValue);
					health.rectTransform.sizeDelta = new Vector2(newSize, 24f); 
				}
			}
		}
	}

	public void UpdateFeedbackAndScore(JSONNode returnData)
	{
		UpdateScores ();
		ReceiveFeedback ();
	}

	public void ReceiveFeedback()
	{
		foreach (JSONNode f in engage.getFeedback())
		{
			// check if feedback is adaptation
			if (string.Equals( f["type"], "ADAPTATION"))
			{
				if (string.Equals(f["name"], "speedGame"))
					astScript.speedGame();
				if (string.Equals(f["name"], "slowGame"))
					astScript.slowGame();
			}
			// set color to write line into
			string color = "black";
			if (string.Equals( f["type"], "POSITIVE"))
				color = "green";
			if (string.Equals( f["type"], "NEGATIVE"))
				color="red";

			txtFeedback.text += "<color=\"" + color + "\">" + f["message"] + "</color>\n";

			// trigger end of game
			if (string.Equals(f["final"], "lose"))
			{
				StartCoroutine (engage.endGameplay(false));
				planet.loseGame();
				restartLoseDialog.SetActive(true);
			}
			else if (string.Equals(f["final"], "win"))
			{
				StartCoroutine (engage.endGameplay(true));
				planet.winGame();
				restartWinDialog.SetActive(true);
			}
		}
	}
}


