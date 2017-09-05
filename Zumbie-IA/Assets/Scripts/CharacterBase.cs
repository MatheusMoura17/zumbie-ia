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
	public float atackDuration = 0.2f;

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
		myAnimator.SetTrigger("attack");
		damageObject.SetActive(true);
		Invoke("DisableAtack", atackDuration);
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
