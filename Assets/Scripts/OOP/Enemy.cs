using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float betweenAttack = 1.2f;
    [SerializeField, Min(0f)] private float attackDamageDelay = 0.25f;
    [SerializeField] private float patrolRadius = 3f;
    [SerializeField] private float movementSpeed = 2.5f;
    [SerializeField, Min(0f)] private float deathAnimationDuration = 1f;
    [SerializeField] private float knockbackRecovery = 12f;
    [SerializeField] private float playerKnockbackSpeed = 4f;

    [Header("Item Drop")]
    [SerializeField] private GameObject itemDropPrefab;
    [SerializeField] private ItemDetails itemDropDetails;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private Vector2 patrolDirection;
    private Vector2 startPosition;
    private EnemyState state = EnemyState.IDLE;
    private float lastAttackTime = float.NegativeInfinity;
    private Vector2 previousPosition;
    private Vector2 knockbackVelocity;
    private bool hasIsMovingParameter;
    private bool hasMoveXParameter;
    private bool hasMoveYParameter;
    private bool hasAttackTrigger;
    private bool hasHurtTrigger;
    private bool hasDeathTrigger;
    private bool isDead;
    private bool isAttacking;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    protected Transform player;

    public float Health
    {
        get => health;
        protected set => health = Mathf.Max(0f, value);
    }

    public float MovementSpeed
    {
        get => movementSpeed;
        protected set => movementSpeed = Mathf.Max(0.1f, value);
    }

    protected virtual void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        CacheAnimationParameters();
        previousPosition = transform.position;
        FindPlayer();
        startPosition = transform.position;
        NewPatrolDirection();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        TransitionCheck();

        switch (state)
        {
            case EnemyState.IDLE:
                Idle();
                break;
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.CHASE:
                Chase();
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
        }

        transform.position += (Vector3)(knockbackVelocity * Time.deltaTime);
        knockbackVelocity = Vector2.MoveTowards(
            knockbackVelocity,
            Vector2.zero,
            knockbackRecovery * Time.deltaTime
        );

        UpdateAnimation();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    public virtual void Chase()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            MovementSpeed * Time.deltaTime
        );
    }

    public virtual void Attack()
    {
        if (player == null) return;
        if (isAttacking || Time.time < lastAttackTime + betweenAttack) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > attackRange) return;

        lastAttackTime = Time.time;
        isAttacking = true;
        if (animator != null && hasAttackTrigger)
        {
            animator.SetTrigger(AttackHash);
        }

        StartCoroutine(ApplyAttackAfterDelay());
    }

    private System.Collections.IEnumerator ApplyAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackDamageDelay);

        if (!isDead && player != null &&
            Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                Vector2 knockbackDirection = (player.position - transform.position).normalized;
                pc.TakeDamage(attackDamage, knockbackDirection, playerKnockbackSpeed);
                Debug.Log("Enemy attacked the player.");
            }
        }

        isAttacking = false;
    }

    public virtual void Idle()
    {
        if (player == null)
        {
            state = EnemyState.PATROL;
        }
    }

    public virtual void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            patrolDirection,
            MovementSpeed * 0.5f * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, patrolDirection) < 0.2f)
        {
            NewPatrolDirection();
        }
    }

    protected virtual void NewPatrolDirection()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        patrolDirection = startPosition + randomOffset;
    }

    protected virtual void TransitionCheck()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null)
            {
                state = EnemyState.PATROL;
                return;
            }
        }

        float rangeToPlayer = Vector2.Distance(transform.position, player.position);

        if (rangeToPlayer <= attackRange)
        {
            state = EnemyState.ATTACK;
        }
        else if (rangeToPlayer <= detectionRange)
        {
            state = EnemyState.CHASE;
        }
        else
        {
            state = EnemyState.PATROL;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        TakeDamage(amount, Vector2.zero, 0f);
    }

    public virtual void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackSpeed)
    {
        if (isDead || amount <= 0f) return;

        Health -= amount;
        if (knockbackSpeed > 0f && knockbackDirection.sqrMagnitude > 0f)
        {
            knockbackVelocity = knockbackDirection.normalized * knockbackSpeed;
        }

        Debug.Log($"Enemy took {amount} damage. HP left: {Health}");

        if (Health <= 0f)
        {
            Die();
            return;
        }

        if (animator != null && hasHurtTrigger)
        {
            animator.SetTrigger(HurtHash);
        }
    }

    protected virtual void Die()
    {
        if (isDead || Health > 0f) return;

        isDead = true;
        DropItem();

        foreach (Collider2D enemyCollider in GetComponents<Collider2D>())
        {
            enemyCollider.enabled = false;
        }

        if (animator == null || !hasDeathTrigger)
        {
            Destroy(gameObject);
            return;
        }

        animator.SetTrigger(DeathHash);
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private void DropItem()
    {
        if (itemDropPrefab == null || itemDropDetails == null)
        {
            return;
        }

        GameObject droppedObject = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        Item droppedItem = droppedObject.GetComponent<Item>();
        if (droppedItem == null)
        {
            Debug.LogError("The item drop prefab needs an Item component on its root object.", droppedObject);
            Destroy(droppedObject);
            return;
        }

        droppedItem.SetItemDetails(itemDropDetails);
    }

    private System.Collections.IEnumerator DestroyAfterDeathAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }

    private void CacheAnimationParameters()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == IsMovingHash && parameter.type == AnimatorControllerParameterType.Bool)
            {
                hasIsMovingParameter = true;
            }
            else if (parameter.nameHash == MoveXHash && parameter.type == AnimatorControllerParameterType.Float)
            {
                hasMoveXParameter = true;
            }
            else if (parameter.nameHash == MoveYHash && parameter.type == AnimatorControllerParameterType.Float)
            {
                hasMoveYParameter = true;
            }
            else if (parameter.nameHash == AttackHash && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                hasAttackTrigger = true;
            }
            else if (parameter.nameHash == HurtHash && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                hasHurtTrigger = true;
            }
            else if (parameter.nameHash == DeathHash && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                hasDeathTrigger = true;
            }
        }
    }

    private void UpdateAnimation()
    {
        Vector2 currentPosition = transform.position;
        Vector2 movement = currentPosition - previousPosition;
        previousPosition = currentPosition;

        bool isMoving = movement.sqrMagnitude > 0.000001f;
        if (animator != null)
        {
            if (hasIsMovingParameter)
            {
                animator.SetBool(IsMovingHash, isMoving);
            }

            if (isMoving)
            {
                Vector2 direction = movement.normalized;
                if (hasMoveXParameter)
                {
                    animator.SetFloat(MoveXHash, direction.x);
                }

                if (hasMoveYParameter)
                {
                    animator.SetFloat(MoveYHash, direction.y);
                }
            }
        }
    }

    private void OnValidate()
    {
        health = Mathf.Max(0f, health);
        detectionRange = Mathf.Max(0.1f, detectionRange);
        attackRange = Mathf.Max(0.1f, attackRange);
        betweenAttack = Mathf.Max(0.1f, betweenAttack);
        attackDamageDelay = Mathf.Max(0f, attackDamageDelay);
        patrolRadius = Mathf.Max(0.1f, patrolRadius);
        movementSpeed = Mathf.Max(0.1f, movementSpeed);
        deathAnimationDuration = Mathf.Max(0f, deathAnimationDuration);

        if (attackRange > detectionRange)
        {
            detectionRange = attackRange;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}