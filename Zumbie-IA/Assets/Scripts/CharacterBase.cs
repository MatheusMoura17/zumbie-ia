using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour {

	[Header("LifeSettings")]
	public int life = 100;

	[Header("Animation")]
	public Animator myAnimator;

	[Header("DamageSettings")]
	public GameObject damageObject;
	public int damage = 5;
	public float atackDuration = 0.1f;

	private bool isKilled;

	public abstract void OnKilled();
	public abstract void OnReceiveHit();

	public void Attack() {
		myAnimator.SetTrigger("attack");
		damageObject.SetActive(true);
		Invoke("DisableAtack", atackDuration);
	}

	private void DisableAtack() {
		damageObject.SetActive(false);
	}

	public void SetRunningAnimation(bool running) {
		myAnimator.SetBool("running", running);
	}

	void OnTriggerEnter(Collider other) {
		if (!isKilled && other.tag.Equals("Damage")) {
			myAnimator.SetTrigger("hit");
			OnReceiveHit();
			if (life <= 0) {
				life = 0;
				myAnimator.SetTrigger("killed");
				isKilled = true;
				OnKilled();
			}
		}
	}
}
