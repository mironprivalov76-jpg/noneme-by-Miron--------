using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Характеристики врага")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float damage = 10f; // Урон игроку
    
    [Header("Радиусы")]
    public float detectionRange = 10f; // Радиус обнаружения игрока
    public float attackRange = 1f; // Радиус атаки (serialized field = 1)
    public float stopRange = 1.5f; // Расстояние остановки перед игроком
    
    [Header("Скорость")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    
    [Header("Атака")]
    public float attackCooldown = 1f; // Задержка между атаками
    public float attackDamage = 10f;
    
    [Header("Компоненты")]
    public Transform player; // Ссылка на игрока
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool isDead = false;
    private Vector2 movement;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // Автоматически находим игрока, если не назначен
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        // Получаем компоненты
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (isDead || player == null) return;
        
        // Рассчитываем расстояние до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Логика поведения
        if (distanceToPlayer <= attackRange)
        {
            // В радиусе атаки - атакуем
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // В радиусе обнаружения - преследуем
            ChasePlayer();
        }
        else
        {
            // Вне радиуса - стоим на месте
            Idle();
        }
        
        // Анимации
        UpdateAnimations();
    }
    
    void FixedUpdate()
    {
        if (isDead || player == null) return;
        
        // Движение в FixedUpdate
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Двигаемся только если игрок в радиусе обнаружения и НЕ в радиусе атаки
        if (distanceToPlayer <= detectionRange && distanceToPlayer > stopRange)
        {
            // Направление к игроку
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction * moveSpeed;
            
            // Применяем движение
            rb.linearVelocity = movement;
        }
        else
        {
            // Останавливаемся
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    void ChasePlayer()
    {
        // Логика преследования (анимации и т.д.)
        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsAttacking", false);
        }
        
        Debug.Log("Враг преследует игрока! Расстояние: " + Vector2.Distance(transform.position, player.position));
    }
    
    void Attack()
    {
        // Проверяем, прошло ли достаточно времени для новой атаки
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            
            // Анимация атаки
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("IsAttacking", true);
            }
            
            // Наносим урон игроку
            if (player != null)
            {
                Player2D playerScript = player.GetComponent<Player2D>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(attackDamage);
                    Debug.Log("Враг атаковал игрока! Урон: " + attackDamage);
                }
            }
            
            Debug.Log("Враг атакует!");
        }
    }
    
    void Idle()
    {
        // Состояние покоя
        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking", false);
        }
        
        Debug.Log("Враг в состоянии покоя");
    }
    
    void UpdateAnimations()
    {
        if (animator != null && spriteRenderer != null && player != null)
        {
            // Поворот спрайта в сторону игрока
            float direction = player.position.x - transform.position.x;
            spriteRenderer.flipX = direction < 0;
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log("Враг получил урон: " + damage + " Осталось здоровья: " + currentHealth);
        
        // Эффект получения урона
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
        Debug.Log("Враг погиб!");
        
        // Анимация смерти
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // Останавливаем движение
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Отключаем коллайдер
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Уничтожаем через некоторое время
        Destroy(gameObject, 1f);
    }
    
    // Визуализация радиусов в редакторе
    void OnDrawGizmosSelected()
    {
        // Радиус обнаружения (желтый)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Радиус атаки (красный) - должен быть = 1
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Радиус остановки (синий)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }
}