using UnityEngine;
using System;

public class Enemy : MonoBehaviour , IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] public float Health = 100f;
    [SerializeField] private float DetectionRange = 4f;
    [SerializeField] private float AttackRange = 1.5f;
    [SerializeField] private float BetweenAttack = 1f;
    [SerializeField] private float PatrolRadius = 3f;
    [SerializeField] public float MovementSpeed = 2f;

    private Vector2 PatrolDirection;
    private Vector2 StartPosition;
    private EnemyState state = EnemyState.IDLE;
    private float LastAttackTime;

    protected Transform player;

    protected virtual void Start()
    {
        FindPlayer();
        StartPosition = transform.position;
        NewPatrolDirection();
    }

    void Update()
    {
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
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    

    public void Chase()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            MovementSpeed * Time.deltaTime
        );
        Debug.Log("Enemy Chasing");
    }

    public virtual void Attack()
    {
        if (Time.time >= LastAttackTime + BetweenAttack)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, AttackRange);

            if (hit != null && hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    
                }
            }

            LastAttackTime = Time.time;
            Debug.Log("Enemy Attacking");
        }
    }

    public virtual void Idle()
    {
        Debug.Log("Enemy Idle");
    }

    public virtual void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            PatrolDirection,
            MovementSpeed * 0.5f * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, PatrolDirection) < 0.2f)
        {
            NewPatrolDirection();
            Debug.Log("Enemy Patrol");
        }
    }

    void NewPatrolDirection()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * PatrolRadius;
        PatrolDirection = StartPosition + randomDirection;
    }

    void TransitionCheck()
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

        float RangeToPlayer = Vector2.Distance(transform.position, player.position);

        if (RangeToPlayer <= AttackRange)
        {
            state = EnemyState.ATTACK;
        }
        else if (RangeToPlayer <= DetectionRange)
        {
            state = EnemyState.CHASE;
        }
        else
        {
            state = EnemyState.PATROL;
        }
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log("Zombie terkena damage sebesar: " + amount + ". Sisa darah: " + Health);

        if (Health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Zombie mati!");
        Destroy(gameObject);
    }
}