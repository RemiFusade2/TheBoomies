using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public float speed;
	public float xMin;
	public float xMax;
	public float zMin;
	public float zMax;

	public float zoomStep;
	public float zoomMin;
	public float zoomMax;
	public float cameraMargins;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x < cameraMargins)
		{
			this.transform.position = new Vector3(this.transform.position.x-speed < xMin ? xMin : this.transform.position.x-speed, this.transform.position.y, this.transform.position.z);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x > Screen.width - cameraMargins)
		{
			this.transform.position = new Vector3(this.transform.position.x+speed > xMax ? xMax : this.transform.position.x+speed, this.transform.position.y, this.transform.position.z);
		}
		if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y > Screen.height - cameraMargins)
		{
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z+speed > zMax ? zMax : this.transform.position.z+speed);
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y < cameraMargins)
		{
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z-speed < zMin ? zMin : this.transform.position.z-speed);
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y+zoomStep > zoomMax ? zoomMax : this.transform.position.y+zoomStep , this.transform.position.z);
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y-zoomStep < zoomMin ? zoomMin : this.transform.position.y-zoomStep , this.transform.position.z);
		}
	}
}
