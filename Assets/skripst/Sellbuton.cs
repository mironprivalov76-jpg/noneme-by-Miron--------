using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SellBlocks : MonoBehaviour
{
    public int money = 0;           // баланс игрока
    public Text moneyText;           // UI Text (Legacy)
    public KeyCode sellKey = KeyCode.E;

    private List<BlockValue> collectedBlocks = new List<BlockValue>();

    void Update()
    {
        if (Input.GetKeyDown(sellKey))
        {
            SellAllBlocks();
        }
    }

    // вызываем, когда игрок подбирает блок
    public void AddBlock(BlockValue block)
    {
        if(block != null)
            collectedBlocks.Add(block);
    }

    void SellAllBlocks()
    {
        int total = 0;

        // суммируем стоимость всех собранных блоков
        foreach (var block in collectedBlocks)
        {
            if(block != null)
                total += block.value;
        }

        if(total > 0)
        {
            money += total;          // добавляем деньги
            collectedBlocks.Clear();  // очищаем инвентарь
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if(moneyText != null)
            moneyText.text = "Money: " + money;
    }
}