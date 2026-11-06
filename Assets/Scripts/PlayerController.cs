using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Player Settings")]
    [SerializeField] public float Speed = 5f;
    [SerializeField] public float MaxHealth = 100f;
    [SerializeField] public float CurrentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Knockback")]
    [SerializeField] private float knockbackRecovery = 12f;

    private Vector2 MoveDirection;
    private Vector2 LastFacingDirection = Vector2.down;
    private Vector2 knockbackVelocity;
    private bool isMoving;

    public Vector2 FacingDirection => LastFacingDirection;

    void Awake()
    {
        DetachScreenSpaceCanvasesFromPlayer();

        CurrentHealth = MaxHealth;
        UpdateHealthUI();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void DetachScreenSpaceCanvasesFromPlayer()
    {
        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);

        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                continue;
            }

            if (canvas.transform == transform)
            {
                Debug.LogError("A screen-space HUD Canvas cannot be on the Player object. Place it on a separate GameObject.", canvas);
                continue;
            }

            canvas.transform.SetParent(null, true);
        }
    }

    void OnMove(InputValue value)
    {
        MoveDirection = value.Get<Vector2>();

        if (MoveDirection.sqrMagnitude > 0.01f)
        {
            LastFacingDirection = MoveDirection;
        }
    }

    void Update()
    {
        Vector3 movement = new Vector3(
            MoveDirection.x * Speed + knockbackVelocity.x,
            MoveDirection.y * Speed + knockbackVelocity.y,
            0f
        );
        transform.position += movement * Time.deltaTime;
        knockbackVelocity = Vector2.MoveTowards(
            knockbackVelocity,
            Vector2.zero,
            knockbackRecovery * Time.deltaTime
        );

        isMoving = MoveDirection.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            Vector2 animationDirection = isMoving ? MoveDirection : LastFacingDirection;
            animator.SetFloat("MoveX", animationDirection.x);
            animator.SetFloat("MoveY", animationDirection.y);
        }

    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector2.zero, 0f);
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackSpeed)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        UpdateHealthUI();

        if (knockbackSpeed > 0f && knockbackDirection.sqrMagnitude > 0f)
        {
            knockbackVelocity = knockbackDirection.normalized * knockbackSpeed;
        }

        Debug.Log($"Player took {amount} damage. HP left: {CurrentHealth}");

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(CurrentHealth)} / {Mathf.CeilToInt(MaxHealth)}";
        }
    }

    void Die()
    {
        Debug.Log("Player Die");
        Time.timeScale = 0f;
        enabled = false;
    }
}