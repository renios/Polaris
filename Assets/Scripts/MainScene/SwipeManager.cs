using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public enum SwipeDirection
    {
        None = 0, Up = 1, Down = 2
    }

    private static SwipeManager _instance;

    public static SwipeManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = new GameObject("SwipeManager").AddComponent<SwipeManager>();
            }
            return _instance;
        }
    }

    public SwipeDirection Direction { get; private set; }

    private Vector3 firstPosition;
    //이 길이보다 짧게 스와이프하면 스와이프로 인정되지 않습니다.
    private float m_swipeResistanceY = 100f;
    private float startTime;
    //이 시간보다 짧게 스와이프하면 스와이프로 인정되지 않습니다.
    private float SwipeTime = 0.5f;

    private void Start()
    {
        _instance = this;
    }

    private void Update()
    {
        Direction = SwipeDirection.None;
        if (Input.GetMouseButtonDown(0))
        {
            firstPosition = Input.mousePosition;
            startTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            float duration = Time.time - startTime; startTime = 0f;
            Vector2 deltaSwipe = firstPosition - Input.mousePosition;
            if (Mathf.Abs(deltaSwipe.y) > m_swipeResistanceY && (duration < SwipeTime))
            {
                Direction |= (deltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }

    }

    public bool IsSwiping(SwipeDirection dir)
    {
        return (Direction & dir) == dir;
    }

}
