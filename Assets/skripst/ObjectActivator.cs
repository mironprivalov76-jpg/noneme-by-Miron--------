using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject; // Объект для включения/выключения
    
    [Header("Key Settings")]
    public KeyCode activationKey = KeyCode.E; // Кнопка для активации
    public bool toggleMode = true; // Вкл/Выкл при каждом нажатии
    public bool activateOnPress = true; // Включить при нажатии (если toggleMode = false)
    public bool deactivateOnPress = false; // Выключить при нажатии (если toggleMode = false)
    
    [Header("Settings")]
    public bool oneTimeOnly = false; // Сработать только один раз
    public float delay = 0f; // Задержка перед активацией
    
    private bool hasActivated = false;
    private bool isActive = false;
    
    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target object not assigned! Please assign a GameObject in the inspector.");
        }
        else
        {
            // Запоминаем начальное состояние
            isActive = targetObject.activeSelf;
        }
    }
    
    void Update()
    {
        // Проверяем нажатие кнопки
        if (Input.GetKeyDown(activationKey) && !hasActivated)
        {
            if (oneTimeOnly)
            {
                hasActivated = true;
            }
            
            // Выполняем с задержкой или без
            if (delay > 0)
            {
                Invoke(nameof(ActivateObject), delay);
            }
            else
            {
                ActivateObject();
            }
        }
    }
    
    void ActivateObject()
    {
        if (targetObject == null) return;
        
        if (toggleMode)
        {
            // Переключаем состояние
            isActive = !isActive;
            targetObject.SetActive(isActive);
            Debug.Log($"Object {(isActive ? "activated" : "deactivated")}");
        }
        else
        {
            // Включаем или выключаем в зависимости от настроек
            if (activateOnPress)
            {
                targetObject.SetActive(true);
                Debug.Log("Object activated");
            }
            else if (deactivateOnPress)
            {
                targetObject.SetActive(false);
                Debug.Log("Object deactivated");
            }
        }
    }
}