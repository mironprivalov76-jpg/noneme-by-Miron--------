using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    public float animationThreshold = 0.1f; // Порог скорости для смены анимации
    
    [Header("Настройки анимации")]
    public string speedParameter = "Speed";
    public string attackTrigger = "Attack";
    
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    void Update()
    {
        // Получаем ввод с клавиатуры
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // Нормализуем вектор для диагонального движения
        movement = movement.normalized;
        
        // Вычисляем текущую скорость
        currentSpeed = movement.magnitude;
        
        // Управление анимациями на основе скорости
        UpdateMovementAnimation();
        
        // Проверка нажатия ЛКМ для атаки
        if (Input.GetMouseButtonDown(0)) // 0 - левая кнопка мыши
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        // Перемещаем объект
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateMovementAnimation()
    {
        if (animator == null) return;

        // Если скорость больше порога (0.1) - анимация движения
        if (currentSpeed > animationThreshold)
        {
            animator.SetFloat(speedParameter, currentSpeed);
            Debug.Log("Скорость больше 0.1: анимация движения");
        }
        // Если скорость меньше или равна порогу - анимация покоя
        else
        {
            animator.SetFloat(speedParameter, 0f);
            Debug.Log("Скорость меньше или равна 0.1: анимация покоя");
        }
    }

    void Attack()
    {
        if (animator == null) return;
        
        // Запускаем анимацию атаки
        animator.SetTrigger(attackTrigger);
        Debug.Log("Атака! ЛКМ нажата");
        
        // Дополнительно можно остановить движение во время атаки
        // movement = Vector2.zero;
    }
}