using UnityEngine;
using System.Collections;

public class TopDownAI2D : MonoBehaviour
{
    [Header("Компоненты")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    [Header("Настройки обнаружения")]
    public float detectionRange = 5f; // Радиус обнаружения
    public float viewAngle = 90f; // Угол обзора (в градусах)
    public LayerMask obstacleLayer; // Слой препятствий
    public LayerMask targetLayer; // Слой цели
    
    [Header("Настройки преследования")]
    public float chaseSpeed = 3f;
    public float stopDistance = 1f; // Дистанция остановки перед целью
    public float loseTargetDistance = 8f; // Дистанция потери цели
    
    [Header("Настройки патрулирования")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float waitTimeAtPoint = 2f;
    
    [Header("Настройки idle")]
    public float idleTimeMin = 1f;
    public float idleTimeMax = 3f;
    public float idleRotateSpeed = 50f;
    
    // Состояния AI
    private enum AIState
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking
    }
    
    private AIState currentState = AIState.Patrolling;
    private Transform target;
    private Vector2 lastKnownPosition;
    private float stateTimer = 0f;
    private int currentPatrolIndex = 0;
    private float idleRotateDirection = 1f;
    
    void Start()
    {
        // Получаем компоненты
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Начинаем с патрулирования
        if (patrolPoints.Length > 0)
        {
            currentState = AIState.Patrolling;
            MoveToNextPatrolPoint();
        }
        else
        {
            currentState = AIState.Idle;
            StartCoroutine(IdleRoutine());
        }
    }
    
    void Update()
    {
        // Постоянно проверяем, видим ли мы цель
        CheckForTarget();
        
        // Обновляем анимации
        UpdateAnimations();
    }
    
    void FixedUpdate()
    {
        // Выполняем действия в зависимости от состояния
        switch (currentState)
        {
            case AIState.Idle:
                IdleBehavior();
                break;
            case AIState.Patrolling:
                PatrolBehavior();
                break;
            case AIState.Chasing:
                ChaseBehavior();
                break;
            case AIState.Attacking:
                AttackBehavior();
                break;
        }
    }
    
    void CheckForTarget()
    {
        // Находим все цели в радиусе
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayer);
        
        bool foundTarget = false;
        
        foreach (Collider2D potentialTarget in targetsInRange)
        {
            Transform potentialTransform = potentialTarget.transform;
            
            // Проверяем, видим ли мы цель
            if (CanSeeTarget(potentialTransform))
            {
                // Нашли цель
                target = potentialTransform;
                foundTarget = true;
                
                // Если не преследовали, переключаемся на преследование
                if (currentState != AIState.Chasing && currentState != AIState.Attacking)
                {
                    SwitchState(AIState.Chasing);
                }
                break;
            }
        }
        
        // Если не нашли цель, но были в состоянии преследования
        if (!foundTarget && currentState == AIState.Chasing)
        {
            // Проверяем, не потеряли ли цель окончательно
            if (target != null)
            {
                float distToTarget = Vector2.Distance(transform.position, target.position);
                if (distToTarget > loseTargetDistance)
                {
                    // Потеряли цель
                    target = null;
                    SwitchToPatrolOrIdle();
                }
            }
            else
            {
                SwitchToPatrolOrIdle();
            }
        }
    }
    
    bool CanSeeTarget(Transform potentialTarget)
    {
        if (potentialTarget == null) return false;
        
        Vector2 directionToTarget = (potentialTarget.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, potentialTarget.position);
        
        // Проверка угла обзора
        Vector2 forwardDirection = GetForwardDirection();
        float angleToTarget = Vector2.Angle(forwardDirection, directionToTarget);
        
        if (angleToTarget > viewAngle / 2)
            return false;
        
        // Raycast для проверки препятствий
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer);
        
        return hit.collider == null;
    }
    
    Vector2 GetForwardDirection()
    {
        // Для 2D обычно используем right (право) как направление вперед
        return transform.right;
    }
    
    void IdleBehavior()
    {
        // Просто стоим и крутимся
        stateTimer -= Time.deltaTime;
        
        // Медленно поворачиваемся
        transform.Rotate(0, 0, idleRotateSpeed * idleRotateDirection * Time.deltaTime);
        
        if (stateTimer <= 0)
        {
            SwitchToPatrolOrIdle();
        }
    }
    
    void PatrolBehavior()
    {
        if (patrolPoints.Length == 0)
        {
            SwitchState(AIState.Idle);
            return;
        }
        
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        float distanceToPoint = Vector2.Distance(transform.position, targetPoint.position);
        
        if (distanceToPoint < 0.5f)
        {
            // Достигли точки, ждем
            SwitchState(AIState.Idle);
            stateTimer = waitTimeAtPoint;
            
            // Переходим к следующей точке
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            // Двигаемся к точке
            Vector2 direction = (targetPoint.position - transform.position).normalized;
            rb.linearVelocity = direction * patrolSpeed;
            
            // Поворачиваемся в сторону движения
            RotateTowardsDirection(direction);
        }
    }
    
    void ChaseBehavior()
    {
        if (target == null)
        {
            SwitchToPatrolOrIdle();
            return;
        }
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // Проверяем, не слишком ли близко к цели
        if (distanceToTarget <= stopDistance)
        {
            // Достаточно близко для атаки
            SwitchState(AIState.Attacking);
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        // Двигаемся к цели
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        
        // Поворачиваемся в сторону движения
        RotateTowardsDirection(direction);
        
        // Сохраняем последнюю известную позицию
        lastKnownPosition = target.position;
    }
    
    void AttackBehavior()
    {
        if (target == null)
        {
            SwitchToPatrolOrIdle();
            return;
        }
        
        // Стоим на месте и "атакуем" (смотрим на цель)
        rb.linearVelocity = Vector2.zero;
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // Если цель отошла, снова преследуем
        if (distanceToTarget > stopDistance + 0.5f)
        {
            SwitchState(AIState.Chasing);
        }
        else
        {
            // Поворачиваемся к цели
            Vector2 direction = (target.position - transform.position).normalized;
            RotateTowardsDirection(direction);
            
            // Здесь можно добавить логику атаки
            Debug.Log("Атакую цель: " + target.name);
        }
    }
    
    void RotateTowardsDirection(Vector2 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    
    void SwitchState(AIState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        
        // Сбрасываем скорость при смене состояния
        rb.linearVelocity = Vector2.zero;
        
        switch (newState)
        {
            case AIState.Idle:
                StartCoroutine(IdleRoutine());
                break;
            case AIState.Patrolling:
                MoveToNextPatrolPoint();
                break;
            case AIState.Chasing:
                Debug.Log("Начинаю преследование!");
                break;
            case AIState.Attacking:
                Debug.Log("Начинаю атаку!");
                break;
        }
    }
    
    void SwitchToPatrolOrIdle()
    {
        if (patrolPoints.Length > 0)
        {
            SwitchState(AIState.Patrolling);
        }
        else
        {
            SwitchState(AIState.Idle);
        }
    }
    
    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
    
    IEnumerator IdleRoutine()
    {
        // Случайное время бездействия
        stateTimer = Random.Range(idleTimeMin, idleTimeMax);
        
        // Случайное направление вращения
        idleRotateDirection = Random.value > 0.5f ? 1f : -1f;
        
        yield return new WaitForSeconds(stateTimer);
        
        // Если всё ещё в idle, переключаемся на патрулирование
        if (currentState == AIState.Idle)
        {
            SwitchToPatrolOrIdle();
        }
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Передаем скорость в аниматор
            float speed = rb.linearVelocity.magnitude;
            animator.SetFloat("Speed", speed);
            
            // Передаем состояние
            animator.SetInteger("State", (int)currentState);
            
            // Для спрайтов, которые нужно отражать по горизонтали
            if (spriteRenderer != null && rb.linearVelocity.x != 0)
            {
                spriteRenderer.flipX = rb.linearVelocity.x < 0;
            }
        }
    }
    
    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        // Рисуем круг обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Рисуем конус обзора
        Vector3 forward = transform.right * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, viewAngle/2) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -viewAngle/2) * forward;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        // Рисуем дистанцию потери цели
        if (currentState == AIState.Chasing && target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, loseTargetDistance);
        }
        
        // Рисуем точки патрулирования
        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.3f);
                }
            }
        }
    }
}