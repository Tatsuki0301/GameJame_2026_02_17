using UnityEngine;

public class NoiseFloor : MonoBehaviour
{
    [SerializeField] private int detectionRadiusCells = 5;
    [SerializeField] private string playerTag = "Player";

    public void ActivateIfDanger(GameObject player)
    {
        if (player == null) return;
        if (EnemyCellTracker.I == null) return;

        if (!EnemyCellTracker.I.TryWorldToCell(transform.position, out var center)) return;

        if (!HasEnemyWithin(center, detectionRadiusCells)) return;

        var rb = player.GetComponent<Rigidbody2D>();
        Destroy(rb != null ? rb.gameObject : player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        ActivateIfDanger(collision.gameObject);
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
