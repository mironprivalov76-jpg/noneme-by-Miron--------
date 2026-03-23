using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningCounter : MonoBehaviour
{
    // Ссылка на текст в интерфейсе (перетащить в инспекторе)
    public Text counterText;
    
    // Счётчики для каждого типа ресурса
    private int stoneCount = 0;
    private int coalCount = 0;
    private int ironCount = 0;
    private int copperCount = 0;
    private int redstoneCount = 0;
    private int goldCount = 0;
    private int emeraldCount = 0;
    private int diamondCount = 0;
    
    // Ценность каждого ресурса (очки)
    private int stoneValue = 1;
    private int coalValue = 5;
    private int ironValue = 10;
    private int copperValue = 15;
    private int redstoneValue = 20;
    private int goldValue = 25;
    private int emeraldValue = 30;
    private int diamondValue = 35;
    
    // Общее количество очков
    private int totalScore = 0;

    void Start()
    {
        UpdateCounterText();
    }

    // Вызывать этот метод, когда игрок сломал блок
    public void AddBlock(string blockType)
    {
        switch (blockType.ToLower())
        {
            case "stone":
                stoneCount++;
                totalScore += stoneValue;
                break;
            case "coal":
                coalCount++;
                totalScore += coalValue;
                break;
            case "iron":
                ironCount++;
                totalScore += ironValue;
                break;
            case "copper":
                copperCount++;
                totalScore += copperValue;
                break;
            case "redstone":
                redstoneCount++;
                totalScore += redstoneValue;
                break;
            case "gold":
                goldCount++;
                totalScore += goldValue;
                break;
            case "emerald":
                emeraldCount++;
                totalScore += emeraldValue;
                break;
            case "diamond":
                diamondCount++;
                totalScore += diamondValue;
                break;
            default:
                Debug.LogWarning("Неизвестный тип блока: " + blockType);
                break;
        }
        
        UpdateCounterText();
    }

    // Обновление текста на экране
    void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = 
                $"Камень: {stoneCount} (очки: {stoneCount * stoneValue})\n" +
                $"Уголь: {coalCount} (очки: {coalCount * coalValue})\n" +
                $"Железо: {ironCount} (очки: {ironCount * ironValue})\n" +
                $"Медь: {copperCount} (очки: {copperCount * copperValue})\n" +
                $"Редстоун: {redstoneCount} (очки: {redstoneCount * redstoneValue})\n" +
                $"Золото: {goldCount} (очки: {goldCount * goldValue})\n" +
                $"Изумруд: {emeraldCount} (очки: {emeraldCount * emeraldValue})\n" +
                $"Алмаз: {diamondCount} (очки: {diamondCount * diamondValue})\n" +
                $"━━━━━━━━━━━━━━━━\n" +
                $"Всего очков: {totalScore}";
        }
    }
    
    // Дополнительно: метод для сброса счётчиков (если понадобится)
    public void ResetCounters()
    {
        stoneCount = 0;
        coalCount = 0;
        ironCount = 0;
        copperCount = 0;
        redstoneCount = 0;
        goldCount = 0;
        emeraldCount = 0;
        diamondCount = 0;
        totalScore = 0;
        
        UpdateCounterText();
    }
    
    // Геттеры для получения отдельных значений (если понадобятся для прокачки)
    public int GetTotalScore() => totalScore;
    public int GetStoneCount() => stoneCount;
    public int GetCoalCount() => coalCount;
    // и так далее...
}