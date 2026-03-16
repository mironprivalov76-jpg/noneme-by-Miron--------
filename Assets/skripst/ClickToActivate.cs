using UnityEngine;

public class ClickToActivate : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject; // Объект для включения (если не назначен, включается этот объект)
    
    [Header("Click Settings")]
    public bool requireLeftClick = true; // Только ЛКМ
    public bool requireRightClick = false; // Использовать ПКМ
    public bool toggleMode = true; // Вкл/Выкл при каждом клике
    public bool activateOnly = false; // Только включить (если toggleMode = false)
    public bool deactivateOnly = false; // Только выключить (если toggleMode = false)
    
    [Header("Effects")]
    public GameObject clickEffect; // Эффект при клике
    public AudioClip clickSound; // Звук при клике
    public float effectDuration = 1f;
    
    [Header("Settings")]
    public bool oneTimeOnly = false; // Сработать только один раз
    public float delay = 0f; // Задержка перед активацией
    public bool requireCollider = true; // Требуется ли коллайдер (должен быть на объекте)
    
    private bool hasActivated = false;
    private AudioSource audioSource;
    private Camera mainCamera;
    
    void Start()
    {
        // Если целевой объект не назначен, используем этот объект
        if (targetObject == null)
        {
            targetObject = gameObject;
        }
        
        // Добавляем AudioSource если есть звук
        if (clickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = clickSound;
        }
        
        mainCamera = Camera.main;
        
        // Проверяем наличие коллайдера
        if (requireCollider && GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning($"Object {gameObject.name} has no Collider2D! Click detection won't work.");
        }
    }
    
    void Update()
    {
        // Проверяем клик мыши
        if (hasActivated && oneTimeOnly) return;
        
        bool clicked = false;
        
        if (requireLeftClick && Input.GetMouseButtonDown(0))
        {
            clicked = true;
        }
        else if (requireRightClick && Input.GetMouseButtonDown(1))
        {
            clicked = true;
        }
        else if (!requireLeftClick && !requireRightClick && Input.GetMouseButtonDown(0))
        {
            clicked = true; // По умолчанию ЛКМ
        }
        
        if (clicked)
        {
            CheckClick();
        }
    }
    
    void CheckClick()
    {
        if (mainCamera == null) return;
        
        // Получаем позицию мыши в мировых координатах
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Делаем луч
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        
        // Проверяем, попали ли в этот объект
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Activate();
        }
    }
    
    // Альтернативный метод через OnMouseDown (проще, но требует коллайдера)
    void OnMouseDown()
    {
        // Этот метод вызывается автоматически при клике на объект с коллайдером
        if (requireLeftClick && Input.GetMouseButtonDown(0))
        {
            Activate();
        }
        else if (requireRightClick && Input.GetMouseButtonDown(1))
        {
            Activate();
        }
        else if (!requireLeftClick && !requireRightClick)
        {
            Activate();
        }
    }
    
    void Activate()
    {
        if (hasActivated && oneTimeOnly) return;
        
        if (oneTimeOnly)
        {
            hasActivated = true;
        }
        
        if (delay > 0)
        {
            Invoke(nameof(DoActivate), delay);
        }
        else
        {
            DoActivate();
        }
    }
    
    void DoActivate()
    {
        if (targetObject == null) return;
        
        if (toggleMode)
        {
            // Переключаем состояние
            targetObject.SetActive(!targetObject.activeSelf);
            Debug.Log($"Object {(targetObject.activeSelf ? "activated" : "deactivated")} by click");
        }
        else if (activateOnly)
        {
            targetObject.SetActive(true);
            Debug.Log("Object activated by click");
        }
        else if (deactivateOnly)
        {
            targetObject.SetActive(false);
            Debug.Log("Object deactivated by click");
        }
        else
        {
            // По умолчанию включаем
            targetObject.SetActive(true);
        }
        
        // Эффекты
        PlayEffects();
    }
    
    void PlayEffects()
    {
        // Эффект
        if (clickEffect != null)
        {
            GameObject effect = Instantiate(clickEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
        
        // Звук
        if (audioSource != null && clickSound != null)
        {
            audioSource.Play();
        }
    }
}