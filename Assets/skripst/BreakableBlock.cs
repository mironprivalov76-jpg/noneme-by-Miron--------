using UnityEngine;
using UnityEngine.UI;

public class LomatBlock : MonoBehaviour
{
    public int points = 10; 
     [Range(0f, 1f)]
    public float spawnChance = 1f; // шанс появления
    public Text scoreText;

    private static int totalScore = 0;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AddScore(points);
            Destroy(gameObject);
        }
    }

    void AddScore(int amount)
    {
        totalScore += amount;
        scoreText.text = "Score: " + totalScore;
    }
}