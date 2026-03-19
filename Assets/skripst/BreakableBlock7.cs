using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public string blockType = "diamond"; // stone, coal, iron, copper, redstone, gold, emerald, diamond
    
    void OnMouseDown() // Или любое другое событие разрушения
    {
        Destroy(gameObject);
        
        // Находим MiningCounter на сцене и добавляем блок
        MiningCounter counter = FindObjectOfType<MiningCounter>();
        if (counter != null)
        {
            counter.AddBlock(blockType);
        }
    }
}