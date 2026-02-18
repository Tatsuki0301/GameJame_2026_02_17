using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyManager : MonoBehaviour
{
    [Header("視界設定")]
    [SerializeField] protected float visionRange = 5f;     // 視界の距離
    [SerializeField] protected float visionAngle = 90f;    // 視界の角度

    [Header("遮蔽（グリッド判定）")]
    [SerializeField] private GridOcclusionMap gridOcclusion;

    [Header("デバッグ表示")]
    [SerializeField] private bool drawFovSector = true;
    [SerializeField] private bool drawFovOutline = false;

    private void Reset()
    {
        drawFovSector = true;
        drawFovOutline = false;
    }

    private void Awake()
    {
        if (gridOcclusion == null)
        {
            gridOcclusion = FindAnyObjectByType<GridOcclusionMap>();
        }
    }
    
    protected Transform player;
    protected bool canSeePlayer = false;

    protected void KillPlayer()
    {
        if (player == null) return;

        // プレイヤーを破壊する
        var playerCollider = player.GetComponent<Collider2D>();
        // プレイヤーのCollider2Dが存在し、Rigidbody2Dもアタッチされている場合は、そのゲームオブジェクトを破壊
        var target = playerCollider != null && playerCollider.attachedRigidbody != null
            ? playerCollider.attachedRigidbody.gameObject
            : player.gameObject;

        Destroy(target);
    }
    
    protected virtual void Update()
    {
        DetectPlayer();
        
        if (canSeePlayer)
        {
            KillPlayer();
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
            else
            {
                canSeePlayer = false;
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

        // 遮蔽チェック
        canSeePlayer = gridOcclusion != null
            && gridOcclusion.IsInitialized
            && gridOcclusion.HasLineOfSight(transform.position, player.position);
    }
    
    // 音のなる床から呼ばれる
    public virtual void OnNoiseHeard(Vector2 noisePosition)
    {
        Debug.Log($"{gameObject.name}が音を聞いた！");
    }
    
    // デバッグ用：視界を可視化
    protected virtual void OnDrawGizmosSelected()
    {
        var baseColor = canSeePlayer ? Color.red : Color.green;

        // 視界の範囲を描画（扇状のみ）
        Vector3 forward = transform.right;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2f) * forward * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2f) * forward * visionRange;

#if UNITY_EDITOR
        if (drawFovSector)
        {
            Handles.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.15f);
            Handles.DrawSolidArc(
                transform.position,
                Vector3.forward,
                rightBoundary.normalized,
                visionAngle,
                visionRange
            );
        }
#endif

        if (drawFovOutline)
        {
            Gizmos.color = baseColor;
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        }
    }
}