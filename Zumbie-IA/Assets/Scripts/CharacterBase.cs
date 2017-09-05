using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour {

	[Header("DefaultSettings")]
	public int life = 100;
	public int kills = 0;

	[Header("Animation")]
	public Animator myAnimator;

	[Header("DamageSettings")]
	public GameObject damageObject;
	private DamageAssistent damageAssistent;
	public int damage = 5;
	public float attackRatio = 0.5f;
	private float attackTimer = 0;

	public bool isKilled;

	public abstract void OnKilled();
	public abstract void OnReceiveHit();

	public void AddKill() {
		kills++;
	}

	private void Start() {
		damageAssistent = damageObject.GetComponent<DamageAssistent>();
		damageAssistent.damage = damage;
		damageAssistent.Owner = this;
	}

	public void Attack() {
		if (Time.time > attackTimer) {
			attackTimer =Time.time+attackRatio;
			myAnimator.SetTrigger("attack");
			damageObject.SetActive(true);
			Invoke("DisableAtack", attackRatio/2);
		}
	}

	private void DisableAtack() {
		damageObject.SetActive(false);
	}

	public void SetRunningAnimation(bool running, int direction) {
		myAnimator.SetBool("running", running);
		myAnimator.SetInteger("direction", direction);
	}

	public void OnKilledEnemy() {
		kills++;
	}

	void OnTriggerEnter(Collider other) {
		if (!isKilled && other.tag.Equals("Damage")) {
			myAnimator.SetTrigger("hit");
			DamageAssistent da = other.GetComponent<DamageAssistent>();
			life -= da.damage;
			OnReceiveHit();
			if (life <= 0) {
				life = 0;
				da.Owner.AddKill();
				myAnimator.SetTrigger("killed");
				isKilled = true;
				OnKilled();
			}
		}
	}
}
