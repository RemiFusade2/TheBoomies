using UnityEngine;
using System.Collections;

public class MainMenuBehaviour : MonoBehaviour {

	public GameObject mainMenuENPanel;
	public GameObject mainMenuFRPanel;
	public GameObject creditsENPanel;
	public GameObject creditsFRPanel;
	public GameObject settingsENPanel;
	public GameObject settingsFRPanel;

	private string language;
	private bool mainMenuVisible;
	private bool creditsVisible;
	private bool settingsVisible;

	private const string boomiesLanguageKey = "boomiesLanguage";

	// Use this for initialization
	void Start () 
	{
		if (PlayerPrefs.HasKey(boomiesLanguageKey) && PlayerPrefs.GetString(boomiesLanguageKey).Equals("FR"))
		{
			language = "FR";
		}
		else
		{
			language = "EN";
		}
		OpenMainMenu ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (mainMenuVisible)
			{				
				ExitApplication();
			}
			else
			{
				OpenMainMenu();
			}
		}
	}

	public void OpenMainMenu()
	{
		mainMenuVisible = true;
		creditsVisible = false;
		settingsVisible = false;
		UpdateVisiblePanel ();
	}

	public void OpenCredits()
	{
		mainMenuVisible = false;
		creditsVisible = true;
		settingsVisible = false;
		UpdateVisiblePanel ();
	}

	public void OpenSettings()
	{
		mainMenuVisible = false;
		creditsVisible = false;
		settingsVisible = true;
		UpdateVisiblePanel ();
	}

	public void ChangeLanguage(string lang)
	{
		PlayerPrefs.SetString (boomiesLanguageKey, lang);
		language = lang;
		UpdateVisiblePanel ();
	}

	public void ClearCache()
	{
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.SetString (boomiesLanguageKey, language);
		PlayerPrefs.Save ();
	}

	public void ExitApplication()
	{
		Application.Quit ();
	}

	public void StartGame()
	{
		Application.LoadLevel ("scene");
	}

	private void UpdateVisiblePanel()
	{
		mainMenuENPanel.SetActive (false);
		mainMenuFRPanel.SetActive (false);
		creditsENPanel.SetActive (false);
		creditsFRPanel.SetActive (false);
		settingsENPanel.SetActive (false);
		settingsFRPanel.SetActive (false);

		if (language.Equals("FR"))
		{
			if (mainMenuVisible)
			{
				mainMenuFRPanel.SetActive (true);
			}
			else if (creditsVisible)
			{
				creditsFRPanel.SetActive (true);
			}
			else if (settingsVisible)
			{
				settingsFRPanel.SetActive (true);
			}
		}
		else
		{
			if (mainMenuVisible)
			{
				mainMenuENPanel.SetActive (true);
			}
			else if (creditsVisible)
			{
				creditsENPanel.SetActive (true);
			}
			else if (settingsVisible)
			{
				settingsENPanel.SetActive (true);
			}
		}
	}
}
