using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class NarrationSpeech
{
	public List<string> narrationFR;
	public List<string> narrationEN;

	public List<string> narration()
	{
		List<string> result = new List<string> ();
		if (PlayerPrefs.HasKey("boomiesLanguage") && PlayerPrefs.GetString("boomiesLanguage").Equals("FR"))
		{
			result = narrationFR;
		}
		else 
		{
			result = narrationEN;
		}
		return result;
	}
}

public class GameEngine : MonoBehaviour {

	public List<GameObject> listOfBoomies;

	public GameObject sphereGeneratorPrefab;
	public GameObject cubeGeneratorPrefab;
	public GameObject boomieGeneratorPrefab;

	public List<GameObject> listOfGenerators;
	public List<GameObject> listOfBoomieGenerators;

	public float minDistanceBetweenGenerators;
	public float minDistanceFromBoomieGenerators;

	public float maxNumberOfBoomies;

	public AudioSource screamSound;
	public AudioSource backgroundMusic;

	private Coroutine backgroundMusicPlayCoroutine;

	private bool stopAllTextEventsExceptEnd;

	public Text exitText;
	public Text returnText;
	public GameObject menuPanel;
	private bool menuPanelIsActive;

	// Use this for initialization
	void Start () 
	{
		menuPanelIsActive = false;
		menuPanel.SetActive (menuPanelIsActive);
		if (PlayerPrefs.HasKey("boomiesLanguage") && PlayerPrefs.GetString("boomiesLanguage").Equals("FR"))
		{
			exitText.text = "QUITTER";
			returnText.text = "RETOUR";
		}
		else
		{
			exitText.text = "EXIT";
			returnText.text = "RETURN";
		}

		if (PlayerPrefs.HasKey("boomiesStatus") && PlayerPrefs.GetInt("boomiesStatus").Equals(0))
		{
			foreach (GameObject boomie in listOfBoomies)
			{
				Destroy (boomie);
			}
			foreach (GameObject house in listOfBoomieGenerators)
			{
				Destroy (house);
			}
			foreach (GameObject generator in listOfGenerators)
			{
				Destroy (generator);
			}
			currentStory = startGameEveryoneDeadNarration.narration();
			currentTextIsNice = false;
			normalNarrationHasBeenFinished = true;
			backgroundMusic.Stop ();
			zeroHousesAliveTriggered = true;
			zeroHousesAliveEventCount = 1;
			zeroBoomiesAliveEventCount = 1;
			stopAllTextEventsExceptEnd = true;
			endGameTriggered = true;
		}
		else
		{
			currentStory = normalNarration.narration();
			currentTextIsNice = true;
			normalNarrationHasBeenFinished = false;
			backgroundMusic.Play ();
			zeroHousesAliveTriggered = false;
			zeroHousesAliveEventCount = 0;
			zeroBoomiesAliveEventCount = 0;
			stopAllTextEventsExceptEnd = false;
			endGameTriggered = false;
		}

		currentStoryIndex = 0;
		numberOfBoomieKills = 0;
		numberOfGeneratorKills = 0;
		numberOfHouseKills = 0;
		generatorHitCount = 0;

		NextText ();
	}

	public void ExitApp()
	{
		Application.Quit ();
	}

	public void ReturnToGame()
	{
		menuPanelIsActive = false;
		menuPanel.SetActive (menuPanelIsActive);
	}

	private bool zeroHousesAliveTriggered;
	private int zeroHousesAliveEventCount;
	private bool zeroBoomiesAliveTriggered;
	private int zeroBoomiesAliveEventCount;

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			menuPanelIsActive = !menuPanelIsActive;
			menuPanel.SetActive (menuPanelIsActive);
		}

		if (listOfBoomieGenerators.Count == 0 && !zeroHousesAliveTriggered)
		{
			zeroHousesAliveEventCount++;
			if (zeroHousesAliveEventCount == 1)
			{
				currentStory = firstZeroHouseAliveNarration.narration();
			}
			else
			{
				int randomStory = Random.Range(0, zeroHouseAliveRandomNarration.Count);
				currentStory = zeroHouseAliveRandomNarration[randomStory].narration();
			}
			currentStoryIndex = 0;
			currentTextIsNice = false;
			zeroHousesAliveTriggered = true;
			NextText ();
		}
		if (listOfBoomies.Count == 0 && !zeroBoomiesAliveTriggered)
		{
			zeroBoomiesAliveEventCount++;
			if (zeroBoomiesAliveEventCount == 1)
			{
				currentStory = firstZeroBoomieAliveNarration.narration();
			}
			else
			{
				int randomStory = Random.Range(0, zeroBoomieAliveRandomNarration.Count);
				currentStory = zeroBoomieAliveRandomNarration[randomStory].narration();
			}
			currentStoryIndex = 0;
			currentTextIsNice = false;
			zeroBoomiesAliveTriggered = true;
			NextText ();
		}
		UpdateEndGameCondition ();
	}
	
	private bool endGameTriggered;

	private bool UpdateEndGameCondition()
	{
		if (listOfBoomies.Count == 0 && listOfBoomieGenerators.Count == 0 && !endGameTriggered)
		{
			endGameTriggered = true;
			PlayerPrefs.SetInt ("boomiesStatus", 0);
			PlayerPrefs.Save ();
			backgroundMusic.Stop ();
			stopAllTextEventsExceptEnd = true;
			currentStory = endGameEveryoneDeadNarration.narration();
			currentStoryIndex = 0;
			currentTextIsNice = false;
			backgroundMusic.volume = 0;
			NextText ();
			return true;
		}
		return false;
	}

	public void AddBoomie(GameObject boomie)
	{
		listOfBoomies.Add (boomie);
		zeroBoomiesAliveTriggered = false;
	}

	public void ScareAllBoomies(Vector3 scaryPosition, float radius)
	{		
		foreach (GameObject boomie in listOfBoomies)
		{
			if ( (boomie.transform.position - scaryPosition).magnitude < radius)
			{
				boomie.GetComponent<BoomieBehaviour>().RunawayFromPosition(scaryPosition, true);
			}
		}
	}

	public void BoomieDied(GameObject deadBoomie)
	{
		StartCoroutine (WaitAndPlayStopSound (0.1f, backgroundMusic, false));
		listOfBoomies.Remove (deadBoomie);
		Vector3 deadBoomiePosition = deadBoomie.transform.position;
		bool playScreams = false;
		foreach (GameObject boomie in listOfBoomies)
		{
			if ( (boomie.transform.position - deadBoomiePosition).magnitude < 40)
			{
				boomie.GetComponent<BoomieBehaviour>().RunawayFromPosition(deadBoomiePosition, true);
			}
			if (boomie.GetComponent<BoomieBehaviour>().IsScared())
			{
				playScreams = true;
			}
		}
		float holdSpawnTimer = playScreams ? 30 : 50;
		foreach (GameObject house in listOfBoomieGenerators)
		{
			if (house.transform.childCount == 1)
			{
				house.transform.GetChild(0).GetComponent<HouseBehaviour>().HoldSpawnForSeconds(holdSpawnTimer);
			}
		}
		if (playScreams)
		{
			screamSound.Stop ();
			StartCoroutine (WaitAndPlayStopSound (0.5f, screamSound, true));
			if (backgroundMusicPlayCoroutine != null)
			{
				StopCoroutine(backgroundMusicPlayCoroutine);
			}
			backgroundMusicPlayCoroutine = StartCoroutine (WaitAndPlayStopSound (30, backgroundMusic, true));
		}
		else
		{
			StartCoroutine (WaitAndPlayStopSound (0, screamSound, false));
			if (backgroundMusicPlayCoroutine != null)
			{
				StopCoroutine(backgroundMusicPlayCoroutine);
			}
			backgroundMusicPlayCoroutine = StartCoroutine (WaitAndPlayStopSound (50, backgroundMusic, true));
		}

		// Texts
		numberOfBoomieKills++;
		if (numberOfBoomieKills == 1)
		{
			currentStory = firstBoomieKillNarration.narration();
			currentStoryIndex = 0;
			currentTextIsNice = false;
			NextText();
		}
		else
		{
			int randomStory = Random.Range(0, boomieKillRandomNarration.Count);
			currentStory = boomieKillRandomNarration[randomStory].narration();
			currentStoryIndex = 0;
			currentTextIsNice = false;
			NextText();
		}			
	}

	public void SuspendBackgroundMusicFor(float time)
	{
		StartCoroutine (WaitAndPlayStopSound (0, backgroundMusic, false));
		if (backgroundMusicPlayCoroutine != null)
		{
			StopCoroutine(backgroundMusicPlayCoroutine);
		}
		backgroundMusicPlayCoroutine = StartCoroutine (WaitAndPlayStopSound (time, backgroundMusic, true));
	}

	IEnumerator WaitAndPlayStopSound(float time, AudioSource sound, bool play)
	{
		yield return new WaitForSeconds (time);
		if (play)
		{
			sound.Play();
		}
		else
		{
			sound.Pause();
		}
	}

	public bool BuildASphereGenerator(GameObject boomieBuilder1, GameObject boomieBuilder2)
	{
		bool generatorBuilt = false;
		Vector3 position = Vector3.Lerp(boomieBuilder1.transform.position, boomieBuilder2.transform.position, 0.5f);
		if (CheckForDistanceBetweenGenerators(position, false))
		{
			GameObject newSphereGen = (GameObject) Instantiate (sphereGeneratorPrefab, new Vector3(position.x, 1.093f, position.z), Quaternion.identity);
			newSphereGen.transform.GetChild (0).GetComponent<GeneratorBehaviour> ().gameEngine = this;
			listOfGenerators.Add ( newSphereGen );
			generatorBuilt = true;
		}
		return generatorBuilt;
	}

	public bool BuildACubeGenerator(GameObject boomieBuilder1, GameObject boomieBuilder2)
	{
		bool generatorBuilt = false;
		Vector3 position = Vector3.Lerp(boomieBuilder1.transform.position, boomieBuilder2.transform.position, 0.5f);
		if (CheckForDistanceBetweenGenerators (position, false)) 
		{
			GameObject newCubeGen = (GameObject)Instantiate (cubeGeneratorPrefab, new Vector3 (position.x, 2.5f, position.z), Quaternion.LookRotation (boomieBuilder2.transform.position - boomieBuilder1.transform.position));
			newCubeGen.transform.GetChild (0).GetComponent<GeneratorBehaviour> ().gameEngine = this;
			listOfGenerators.Add (newCubeGen);
			generatorBuilt = true;
		}
		return generatorBuilt;
	}

	public bool BuildABoomieGenerator(GameObject boomieBuilder1, GameObject boomieBuilder2)
	{
		bool generatorBuilt = false;
		Vector3 position = Vector3.Lerp(boomieBuilder1.transform.position, boomieBuilder2.transform.position, 0.5f);
		if (CheckForDistanceBetweenGenerators (position, true))
		{
			GameObject newHouse = (GameObject)Instantiate (boomieGeneratorPrefab, new Vector3 (position.x, 4, position.z), Quaternion.LookRotation (boomieBuilder2.transform.position - boomieBuilder1.transform.position));
			newHouse.transform.GetChild (0).GetComponent<HouseBehaviour> ().gameEngine = this;
			newHouse.transform.GetChild (0).GetComponent<GeneratorBehaviour> ().gameEngine = this;
			listOfBoomieGenerators.Add (newHouse);
			generatorBuilt = true;
			zeroHousesAliveTriggered = false;
		}
		return generatorBuilt;
	}

	private bool CheckForDistanceBetweenGenerators(Vector3 candidateGeneratorPosition, bool isBoomieGenerator)
	{
		foreach (GameObject generator in listOfGenerators)
		{
			if ( (generator.transform.position - candidateGeneratorPosition).magnitude < (isBoomieGenerator ? minDistanceFromBoomieGenerators : minDistanceBetweenGenerators) )
			{
				return false;
			}
		}
		foreach (GameObject boomieGenerator in listOfBoomieGenerators)
		{
			if ( (boomieGenerator.transform.position - candidateGeneratorPosition).magnitude < minDistanceFromBoomieGenerators )
			{
				return false;
			}
		}
		return true;
	}

	public void KillGenerator (GameObject generator)
	{
		if (listOfGenerators.Contains(generator.transform.parent.gameObject))
		{
			listOfGenerators.Remove(generator.transform.parent.gameObject);

			// Texts
			if (!stopAllTextEventsExceptEnd)
			{
				numberOfGeneratorKills++;
				if (numberOfGeneratorKills == 1)
				{
					currentStory = firstGeneratorKillNarration.narration();
				}
				else
				{
					int randomStory = Random.Range(0, generatorKillRandomNarration.Count);
					currentStory = generatorKillRandomNarration[randomStory].narration();
				}	
				currentStoryIndex = 0;
				currentTextIsNice = false;
				NextText();
				SuspendBackgroundMusicFor(10);
			}
		}
		if (listOfBoomieGenerators.Contains(generator.transform.parent.gameObject))
		{
			listOfBoomieGenerators.Remove(generator.transform.parent.gameObject);
			foreach (GameObject boomie in listOfBoomies)
			{
				boomie.GetComponent<Collider>().enabled = true;
			}
			
			// Texts
			if (!stopAllTextEventsExceptEnd)
			{
				numberOfHouseKills++;
				if (numberOfHouseKills == 1)
				{
					currentStory = firstHouseKillNarration.narration();
				}
				else
				{
					int randomStory = Random.Range(0, houseKillRandomNarration.Count);
					currentStory = houseKillRandomNarration[randomStory].narration();
				}	
				currentStoryIndex = 0;
				currentTextIsNice = false;
				NextText();
				SuspendBackgroundMusicFor(10);	
			}
		}
		Destroy (generator.transform.parent.gameObject);
	}

	public bool CanSpawnBoomie()
	{
		return listOfBoomies.Count < maxNumberOfBoomies;
	}

	public void WaitAndKillGameObject(float time, GameObject gameObject)
	{
		StartCoroutine (WaitAndDestroyGameObject (time, gameObject));
	}

	IEnumerator WaitAndDestroyGameObject (float time, GameObject gameObject)
	{
		yield return new WaitForSeconds (time);
		Destroy (gameObject);
	}

	public Text niceText;
	public Text angryText;

	public Button narrationSupport;

	public NarrationSpeech normalNarration;
	public NarrationSpeech firstBoomieKillNarration;
	public List<NarrationSpeech> boomieKillRandomNarration;
	public NarrationSpeech firstGeneratorHitNarration;
	public List<NarrationSpeech> generatorHitRandomNarration;
	public NarrationSpeech firstGeneratorKillNarration;
	public List<NarrationSpeech> generatorKillRandomNarration;
	public NarrationSpeech firstHouseKillNarration;
	public List<NarrationSpeech> houseKillRandomNarration;
	public NarrationSpeech firstZeroBoomieAliveNarration;
	public List<NarrationSpeech> zeroBoomieAliveRandomNarration;
	public NarrationSpeech firstZeroHouseAliveNarration;
	public List<NarrationSpeech> zeroHouseAliveRandomNarration;
	public NarrationSpeech endGameEveryoneDeadNarration;
	public NarrationSpeech startGameEveryoneDeadNarration;
	public NarrationSpeech afterAWhileIfNormalNarrationHasBeenInterruptedNarration;

	private List<string> currentStory;
	private int currentStoryIndex;
	private bool currentTextIsNice;

	private int numberOfBoomieKills;
	private int numberOfGeneratorKills;
	private int numberOfHouseKills;

	private bool normalNarrationHasBeenFinished;

	private Coroutine restartNormalNarrationCoroutine;

	public void NextText()
	{
		if (currentStory.Count <= currentStoryIndex)
		{
			narrationSupport.gameObject.SetActive(false);
			if (currentStory.Equals(normalNarration.narration()))
			{
				normalNarrationHasBeenFinished = true;
			}
			else if (currentStory.Equals(afterAWhileIfNormalNarrationHasBeenInterruptedNarration.narration()))
			{
				currentStory = normalNarration.narration();
				currentStoryIndex = 0;
				currentTextIsNice = true;
				normalNarrationHasBeenFinished = false;
				NextText();
			}
			else
			{
				if (!normalNarrationHasBeenFinished)
				{
					if (restartNormalNarrationCoroutine != null)
					{
						StopCoroutine(restartNormalNarrationCoroutine);
					}
					restartNormalNarrationCoroutine = StartCoroutine(WaitAndRestartNormalNarration(80));
				}
			}
		}
		else
		{
			if (restartNormalNarrationCoroutine != null)
			{
				StopCoroutine(restartNormalNarrationCoroutine);
			}
			narrationSupport.gameObject.SetActive(true);
			string currentText = currentStory[currentStoryIndex];
			if (currentTextIsNice)
			{
				angryText.text = "";
				angryText.enabled = false;
				niceText.enabled = true;
				niceText.text = currentText;
			}
			else
			{
				niceText.text = "";
				niceText.enabled = false;
				angryText.enabled = true;
				angryText.text = currentText;
			}
			currentStoryIndex++;
		}
	}

	IEnumerator WaitAndRestartNormalNarration(float time)
	{
		yield return new WaitForSeconds(time);
		currentStory = afterAWhileIfNormalNarrationHasBeenInterruptedNarration.narration ();
		currentStoryIndex = 0;
		currentTextIsNice = true;
		NextText ();
	}

	private int generatorHitCount;

	public void ChangeTextsAfterGeneratorHit()
	{
		if (!stopAllTextEventsExceptEnd)
		{
			generatorHitCount++;
			if (generatorHitCount == 1)
			{
				currentStory = firstGeneratorHitNarration.narration();
				
			}
			else
			{
				int randomIndex = Random.Range(0, generatorHitRandomNarration.Count);
				currentStory = generatorHitRandomNarration[randomIndex].narration();
			}
			currentStoryIndex = 0;
			currentTextIsNice = false;
			NextText ();
		}
	}
}
