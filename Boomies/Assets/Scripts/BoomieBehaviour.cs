using UnityEngine;
using System.Collections;

public class BoomieBehaviour : MonoBehaviour {

	public GameEngine gameEngine;

	public GameObject boomiePart;

	public Material normalMat;
	public Material sorryMat;

	private Vector3 direction;
	public float speed;

	private float lastDirectionChangeTime;
	private float directionChangeTimer;

	private bool holdDirectionChange;

	private bool scared;
	private bool busy;

	private int spheres;
	private int cubes;

	public GameObject cube;
	public GameObject sphere;

	private bool canHoldACube;
	private bool canHoldASphere;

	public GameObject boomieMesh;

	private float lastConversationTime;
	public float conversationTimer;

	// Use this for initialization
	void Start () 
	{
		lastDirectionChangeTime = Time.time;
		directionChangeTimer = 1;
		holdDirectionChange = false;
		spheres = 0;
		cubes = 0;
		UpdateSphereAndCube ();
		scared = false;
		UpdateScaredMaterial ();
		busy = false;
		lastConversationTime = Time.time;
		canHoldACube = true;
		canHoldASphere = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!busy && !holdDirectionChange && Time.time - lastDirectionChangeTime > directionChangeTimer)
		{
			direction = (direction + new Vector3 (Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f))).normalized;
			lastDirectionChangeTime = Time.time;
			UpdateDirection ();
		}

		// ray cast
		if (!busy && !scared && (Time.time - lastConversationTime) > conversationTimer)
		{
			Ray ray = new Ray(this.transform.position, direction);
			RaycastHit raycastHitInfo;
			float maxDistance = 10;
			if (Physics.Raycast(ray, out raycastHitInfo, maxDistance))
			{
				if (raycastHitInfo.collider.tag.Equals("Boomie"))
				{
					GameObject otherBoomie = raycastHitInfo.collider.gameObject;
					if (otherBoomie.GetComponent<BoomieBehaviour>().AskForConversation(this.gameObject))
					{
						lastConversationTime = Time.time;
						busy = true;
						direction = otherBoomie.transform.position - this.transform.position;
						UpdateDirection();
						StartCoroutine(WaitAndEndConversation(4, otherBoomie));
						boomieMesh.GetComponent<Animator> ().SetBool ("Talking", true);
					}
				}
			}
		}
	}

	public bool IsScared()
	{
		return scared;
	}

	public bool AskForConversation(GameObject otherBoomie)
	{
		bool accept = !busy && !scared && (Time.time - lastConversationTime) > conversationTimer;
		if (accept)
		{
			lastConversationTime = Time.time;
			busy = true;
			direction = otherBoomie.transform.position - this.transform.position;
			UpdateDirection();
			StartCoroutine(WaitAndEndConversation(4, otherBoomie));
			boomieMesh.GetComponent<Animator> ().SetBool ("Talking", true);
		}
		return accept;
	}

	IEnumerator WaitAndEndConversation(float time, GameObject interlocuteur)
	{
		yield return new WaitForSeconds (time);
		RunawayFromPosition (interlocuteur.transform.position, false);
		busy = false;
		boomieMesh.GetComponent<Animator> ().SetBool ("Talking", false);
	}

	void OnMouseDown()
	{
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
		{
			gameEngine.BoomieDied (this.gameObject);
			Die ();
		}
	}

	public void Die()
	{
		for (int i = 0 ; i < 10 ; i++)
		{
			GameObject part = (GameObject) Instantiate (boomiePart, this.transform.position + new Vector3( Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), this.transform.rotation);
			if (i==0) 
			{
				part.GetComponent<AudioSource> ().Play ();
			}
			gameEngine.WaitAndKillGameObject(900, part);
		}
		Destroy (this.gameObject);
	}

	private void UpdateDirection()
	{
		Vector3 lookAtVec = -Vector3.Cross (direction, Vector3.up);
		this.transform.LookAt (this.transform.position + lookAtVec);
		this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		this.GetComponent<Rigidbody> ().AddForce (-speed * this.transform.right);
	}

	public void RunawayFromPosition(Vector3 position, bool isScared)
	{
		scared = isScared;
		if (scared)
		{
			speed = 300;
		}
		boomieMesh.GetComponent<Animator> ().SetBool ("Scared", isScared);
		UpdateScaredMaterial ();
		holdDirectionChange = true;
		direction = (this.transform.position - position).normalized;
		UpdateDirection ();
		StartCoroutine (WaitAndRedoStandardBehaviour (12));
	}

	IEnumerator WaitAndRedoStandardBehaviour(float time)
	{
		yield return new WaitForSeconds(time);
		holdDirectionChange = false;
		scared = false;
		UpdateScaredMaterial ();
		speed = 200;
		boomieMesh.GetComponent<Animator> ().SetBool ("Talking", false);
		boomieMesh.GetComponent<Animator> ().SetBool ("Scared", false);
	}

	private void UpdateScaredMaterial()
	{
		if (scared) 
		{
			boomieMesh.GetComponent<Renderer>().material = sorryMat;
		}
		else
		{
			boomieMesh.GetComponent<Renderer>().material = normalMat;
		}
	}

	private void UpdateSphereAndCube()
	{
		sphere.SetActive((spheres > 0));
		cube.SetActive((cubes > 0));
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.collider.tag.Equals("SphereGenerator") && spheres <= 0 && canHoldASphere) 
		{
			spheres++;
			UpdateSphereAndCube();
		}
		if (col.collider.tag.Equals("CubeGenerator") && cubes <= 0 && canHoldACube) 
		{
			cubes++;
			UpdateSphereAndCube();
		}
		if (col.collider.tag.Equals("Boomie"))
		{
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
			if (cubes > 0 && spheres > 0)
			{
				cube.GetComponent<Animator>().SetTrigger("Show");
				sphere.GetComponent<Animator>().SetTrigger("Show");
				if (col.collider.GetComponent<BoomieBehaviour>().cubes > 0 && col.collider.GetComponent<BoomieBehaviour>().spheres > 0)
				{
					canHoldACube = false;
					canHoldASphere = false;
					StartCoroutine(WaitAndRestoreHoldCubeStatus(10));
					StartCoroutine(WaitAndRestoreHoldSphereStatus(10));
					if (gameEngine.BuildABoomieGenerator(this.gameObject, col.collider.gameObject))
					{
						boomieMesh.GetComponent<Animator>().SetTrigger("Happy");
						cubes = -1;
						col.collider.GetComponent<BoomieBehaviour>().cubes = -1;
						spheres = -1;
						col.collider.GetComponent<BoomieBehaviour>().spheres = -1;
					}
				}
			}
			else if (cubes > 0)
			{
				cube.GetComponent<Animator>().SetTrigger("Show");
				if (col.collider.GetComponent<BoomieBehaviour>().cubes > 0 && col.collider.GetComponent<BoomieBehaviour>().spheres <= 0)
				{
					canHoldACube = false;
					StartCoroutine(WaitAndRestoreHoldCubeStatus(15));
					if (gameEngine.BuildACubeGenerator(this.gameObject, col.collider.gameObject))
					{
						boomieMesh.GetComponent<Animator>().SetTrigger("Happy");
						cubes = -1;
						col.collider.GetComponent<BoomieBehaviour>().cubes = -1;
					}
				}
			}
			else if (spheres > 0)
			{
				sphere.GetComponent<Animator>().SetTrigger("Show");
				if (col.collider.GetComponent<BoomieBehaviour>().spheres > 0 && col.collider.GetComponent<BoomieBehaviour>().cubes <= 0)
				{
					canHoldASphere = false;
					StartCoroutine(WaitAndRestoreHoldSphereStatus(15));
					if (gameEngine.BuildASphereGenerator(this.gameObject, col.collider.gameObject))
					{
						boomieMesh.GetComponent<Animator>().SetTrigger("Happy");
						spheres = -1;
						col.collider.GetComponent<BoomieBehaviour>().spheres = -1;
					}
				}
			}
			else
			{
				// not interesting conversation
				StartCoroutine(WaitAndEndConversation(1, col.collider.gameObject));
			}
			UpdateSphereAndCube();
		}
	}

	IEnumerator WaitAndRestoreHoldCubeStatus(float time)
	{
		yield return new WaitForSeconds (time);
		canHoldACube = true;
	}

	IEnumerator WaitAndRestoreHoldSphereStatus(float time)
	{
		yield return new WaitForSeconds (time);
		canHoldASphere = true;
	}
}
