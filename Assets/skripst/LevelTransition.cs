using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public string nextLevelName = "Level2"; // Имя следующей сцены
    public float transitionDelay = 0.5f; // Задержка перед переходом
    public bool useIndexInstead = false; // Использовать индекс вместо имени
    public int nextLevelIndex = 1; // Индекс следующей сцены (если useIndexInstead = true)
    
    [Header("Effects")]
    public GameObject transitionEffect; // Эффект при переходе (опционально)
    public AudioClip transitionSound; // Звук при переходе (опционально)
    
    [Header("Settings")]
    public string playerTag = "Player"; // Тег игрока
    public bool destroyOnTrigger = true; // Уничтожить объект после перехода
    public bool oneTimeOnly = true; // Сработать только один раз
    
    private bool hasTriggered = false;
    private AudioSource audioSource;
    
    void Start()
    {
        // Добавляем AudioSource если есть звук
        if (transitionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = transitionSound;
        }
    }
    
    // Срабатывает при входе в триггер
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            TriggerTransition();
        }
    }
    
    // Срабатывает при столкновении (если не триггер)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasTriggered && collision.gameObject.CompareTag(playerTag))
        {
            TriggerTransition();
        }
    }
    
    void TriggerTransition()
    {
        if (oneTimeOnly)
        {
            hasTriggered = true;
        }
        
        // Эффект перехода
        if (transitionEffect != null)
        {
            Instantiate(transitionEffect, transform.position, Quaternion.identity);
        }
        
        // Звук перехода
        if (audioSource != null && transitionSound != null)
        {
            audioSource.Play();
        }
        
        // Загружаем следующий уровень
        Invoke(nameof(LoadNextLevel), transitionDelay);
        
        // Опционально: скрыть объект
        if (destroyOnTrigger)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    
    void LoadNextLevel()
    {
        if (useIndexInstead)
        {
            // Проверяем, существует ли сцена с таким индексом
            if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextLevelIndex);
            }
            else
            {
                Debug.LogError($"Сцены с индексом {nextLevelIndex} не существует!");
            }
        }
        else
        {
            // Загружаем по имени
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogError("Имя следующей сцены не указано!");
            }
        }
    }
    
    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        // Рисуем иконку объекта перехода
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        
        // Рисуем текст с именем следующей сцены
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up, "Переход: " + nextLevelName);
        #endif
    }
}