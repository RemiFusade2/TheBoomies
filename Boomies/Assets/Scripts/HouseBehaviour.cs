using UnityEngine;
using System.Collections;

public class HouseBehaviour : MonoBehaviour {
	
	public GameEngine gameEngine;

	public float spawnTimer;
	private float lastSpawnTime;

	public GameObject boomie;

	public Transform popDirection;

	private bool holdSpawn;

	void Start()
	{
		lastSpawnTime = Time.time;
		holdSpawn = false;
	}

	// Update is called once per frame
	void Update () 
	{
		if (Time.time - lastSpawnTime > spawnTimer) 
		{
			lastSpawnTime = Time.time;
			PopBoomie();
		}
	}

	private void PopBoomie()
	{
		if (gameEngine.CanSpawnBoomie() && !holdSpawn && this.GetComponent<GeneratorBehaviour>().hp > 0)
		{
			Vector3 boomiePosition = new Vector3 (this.transform.position.x, 2.650939f, this.transform.position.z);
			GameObject newBoomie = (GameObject) Instantiate (boomie, boomiePosition, this.transform.rotation);
			newBoomie.GetComponent<BoomieBehaviour> ().gameEngine = gameEngine;
			newBoomie.GetComponent<Collider> ().enabled = false;
			newBoomie.GetComponent<Rigidbody> ().AddForce (30 * (popDirection.transform.position - this.transform.position));
			gameEngine.AddBoomie (newBoomie);
			StartCoroutine(EnableColliderOnBoomieAfterTimer(1, newBoomie));
		}
	}

	IEnumerator EnableColliderOnBoomieAfterTimer(float timer, GameObject go)
	{
		yield return new WaitForSeconds(timer);
		go.GetComponent<Collider> ().enabled = true;
	}

	public void HoldSpawnForSeconds(float time)
	{
		holdSpawn = true;
		StartCoroutine (WaitAndHoldSpawn (time, false));
	}
	
	IEnumerator WaitAndHoldSpawn(float timer, bool holdSpawnStatus)
	{
		yield return new WaitForSeconds(timer);
		holdSpawn = holdSpawnStatus;
	}

}
