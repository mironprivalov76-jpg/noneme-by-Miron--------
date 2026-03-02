using UnityEngine;

public class WeaponPickup2D : MonoBehaviour
{
    public GameObject weaponPrefab;
    public float rotationSpeed = 100f;
    public float floatHeight = 0.2f;
    public float floatSpeed = 1f;
    
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Добавляем тег для удобства поиска
        gameObject.tag = "Weapon";
    }
    
    void Update()
    {
        // Эффект парения
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = newPosition;
        
        // Вращение
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
    
    public GameObject Pickup(Transform parent)
    {
        if (weaponPrefab != null)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, parent.position, Quaternion.identity, parent);
            
            // Очищаем от лишних компонентов
            Rigidbody2D rb = newWeapon.GetComponent<Rigidbody2D>();
            if (rb != null) Destroy(rb);
            
            Collider2D col = newWeapon.GetComponent<Collider2D>();
            if (col != null) Destroy(col);
            
            WeaponPickup2D pickup = newWeapon.GetComponent<WeaponPickup2D>();
            if (pickup != null) Destroy(pickup);
            
            return newWeapon;
        }
        return null;
    }
}