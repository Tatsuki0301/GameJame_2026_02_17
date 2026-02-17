using UnityEngine;

public class NoiseFloor : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f; // 敵の検索範囲
    [SerializeField] private LayerMask enemyLayer; // 敵のレイヤー
    [SerializeField] private string playerTag = "Player";
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            CheckForNearbyEnemies(collision.gameObject);
        }
    }
    
    private void CheckForNearbyEnemies(GameObject player)
    {
        // 範囲内の敵を検索
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position, 
            detectionRadius, 
            enemyLayer
        );
        
        if (enemies.Length > 0)
        {
            Debug.Log($"音が鳴った！{enemies.Length}体の敵が気づいた");
            
            // プレイヤーを死亡させる（HP概念なし）
            var playerCollider = player.GetComponent<Collider2D>();
            var target = playerCollider != null && playerCollider.attachedRigidbody != null
                ? playerCollider.attachedRigidbody.gameObject
                : player;
            Destroy(target);
            
            // 敵に通知（オプション）
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SendMessage(
                    "OnNoiseHeard",
                    transform.position,
                    SendMessageOptions.DontRequireReceiver
                );
            }
        }
    }
    
    // デバッグ用：範囲を可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}