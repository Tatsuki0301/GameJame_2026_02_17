using UnityEngine;

public class NoiseFloor : MonoBehaviour
{
    [SerializeField] private int detectionRadiusCells = 5;

    private const int EnemyValue = 4;

    private GameManager_T gameManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager_T>();
        if (gameManager == null)
        {
            var gmObj = GameObject.Find("GameManager");
            if (gmObj != null) gameManager = gmObj.GetComponent<GameManager_T>();
        }
    }

    public void ActivateIfDanger(GameObject player)
    {
        if (player == null) return;

        if (gameManager == null) return;

        var p = player.GetComponent<Player>();
        if (p == null) return;

        // 「踏んだセル」を中心に探索（床オブジェクトの座標変換は不要）
        Vector2Int center = new Vector2Int(p.PX, p.PY);

        if (!HasEnemyWithin(center, detectionRadiusCells)) return;

        var rb = player.GetComponent<Rigidbody2D>();
        Destroy(rb != null ? rb.gameObject : player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null) return;
        ActivateIfDanger(collision.gameObject);
    }

    private bool HasEnemyWithin(Vector2Int center, int r)
    {
        if (gameManager == null) return false;

        int rr = r * r;
        for (int y = -r; y <= r; y++)
        for (int x = -r; x <= r; x++)
        {
            if (x * x + y * y > rr) continue;
            var cell = new Vector2Int(center.x + x, center.y + y);
            if (IsEnemyCell(cell)) return true;
        }
        return false;
    }

    private bool IsEnemyCell(Vector2Int cell)
    {
        if (gameManager == null) return false;

        int value = gameManager.GetMasValue(cell.y, cell.x);
        return value == EnemyValue;
    }
}
