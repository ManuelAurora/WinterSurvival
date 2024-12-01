using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;
using static Logic;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] public List<SceneReference> levels;

    [Header("Levels")] [SerializeField] private GameState_SO gameState;
    
    private int _meatCount, _WoodCount;

    [Header("In game items")] public Player player;
    public GameObject[] woods;
    public GameObject[] meats;
    public GameObject soup;
    public SpriteRenderer Background;

    [Header("UI")] 
    public GameObject fireCaution;
    public GameObject foodCaution;
    public GameObject arrowsCaution;

    [SerializeField] private UIUpgrade uiUpgradeScreen;
    [SerializeField] private TextMeshProUGUI _textTempWood;
    [SerializeField] private TextMeshProUGUI textMeat, textWood, textTemp;
    [SerializeField] private TMP_Text woodTowBurnLabel, meetToCookLabel;
    [SerializeField] private Image[] _imageHearts;
    [SerializeField] private Image[] _imageHearts2;
    [SerializeField] private ArrowsCollectionView arrowsCollectionView;
    [SerializeField] private Button meatButton;
    [SerializeField] private Button woodButton;
    [SerializeField] private Button makeArrowsButton;
    [SerializeField] private CoverView huntCoverView;
    [SerializeField] private CoverView cutCoverView;
    public Sprite darkBackground;
    public Sprite lightBackground;
    public GameObject upgradePopup;

    public Color textColor;

    public Axe_SO[] axes;
    public Bow_SO[] bows;
    public Bow_SO bowToUpgrade;
    public Axe_SO axeToUpgrade;

    public GameObject campFire;
    public GameObject smoke;
    public GameObject LightNight;
    
    private void Awake()
    {
        cutCoverView.openFeedback.StopFeedbacks(true);
        cutCoverView.closeFeedback.StopFeedbacks(true);
        huntCoverView.openFeedback.StopFeedbacks(true);
        huntCoverView.closeFeedback.StopFeedbacks(true);
        player.animator.SetTrigger("sit");
    }

    private void Update()
    {
        int woodRequired = Mathf.FloorToInt((0 - gameState.temperature) / 2.0f);
        gameState.temperature = 0 - gameState.level / 2;
        gameState.woodsToBurn = 1 + woodRequired;
        
        for (var i = 0; i < arrowsCollectionView.ArrowViews.Length; i++)
        {
            arrowsCollectionView.ArrowViews[i].gameObject.SetActive(i < gameState.arrows);
        }
        
        campFire.SetActive(gameState.didBurnWood);
        smoke.SetActive(gameState.didBurnWood == false);
        textMeat.text = gameState.meat.ToString();
        textWood.text = gameState.wood.ToString();
        _textTempWood.text = "-" + gameState.woodsToBurn + "/day";
        woodTowBurnLabel.text = $"{gameState.woodsToBurn}";
        meetToCookLabel.text = $"{gameState.meatToCook}";

        if (gameState.playerHealth < 3 && gameState.didCook)
        {
            meatButton.interactable = gameState.meat >= gameState.meatToCook && gameState.didBurnWood;
        }
        else
        {
            meatButton.interactable = gameState.meat >= gameState.meatToCook && gameState.didCook == false && gameState.didBurnWood;
        }
        woodButton.interactable = gameState.wood >= gameState.woodsToBurn && gameState.didBurnWood == false;
        makeArrowsButton.interactable = gameState.wood > 0;

        if (gameState.didCook == false)
        {
            gameState.meatToCook = 1;
        }
        else
        {
            if (gameState.playerHealth < 3)
            {
                gameState.meatToCook = 3;
            }
            else
            {
                gameState.meatToCook = 1;
            }
        }

        for (var i = 0; i < 3; i++)
        {
            if (gameState.playerHealth >= (i + 1))
            {
                _imageHearts[i].gameObject.SetActive(true);
                _imageHearts2[i].gameObject.SetActive(false);
            }
            else
            {
                _imageHearts2[i].gameObject.SetActive(true);
            }
        }
        
        UpdatetextTexmp();
        
        for (var i = 0; i < woods.Length; i++)
        {
            var wood = woods[i];
            
            wood.SetActive(i < gameState.wood);
        }
        
        for (var i = 0; i < meats.Length; i++)
        {
            var meat = meats[i];
            
            meat.SetActive(i < gameState.meat);
        }
        
        soup.SetActive(gameState.didCook);
        
        // meatButton.gameObject.SetActive(meatButton.interactable);
        // woodButton.gameObject.SetActive(woodButton.interactable);

        if (gameState.wood < gameState.woodsToBurn)
        {
            woodTowBurnLabel.color = Color.red;
        }
        else
        {
            woodTowBurnLabel.color = textColor;
        }
        
        if (gameState.meat < gameState.meatToCook)
        {
            meetToCookLabel.color = Color.red;
        }
        else
        {
            meetToCookLabel.color = textColor;
        }

        if (uiUpgradeScreen.gameObject.activeSelf)
        {
            var axeIndex = Array.IndexOf(axes, axeToUpgrade);
            var bowIndex = Array.IndexOf(bows, bowToUpgrade);
            var currentAxeIndex = Array.IndexOf(axes, gameState.currentAxe);
            var currentBowIndex = Array.IndexOf(bows, gameState.currentBow);

            uiUpgradeScreen.axeAttackLabel.text = $"{(int)gameState.currentAxe.secToCut}";
            uiUpgradeScreen.axeLevelLabel.text = $"Lv. {currentAxeIndex + 1}";

            if (axeToUpgrade != null)
            {
                uiUpgradeScreen.axeLevelSlider.value = (float)axeToUpgrade.upgrade / (float)axeToUpgrade.valueToUpgrade;
                uiUpgradeScreen.axeUpgradeCostLabel.text = $"{axeToUpgrade.valueToUpgrade}";
            }
            else
            {
                uiUpgradeScreen.axeLevelSlider.value = (float)gameState.currentAxe.upgrade / (float)gameState.currentAxe.valueToUpgrade;
                uiUpgradeScreen.axeUpgradeCostLabel.text = $"{gameState.currentAxe.valueToUpgrade}";
            }

            uiUpgradeScreen.axeMaxLabel.text = $"{gameState.currentAxe.logsMax}";
            uiUpgradeScreen.bowAttackLabel.text = $"{(int)gameState.currentBow.damage}";
            uiUpgradeScreen.bowLevelLabel.text = $"Lv. {currentBowIndex + 1}";
            uiUpgradeScreen.bowMaxLabel.text = $"{gameState.currentBow.meatMax}";
            
            if (bowToUpgrade != null)
            {
                uiUpgradeScreen.bowLevelSlider.value = (float)bowToUpgrade.upgrade / (float)bowToUpgrade.valueToUpgrade;
                uiUpgradeScreen.bowUpgradeCostLabel.text = $"{bowToUpgrade.valueToUpgrade}";
            }
            else
            {
                uiUpgradeScreen.bowLevelSlider.value = (float)gameState.currentBow.upgrade / (float)gameState.currentBow.valueToUpgrade;
                uiUpgradeScreen.bowUpgradeCostLabel.text = $"{gameState.currentBow.valueToUpgrade}";
            }

            uiUpgradeScreen.bowMaxLabel.text = $"{gameState.currentBow.meatMax}";
         
            uiUpgradeScreen.axeIcon.sprite = gameState.currentAxe.icon;
            uiUpgradeScreen.bowIcon.sprite = gameState.currentBow.icon;

            uiUpgradeScreen.axeUpgradeButtonIcon.sprite = currentAxeIndex + 1 < axes.Length
                ? uiUpgradeScreen.upgradeAvailable
                : uiUpgradeScreen.upgradeClicked;
            
            uiUpgradeScreen.bowUpgradeButtonIcon.sprite = currentBowIndex + 1 < bows.Length
                ? uiUpgradeScreen.upgradeAvailable
                : uiUpgradeScreen.upgradeClicked;

            uiUpgradeScreen.axeUpgradeButton.interactable = currentAxeIndex + 1 < axes.Length;
            uiUpgradeScreen.bowUpgradeButton.interactable = currentBowIndex + 1 < bows.Length;

            uiUpgradeScreen.teaCountLabel.text = $"{gameState.tea}";
            
            for (var i = 0; i < uiUpgradeScreen.teaImageViews.Length; i++)
            {
                uiUpgradeScreen.teaImageViews[i].gameObject.SetActive(i < gameState.teaPlants);
            }
        }
    }

    private void Start()
    {
        Background.sprite = darkBackground;
        gameState.didShowFireCaution = false;
        gameState.didShowFoodCaution = false;
        
        Time.timeScale = 1;
        
        if (gameState.playerHealth < 0)
        {
            gameState.playerHealth = 0;
        }
        gameState.current = GameState.Lobby;
        gameState.didBurnWood = false;
        gameState.didCook = false;

        if (gameState.didGoCut)
        {
            cutCoverView.Open();
            gameState.didGoCut = false;
        }
        
        if (gameState.didGoHunt)
        {
            huntCoverView.Open();
            gameState.didGoHunt = false;
        }
        
        while (gameState.teaPlants >= 5)
        {
            gameState.teaPlants -= 5;
            gameState.tea += 1;
        }

        var axe = axes.Last(a => a.upgrade == a.valueToUpgrade);
        var bow = bows.Last(b => b.upgrade == b.valueToUpgrade);

        gameState.currentAxe = axe;
        gameState.currentBow = bow;

        var axeIndex = Array.IndexOf(axes, axe);
        var bowIndex = Array.IndexOf(bows, bow);

        if (axeIndex + 1 < axes.Length)
        {
            axeToUpgrade = axes[axeIndex + 1];
        }

        if (bowIndex + 1 < bows.Length)
        {
            bowToUpgrade = bows[bowIndex + 1];
        }
        
        foreach (var playerArrow in player.arrows)
        {
            playerArrow.SetActive(false);
        }
            
        for (int i = 0; i < gameState.arrows; i++)
        {
            player.arrows[i].SetActive(true);
        }
    }

    public void UpdatetextTexmp()
    {
        textTemp.text = gameState.temperature+"°C";
    }

    public void BurnWoodPressed()
    {
        gameState.wood -= gameState.woodsToBurn;
        gameState.didBurnWood = true;
        woodButton.gameObject.SetActive(false);
        Background.sprite = lightBackground;
        LightNight.SetActive(false);
    }
    
    public void CutWoodsPressed()
    {
        if (gameState.didBurnWood == false)
        {
            if (gameState.didShowFireCaution == false)
            {
                fireCaution.SetActive(true);
                gameState.didShowFireCaution = true;
                return;
            }


            if (gameState.playerHealth > 0)
                gameState.playerHealth -= 2;
        }
        else if (gameState.didCook == false)
        {
            if (gameState.didShowFoodCaution == false)
            {
                foodCaution.SetActive(true);
                gameState.didShowFoodCaution = true;

                return;
            }

            if (gameState.playerHealth > 0)
                gameState.playerHealth -= 1;
        }

        cutCoverView.Close();
        
        gameState.level += 1;
        gameState.current = GameState.Wood;
        gameState.didGoCut = true;
        
        LoadScene(3, levels);
    }

    public void HuntPressed()
    {
        if (gameState.arrows == 0)
        {
            arrowsCaution.gameObject.SetActive(true);
            return;
        }
        
        if (gameState.didBurnWood == false)
        {
            if (gameState.didShowFireCaution == false)
            {
                fireCaution.SetActive(true);
                gameState.didShowFireCaution = true;
                return;
            }
            
            if (gameState.playerHealth > 0)
                gameState.playerHealth -= 2;
        }
        else if (gameState.didCook == false)
        {
            if (gameState.didShowFoodCaution == false)
            {
                foodCaution.SetActive(true);
                gameState.didShowFoodCaution = true;

                return;
            }

            if (gameState.playerHealth > 0)
                gameState.playerHealth -= 1;
        }

        huntCoverView.Close();

        gameState.level += 1;
        gameState.current = GameState.Hunt;
        gameState.didGoHunt = true;
        
        LoadScene(1, levels); 
    }

    async void LoadScene(int level, List<SceneReference> levels)
    {
        await Task.Delay(1000);
        LoadSceneForLevel(level, levels);
    }

    public void CookPressed()
    {
        if (gameState.didCook == false)
        {
            gameState.didCook = true;

            if (gameState.playerHealth == 3)
            {
                meatButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (gameState.playerHealth < 3)
            {
                gameState.playerHealth += 1;
            }

            if (gameState.playerHealth == 3)
            {
                meatButton.gameObject.SetActive(false);
            }
        }
      
        gameState.meat -= gameState.meatToCook;
    }

    public void GetArrowsPressed()
    {
        if (gameState.wood > 0 && gameState.arrows < 5)
        {
            gameState.arrows += 1;
            gameState.wood -= 1;
            
            foreach (var playerArrow in player.arrows)
            {
                playerArrow.SetActive(false);
            }
            
            for (int i = 0; i < gameState.arrows; i++)
            {
                player.arrows[i].SetActive(true);
            }
        }
    }

    public void DidPressOpenBackpack()
    {
        uiUpgradeScreen.background.gameObject.SetActive(true);
        //upgradePopup.SetActive(true);
    }

    public void DidPressCloseBackpack()
    {
        //upgradePopup.SetActive(false);
    }
    
    public void DidTapUpgradeBow()
    {
        if (gameState.wood < 1) return;
        
        bowToUpgrade.upgrade += 1;
        gameState.wood -= 1;

        if (bowToUpgrade.upgrade == bowToUpgrade.valueToUpgrade)
        {
            var index = Array.IndexOf(bows, bowToUpgrade);

            gameState.currentBow = bowToUpgrade;
            
            if (index + 1 < bows.Length)
            {
                bowToUpgrade = bows[index + 1];
            }
        }
    }
    
    public void DidTapUpgradeAxe()
    {
        if (gameState.wood < 1) return;
        
        axeToUpgrade.upgrade += 1;
        gameState.wood -= 1;

        if (axeToUpgrade.upgrade == axeToUpgrade.valueToUpgrade)
        {
            var index = Array.IndexOf(axes, axeToUpgrade);

            gameState.currentAxe = axeToUpgrade;
            
            if (index + 1 < axes.Length)
            {
                axeToUpgrade = axes[index + 1];
            }
        }
    }
    
    public void UIDidTapCautionOK()
    {
        fireCaution.SetActive(false);
        foodCaution.SetActive(false);
        arrowsCaution.SetActive(false);
    }
    
}
