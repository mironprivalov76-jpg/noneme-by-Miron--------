using UnityEngine;

public class Enemy2D : MonoBehaviour
{
    [Header("Характеристики врага")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float damageToPlayer = 10f;
    
    [Header("Выпадение оружия")]
    public GameObject weaponDropPrefab;
    public float dropChance = 0.5f;
    
    [Header("Компоненты")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb;
    
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log("Враг получил урон! Осталось здоровья: " + currentHealth);
        
        // Анимация получения урона
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        // Мигание красным
        StartCoroutine(FlashRed());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("Враг уничтожен!");
        
        // Анимация смерти
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // Выпадение оружия
        if (weaponDropPrefab != null && Random.value <= dropChance)
        {
            DropWeapon();
        }
        
        // Уничтожаем объект через небольшое время (для проигрывания анимации)
        Destroy(gameObject, 0.5f);
    }
    
    void DropWeapon()
    {
        GameObject droppedWeapon = Instantiate(weaponDropPrefab, transform.position, Quaternion.identity);
        
        // Добавляем физику в 2D
        Rigidbody2D rb = droppedWeapon.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = droppedWeapon.AddComponent<Rigidbody2D>();
        }
        
        // Случайная сила для разлета
        Vector2 randomDirection = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f)
        ).normalized;
        
        rb.AddForce(randomDirection * 5f, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
        
        // Добавляем компонент подбора
        if (droppedWeapon.GetComponent<WeaponPickup2D>() == null)
        {
            WeaponPickup2D pickup = droppedWeapon.AddComponent<WeaponPickup2D>();
            pickup.weaponPrefab = droppedWeapon;
        }
        
        // Уничтожаем через 30 секунд
        Destroy(droppedWeapon, 30f);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            Player2D player = collision.gameObject.GetComponent<Player2D>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
            }
        }
    }
}