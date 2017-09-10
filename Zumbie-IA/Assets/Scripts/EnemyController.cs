using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController:CharacterBase {

	public Transform target;

	private NavMeshAgent agent;
	private Rigidbody myRigdbody;

	private bool isRunning;

	private void Start() {
		agent = GetComponent<NavMeshAgent>();
		agent.autoBraking = true;
	}

	public override void OnKilled() {
		//atualizar hud
	}

	public override void OnReceiveHit() {
		//atualizar hud
	}

	// Update is called once per frame
	void Update() {


		if (!isKilled) {
			agent.destination = target.position;
			isRunning = agent.velocity!=Vector3.zero;
			SetRunningAnimation(isRunning, 0);
		}
	}
}
