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
    private float m_swipeResistanceY = 100f;

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
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 deltaSwipe = firstPosition - Input.mousePosition;
            //Debug.Log(deltaSwipe.y);
            if (Mathf.Abs(deltaSwipe.y) > m_swipeResistanceY)
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
