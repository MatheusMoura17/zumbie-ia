using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterClass {
	MONKEY,
	ORC
}

public class EnemyController:CharacterBase {

	public EnemyController target;

	public float attackDistance = 4;

	public CharacterClass characterClass;
	private NavMeshAgent agent;
	private Rigidbody myRigdbody;
	public FollowDecision followDecision;

	private bool isRunning;

	private void Start() {
		Init();
		agent = GetComponent<NavMeshAgent>();
		agent.autoBraking = true;
	}

	public void DefineTarget() {
		target = followDecision.FindEnemy(this);
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
			if (target!=null) {
				agent.destination = target.transform.position;
				isRunning = agent.velocity != Vector3.zero;
				SetRunningAnimation(isRunning, 0);
				if (Vector3.Distance(transform.position, target.transform.position)<= attackDistance)
					Attack();
			}
		}
	}
}
