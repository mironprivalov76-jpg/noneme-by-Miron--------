using UnityEngine;

public class TopDownMovement2D : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Получаем ввод
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // Нормализуем диагональное движение
        movement = movement.normalized;
        
        // Анимация (если есть)
        if (animator != null)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        // Перемещаем объект
        print(movement);
        rb.linearVelocity = movement * moveSpeed;
        //rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}