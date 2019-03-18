using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AnyPortrait;

public class Move : MonoBehaviour {

	public LayerMask GroundLayer;
	public Transform CharacterTransform;
	public apPortrait Portrait;
	enum State {Idle, Walk, Fly}
	State state = State.Idle;
	bool isFirstFrame = true;
	float timer = 0;
	float speed = 0.5f;
	float delay;
	Vector2 direction;
	float originalGravity;

	bool IsGround() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.8f, GroundLayer.value);
		if (!hit) return false;
		if (hit.collider.tag == "Ground") return true;
		else return false;
	}

	void Walk () {
		// if (direction.x < 0)

		if (direction.x < 0) transform.localScale = new Vector3(-0.25f, 0.25f, 1);
		else transform.localScale = new Vector3(0.25f, 0.25f, 1); 

		transform.position += Time.deltaTime * speed * (Vector3)direction;
	}

	void Fly () {
		if (direction.x < 0) transform.localScale = new Vector3(-0.25f, 0.25f, 1);
		else transform.localScale = new Vector3(0.25f, 0.25f, 1); 

		transform.position += Time.deltaTime * speed * (Vector3)direction;
	}

	void UpdateIdle() {
		if (isFirstFrame) {
			GetComponent<Rigidbody2D>().gravityScale = originalGravity;
			gameObject.layer = LayerMask.NameToLayer("GroundedCharacter");
			Portrait.CrossFade("Idle");
			isFirstFrame = false;

			delay = Random.Range(0.5f, 3f);
			timer = 0;
		}

		if (timer < delay) return;

		if (Random.Range(0, 2) == 0) {
			state = State.Walk;
			isFirstFrame = true;
		}
		else {
			state = State.Fly;
			isFirstFrame = true;
		}
	}

	void UpdateWalk() {
		if (isFirstFrame) {
			GetComponent<Rigidbody2D>().gravityScale = originalGravity;
			gameObject.layer = LayerMask.NameToLayer("GroundedCharacter");
			Portrait.CrossFade("Walk");
			isFirstFrame = false;

			direction = new Vector2(Random.Range(-1f, 1f), 0).normalized;
			speed = Random.Range(0.1f, 0.5f);

			delay = Random.Range(1f, 5f);
			timer = 0;
		}

		Walk();

		if (timer < delay) return;

		if (Random.Range(0, 2) == 0) {
			state = State.Idle;
			isFirstFrame = true;
		}
		else {
			state = State.Fly;
			isFirstFrame = true;
		}
	}

	void UpdateFly() {
		if (isFirstFrame) {
			GetComponent<Rigidbody2D>().gravityScale = 0;
			gameObject.layer = LayerMask.NameToLayer("FlyingCharacter");
			Portrait.CrossFade("Fly");
			isFirstFrame = false;

			direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
			speed = Random.Range(0.1f, 0.5f);

			delay = Random.Range(3f, 5f);
			timer = 0;
		}

		Fly();

		if (timer < delay) return;

		state = State.Idle;
		isFirstFrame = true;
	}

	// Use this for initialization
	void Start () {
		originalGravity = GetComponent<Rigidbody2D>().gravityScale;
		state = State.Idle;
		isFirstFrame = true;
		timer = 0;
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		switch (state) {
			case State.Idle:
				UpdateIdle();
				break;
			case State.Walk:
				UpdateWalk();
				break;
			case State.Fly:
				UpdateFly();
				break;
		}
	}
}
