using UnityEngine;

public class Weapon2D : MonoBehaviour
{
    [Header("Характеристики оружия")]
    public float damage = 30f;
    public float attackRate = 0.5f;
    
    [Header("Визуальные эффекты")]
    public GameObject hitEffect; // Эффект при попадании
    public AudioClip attackSound; // Звук атаки
    
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    public void Attack()
    {
        // Анимация атаки
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Звук атаки
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
}