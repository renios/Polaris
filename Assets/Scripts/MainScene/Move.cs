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
	enum State {Idle, Walk, Fly, Hold, Touched}
	State state = State.Idle;
	bool isFirstFrame = true;
	float timer = 0;
	float speed = 0.5f;
	float delay;
	Vector2 direction;
	float originalGravity;
	bool picked;

	public void Pick() {
		picked = true;
		state = State.Hold;
		isFirstFrame = true;
	}

	public void Drop() {
		picked = false;
		state = State.Idle;
		isFirstFrame = true;
	}

	bool IsGround() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.8f, GroundLayer.value);
		if (!hit) return false;
		if (hit.collider.tag == "Ground") return true;
		else return false;
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "wall")
        {
            direction.x *= -1;
        }
        else if (collision.collider.name == "ceiling")
        {
            direction.y *= -1;
        }
    }

    void Walk () {
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

			delay = Random.Range(2.0f, 4f);
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

			delay = Random.Range(3f, 7f);
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
            speed = Random.Range(0.3f, 0.5f);
			delay = Random.Range(8f, 10f);
			timer = 0;
		}

		Fly();

		if (timer < delay) return;
        else
        {
            if (transform.localPosition.y > -3.2f && transform.localPosition.y < -2.0f) return;
            if (transform.localPosition.y > -1.5f && transform.localPosition.y < -0.5f) return;
        }
		state = State.Idle;
		isFirstFrame = true;
	}

	void UpdateHold() {
		if (isFirstFrame) {
			GetComponent<Rigidbody2D>().gravityScale = 0;
			gameObject.layer = LayerMask.NameToLayer("FlyingCharacter");
			Portrait.CrossFade("Hold");
			isFirstFrame = false;
		}

		// 마우스(손가락) 위치 따라다니도록 -> LobbyManager에서 함

		// 그랩 풀리면 fly 상태로
	}

	void UpdateTouched() {
		if (isFirstFrame) {
			GetComponent<Rigidbody2D>().gravityScale = 0;
			gameObject.layer = LayerMask.NameToLayer("FlyingCharacter");
			Portrait.CrossFade("Fly");
			isFirstFrame = false;
		}

		// 한번 움직이고 state 변경. Grounded면 idle로 아니면 fly로
	}

	// Use this for initialization
	void Start () {
		originalGravity = GetComponent<Rigidbody2D>().gravityScale;
		state = State.Idle;
		isFirstFrame = true;
		timer = 0;
		picked = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Pick();
		}
		if (Input.GetKeyUp(KeyCode.Space)) {
			Drop();
		}
		timer += Time.deltaTime;

		switch (state) {
			case State.Hold:
				UpdateHold();
				break;
			case State.Touched:
				UpdateTouched();
				break;
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
