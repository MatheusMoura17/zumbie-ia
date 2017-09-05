using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController:MonoBehaviour {

	public Animator myAnimator;
	public float moveSpeed = 3;
	public float rotateSpeed = 10;
	public Transform targetRotationLook;

	private bool isRunning;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

		isRunning = (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

		if (isRunning) {
			transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
			Vector3 eulerAngles = transform.eulerAngles;
			eulerAngles.y = Mathf.Lerp(eulerAngles.y, targetRotationLook.eulerAngles.y, rotateSpeed * Time.deltaTime);
			transform.eulerAngles = eulerAngles;
		}

		myAnimator.SetBool("running", isRunning);
	}
}
