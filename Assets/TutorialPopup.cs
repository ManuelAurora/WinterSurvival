using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    public Sprite woodTutor;
    public Sprite getBackTutorial;
    public Sprite rulesTutorial;
    public Sprite coldTutorial;
    public Sprite meetTutorial;
    public Sprite noArrowsTutorial;

    public Image Popup;

    public TutorType CurrentType;
    [CanBeNull] private Action didClose;

    [SerializeField] private GameState_SO _gameState;
    
    public void ShowTutorial(TutorType type, [CanBeNull] Action didClose)
    {
        gameObject.SetActive(true);

        CurrentType = type;
        this.didClose = didClose;
        Time.timeScale = 0;
        
        Popup.sprite = type switch
        {
            TutorType.None => null,
            TutorType.Wood => woodTutor,
            TutorType.Meat => meetTutorial,
            TutorType.Rules => rulesTutorial,
            TutorType.Cold => coldTutorial,
            TutorType.GetBack => getBackTutorial,
            TutorType.NoArrows => noArrowsTutorial,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public void Close(bool forceClose = false)
    {
        switch (CurrentType)
        {
            case TutorType.None:
                break;
            case TutorType.Wood:
                _gameState.woodTutorShown = true;
                break;
            case TutorType.Meat:
                _gameState.meetTutorialShown = true;
                break;
            case TutorType.Rules:
                _gameState.rulesTutorialShown = true;
                break;
            case TutorType.Cold:
                _gameState.coldTutorialShown = true;
                break;
            case TutorType.GetBack:
                _gameState.getBackTutorialShown = true;
                break;
            case TutorType.NoArrows:
                _gameState.noArrowsTutorialShown = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Time.timeScale = 1;
        didClose?.Invoke();

        if (forceClose)
        {
            gameObject.SetActive(false);
        }
    }
}

public enum TutorType
{
    None,
    Wood,
    Meat,
    Rules,
    Cold,
    GetBack,
    NoArrows
}