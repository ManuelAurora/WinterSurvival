using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgrade : MonoBehaviour
{
    public Image[] teaImageViews;
    public TMP_Text teaCountLabel;
    public TMP_Text bowLevelLabel;
    public TMP_Text bowAttackLabel;
    public TMP_Text bowMaxLabel;
    public Slider bowLevelSlider;
    public TMP_Text bowUpgradeCostLabel;
    public TMP_Text axeLevelLabel;
    public TMP_Text axeAttackLabel;
    public TMP_Text axeMaxLabel;
    public Slider axeLevelSlider;
    public TMP_Text axeUpgradeCostLabel;
    public GameObject background;
    public Image bowIcon;
    public Image axeIcon;
    public Image bowUpgradeButtonIcon;
    public Image axeUpgradeButtonIcon;

    public Button axeUpgradeButton;
    public Button bowUpgradeButton;
    
    public Sprite upgradeClicked;
    public Sprite upgradeAvailable;
}
