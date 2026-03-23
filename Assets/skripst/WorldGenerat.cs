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

    [Header("Parent")]
    [SerializeField] private Transform worldParent;

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
                int randomIndex = Random.Range(0, blocks.Length);

                Vector2 pos = new Vector2(
                    startPosition.x + x * offset,
                    startPosition.y + y * offset
                );

                GameObject block = Instantiate(blocks[randomIndex], pos, Quaternion.identity);

                if (worldParent != null)
                    block.transform.parent = worldParent;
            }
        }
    }
}