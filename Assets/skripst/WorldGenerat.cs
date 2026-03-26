using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("World Size")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 10;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 startPosition = Vector2.zero;
    [SerializeField] private float offset = 1f;

    [Header("Blocks")]
    [SerializeField] private GameObject[] blocks;

    private void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject block = GetRandomBlock();

                Vector2 position = new Vector2(
                    startPosition.x + x * offset,
                    startPosition.y + y * offset
                );

                Instantiate(block, position, Quaternion.identity);
            }
        }
    }

    GameObject GetRandomBlock()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, blocks.Length);
            GameObject candidate = blocks[randomIndex];

           LomatBlock chance = candidate.GetComponent<LomatBlock>();

            float spawnChance = chance != null ? chance.spawnChance : 1f;

            if (Random.value <= spawnChance)
            {
                return candidate;
            }
        }
    }
}