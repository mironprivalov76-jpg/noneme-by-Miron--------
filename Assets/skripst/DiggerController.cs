using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngin;
 // Для работы с тайлами, если используете Tilemap
// Если вы используете не Tilemap, а отдельные спрайты/объекты, логика изменится.

public class DiggerController : MonoBehaviour
{
    [Header("Настройки копания")]
    public float digRange = 1.5f;        // Базовая дальность копания
    public float digCooldown = 0f;      // Задержка между копаниями (чтобы не спамить)
    public LayerMask diggableLayer;        // Слой, на котором находятся копаемые объекты/тайлы
    public KeyCode digKey = KeyCode.Space; // Кнопка для копания

    [Header("Настройки прокачки")]
    public int upgradeCost = 100;           // Стоимость улучшения (можно заменить на ресурсы)
    public float digRangeUpgradeStep = 0.5f; // На сколько увеличивается радиус при прокачке
    public int currentUpgradeLevel = 0;      // Текущий уровень прокачки
    public int maxUpgradeLevel = 5;           // Максимальный уровень прокачки

    private float lastDigTime;               // Время последнего копания
    private Camera mainCamera;                // Основная камера (для определения направления мыши)
    private PlayerResources playerResources;   // Ссылка на ресурсы игрока (если нужна)

    void Start()
    {
        mainCamera = Camera.main;
        playerResources = GetComponent<PlayerResources>(); // Опционально, если есть система ресурсов
        
        // Загружаем сохраненный уровень (если есть)
        LoadUpgradeLevel();
    }

    void Update()
    {
        // Обработка копания
        if (Input.GetKeyDown(digKey) && Time.time > lastDigTime + digCooldown)
        {
            // Получаем направление от игрока к курсору мыши
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 digDirection = (mousePosition - (Vector2)transform.position).normalized;

            // Вызываем функцию копания
            DigInDirection(digDirection);
            
            // Обновляем время последнего копания
            lastDigTime = Time.time;
        }

        // Обработка прокачки (для теста - нажатие U)
        if (Input.GetKeyDown(KeyCode.U))
        {
            TryUpgradeDigRange();
        }
    }

    /// <summary>
    /// Копает в указанном направлении в пределах радиуса
    /// </summary>
    void DigInDirection(Vector2 direction)
    {
        // Рассчитываем точку назначения луча
        Vector2 targetPosition = (Vector2)transform.position + direction * digRange;
        
        // Рисуем луч для визуализации (только в редакторе)
        Debug.DrawLine(transform.position, targetPosition, Color.red, 0.5f);

        // Бросаем луч (Raycast) для поиска объектов для копания
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, digRange, diggableLayer);

        if (hit.collider != null)
        {
            // Проверяем, является ли объект частью Tilemap
            Tilemap tilemap = hit.collider.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                // Копание Tilemap (метод 1: уничтожение тайла в позиции удара)
                Vector3 hitPosition = hit.point;
                Vector3Int tileCell = tilemap.WorldToCell(hitPosition);
                
                // Получаем тайл в этой ячейке
                TileBase hitTile = tilemap.GetTile(tileCell);
                
                if (hitTile != null)
                {
                    // Уничтожаем тайл
                    tilemap.SetTile(tileCell, null);
                    Debug.Log($"Сломан тайл {hitTile.name} на позиции {tileCell}");
                    
                    // Здесь можно добавить выпадение ресурсов
                    SpawnResources(hitTile.name, hitPosition);
                }
            }
            else
            {
                // Альтернатива: уничтожение GameObject'а (если блоки - отдельные объекты)
                DiggableBlock block = hit.collider.GetComponent<DiggableBlock>();
                if (block != null)
                {
                    block.DestroyBlock();
                    Debug.Log($"Сломан блок {hit.collider.gameObject.name}");
                }
                else
                {
                    // Просто уничтожаем объект (если нет специального скрипта)
                    Destroy(hit.collider.gameObject);
                }
            }
        }
        else
        {
            Debug.Log("Ничего не найдено для копания");
        }
    }

    /// <summary>
    /// Спавнит ресурсы при разрушении блока
    /// </summary>
    void SpawnResources(string tileName, Vector3 position)
    {
        // Простая логика: разные тайлы дают разные ресурсы
        // В реальной игре лучше использовать ScriptableObject для конфигурации блоков
        int resourceAmount = 1;
        
        if (tileName.Contains("Gold")) resourceAmount = 5;
        else if (tileName.Contains("Silver")) resourceAmount = 3;
        else if (tileName.Contains("Stone")) resourceAmount = 1;
        
        // Добавляем ресурсы игроку (если есть система)
        if (playerResources != null)
        {
            playerResources.AddResources(resourceAmount);
        }
        
        // Визуальный эффект (можно добавить партиклы или текст)
        Debug.Log($"Получено {resourceAmount} ресурсов");
    }

    /// <summary>
    /// Попытка улучшить радиус копания
    /// </summary>
    public void TryUpgradeDigRange()
    {
        if (currentUpgradeLevel >= maxUpgradeLevel)
        {
            Debug.Log("Достигнут максимальный уровень прокачки!");
            return;
        }

        // Проверяем, хватает ли ресурсов (если есть система)
        if (playerResources != null && playerResources.CanSpend(upgradeCost))
        {
            playerResources.SpendResources(upgradeCost);
            ApplyDigRangeUpgrade();
        }
        else if (playerResources == null)
        {
            // Если нет системы ресурсов - просто прокачиваем
            ApplyDigRangeUpgrade();
        }
        else
        {
            Debug.Log("Недостаточно ресурсов для прокачки");
        }
    }

    /// <summary>
    /// Применяет улучшение радиуса копания
    /// </summary>
    void ApplyDigRangeUpgrade()
    {
        currentUpgradeLevel++;
        digRange += digRangeUpgradeStep;
        
        Debug.Log($"Улучшено! Уровень: {currentUpgradeLevel}, Новый радиус: {digRange}");
        
        // Сохраняем прогресс
        SaveUpgradeLevel();
        
        // Визуальный эффект (можно добавить анимацию или звук)
        OnUpgradePerformed();
    }
    
    /// <summary>
    /// Визуальная обратная связь при прокачке
    /// </summary>
    void OnUpgradePerformed()
    {
        // Например, запустить партиклы
        // Instantiate(upgradeEffect, transform.position, Quaternion.identity);
        
        // Или изменить цвет/масштаб объекта
        // transform.localScale = Vector3.one * (1 + currentUpgradeLevel * 0.1f);
    }
    // Если вы хотите использовать другой метод копания (например, кругом/площадью),
// замените функцию DigInDirection на это:

 void DigArea()
{
    // Капаем область вокруг указателя мыши
    Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mousePos, digRange, diggableLayer);
    
    foreach (var hitCollider in hitColliders)
    {
        Tilemap tilemap = hitCollider.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            Vector3Int cellPos = tilemap.WorldToCell(mousePos);
            
            // Копаем область 3x3
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int targetCell = cellPos + new Vector3Int(x, y, 0);
                    tilemap.SetTile(targetCell, null);
                }
            }
        }
    }
}
    /// <summary>
    /// Сохранение уровня прокачки (PlayerPrefs)
    /// </summary>
    void SaveUpgradeLevel()
    {
        PlayerPrefs.SetInt("DigRangeLevel", currentUpgradeLevel);
        PlayerPrefs.SetFloat("DigRange", digRange);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Загрузка уровня прокачки
    /// </summary>
    void LoadUpgradeLevel()
    {
        if (PlayerPrefs.HasKey("DigRangeLevel"))
        {
            currentUpgradeLevel = PlayerPrefs.GetInt("DigRangeLevel");
            digRange = PlayerPrefs.GetFloat("DigRange");
            Debug.Log($"Загружен уровень прокачки: {currentUpgradeLevel}, радиус: {digRange}");
        }
    }

    // Визуализация радиуса копания в редакторе
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.yellow;
        
        // Получаем направление к мыши для отображения
        if (mainCamera != null)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            Vector2 endPos = (Vector2)transform.position + direction * digRange;
            
            Gizmos.DrawLine(transform.position, endPos);
            Gizmos.DrawWireSphere(endPos, 0.1f);
        }
    }
}

/// <summary>
/// Простой скрипт для блоков (если они не в Tilemap)
/// </summary>
public class DiggableBlock : MonoBehaviour
{
    public int hitPoints = 1;
    public GameObject destroyEffect;
    public int resourceValue = 1;

    public void DestroyBlock()
    {
        // Эффект разрушения
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        
        // Здесь можно создать ресурс на земле
        // Instantiate(resourcePrefab, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}

/// <summary>
/// Простой скрипт для ресурсов игрока (опционально)
/// </summary>
public class PlayerResources : MonoBehaviour
{
    public int currentResources = 0;

    public void AddResources(int amount)
    {
        currentResources += amount;
        Debug.Log($"Ресурсов: {currentResources}");
    }

    public bool CanSpend(int amount)
    {
        return currentResources >= amount;
    }

    public void SpendResources(int amount)
    {
        if (CanSpend(amount))
        {
            currentResources -= amount;
        }
    }
}
