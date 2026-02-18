using UnityEngine;

public class NoiseFloor : MonoBehaviour
{
    [SerializeField] private int detectionRadiusCells = 5;
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;
        if (EnemyCellTracker.I == null) return;

        // ★踏んだ床中心
        var center = EnemyCellTracker.I.WorldToCell(transform.position);

        if (HasEnemyWithin(center, detectionRadiusCells))
        {
            var player = collision.gameObject;
            var col = player.GetComponent<Collider2D>();
            var target = col != null && col.attachedRigidbody != null
                ? col.attachedRigidbody.gameObject
                : player;

            Destroy(target);
        }
    }

    private bool HasEnemyWithin(Vector2Int center, int r)
    {
        int rr = r * r;
        for (int y = -r; y <= r; y++)
        for (int x = -r; x <= r; x++)
        {
            if (x * x + y * y > rr) continue;
            if (EnemyCellTracker.I.HasEnemy(new Vector2Int(center.x + x, center.y + y)))
                return true;
        }
        return false;
    }
}
