using UnityEngine;

public class BlockPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SellBlocks inventory = collision.GetComponent<SellBlocks>();
            BlockValue block = GetComponent<BlockValue>();

            if (inventory != null && block != null)
            {
                inventory.AddBlock(block);
                Destroy(gameObject); // удаляем блок после подбора
            }
        }
    }
}