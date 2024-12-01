using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using static Logic;
using Pathfinding;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public Arrow arrowPrefab;
    public GameObject teaPrefab;
    public GameObject teaImage;
    public List<GameObject> treePrefabs;
    public List<PreyType> preyTypes;
    public Prey animalPrefab;

    [Header("Other")]
    public AstarPath Astar;
    public MeshFilter surfaceMesh;
    public GameObject surface;
    public LayerMask surfaceLayer;

    public int indexModeGame;
    [SerializeField] private GameState_SO gameState;

    [SerializeField] private List<GameObject> _triggersReturnToHome = new List<GameObject>();
    [SerializeField] private GameObject[] UIResources;
    [SerializeField] private GameObject[] textRescourcesOnWindows;
    [SerializeField] private FrostEffect _frostEffect;
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private GameUI UI;
    [SerializeField] private TutorialPopup _tutorialPopup;

    [SerializeField] private List<Transform> spawnPlayers = new List<Transform>();
    [SerializeField] private List<GameObject> portals = new List<GameObject>();
    [SerializeField] private GameObject[] windows;

    [SerializeField] private TextMeshProUGUI textWinResourcesCountWood,
        textWinResourcesCountmeat,
        textLostResourcesCountWood,
        textLostResourcesCountmeat;

    [SerializeField] private Player player;

    [Header("Cameras")] [SerializeField] private CinemachineVirtualCamera[] cinemachineVirtualCamera;
    public CinemachineVirtualCamera camera;
    public CinemachineVirtualCamera chopCamera;
    public CinemachineVirtualCamera huntCamera;
    public CinemachineTargetGroup huntCameraGroupTarget;

    [SerializeField] private GameObject homeUI, PlayerUI;
    [SerializeField] private TextMeshProUGUI textTimer;

    [SerializeField] private List<Prey> PreyList;

    private GameObject _lastLevel;
    private bool isLost;

    private Coroutine freezeCoroutine;
    // private bool _menu;

    private int _localWood;
    private int _localMeat;

    private float shootCooldown = 0;

    [SerializeField] private int _hp = 3;

    private void Awake()
    {
        // _menu = true;
        Instance = this;
        PreyList = new List<Prey>();

        var navGraph = (NavMeshGraph)Astar.graphs[0];
        navGraph.sourceMesh = surfaceMesh.mesh;
        // navGraph.offset = surface.transform.position;
        // navGraph.scale = surface.transform.localScale.x;
        // navGraph.rotation = new Vector3(90, 0, surface.transform.localRotation.z);

        Astar.Scan();

        SpawnTrees(50, surface, surfaceLayer, 2, treePrefabs);
    }

    private void OnDisable()
    {
        gameState.meatInThisRun = 0;
        gameState.woodInThisRun = 0;
    }

    private void Start()
    {
        switch (gameState.current)
        {
            case GameState.Hunt:
                SpawnAnimals(
                    surface,
                    10,
                    surfaceLayer,
                    30,
                    4,
                    player.gameObject,
                    animalPrefab,
                    preyTypes,
                    ref PreyList
                );
                break;
        }

        SpawnTeaObjects(teaPrefab, surfaceMesh.GetComponent<MeshCollider>(), 3, 5);
        gameState.coldDamageTimerLeft = 0;
        gameState.shouldTakeColdDamage = false;
        gameState.playTime = 0;
        Application.targetFrameRate = 60;
        UI.arrowAnchorOrigin = UI.HuntSliderArrow.anchoredPosition;
        SetTimeCinemachine();
        ActivatePortal();

        switch (gameState.current)
        {
            case GameState.Lobby:
                break;
            case GameState.Hunt:
                UI.cutCoverView.gameObject.SetActive(false);
                UI.huntCoverView.Open();
                player.animator.SetBool("isHunt", true);
                player.chopItems.ToList().ForEach(item => item.SetActive(false));
                player.huntItems.ToList().ForEach(item => item.SetActive(true));
                player.interactionScanner.size = new Vector3(8, 20, player.shootRange);
                player.interactionScanner.center = new Vector3(0, 8, player.shootRange/2);
                UI.MeatIcon.SetActive(true);
                UI.WoodIcon.SetActive(false);
                player.interactionScanner.gameObject.SetActive(true);
                
                if (gameState.meetTutorialShown == false)
                {
                    _tutorialPopup.gameObject.SetActive(true);
                    _tutorialPopup.ShowTutorial(TutorType.Meat, () =>
                    {
                        if (gameState.rulesTutorialShown == false)
                        {
                            _tutorialPopup.gameObject.SetActive(true);
                            _tutorialPopup.ShowTutorial(TutorType.Rules, () =>
                            {
                                if (gameState.coldTutorialShown == false)
                                {
                                    _tutorialPopup.gameObject.SetActive(true);
                                    _tutorialPopup.ShowTutorial(TutorType.Cold, null);
                                }
                                else
                                {
                                    _tutorialPopup.gameObject.SetActive(false);
                                }
                            });
                        }
                        else
                        {
                            _tutorialPopup.gameObject.SetActive(false);
                        }
                    });
                }

                break;
            case GameState.Wood: 
                UI.huntCoverView.gameObject.SetActive(false);
                UI.cutCoverView.Open();
                camera = chopCamera;
                player.animator.SetBool("isHunt", false);
                PreyList.ForEach(p => p.gameObject.SetActive(false));
                player.chopItems.ToList().ForEach(item => item.SetActive(true));
                player.huntItems.ToList().ForEach(item => item.SetActive(false));
                player.interactionScanner.size = new Vector3(player.chopRange, player.chopRange, player.chopRange);
                player.interactionScanner.center = new Vector3(0, 2, 0);
                UI.WoodIcon.SetActive(true);
                UI.MeatIcon.SetActive(false);
                UI.arrowsCollectionView.gameObject.SetActive(false);
                player.interactionScanner.gameObject.SetActive(true);
                
                if (gameState.woodTutorShown == false)
                {
                    _tutorialPopup.gameObject.SetActive(true);
                    _tutorialPopup.ShowTutorial(TutorType.Wood, () =>
                    {
                        if (gameState.rulesTutorialShown == false)
                        {
                            _tutorialPopup.gameObject.SetActive(true);
                            _tutorialPopup.ShowTutorial(TutorType.Rules, () =>
                            {
                                if (gameState.coldTutorialShown == false)
                                {
                                    _tutorialPopup.gameObject.SetActive(true);
                                    _tutorialPopup.ShowTutorial(TutorType.Cold, null);
                                }
                                else
                                {
                                    _tutorialPopup.gameObject.SetActive(false);
                                }
                            });
                        }
                        else
                        {
                            _tutorialPopup.gameObject.SetActive(false);
                        }
                    });
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // public void SerResBornFire(int _meat, int _wood)
    // {
    //
    //     meatcountBonfire-=_meat;
    //     woodcountBonFire -= _wood;
    //     
    //     if (meatcountBonfire <0 || woodcountBonFire < 0)
    //     {
    //         TrySetDamage();
    //     }
    //     meatcountBonfire = Mathf.Clamp(meatcountBonfire, 0, 999999);
    //     woodcountBonFire = Mathf.Clamp(woodcountBonFire, 0, 999999);
    //     
    //     SetResourcesValue(0);
    //     SetResourcesValue(1);
    // }

    private void Update()
    {
        switch (gameState.current)
        {
            case GameState.Lobby:
                return;
            case GameState.Hunt:
                UI.MeatIconText.text = gameState.meatInThisRun.ToString();
                break;
            case GameState.Wood:
                UI.WoodIconText.text = gameState.woodInThisRun.ToString();
                break;
        }

        if (isLost)
        {
            Time.timeScale = 0;
            UI.coldFill.gameObject.SetActive(false);
            UI.LostScreen.MeatIcon.SetActive(gameState.current == GameState.Hunt);
            UI.LostScreen.WoodIcon.SetActive(gameState.current == GameState.Wood);
            UI.LostScreen.resourcesCountMeat.text = "0";
            UI.LostScreen.resourcesCountWood.text = "0";
            UI.LostScreen.gameObject.SetActive(true);
            UI.WinScreen.gameObject.SetActive(false); // На случай если убили оленя, но замерзли потом
            return;
        }

        Countdown(ref shootCooldown);
        Countdown(ref gameState.coldDamageTimerLeft);

        if (gameState.timeToCold - gameState.playTime <= gameState.timeToCold / 3)
        {
            if (gameState.getBackTutorialShown == false)
            {
                _tutorialPopup.gameObject.SetActive(true);
                _tutorialPopup.ShowTutorial(TutorType.GetBack, null);
            }
        }

        foreach (var prey in PreyList)
        {
            Countdown(ref prey.idleTimer);
            SimulatePrey(prey, prey.AI, prey.seeker, player);
        }

        SimulatePlayer(player, UI, gameState, this, huntCameraGroupTarget, camera, huntCamera);


        if (gameState.playTime >= gameState.timeToCold && gameState.shouldTakeColdDamage == false)
        {
            gameState.shouldTakeColdDamage = true;
            freezeCoroutine = StartCoroutine(SetDamageFromFreez());
        }
        
        for (var i = 0; i < UI.arrowsCollectionView.ArrowViews.Length; i++)
        {
            UI.arrowsCollectionView.ArrowViews[i].gameObject.SetActive(i < gameState.arrows);
        }
        
        for (var i = 0; i < 3; i++)
        {
            if (gameState.playerHealth >= (i + 1))
            {
                UI._imageHearts[i].gameObject.SetActive(true);
                UI._imageHearts2[i].gameObject.SetActive(false);
            }
            else
            {
                UI._imageHearts2[i].gameObject.SetActive(true);
            }
        }

        gameState.playTime += Time.deltaTime;

        var value = gameState.playTime / gameState.timeToCold;

        UI.coldFill.fillAmount = value;
        
        if (gameState.playerHealth < 0)
        {
            isLost = true;
            gameState.playerHealth = 0;
        }
        
        if (gameState.shouldTakeColdDamage && gameState.coldDamageTimerLeft <= 0)
        {
            gameState.coldDamageTimerLeft = gameState.coldDamageTimer;
            UI.coldFill.transform.parent.gameObject.SetActive(false);
            UI.heart.gameObject.SetActive(true);
            UI.heart.AnimateHeart(() =>
            {
                gameState.playerHealth -= 1;
                UI.coldFill.transform.parent.gameObject.SetActive(true);
                UI.heart.gameObject.SetActive(false);
            });
        }

        UI.teaIcon.SetActive(gameState.tea > 0);
        UI.teaLabel.text = $"{gameState.tea}";
    }

    async void ActivatePortal()
    {
        await Task.Delay(3000);
        portals[0].SetActive(true);
    }

    private IEnumerator SetDamageFromFreez()
    {
        _frostEffect.enabled = true;
        for (int i = 0; i < 36; i++)
        {
            _frostEffect.FrostAmount += 0.01f;
            yield return new WaitForSeconds(0.05f);
        }

        // while (true)
        // {
        //     if (!TrySetDamage())
        //     {
        //         GameManager.Instance.Lost();
        //         yield break;
        //     }
        //
        //     
        //
        //     yield return new WaitForSeconds(2);
        // }
    }
    // public bool TrySetDamage()
    // {
    //     _hp -= 1;
    //     PlayerPrefs.SetInt("HP", _hp);
    //     for (int i = 0; i < 3; i++)
    //     {
    //         if (_hp>=(i + 1))
    //         {
    //             _imageHearts[i].gameObject.SetActive(false);
    //             _imageHearts2[i].gameObject.SetActive(false);
    //         }
    //         else
    //         {
    //             _imageHearts[i].gameObject.SetActive(true);
    //             _imageHearts2[i].gameObject.SetActive(true);
    //         }
    //     }
    //
    //     if (_hp>0)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // public void SetResourcesValue(int index)
    // {
    //     switch (index)
    //     {
    //         case 0:
    //         {
    //        
    //             textMeat.text = meat.ToString();
    //             textMeat2.text = meat.ToString();
    //             textBonFireMeat.text = meatcountBonfire.ToString();
    //             PlayerPrefs.SetInt("MeatBornFire", meatcountBonfire);
    //             PlayerPrefs.SetInt("Meat", meat);
    //             break;
    //         }
    //         case 1:
    //         {
    //        
    //             textWood.text = wood.ToString();
    //             textWood2.text = wood.ToString();
    //             
    //             textBonFireWood.text = woodcountBonFire.ToString();
    //             PlayerPrefs.SetInt("WoodBornFire", woodcountBonFire);
    //             PlayerPrefs.SetInt("Wood", wood);
    //             break;
    //         } 
    //     }
    // }
    //
    //

    public async void Hunt(int index)
    {
        _triggersReturnToHome.ForEach(x => x.gameObject.SetActive(false));
        _hp = 3;
        UIResources.ToList().ForEach(x => x.gameObject.SetActive(false));
        UIResources[index].SetActive(true);

        _localMeat = 0;
        _localWood = 0;
        int level = 2;
        Transform levelprefabClone = Instantiate(levelPrefabs[level]).transform;
        levelprefabClone.position = levelPrefabs[level].transform.position;
        levelprefabClone.localScale = levelPrefabs[level].transform.localScale;
        levelprefabClone.gameObject.SetActive(true);

        indexModeGame = index;
        Vector3 pos = spawnPlayers[0].transform.position;
        // Transform playerClone = Instantiate(prefabPlayer.transform, pos,Quaternion.identity);
        // cinemachineVirtualCamera.ToList().ForEach(x=>x.Follow = playerClone);
        // cinemachineVirtualCamera.ToList().ForEach(x => x.LookAt = playerClone);
        // cinemachineVirtualCamera[1].gameObject.SetActive(true);
        //
        // playerClone.parent = levelprefabClone;
        homeUI.SetActive(false);
        PlayerUI.SetActive(true);
        _lastLevel = levelprefabClone.gameObject;
        // DisactiveCam(1);

        await Task.Delay(3000);
        portals[0].SetActive(true);
    }

    // public void Home()
    // {
    //     StopAllCoroutines();
    //     _frostEffect.enabled = false;
    //     _frostEffect.FrostAmount = 0;
    //
    //     _triggersReturnToHome.ForEach(x=>x.gameObject.SetActive(false));
    //
    //     PlayerPrefs.SetInt("Degree", PlayerPrefs.GetInt("Degree") + 1);
    //          // SerResBornFire( 1, (int)(PlayerPrefs.GetInt("Degree")/5) * 2);
    //    _textTempWood.text = "-" + (PlayerPrefs.GetInt("Degree") / 5) * 2 + "/DAY";
    //     // UpdatetextTexmp();
    //     
    //     Destroy(_lastLevel, 0.5f);
    //     FindObjectOfType<CinemachineBrain>().m_DefaultBlend.m_Time = 0;
    //     cinemachineVirtualCamera[0].gameObject.SetActive(false);
    //     
    //     homeUI.SetActive(true);
    //     PlayerUI.SetActive(false);
    //
    // }

    public void Win()
    {
        
    }

    public void ShowWinScreen()
    {
        StopAllCoroutines();

        UI.heart.StopAllCoroutines();
        Time.timeScale = 0;
        UI.WinScreen.MeatIcon.SetActive(gameState.current == GameState.Hunt);
        UI.WinScreen.WoodIcon.SetActive(gameState.current == GameState.Wood);
        UI.WinScreen.resourcesCountMeat.text = gameState.meatInThisRun.ToString();
        UI.WinScreen.resourcesCountWood.text = gameState.woodInThisRun.ToString();
        UI.WinScreen.gameObject.SetActive(true);
    }

    public void Lost()
    {
        StopAllCoroutines();
        windows[1].SetActive(true);
        textLostResourcesCountmeat.text = _localMeat.ToString();
        textLostResourcesCountWood.text = _localWood.ToString();

        if (_localMeat > 0)
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

    async void CollectResources(Prey prey)
    {
        var meatFromHunt = Mathf.Min(prey.animal.stats.meat, gameState.currentBow.meatMax);

        for (int i = 0; i < meatFromHunt; i++)
        {
            PickResources(gameState.current == GameState.Hunt, player.transform, gameState);
            await Task.Delay(100);
        }
    }

    public void ActiveCam(int index)
    {
        Debug.Log("ActiveCam" + index);
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

    public void PickResources(bool isMeat, Transform transform, GameState_SO state)
    {
        GameObject iconObj = isMeat ? UI.meat : UI.woods;
        Transform meatIconClone = Instantiate(iconObj).transform;
        meatIconClone.gameObject.SetActive(true);
        meatIconClone.parent = iconObj.transform.parent;
        meatIconClone.position = Camera.main.WorldToScreenPoint(player.progressParent.transform.position);
        meatIconClone.localScale = iconObj.transform.localScale;
        meatIconClone.DOMove(iconObj.transform.position, 1f);
        meatIconClone.DOScale(Vector3.zero, 0.25f).SetDelay(0.9f);

        state.meatInThisRun += isMeat ? 1 : 0;
        state.woodInThisRun += isMeat ? 0 : 1;
    }
    
    private void CollectTea()
    {
        Transform teaIcon = Instantiate(teaImage).transform;
        teaIcon.gameObject.SetActive(true);
        teaIcon.parent = player.progressParent.transform.parent;
        teaIcon.position = player.progressBar.transform.position;
        teaIcon.localScale = teaIcon.transform.localScale;
        teaIcon.DOMove(teaIcon.transform.position + Vector3.up * 10, 1f);
        teaIcon.DOScale(Vector3.zero, 1f);
    }

    // private IEnumerator ResourcesMeatLess()
    // {
    //    
    //         meatcountBonfire--;
    //         textBonFireMeat.text = meatcountBonfire.ToString();
    //         yield return new WaitForSeconds(1);
    //    
    // }
    // private IEnumerator ResourcesWoodLess()
    // {
    //    
    //         woodcountBonFire--;
    //         textBonFireWood.text = woodcountBonFire.ToString();
    //         yield return new WaitForSeconds(1);
    //    
    // }

    public void DidFoundPrey(GameObject otherGameObject)
    {
        if (gameState.arrows == 0)
        {
            if (gameState.noArrowsTutorialShown == false)
            {
                _tutorialPopup.gameObject.SetActive(true);
                _tutorialPopup.ShowTutorial(TutorType.NoArrows, null);
            }
            return;
        };
        
        var prey = otherGameObject.GetComponent<Prey>();

        if (prey.isBeingAttacked)
        {
            return;
        }

        player.state = PlayerState.Hunting;
        player.CurrentPrey = prey;
    }

    public void DidTapToShoot()
    {
        DOTween.Kill(UI.huntArrowMoveTween);
        UI.huntArrowMoveTween = null;
        player.didShot = true;
        gameState.arrows -= 1;
        
        Shoot(UI, player, state =>
        {
            var arrow = Instantiate(arrowPrefab);
            arrow.launchPoint = player.launchPosition;
            arrow.prey = player.CurrentPrey;
            arrow.game = this;
            
            foreach (var playerArrow in player.arrows)
            {
                playerArrow.SetActive(false);
            }
            
            for (int i = 0; i < gameState.arrows; i++)
            {
                player.arrows[i].SetActive(true);
            }

            player.CurrentPrey.isBeingAttacked = true;
            player.CurrentPrey.lastHitState = state;
            
            switch (state)
            {
                case HitState.Headshot:
                    arrow.target = player.CurrentPrey.head.transform;
                    break;
                case HitState.Body:
                    arrow.target = player.CurrentPrey.body.transform;
                    break;
                case HitState.Miss:
                    arrow.target = player.CurrentPrey.transform;
                    player.CurrentPrey = null;
                    arrow.launchAngle = 50;
                    player.state = PlayerState.Idle;
                    player.didShot = false;
                    player.animator.SetBool("Aim", false);
                    break;
            }
        });
    }

    public void DidHitPrey(Prey prey)
    {
        bool DieIfNeeded(Prey prey)
        {
            if (prey.hp <= 0)
            {
                prey.state = PreyState.Die;
                prey.isFollowingPath = false;
                CollectResources(prey);
                return true;
            }

            return false;
        }

        var damage = gameState.currentBow.damage;

        switch (prey.lastHitState)
        {
            case HitState.Headshot:
                prey.hp -= damage + 2;
                prey.collider.enabled = false;
                prey.isFollowingPath = false;

                if (DieIfNeeded(prey) == false)
                {
                    prey.state = PreyState.Run;
                }               
                break;
            case HitState.Body:
                prey.collider.enabled = false;
                prey.isFollowingPath = false;
                
                prey.hp -= damage;
                
                if (DieIfNeeded(prey) == false)
                {
                    prey.state = PreyState.Run;
                }

                break;
            case HitState.Miss:
                prey.collider.enabled = false;
                prey.state = PreyState.Run;
                prey.isFollowingPath = false;
                break;
        }
        
        player.CurrentPrey = null;
        player.state = PlayerState.Idle;
        player.didShot = false;
        player.animator.SetBool("Aim", false);
    }

    public void WinButtonPressed()
    {
        UI.WinScreen.gameObject.SetActive(false);
        gameState.meat += gameState.meatInThisRun;
        gameState.wood += gameState.woodInThisRun;
        gameState.teaPlants += gameState.teaPlantsThisRun;
        gameState.meatInThisRun = 0;
        gameState.woodInThisRun = 0;
        gameState.teaPlantsThisRun = 0;
        
        switch (gameState.current)
        {
            case GameState.Hunt:
                UI.huntCoverView.Close();
                break;
            case GameState.Wood:
                UI.cutCoverView.Close();
                break;
            default: break;
        }
        LoadLobbyScene();
    }
    
    public void LostButtonPressed()
    {
        UI.LostScreen.gameObject.SetActive(false);
        gameState.meatInThisRun = 0;
        gameState.woodInThisRun = 0;
        gameState.teaPlantsThisRun = 0;
        
        switch (gameState.current)
        {
            case GameState.Hunt:
                UI.huntCoverView.Close();
                break;
            case GameState.Wood:
                UI.cutCoverView.Close();
                break;
            default: break;
        }
        LoadLobbyScene();
    }

    async void LoadLobbyScene()
    {
        gameState.current = GameState.Lobby;
        await Task.Delay(1000);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void DidCollectTeaPlant(GameObject teaPlant)
    {
        gameState.teaPlantsThisRun += 1;
        teaPlant.SetActive(false);
        CollectTea();
    }
    
    public void UIDidTapTutorialOK()
    {
        _tutorialPopup.gameObject.SetActive(false);
        _tutorialPopup.Close();
    }

    public void UIDidTapTea()
    {
        gameState.shouldTakeColdDamage = false;
        gameState.coldDamageTimerLeft = gameState.coldDamageTimer;
        gameState.tea -= 1;
        gameState.playTime = 0;
        StartCoroutine(DisableFrostEffect());
        
        if (freezeCoroutine != null) 
            StopCoroutine(freezeCoroutine);
    }

    IEnumerator DisableFrostEffect()
    {
        for (int i = 0; i < 36; i++)
        {
            _frostEffect.FrostAmount -= 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
        _frostEffect.enabled = false;
        _frostEffect.FrostAmount = 0;
    }

}