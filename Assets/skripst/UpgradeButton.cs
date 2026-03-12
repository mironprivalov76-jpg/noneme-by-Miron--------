using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class UpgradeButton : MonoBehaviour
{
    public DiggerController digger;
    public Text levelText;
    public Text costText;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnUpgradeClick);
        UpdateUI();
    }

    void OnUpgradeClick()
    {
        digger.TryUpgradeDigRange();
        UpdateUI();
    }

    void UpdateUI()
    {
        levelText.text = $"Уровень: {digger.currentUpgradeLevel}/{digger.maxUpgradeLevel}";
        costText.text = $"Цена: {digger.upgradeCost}";
    }
}