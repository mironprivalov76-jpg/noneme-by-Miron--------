using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    [Header("Mining Settings")]
    public Tilemap destructibleTilemap; // Tilemap который можно копать
    public float miningRange = 2f; // Дальность копания
    public float miningCooldown = 0.3f; // Задержка между копаниями
    public GameObject miningEffect; // Эффект при копании (опционально)
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private float lastMiningTime;
    private Vector2 moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (destructibleTilemap == null)
        {
            Debug.LogWarning("Destructible Tilemap not assigned! Please assign it in the inspector.");
        }
    }
    
    void Update()
    {
        // Получаем ввод от игрока
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical"); // Для движения вверх/вниз
        
        // Проверка земли
       
        
        // Прыжок (если нужно)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        
        // Копание при нажатии ЛКМ
        if (Input.GetMouseButton(0) && Time.time >= lastMiningTime + miningCooldown)
        {
            MineTile();
            lastMiningTime = Time.time;
        }
        
        // Анимации
        UpdateAnimations();
    }
    
    void FixedUpdate()
    {
        // Горизонтальное движение
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        
        // Вертикальное движение (если нужно перемещаться вверх/вниз)
        // Закомментируйте если используете физику только для горизонтали
        if (moveInput.y != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveInput.y * moveSpeed);
        }
    }
    
    void MineTile()
    {
        if (destructibleTilemap == null) return;
        
        // Получаем позицию мыши в мировых координатах
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        // Проверяем дистанцию до игрока
        float distance = Vector3.Distance(transform.position, mouseWorldPos);
        if (distance > miningRange)
        {
            Debug.Log("Too far to mine!");
            return;
        }
        
        // Конвертируем мировые координаты в координаты клетки Tilemap
        Vector3Int cellPosition = destructibleTilemap.WorldToCell(mouseWorldPos);
        
        // Проверяем есть ли там тайл
        TileBase tile = destructibleTilemap.GetTile(cellPosition);
        if (tile != null)
        {
            // Удаляем тайл
            destructibleTilemap.SetTile(cellPosition, null);
            
            // Создаем эффект (если есть)
            if (miningEffect != null)
            {
                Instantiate(miningEffect, mouseWorldPos, Quaternion.identity);
            }
            
            Debug.Log($"Mined tile at {cellPosition}");
        }
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Устанавливаем параметры аниматора
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
            
            // Поворот спрайта в зависимости от направления движения
            if (moveInput.x != 0)
            {
                spriteRenderer.flipX = moveInput.x < 0;
            }
        }
    }
    
    // Визуализация радиуса проверки земли (для отладки)
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Визуализация радиуса копания
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, miningRange);
    }
}
