using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

	// Update is called once per frame
	public float speed = 1;

	void Update()
	{
		var position = new Vector3();
		if(Input.GetKey(KeyCode.W))
		{
			position.y += speed;
		}

		if(Input.GetKey(KeyCode.A)) {
			position.x -= speed;
		}

		if(Input.GetKey(KeyCode.S)) {
			position.y -= speed;
		}

		if(Input.GetKey(KeyCode.D)) {
			position.x += speed;
		}

		transform.position += position;
	}
}
