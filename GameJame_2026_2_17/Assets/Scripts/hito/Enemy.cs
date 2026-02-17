using UnityEngine;

public abstract class EnemyManager : MonoBehaviour
{
    [Header("視界設定")]
    [SerializeField] protected float visionRange = 5f;     // 視界の距離
    [SerializeField] protected float visionAngle = 90f;    // 視界の角度
    [SerializeField] protected LayerMask playerLayer;
    
    [Header("攻撃設定")]
    [SerializeField] protected float attackRange = 1f;     // 攻撃範囲
    
    protected Transform player;
    protected bool canSeePlayer = false;
    
    protected virtual void Update()
    {
        DetectPlayer();
        
        if (canSeePlayer)
        {
            OnPlayerDetected();
        }
    }
    
    // プレイヤーを検知
    protected virtual void DetectPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            return;
        }
        
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 距離チェック
        if (distanceToPlayer > visionRange)
        {
            canSeePlayer = false;
            return;
        }
        
        // 角度チェック
        float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);
        if (angleToPlayer > visionAngle / 2f)
        {
            canSeePlayer = false;
            return;
        }
        
        // 障害物チェック（Raycast）
        // playerLayer が未設定(0)でも動くように、基本は「敵レイヤーが衝突する対象」をRaycastする。
        // さらに playerLayer をORしておくことで、プレイヤーが衝突設定外でも検知できるようにする。
        int losMask = Physics2D.GetLayerCollisionMask(gameObject.layer) | playerLayer.value;
        Vector2 rayOrigin = (Vector2)transform.position + directionToPlayer * 0.05f;
        float rayDistance = Mathf.Max(0f, distanceToPlayer - 0.05f);

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            directionToPlayer,
            rayDistance,
            losMask
        );

        canSeePlayer = (hit.collider != null && hit.collider.CompareTag("Player"));
    }
    
    // プレイヤーを発見した時の処理（サブクラスで実装）
    protected abstract void OnPlayerDetected();
    
    // 音のなる床から呼ばれる
    public virtual void OnNoiseHeard(Vector2 noisePosition)
    {
        Debug.Log($"{gameObject.name}が音を聞いた！");
    }
    
    // デバッグ用：視界を可視化
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = canSeePlayer ? Color.red : Color.green;
        
        // 視界の範囲を描画
        Vector3 forward = transform.right;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * forward * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * forward * visionRange;
        
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // 攻撃範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}