using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AnyPortrait;
using Random = UnityEngine.Random;

public class Move : MonoBehaviour
{
	public apPortrait Portrait;
	[HideInInspector] public bool allowTouch = true;
	
	enum State {Idle, Walk, Fly, Falling, Hold, Touched}
	State state = State.Idle;
	float delay;
	Vector2 direction;
	float originalGravity;
	bool picked;
	
	byte floor;
	bool isInGround;
	bool isInWall;

	RaycastHit2D[] hits = new RaycastHit2D[5];
	
	void Start()
	{
		picked = false;
		// Idle 상태에서 시작.
		StartCoroutine(Idle(true));
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.CompareTag("Wall"))
		{
			Debug.Log(gameObject.name + ": Wall Collided.");
			direction.x *= -1;
			
			if (direction.x < 0) 
				transform.localScale = new Vector3(-0.9f, 0.9f, 1);
			else 
				transform.localScale = new Vector3(0.9f, 0.9f, 1);
		}
		else if (collision.collider.CompareTag("Ceiling"))
		{
			Debug.Log(gameObject.name + ": Ceiling Collided.");
			direction.y *= -1;
		}
	}

	float GetZPos(float y)
	{
		if (floor == 1)
			return Mathf.LerpUnclamped(-0.1f, -4, Mathf.InverseLerp(0, -5.8f, y));
		else
			return Mathf.LerpUnclamped(-5.1f, -9, Mathf.InverseLerp(7.6f, 3.5f, y));
	}

	IEnumerator Idle(bool shouldFall)
	{
		state = State.Idle;
		GetComponent<Rigidbody2D>().gravityScale = 0;
		Debug.Log(gameObject.name + " State: IDLE");

		// 현재 좌표에 대한 정보를 얻어온다. 지금 몇 층에 있는가? 지금 서 있는가 아니면 떠 있는가?
		GetCurrentPosInfo();
		transform.position = new Vector3(transform.position.x, transform.position.y, GetZPos(transform.position.y));
		Debug.Log(gameObject.name + " is now in floor " + floor + ", while " + (isInGround ? "" : "not ") + "on the ground.");
		
		// 만약 캐릭터가 (최초 스폰, 유저에 의한 이동 등으로) 떠 있거나 땅이 아닌 곳에 있다면,
		// 층에 따라 지정된 y좌표까지 중력을 이용하여 땅으로 떨어뜨리거나 밀어낸다
		if (shouldFall)
		{
			if (floor == 1 && transform.position.y > -2.9f)
			 	yield return Fall(transform.position.x);
			else if (floor == 2 && transform.position.y > 5.4f)
			 	yield return Fall(transform.position.x);
		}
		else if (!isInGround)
			yield return Fall(transform.position.x);
		
		// 애니메이션 재생
		gameObject.layer = LayerMask.NameToLayer("GroundedCharacter");
		Portrait.CrossFade("Idle");
		Portrait.SetControlParamInt("Emotion", 0);

		// 2~4초 동안 대기. 중간에 입력이 있으면 빠져나온다
		float time = Random.Range(2f, 4f);
		Debug.Log(gameObject.name + " will wait for " + time + " seconds.");
		float t = 0;
		while (t < time)
		{
			if (state != State.Idle)
				yield break;

			t += Time.deltaTime;
			yield return null;
		}

		// 이동을 위한 방향벡터 설정
		int angle = Random.Range(0, 360);
		direction = new Vector2(Mathf.Cos(angle / 180f * Mathf.PI), Mathf.Sin(angle / 180f * Mathf.PI));
		if (direction.x < 0) 
			transform.localScale = new Vector3(-0.9f, 0.9f, 1);
		else 
			transform.localScale = new Vector3(0.9f, 0.9f, 1);
		
		// 60% 확률로 걷고, 40% 확률로 난다
		int rnd = Random.Range(0, 100);
		if (rnd < 60)
			StartCoroutine(Walk());
		else
			StartCoroutine(Fly());
	}

	IEnumerator Fall(float originX)
	{
		state = State.Falling;
		Debug.Log(gameObject.name + " State: FALLING");

		// 터치를 막고, 층마다 지정된 지점까지 떨어뜨린다
		allowTouch = false;
		float destY = floor == 1 ? -2.9f : 5.4f;
		GetComponent<Rigidbody2D>().gravityScale = 1;

		while (transform.position.y > destY)
			yield return null;
		
		// 다 떨어졌으면 원상복구 한다
		GetComponent<Rigidbody2D>().gravityScale = 0;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		transform.position = new Vector3(originX, destY, GetZPos(destY));

		allowTouch = true;
		state = State.Idle;
		Debug.Log(gameObject.name + " State: IDLE");
	}

	IEnumerator Walk()
	{
		state = State.Walk;
		Debug.Log(gameObject.name + " State: WALK");
		
		// 애니메이션 재생
		gameObject.layer = LayerMask.NameToLayer("GroundedCharacter");
		Portrait.CrossFade("Walk");

		// 3~7초 동안 걷는다. 중간에 입력이 있으면 빠져나온다.
		var time = Random.Range(3f, 7f);
		float t = 0;
		Debug.Log(gameObject.name + " will walk " + time + " seconds in direction " + direction);
		while (t < time)
		{
			if (state != State.Walk)
				yield break;
			
			// 지정된 지면 영역(Polygon Collider로 지정함)을 벗어나면 방향을 반대로 바꾸는데, 약간의 랜덤성을 줬다.
			if(!IsInWalkingArea((Vector2)transform.position - new Vector2(0, 1.45f) + 
			                    new Vector2(direction.x, direction.y * 0.5f) * (Time.deltaTime * 0.2f)))
				TurnDirection();

			transform.Translate(direction.x * 0.3f * Time.deltaTime, direction.y * 0.15f * Time.deltaTime,
				GetZPos(transform.position.y + direction.y * 0.15f * Time.deltaTime) - transform.position.z);
			t += Time.deltaTime;
			yield return null;
		}

		// 다 걸었으면 Idle 상태로 바꾼다
		StartCoroutine(Idle(false));
	}

	IEnumerator Fly()
	{
		state = State.Fly;
		Debug.Log(gameObject.name + " State: FLY");
		
		// 애니메이션 재생
		gameObject.layer = LayerMask.NameToLayer("FlyingCharacter");
		Portrait.CrossFade("Fly");
		Portrait.SetControlParamInt("Emotion", 0);

		// 현재 바라보고 있는 방향의 위로 날아간다.
		// 8~10초 동안 난다. 중간에 입력이 있을 경우 빠져나온다.
		direction = new Vector2(transform.localScale.x > 0 ? 0.3f : -0.3f, 0.5f).normalized;
		var time = Random.Range(8f, 10f);
		Debug.Log(gameObject.name + " will fly " + time + " seconds in direction " + direction);
		float t = 0;
		while (t < time)
		{
			if (state != State.Fly)
				yield break;

			transform.Translate(direction.x * Time.deltaTime, direction.y * Time.deltaTime,
				GetZPos(transform.position.y + direction.y * Time.deltaTime) - transform.position.z);
			t += Time.deltaTime;
			yield return null;
		}

		// 다 날았으면 Idle 상태로 바꾼다. 이 때, 강제 떨어짐을 활성화 시킨다. 날아다녔다는 점을 부각하기 위해서이다.
		StartCoroutine(Idle(true));
	}

	IEnumerator Touched()
	{
		// 이전 상태를 저장하고 상태를 바꾼다.
		var befState = state;
		state = State.Touched;
		
		// 애니메이션 재생. 1초 동안 애니메이션이 재생된다. 그동안 다른 입력을 막는다.
		gameObject.layer = LayerMask.NameToLayer("FlyingCharacter");
		Portrait.CrossFade("Touch");
		allowTouch = false;
		yield return new WaitForSeconds(1);
		
		// 입력 제한을 풀고, 이전 상태를 다시 시행한다.
		allowTouch = true;
		if (befState == State.Idle)
			StartCoroutine(Idle(false));
		else if (befState == State.Walk)
			StartCoroutine(Walk());
		else if (befState == State.Fly)
			StartCoroutine(Fly());
	}

	void GetCurrentPosInfo()
	{
		var hitCount = Physics2D.RaycastNonAlloc((Vector2)transform.position - new Vector2(0, 1.45f), Vector2.zero, hits);
		isInGround = false;
		for(int i = 0; i < hitCount; i++)
		{
			if (hits[i].collider.name == "1F")
				floor = 1;
			else if (hits[i].collider.name == "2F")
				floor = 2;
			else if (hits[i].collider.CompareTag("Ground"))
				isInGround = true;
		}
	}

	bool IsInWalkingArea(Vector2 pos)
	{
		var hitCount = Physics2D.RaycastNonAlloc(pos, Vector2.zero, hits);
		for(int i = 0; i < hitCount; i++)
		{
			if (hits[i].collider.CompareTag("Ground"))
				return true;
		}

		return false;
	}

	void TurnDirection()
	{
		var deg = Vector2.Angle(Vector2.right, direction);
		deg += 180 + Random.Range(-30f, 30f);
		direction = new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad));
		
		if (direction.x < 0) 
			transform.localScale = new Vector3(-0.9f, 0.9f, 1);
		else 
			transform.localScale = new Vector3(0.9f, 0.9f, 1);
	}

	public bool IsPicked() {
		return state == State.Hold;
	}
	public void Pick() 
	{
		picked = true;
		state = State.Hold;
		Debug.Log(gameObject.name + " State: HOLD");
		
		// 애니메이션 재생
		gameObject.layer = LayerMask.NameToLayer("FlyingCharacter");
		Portrait.CrossFade("Hold");
		Portrait.SetControlParamInt("Emotion", 1);
	}

	public void Drop() 
	{
		picked = false;
		StartCoroutine(Idle(false));
	}
	public void Touch()
	{
		StartCoroutine(Touched());
	}
}
