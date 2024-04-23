    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class InteractionTrigger : MonoBehaviour
    {
        
        [SerializeField] private Transform parentHP;
        [SerializeField] private TextMeshProUGUI textHp;
        [SerializeField] private int indexTrigger = 0;
        [SerializeField] public int _hp = 2;
        [SerializeField] private float yScaling=  0.5f;

        private bool _isNearbyPlayer;
        private Player player;


        private void Start()
        {
            if (textHp)
            {
                GetComponent<BoxCollider>().size *= 1.5f;
            }
        }

        public void ChangePos()
        {
            transform.DOMove(FindObjectOfType<Level>().posAnimal[Random.Range(0, 7)].transform.position, 1.5f);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") )
            {
                if (indexTrigger != GameManager.Instance.indexModeGame)
                {
                    return;
                }
                player = other.GetComponent<Player>();
                player.interactionTriggers.Add(this);
                if (player.interactionTrigger)
                {
                    
                    return;
                }
                player.interactionTrigger = this;
                if (indexTrigger == 0)
                {
                 
                    StartCoroutine(Cutt());
                    _isNearbyPlayer = true;
                }
                else
                {
                    Debug.Log("attack" + gameObject.name + Random.Range(0, 9999));
                    player.transform.DOLookAt(transform.position, 0.2f);
                    player.Attack();
                }
//gameObject.SetActive(false);
            }
        }

        private void FindNewIntercation(Player player)
        {
           
      
            player.interactionTriggers.Add(this);
         
            player.interactionTrigger = this;
            if (indexTrigger == 0)
            {
                 
                StartCoroutine(Cutt());
                _isNearbyPlayer = true;
            }
            else
            {
                Debug.Log("attack" + gameObject.name + Random.Range(0, 9999));
                player.transform.DOLookAt(transform.position, 0.2f);
                player.Attack();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (indexTrigger != GameManager.Instance.indexModeGame)
                {
                    return;
                }
                player.interactionTriggers.Remove(this);
                player.interactionTrigger = null;
                if (indexTrigger == 0)
                {
                    StopAllCoroutines();
                    player.GetComponent<Player>().StopCutt();
                    _isNearbyPlayer = false;
                    //  gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator Cutt()
        {
            //gameObject.SetActive(false);
            Debug.Log(Random.Range(0, 9999)+"OFF"+gameObject.name);
         
            if (_isNearbyPlayer)
            {
                yield break;
            }
            for (int i = 0; i < 5; i++)
            {
                yScaling = 0.1f;
                transform.DOPunchRotation(new Vector3(yScaling, yScaling, 0), 0.3f);
                player.ProgressCutt((i+1)+"/5", ((float)i +1.0f) / 5.0f);
                yield return new WaitForSeconds(0.1f);
            }

           // transform.DOScaleY(0, 0.2f);
            player.StopCutt();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            
            for (int i = 0; i < 5; i++)
            {
                FindObjectOfType<GameManager>().SetResources(1, player.transform);
                yield return new WaitForSeconds(0.1f);
            }

            player.interactionTrigger = null;
         //   GameManager.Instance.Home();
gameObject.SetActive(false);
        }

        public async void SetDamage(int damage)
        {
           
            if (textHp)
            {
                _hp-=damage;
                parentHP.DOPunchRotation( new Vector3(0.03f, 0.03f, 0), 0.3f);
                textHp.text = _hp.ToString();
                if (_hp <= 0)
                {
                    player.interactionTrigger = null;
                    
                    for (int i = 0; i < 2; i++)
                    {
                        FindObjectOfType<GameManager>().SetResources(0, player.transform);
                        await Task.Delay(100);
                    }
                    gameObject.SetActive(false);
         
                    //    transform.DOScale(Vector3.zero, 0.2f);
                }
            }
        }
    }
