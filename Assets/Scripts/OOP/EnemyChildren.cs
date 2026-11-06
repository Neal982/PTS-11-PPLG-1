using UnityEngine;

public class EnemyChildren : Enemy
{
    public override void Attack()
    {
        base.Attack();
        Debug.Log("Enemy Children Menyerang");
    }
}
