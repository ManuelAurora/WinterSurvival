using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int meat, wood, temp;
    public int indexModeGame;

    [SerializeField] private GameObject[] UIResources;
    [SerializeField] private TextMeshProUGUI _textTempWood;
    [SerializeField] private GameObject[] textRescourcesOnWindows;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private FrostEffect _frostEffect;
    [SerializeField] private GameObject[] levelPrefabs;
    
    [SerializeField] private GameObject joystic;
    [SerializeField] private TextMeshProUGUI textMeat, textMeat2, textWood, textWood2, textTemp;
    [SerializeField] private List<Transform> spawnPlayers = new List<Transform>();
    [SerializeField] private List<GameObject> portals = new List<GameObject>();
    [SerializeField] private GameObject[] windows;

    [SerializeField] private TextMeshProUGUI textWinResourcesCountWood, textWinResourcesCountmeat, textLostResourcesCountWood, textLostResourcesCountmeat;
    
    [SerializeField] private Player prefabPlayer;
    [SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCamera;

    [SerializeField] private GameObject homeUI, PlayerUI;

    [SerializeField] private GameObject meatIcon, woodIcon;
    [SerializeField] private TextMeshProUGUI textBonFireMeat,textBonFireWood ;
    [SerializeField] private TextMeshProUGUI textTimer;
    [SerializeField] private Image[] _imageHearts;
    [SerializeField] private Image[] _imageHearts2;
    private int _meatCount, _WoodCount;
    private int meatcountBonfire, woodcountBonFire;
    private GameObject _lastLevel;

    private bool _gameTimer;
    private float _timerLeft;

    private int _localWood;
    private int _localMeat;

   [SerializeField] private int _hp = 3;
    private void Awake()
    {
        Instance = this;

        meat = PlayerPrefs.GetInt("Meat");
        wood = PlayerPrefs.GetInt("Wood");

             meatcountBonfire = PlayerPrefs.GetInt("MeatBornFire");
        woodcountBonFire = PlayerPrefs.GetInt("WoodBornFire");
        SetResourcesValue(0);
        SetResourcesValue(1);


        if (!PlayerPrefs.HasKey("Degree"))
        {
            PlayerPrefs.SetInt("Degree", 5);
        }
        UpdatetextTexmp();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void SetResources(int _meat, int _wood)
    {
        meat += _meat;
        wood += _wood;

        meatcountBonfire-=_meat;
        woodcountBonFire -= _wood;
        
        meatcountBonfire = Mathf.Clamp(meatcountBonfire,0, 99999);
        woodcountBonFire = Mathf.Clamp(woodcountBonFire,0, 99999);
        SetResourcesValue(0);
        SetResourcesValue(1);
    }

    public void SerResBornFire(int _meat, int _wood)
    {
        meatcountBonfire-=_meat;
        woodcountBonFire -= _wood;
        
        SetResourcesValue(0);
        SetResourcesValue(1);
    }

    private void Update()
    {
        if (_gameTimer)
        {
            _timerLeft -= Time.deltaTime;

            if (_timerLeft>=60)
            {
                
            
            float min = _timerLeft / 60.0f;
            float second = (_timerLeft %  60.0f);
            textTimer.text = min.ToString("0") + ":"+second.ToString("00");
            }
            else
            {
                float min = 0;
                float second = _timerLeft;
                textTimer.text = min.ToString("0") + ":"+second.ToString("00");
            }
            if (_timerLeft<=0)
            {
                _gameTimer = false;
                StartCoroutine(SetDamageFromFreez());

                //  Home();
            }
        }
    }
    

   private IEnumerator SetDamageFromFreez()
    {
        for (int i = 0; i < 36; i++)
        {
           
                _frostEffect.FrostAmount += 0.01f;
                yield return new WaitForSeconds(0.01f);
        }
   
        while (true)
        {
            if (!TrySetDamage())
            {
                GameManager.Instance.Lost();
                yield break;
            }

            

            yield return new WaitForSeconds(2);
        }
    }
    public bool TrySetDamage()
    {
        _hp -= 1;
        for (int i = 0; i < 3; i++)
        {
            if (_hp>=(i + 1))
            {
                _imageHearts[i].gameObject.SetActive(false);
                _imageHearts2[i].gameObject.SetActive(false);
            }
            else
            {
                _imageHearts[i].gameObject.SetActive(true);
                _imageHearts2[i].gameObject.SetActive(true);
            }
        }

        if (_hp>0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetResourcesValue(int index)
    {
        switch (index)
        {
            case 0:
            {
           
                textMeat.text = meat.ToString();
                textMeat2.text = meat.ToString();
                textBonFireMeat.text = meatcountBonfire.ToString();
                PlayerPrefs.SetInt("MeatBornFire", meatcountBonfire);
                PlayerPrefs.SetInt("Meat", meat);
                break;
            }
            case 1:
            {
           
                textWood.text = wood.ToString();
                textWood2.text = wood.ToString();
                
                textBonFireWood.text = woodcountBonFire.ToString();
                PlayerPrefs.SetInt("WoodBornFire", woodcountBonFire);
                PlayerPrefs.SetInt("Wood", wood);
                break;
            } 
        }
    }

 

    public async void Hunt(int index)
    {
        _hp = 3;
        UIResources.ToList().ForEach(x=>x.gameObject.SetActive(false));
        UIResources[index].SetActive(true);
        
        _localMeat = 0;
        _localWood = 0;
        int level =2;
      Transform levelprefabClone = Instantiate(levelPrefabs[level]).transform;
      levelprefabClone.position = levelPrefabs[level].transform.position;
      levelprefabClone.localScale = levelPrefabs[level].transform.localScale;
      levelprefabClone.gameObject.SetActive(true);
      
        indexModeGame = index;
        Vector3 pos = spawnPlayers[0].transform.position; 
        Transform playerClone = Instantiate(prefabPlayer.transform, pos,Quaternion.identity);
        cinemachineVirtualCamera.ToList().ForEach(x=>x.Follow = playerClone);
        cinemachineVirtualCamera.ToList().ForEach(x => x.LookAt = playerClone);
        cinemachineVirtualCamera[0].gameObject.SetActive(true);

        playerClone.parent = levelprefabClone;
        homeUI.SetActive(false);
        PlayerUI.SetActive(true);
        _lastLevel = levelprefabClone.gameObject;
        DisactiveCam(1);

        _gameTimer = true;
        _timerLeft = 30;

        await Task.Delay(3000);
        portals[0].SetActive(true);
    }

    public void UpdatetextTexmp()
    {
        textTemp.text = "-"+PlayerPrefs.GetInt("Degree")+"°C";
    }
    public void Home()
    {
        
     StopAllCoroutines();
        PlayerPrefs.SetInt("Degree", PlayerPrefs.GetInt("Degree") + 1);
             SerResBornFire( 1, (int)(PlayerPrefs.GetInt("Degree")/5) * 2);
       _textTempWood.text = "-" + (PlayerPrefs.GetInt("Degree") / 5) * 2 + "/DAY";
        UpdatetextTexmp();
        
        _frostEffect.FrostAmount = 0;
        Destroy(_lastLevel, 0.5f);
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend.m_Time = 0;
        cinemachineVirtualCamera[0].gameObject.SetActive(false);
        
        homeUI.SetActive(true);
        PlayerUI.SetActive(false);

    }

    public void Win()
    {
        windows[0].SetActive(true);
        textWinResourcesCountmeat.text = _localMeat.ToString();
        textWinResourcesCountWood.text = _localWood.ToString();
        if (_localMeat>0)
        {
            textRescourcesOnWindows[0].SetActive(true);
            textRescourcesOnWindows[1].SetActive(false);
            textRescourcesOnWindows[2].SetActive(false);
            textRescourcesOnWindows[3].SetActive(false);
        }
        else
        {
            textRescourcesOnWindows[0].SetActive(false);
            textRescourcesOnWindows[1].SetActive(true);
            textRescourcesOnWindows[2].SetActive(false);
            textRescourcesOnWindows[3].SetActive(false);
        }
    }
    public void Lost()
    {
        windows[1].SetActive(true);
        textLostResourcesCountmeat.text = _localMeat.ToString();
        textLostResourcesCountWood.text = _localWood.ToString();
        
        if (_localMeat>0)
        {
            textRescourcesOnWindows[0].SetActive(false);
            textRescourcesOnWindows[1].SetActive(false);
            textRescourcesOnWindows[2].SetActive(false);
            textRescourcesOnWindows[3].SetActive(true);
        }
        else
        {
            textRescourcesOnWindows[0].SetActive(false);
            textRescourcesOnWindows[1].SetActive(false);
            textRescourcesOnWindows[2].SetActive(true);
            textRescourcesOnWindows[3].SetActive(false);
        }
    }
    private void SetTimeCinemachine()
    {
        FindObjectOfType<CinemachineBrain>().m_DefaultBlend.m_Time = 0.5f;
    }
    public void ActiveCam(int index)
    {
        Debug.Log("ActiveCam"+ index);
        if (index == 1)
        {
            SetTimeCinemachine();
        }
        cinemachineVirtualCamera[index].gameObject.SetActive(true);
    
    }
    public void DisactiveCam(int index)
    {
     
        
        cinemachineVirtualCamera[index].gameObject.SetActive(false);
      
    }
    public void JosticOff()
    {
        joystic.SetActive(false);
    }
 
    public void JosticOn()
    {
        joystic.SetActive(true); 
    }
    public void SetResources(int index, Transform pos)
    {
        GameObject iconObj = index == 0 ? meatIcon : woodIcon;
        Transform meatIconClone = Instantiate(iconObj).transform;
       meatIconClone.gameObject.SetActive(true);
       meatIconClone.position = Camera.main.WorldToScreenPoint(pos.position);
       meatIconClone.parent = iconObj.transform.parent;
       meatIconClone.localScale = iconObj.transform.localScale;
       meatIconClone.DOMove(iconObj.transform.position, 1f);
       meatIconClone.DOScale(Vector3.zero, 0.25f).SetDelay(0.9f);
       
       meat += index == 0 ? 1 : 0;
       wood += index == 0 ? 0 : 1;
       _localMeat += index == 0 ? 1 : 0;
       _localWood+= index == 0 ? 0 : 1;
       SetResourcesValue(index);

       
    }

    private IEnumerator ResourcesMeatLess()
    {
       
            meatcountBonfire--;
            textBonFireMeat.text = meatcountBonfire.ToString();
            yield return new WaitForSeconds(1);
       
    }
    private IEnumerator ResourcesWoodLess()
    {
       
            woodcountBonFire--;
            textBonFireWood.text = woodcountBonFire.ToString();
            yield return new WaitForSeconds(1);
       
    }
}
