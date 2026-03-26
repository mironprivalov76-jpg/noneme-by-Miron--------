using UnityEngine;

public class BlockPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BlockInventory inventory = collision.GetComponent<BlockInventory>();
            BlockValue block = GetComponent<BlockValue>();

            if (inventory != null && block != null)
            {
                inventory.AddBlock(block);
                Destroy(gameObject); // удаляем блок после подбора
            }
        }
    }
}