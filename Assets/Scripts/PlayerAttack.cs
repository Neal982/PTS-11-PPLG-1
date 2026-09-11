using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;    
    public float attackDamage = 10f;
    public Transform attackPoint;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Player melakukan serangan!");

        Vector2 attackPos = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageableTarget = enemy.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(attackDamage);
            }
        }
    }
}