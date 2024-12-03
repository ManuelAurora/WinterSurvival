using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    //Hunt
    public float headshotCenterThreshold = 120f; // Порог для определения центра
    public float bodyCenterThreshold = 120f; // Порог для определения центра
    public float chopRange;
    public float shootRange;
    public GameObject shootRangePoint;
    public GameObject[] huntItems;
    public GameObject[] chopItems;
    public GameObject[] arrows;
    public Canvas worldCanvas;
    
    // Chop
    public Color green;
    public Color red;
    public List<Tree> treesAround;
    public Tree nearestTree;

    public float chopTurnSpeed;
    public float huntTurnSpeed;
    public Transform launchPosition;
    public InteractionTrigger interactionTrigger;
    public List<InteractionTrigger> interactionTriggers;
    public bool stop;
    public bool attack;
    [SerializeField] public BoxCollider interactionScanner;
    [SerializeField] private float _speedArrow;
    [SerializeField] private List<LevelAttack> levelAttacks = new List<LevelAttack>();
    [SerializeField] public float speed;
    [SerializeField] private TextMeshProUGUI textProgressCutt;
    [SerializeField] public GameObject progressParent;
    [SerializeField] public Image progressBar;

    [SerializeField] private Transform arrow;
    [SerializeField] private float startAngle, endAngle;

    [SerializeField] private GameObject attackObjectsStart;
    [SerializeField] private GameObject arrowAttack;
    
    public CharacterController controller;
    public Animator animator;
    private GameManager game;
    public PlayerState state;
    public Coroutine cutCoroutine;

    private AIPath _ai;

    // public Tweener huntTweener;
    public bool isAimingAtarget;
    public bool didShot;
    public Prey CurrentPrey;

    public List<Transform> CutSceneMovePoints;

    private void Awake()
    {
        _ai = GetComponent<AIPath>();
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void Update()
    {
//         Vector3 angles = arrow.localEulerAngles;
//         if (angles.y>=300)
//         {
//             angles.y -= 360;
//         }
// Debug.Log("Angles"+angles);
//         if (!attack && stop && Input.GetMouseButtonDown(0))
//         {
//             stop = false;
//         }
//         if (stop)
//         {
//             _animator.SetBool("Run", false);
//             if (Input.GetMouseButtonDown(0))
//             {
//                 //  StopAllCoroutines();
//                 if (!attack )
//                 {
//                     stop = false;
//                 }
//                 for (int i = 0; i < levelAttacks.Count; i++)
//                 {
//                     if (angles.y>=levelAttacks[i].start && angles.y<=levelAttacks[i].end)
//                     {
//                         Debug.Log(i+"---");
//                         switch (i)
//                         {
//                             case 0:
//                             {
//                           //      interactionTrigger.gameObject.SetActive(false);
//                                 interactionTrigger.ChangePos();
//                               StopAttack();
//                                 break;
//                             }
//                             case 3:
//                             {
//                                 if (interactionTrigger == null) return;
//                               //  interactionTrigger.gameObject.SetActive(false);
//                                 interactionTrigger.ChangePos();
//                                 StopAttack();
//                                 break;
//                             }
//                             case 1:
//                             {
//                                 UnityAction unityAction = () =>
//                                 {
//                                     _arrowrot.Pause();
//                                     // attackObjectsStart.transform.DOScale(Vector3.zero, 0.5f);
//                                     arrowAttack.transform.DOScale(Vector3.zero, 0.5f);
//                                 };
//                                 UnityAction continueAction = () =>
//                                 {
//                                     _arrowrot.Play();
//                                     // attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
//                                     arrowAttack.transform.DOScale(Vector3.one, 0.5f);
//                                 };
//                                 MethodWait(1, unityAction, continueAction, 2);
//                                 break;
//                             }
//                             case 2:
//                             {
//                                 UnityAction unityAction = () =>
//                                 {
//                                     _arrowrot.Pause();
//                                     // attackObjectsStart.transform.DOScale(Vector3.zero, 0.5f);
//                                     arrowAttack.transform.DOScale(Vector3.zero, 0.5f);
//                                 };
//                                 UnityAction continueAction = () =>
//                                 {
//                                     _arrowrot.Play();
//                                     // attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
//                                     arrowAttack.transform.DOScale(Vector3.one, 0.5f);
//                                 };
//                                 MethodWait(1, unityAction, continueAction, 1);
//                                 break;
//                             }
//                         }
//                    
//                     }
//                 }
//             }
//
//             return;
//         }
    }

    private Tweener _arrowrot;
    private IEnumerator Rotate(float time)
    {
     
        while (true)
        {
            _arrowrot = arrow.DOLocalRotate(new Vector3(0, endAngle, 0), time);
            yield return new WaitForSeconds(time);
            _arrowrot = arrow.DOLocalRotate(new Vector3(0, startAngle, 0), time);
            yield return new WaitForSeconds(time);
        }
    }

    private async void MethodWait(int time, UnityAction unityAction, UnityAction continueAction, int damage)
    {
   
        await Task.Delay(time);
        Debug.Log("SetDamage"+ Random.Range(0, 9999));
        interactionTrigger.SetDamage(damage);
        unityAction.Invoke();
        await Task.Delay(500);
     
        if (interactionTrigger && interactionTrigger._hp >0)
        {
            continueAction.Invoke();    
        }
        else
        {
         //   stop = false;
            attack = false;
            // GameManager.Instance.DisactiveCam(1);
            StopAttack();

            await Task.Delay(500);
         //  GameManager.Instance.Home();
        }
        //attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
      //  arrowAttack.transform.DOScale(Vector3.one, 0.5f);
       
    }

    private void StopAttack()
    {
        attack = true;
       
        attackObjectsStart.SetActive(false);
      
        arrowAttack.SetActive(false);
        
        attack = false;
        // GameManager.Instance.DisactiveCam(1);
        interactionTrigger = null;
    }
    
    public void ProgressCutt(string text, float percent)
    {
        if (!progressParent.activeSelf)
        {
            progressParent.SetActive(true);
        }
        progressBar.DOFillAmount(percent, 0.2f);
        Debug.Log("Percent"+ percent);
    }

    public void StopCutt()
    {
        progressBar.fillAmount = 0;
        progressParent.SetActive(false);
    }
    
[System.Serializable]
   private class LevelAttack
   {
       public float start, end;
   }
   

   private void OnTriggerStay(Collider other)
   {
       if (other.gameObject.CompareTag("Prey") && CurrentPrey == null)
       {
           game.DidFoundPrey(other.gameObject);
       }
   }

   private void OnTriggerEnter(Collider other)
   { 
       if (other.gameObject.CompareTag("Wood"))
       {
           var tree = other.GetComponent<Tree>();
           if (treesAround.Contains(tree) == false)
           {
               treesAround.Add(tree);
           }
       }

       if (other.gameObject.CompareTag("TeaPlant"))
       {
           game.DidCollectTeaPlant(other.gameObject);
       }
   }

   private void OnTriggerExit(Collider other)
   {
       if (other.gameObject.CompareTag("Wood"))
       {
           treesAround.Remove(other.GetComponent<Tree>());
       }
   }

   // CutScene signals

   public void StartIdle()
   {
       animator.SetTrigger("IdleSignal");
   }
   public void StartFishing()
   {
       animator.SetTrigger("FishingSignal");
   }

   public void MoveToCutPoint()
   {
       _ai.target = CutSceneMovePoints.First();
   }

}

public enum PlayerState
{
    Hunting,
    Cutting,
    Idle,
    Running
}

public enum HitState
{
    Headshot,
    Body,
    Miss
}