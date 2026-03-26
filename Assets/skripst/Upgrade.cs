using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Upgrade : MonoBehaviour

{
    public string upgradeName; // название
    public int cost;           // цена
    public Button button;      // кнопка UI
    public bool purchased = false; // куплено?
}

public class UpgradeShop : MonoBehaviour
{
    public SellBlocks playerMoney; // ссылка на скрипт с деньгами
    public Text moneyText;         // UI Text для денег
    public Upgrade[] upgrades;     // список апгрейдов

    void Start()
    {
        foreach (var up in upgrades)
        {
            up.button.onClick.AddListener(() => BuyUpgrade(up));
            UpdateButton(up);
        }
        UpdateMoneyUI();
    }

    void BuyUpgrade(Upgrade up)
    {
        if (up.purchased) return;

        if (playerMoney.money >= up.cost)
        {
            playerMoney.money -= up.cost;
            up.purchased = true;

            UpdateButton(up);
            UpdateMoneyUI();

            // здесь можно добавить эффект апгрейда
            Debug.Log("Куплено: " + up.upgradeName);
        }
        else
        {
            Debug.Log("Недостаточно денег");
        }
    }

    void UpdateButton(Upgrade up)
    {
        up.button.interactable = !up.purchased && playerMoney.money >= up.cost;
        up.button.GetComponentInChildren<Text>().text = up.upgradeName + " (" + up.cost + ")";
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "Money: " + playerMoney.money;
    }

    void Update()
    {
        // обновляем кнопки в реальном времени
        foreach (var up in upgrades)
        {
            if (!up.purchased)
                up.button.interactable = playerMoney.money >= up.cost;
        }
        UpdateMoneyUI();
    }
}