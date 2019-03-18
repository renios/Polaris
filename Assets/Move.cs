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
	float speed = 0.5f;

	bool IsGround() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.8f, GroundLayer.value);
		if (!hit) return false;
		if (hit.collider.tag == "Ground") return true;
		else return false;
	}

	void Walk (Vector2 direction) {
		if (IsGround()) transform.position += Time.deltaTime * speed * Vector3.right;
	}

	void UpdateIdle() {
		if (isFirstFrame) {
			Portrait.CrossFade("Idle");
			isFirstFrame = false;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			state = State.Walk;
			isFirstFrame = true;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			state = State.Fly;
			isFirstFrame = true;
		}
	}

	void UpdateWalk() {
		if (isFirstFrame) {
			Portrait.CrossFade("Walk");
			isFirstFrame = false;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			state = State.Idle;
			isFirstFrame = true;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			state = State.Fly;
			isFirstFrame = true;
		}
	}

	void UpdateFly() {
		if (isFirstFrame) {
			Portrait.CrossFade("Fly");
			isFirstFrame = false;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			state = State.Idle;
			isFirstFrame = true;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			state = State.Walk;
			isFirstFrame = true;
		}
	}

	// Use this for initialization
	void Start () {
		state = State.Idle;
		isFirstFrame = true;
	}

	// Update is called once per frame
	void Update () {
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
