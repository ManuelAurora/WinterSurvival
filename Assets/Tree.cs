using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] public float yScaling=  0.5f;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void toggleAnimator()
    {
        _animator.enabled = true;
    }
    public void AnimateChop()
    {
        transform.DOPunchRotation(new Vector3(yScaling, yScaling, 0), 0.3f);
    }
}
