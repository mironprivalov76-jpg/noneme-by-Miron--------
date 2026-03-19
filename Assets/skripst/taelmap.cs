using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBreaker : MonoBehaviour
{
    public Tilemap destructibleTilemap;
    public float breakForce = 5f; // Минимальная сила для ломания
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (destructibleTilemap == null) return;
        
        // Проверяем силу удара
        // if (collision.relativeVelocity.magnitude > breakForce)
        // {
            // Получаем точку контакта
            ContactPoint2D contact = collision.contacts[0];
            Vector3 hitPosition = contact.point;
            
            // Конвертируем в координаты клетки
            Vector3Int cellPosition = destructibleTilemap.WorldToCell(hitPosition);
            
            // Удаляем тайл
            destructibleTilemap.SetTile(cellPosition, null);
        //}
    }
}