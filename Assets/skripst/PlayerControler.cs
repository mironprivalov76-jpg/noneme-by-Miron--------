using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.W)) y = 1;
        if (Input.GetKey(KeyCode.S)) y = -1;
        if (Input.GetKey(KeyCode.A)) x = -1;
        if (Input.GetKey(KeyCode.D)) x = 1;

        movement = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
    }
}