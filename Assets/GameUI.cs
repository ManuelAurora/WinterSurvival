using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [SerializeField] public GameObject meat;
    [SerializeField] public GameObject woods;

    public GameObject MeatIcon, WoodIcon;
    public TMP_Text MeatIconText, WoodIconText;

    [SerializeField] public ArrowsCollectionView arrowsCollectionView;
    [SerializeField] public GameObject HuntBase;
    [SerializeField] public RectTransform HuntSliderRect;
    [SerializeField] public RectTransform HuntSliderArrow;
    [SerializeField] public CoverView huntCoverView;
    [SerializeField] public CoverView cutCoverView;
    [SerializeField] public Image[] _imageHearts;
    [SerializeField] public Image[] _imageHearts2;
    [SerializeField] public Image coldFill;
    [SerializeField] public Heartbeat heart;
  
    public GameObject teaIcon;
    public TMP_Text teaLabel;
    
    public Win WinScreen;
    public Lost LostScreen;

    [SerializeField] public GameObject joystic;

    public float slideDuration = 3f;
    public bool isMovingLeft = false;
    public Tweener huntArrowMoveTween;
    public Vector2 arrowAnchorOrigin;

}
