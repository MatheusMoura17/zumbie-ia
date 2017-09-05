using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController:MonoBehaviour {

	public Animator myAnimator;
	public float moveSpeed = 3;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
		myAnimator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
		//myAnimator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));
	}
}
