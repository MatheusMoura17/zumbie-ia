using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Animator myAnimator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		myAnimator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
		myAnimator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));
	}
}
