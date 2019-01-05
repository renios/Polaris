using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    public bool IsAnimating;

    private void Update()
    {

    }

    public IEnumerator MoveStraight(float delta, float duration, bool passingStair)
    {
        IsAnimating = true;
        if (passingStair)
            transform.Translate(0, 0, 0.2f);
        yield return transform.DOMove(transform.position + new Vector3(delta, 0, 0), duration);
        if (passingStair)
            transform.Translate(0, 0, -0.2f);
        IsAnimating = false;
    }
}