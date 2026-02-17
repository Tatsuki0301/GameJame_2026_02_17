using UnityEngine;

public class SwordEnemy : EnemyManager
{
    [Header("剣士専用設定")]
    [SerializeField] private float moveSpeed = 2f;
    
    private void Start()
    {
        // 剣士：視界が広く短い
        visionRange = 3f;
        visionAngle = 120f;
        attackRange = 1.5f;
    }
    
    protected override void OnPlayerDetected()
    {
        // プレイヤーに近づく
        MoveTowardsPlayer();
        
        // 攻撃範囲内なら攻撃
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
    }
    
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }
    
    private void Attack()
    {
        Debug.Log("剣士が攻撃！");
        // 攻撃処理を実装
    }
}