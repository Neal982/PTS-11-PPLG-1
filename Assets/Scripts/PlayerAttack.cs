using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2.25f;
    public float attackDamage = 10f;
    public Transform attackPoint;

    [SerializeField, Min(0f)] private float attackCooldown = 0.5f;
    [SerializeField] private float attackPointDistance = 0.75f;
    [SerializeField] private float enemyKnockbackSpeed = 4f;
    [SerializeField] private Animator animator;

    private PlayerController playerController;
    private bool hasAttackTrigger;
    private float nextAttackTime;

    private static readonly int AttackHash = Animator.StringToHash("Attack");

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        playerController = GetComponent<PlayerController>();
        hasAttackTrigger = animator != null &&
            System.Array.Exists(animator.parameters, parameter =>
                parameter.nameHash == AttackHash &&
                parameter.type == AnimatorControllerParameterType.Trigger);

        UpdateAttackPoint();
    }

    void Update()
    {
        UpdateAttackPoint();

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (hasAttackTrigger)
            animator.SetTrigger(AttackHash);

        Debug.Log("Player melakukan serangan!");

        Vector2 attackPos = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Vector2 attackDirection = playerController != null
            ? playerController.FacingDirection
            : Vector2.down;
        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPos, attackRange);

        foreach (Collider2D hit in hitTargets)
        {
            if (hit == null) continue;
            if (hit.transform == transform) continue;

            IDamageable damageableTarget = hit.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    continue;
                }

                Enemy enemyTarget = hit.GetComponent<Enemy>();
                if (enemyTarget != null)
                {
                    enemyTarget.TakeDamage(attackDamage, attackDirection, enemyKnockbackSpeed);
                }
                else
                {
                    damageableTarget.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void UpdateAttackPoint()
    {
        if (attackPoint == null)
            return;

        Vector2 direction = playerController != null
            ? playerController.FacingDirection
            : Vector2.down;

        attackPoint.localPosition = direction.normalized * attackPointDistance;
    }
}