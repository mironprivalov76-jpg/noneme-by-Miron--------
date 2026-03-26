using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.right;

    [Header("Mining")]
    [SerializeField] private float mineTime = 0.3f;
    [SerializeField] private float breakDistance = 3f;
    [SerializeField] private float blockSize = 1f;
    [SerializeField] private int damagePerHit = 1;

    [Header("Mode")]
    [SerializeField] private bool breakOnlyFirstBlock = true;

    [Header("World")]
    [SerializeField] private Transform worldParent;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private Text scoreLegacy;

    private float mineTimer;
    private int score;

    private GameObject currentBlock;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleInput();
        AutoMine();
        HighlightBlock();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    // ---------------- ДВИЖЕНИЕ ----------------
    private void HandleInput()
    {
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.W)) y = 1;
        if (Input.GetKey(KeyCode.S)) y = -1;
        if (Input.GetKey(KeyCode.A)) x = -1;
        if (Input.GetKey(KeyCode.D)) x = 1;

        movement = new Vector2(x, y).normalized;

        if (movement != Vector2.zero)
            lastDirection = movement;
    }

    // ---------------- АВТО-КОПАНИЕ ----------------
    private void AutoMine()
    {
        mineTimer += Time.deltaTime;

        if (mineTimer >= mineTime)
        {
            MineForward();
            mineTimer = 0f;
        }
    }

    // ---------------- ЛОМАНИЕ ----------------
    private void MineForward()
    {
        Vector2 origin = transform.position;
        Vector2 dir = lastDirection.normalized;

        int steps = Mathf.RoundToInt(breakDistance / blockSize);

        for (int i = 1; i <= steps; i++)
        {
            Vector2 checkPos = origin + dir * (i * blockSize);

            Collider2D hit = Physics2D.OverlapPoint(checkPos);

            if (hit == null)
                continue;

            GameObject block = hit.gameObject;

            if (block.transform.parent != worldParent)
                continue;

           Block blockScript = block.GetComponent<Block>();
            if (blockScript == null)
                continue;

            bool destroyed = blockScript.Damage(damagePerHit);

            if (destroyed)
            {
                score += blockScript.scoreValue;
                UpdateUI();
            }

            if (breakOnlyFirstBlock)
                break;
        }
    }

    // ---------------- ПОДСВЕТКА ----------------
    private void HighlightBlock()
    {
        if (currentBlock != null)
        {
            SetColor(currentBlock, Color.white);
            currentBlock = null;
        }

        Vector2 origin = transform.position;
        Vector2 dir = lastDirection.normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, breakDistance);

        if (hit.collider != null)
        {
            GameObject block = hit.collider.gameObject;

            if (block.transform.parent == worldParent)
            {
                currentBlock = block;
                SetColor(block, Color.yellow);
            }
        }
    }

    private void SetColor(GameObject obj, Color color)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = color;
    }

    // ---------------- UI ----------------
    private void UpdateUI()
    {
        string text = "Score: " + score;

        if (scoreTMP != null)
            scoreTMP.text = text;

        if (scoreLegacy != null)
            scoreLegacy.text = text;
    }

    // ---------------- DEBUG ----------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector2 origin = transform.position;
        Vector2 dir = lastDirection.normalized;

        Gizmos.DrawLine(origin, origin + dir * breakDistance);
    }
}