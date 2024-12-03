using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] public float yScaling=  0.5f;


    public void AnimateChop()
    {
        transform.DOPunchRotation(new Vector3(yScaling, yScaling, 0), 0.3f);
    }
}
