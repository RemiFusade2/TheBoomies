using UnityEngine;
using System.Collections;

public class GeneratorBehaviour : MonoBehaviour {

	public GameEngine gameEngine;

	public int hp;

	public AudioClip collapseSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void KillGenerator()
	{
		gameEngine.KillGenerator (this.gameObject);
	}
	
	void OnMouseDown()
	{
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
		{
			hp--;
			this.GetComponent<Animator> ().SetTrigger ("Shake");
			this.GetComponent<Animator> ().SetInteger ("HP", hp);
			if (hp <= 0)
			{
				this.GetComponent<AudioSource>().clip = collapseSound;
				this.GetComponent<AudioSource>().volume = 1;
			}
			this.GetComponent<AudioSource>().Play();
			gameEngine.SuspendBackgroundMusicFor(4);
			gameEngine.ScareAllBoomies(this.transform.position, 20);
			gameEngine.ChangeTextsAfterGeneratorHit();
		}
	}
}
