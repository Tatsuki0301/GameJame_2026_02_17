using UnityEngine;

public class ArcherEnemy : EnemyManager
{
    [Header("弓兵専用設定")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float shootCooldown = 2f;
    
    private float lastShootTime = 0f;
    
    private void Start()
    {
        // 弓兵：視界が狭く長い
        visionRange = 8f;
        visionAngle = 60f;
        attackRange = 7f;
    }
    
    protected override void OnPlayerDetected()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 攻撃範囲内なら射撃
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastShootTime + shootCooldown)
            {
                ShootArrow();
                lastShootTime = Time.time;
            }
        }
    }
    
    private void ShootArrow()
    {
        Debug.Log("弓兵が矢を放った！");
        
        if (arrowPrefab != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            
            // 矢の向きを設定
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // 矢に速度を与える
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 10f; // 矢の速度
            }
        }
    }
}