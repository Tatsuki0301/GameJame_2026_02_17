using UnityEngine;

/// <summary>
/// 敵とプレイヤーの位置をセル座標（2次元配列上の座標）として扱い、
/// 差分(dx,dy)で判定する。
/// - コライダー不要
/// - hitoフォルダだけで完結
///
/// 剣士: 周囲360度（斜め含む、rangeCells以内の正方形）で検知
/// 弓兵: 同じy行で、x方向にrangeCells以内（デフォルト4）で検知
/// </summary>
public sealed class EnemyVisionOnGrid : MonoBehaviour
{
    public enum EnemyType
    {
        Swordsman,
        Archer,
    }

    [Header("種類")]
    [SerializeField] private EnemyType enemyType = EnemyType.Swordsman;

    [Header("敵セル座標（2次元配列上）")]
    [SerializeField] private bool useEnemyCellOverride = true;

    [SerializeField] private Vector2Int enemyCellOverride;

    [Header("剣士（360度）")]
    [Min(0)]
    [SerializeField] private int swordsmanRangeCells = 3;

    [Header("弓兵（x方向）")]
    [Min(0)]
    [SerializeField] private int archerXRangeCells = 4;

    private Player player;

    public void Initialize(EnemyType type, Vector2Int enemyCell)
    {
        enemyType = type;
        useEnemyCellOverride = true;
        enemyCellOverride = enemyCell;
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null) return;
        }

        Vector2Int enemyCell = GetEnemyCell();
        Vector2Int playerCell = GetPlayerCell();

        int dx = playerCell.x - enemyCell.x;
        int dy = playerCell.y - enemyCell.y;

        bool detected = enemyType switch
        {
            EnemyType.Swordsman => IsWithinSquare(dx, dy, swordsmanRangeCells),
            EnemyType.Archer => dy == 0 && Mathf.Abs(dx) <= archerXRangeCells,
            _ => false,
        };

        if (!detected) return;

        KillPlayer();
    }

    private Vector2Int GetEnemyCell()
    {
        if (useEnemyCellOverride) return enemyCellOverride;
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(-transform.position.y));
    }

    private Vector2Int GetPlayerCell()
    {
        if (player == null) return default;

        return new Vector2Int(player.PX, player.PY);
    }

    private static bool IsWithinSquare(int dx, int dy, int range)
    {
        if (range <= 0) return false;
        return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) <= range;
    }

    private void KillPlayer()
    {
        if (player == null) return;

        var rb = player.GetComponent<Rigidbody2D>();
        Destroy(rb != null ? rb.gameObject : player.gameObject);
        player = null;
    }
}
