using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public InteractionTrigger interactionTrigger;
    public List<InteractionTrigger> interactionTriggers;
    public bool stop;
    public bool attack;
    [SerializeField] private float _speedArrow;
    [SerializeField] private List<LevelAttack> levelAttacks = new List<LevelAttack>();
    [SerializeField]  private float speed;
    [SerializeField] private TextMeshProUGUI textProgressCutt;
    [SerializeField] private GameObject progressParent;
    [SerializeField] private Image progressBar;

    [SerializeField] private Transform arrow;
    [SerializeField] private float startAngle, endAngle;

    [SerializeField] private GameObject attackObjectsStart;
    [SerializeField] private GameObject arrowAttack;
    
    private CharacterController _characterController;
    private DynamicJoystick _dynamicJoystick;
    private Animator _animator;


    private bool _notTrigger;
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _dynamicJoystick = FindObjectOfType<DynamicJoystick>();
        _animator = GetComponent<Animator>();
        stop = true;
    }

  [SerializeField]  private bool _left;
    // Update is called once per frame
    void Update()
    {
        Vector3 angles = arrow.localEulerAngles;
        if (angles.y>=300)
        {
            angles.y -= 360;
        }
Debug.Log("Angles"+angles);
        if (!attack && stop && Input.GetMouseButtonDown(0))
        {
            stop = false;
        }
        if (stop)
        {
         
            _animator.SetBool("Run", false);
            if (Input.GetMouseButtonDown(0))
            {
                //  StopAllCoroutines();
                if (!attack )
                {
                    stop = false;
                }
                for (int i = 0; i < levelAttacks.Count; i++)
                {
                    if (angles.y>=levelAttacks[i].start && angles.y<=levelAttacks[i].end)
                    {
                        Debug.Log(i+"---");
                        switch (i)
                        {
                            case 0:
                            {
                          //      interactionTrigger.gameObject.SetActive(false);
                                interactionTrigger.ChangePos();
                              StopAttack();
                                break;
                            }
                            case 3:
                            {
                              //  interactionTrigger.gameObject.SetActive(false);
                                interactionTrigger.ChangePos();
                                StopAttack();
                                break;
                            }
                            case 1:
                            {
                                UnityAction unityAction = () =>
                                {
                                    _arrowrot.Pause();
                                    attackObjectsStart.transform.DOScale(Vector3.zero, 0.5f);
                                    arrowAttack.transform.DOScale(Vector3.zero, 0.5f);
                                };
                                UnityAction continueAction = () =>
                                {
                                    _arrowrot.Play();
                                    attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
                                    arrowAttack.transform.DOScale(Vector3.one, 0.5f);
                                };
                                MethodWait(1, unityAction, continueAction, 2);
                                break;
                            }
                            case 2:
                            {
                                UnityAction unityAction = () =>
                                {
                                    _arrowrot.Pause();
                                    attackObjectsStart.transform.DOScale(Vector3.zero, 0.5f);
                                    arrowAttack.transform.DOScale(Vector3.zero, 0.5f);
                                };
                                UnityAction continueAction = () =>
                                {
                                    _arrowrot.Play();
                                    attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
                                    arrowAttack.transform.DOScale(Vector3.one, 0.5f);
                                };
                                MethodWait(1, unityAction, continueAction, 1);
                                break;
                            }
                        }
                   
                    }
                }
            }

            return;
        }
  
        
        _characterController.Move(new Vector3(_dynamicJoystick.Horizontal * speed * Time.deltaTime
            , -00.1f, _dynamicJoystick.Vertical * speed * Time.deltaTime));

        if (_dynamicJoystick.Direction.magnitude>=0.1f)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(_dynamicJoystick.Direction.x, 0, _dynamicJoystick.Direction.y));

            _animator.SetBool("Run", true);
        }
        else
        {
            _animator.SetBool("Run", false);
        }

  
        
        Debug.Log(angles+"Rotate::");

        
       
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
            GameManager.Instance.DisactiveCam(1);
            _dynamicJoystick.gameObject.SetActive(true);

            await Task.Delay(500);
         //  GameManager.Instance.Home();
        }
        //attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
      //  arrowAttack.transform.DOScale(Vector3.one, 0.5f);
       
    }

    private void StopAttack()
    {
        _notTrigger = true;
        attack = true;
       
        attackObjectsStart.SetActive(false);
      
        arrowAttack.SetActive(false);
        
        attack = false;
        GameManager.Instance.DisactiveCam(1);
        _dynamicJoystick.gameObject.SetActive(true);
        interactionTrigger = null;
    }
    public void ProgressCutt(string text, float percent)
    {
        if (!progressParent.activeSelf)
        {
            progressParent.SetActive(true);
            progressBar.fillAmount = 0;
        }
        textProgressCutt.text = text;
        progressBar.DOFillAmount(percent, 0.2f);
        Debug.Log("Percent"+ percent);
    }

    public void StopCutt()
    {
        progressParent.SetActive(false);
    }
[System.Serializable]
   private class LevelAttack
   {
       public float start, end;
   }

   public void Attack()
   {
       if (_notTrigger)
       {
           _notTrigger = false;
           return;
       }
       attack = true;
       stop = true;
       
       _arrowrot.Play();
       attackObjectsStart.transform.DOScale(Vector3.one, 0.5f);
       arrowAttack.transform.DOScale(Vector3.one, 0.5f);
       
       GameManager.Instance.JosticOff();
       GameManager.Instance.ActiveCam(1);
       attackObjectsStart.SetActive(true);
       StartCoroutine(Rotate(_speedArrow));
       arrowAttack.SetActive(true);
   }
}
