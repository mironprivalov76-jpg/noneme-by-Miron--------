using UnityEngine;

public class Player2D : MonoBehaviour
{
    [Header("Характеристики")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    
    [Header("Радиусы")]
    public float detectionRange = 10f; // Радиус обнаружения
    public float attackRange = 1f; // Радиус атаки (должен быть = 1)
    
    [Header("Атака")]
    public float attackCooldown = 1f;
    
    private Transform player;
    private float lastAttackTime;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (isDead || player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= attackRange)
        {
            // Атака
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else if (distance <= detectionRange)
        {
            // Преследование
            Chase();
        }
    }
    
    void FixedUpdate()
    {
        if (isDead || player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= detectionRange && distance > attackRange)
        {
            // Движение к игроку
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            
            // Поворот спрайта
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void Chase()
    {
        Debug.Log("Преследую игрока! Расстояние: " + Vector2.Distance(transform.position, player.position));
    }
    
    void Attack()
    {
        Debug.Log("АТАКУЮ!");
        
        // Наносим урон игроку
        Player2D playerScript = player.GetComponent<Player2D>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 0.5f);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}