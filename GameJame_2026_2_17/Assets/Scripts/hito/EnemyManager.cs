using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private GameManager_T gm;
    private Transform player;

    private readonly List<Transform> swordsmen = new();
    private readonly List<Transform> archers = new();

    [Header("Map値")]
    [SerializeField] private int wallValue = 3;
    [SerializeField] private int swordsmanValue = 2;
    [SerializeField] private int archerValue = 4;

    [Header("探索")]
    [Min(0)]
    [SerializeField] private int swordsmanRangeCells = 3;
    [Min(0)]
    [SerializeField] private int archerRangeCells = 4;

    [Header("座標変換（MapCreaterと合わせる）")]
    [SerializeField] private Vector2 startCreatePos = new Vector2(-8.39f, -4.50f);

    private int mapHeight;

    private IEnumerator Start()
    {
        var gmObj = GameObject.Find("GameManager");
        if (gmObj == null) yield break;
        gm = gmObj.GetComponent<GameManager_T>();
        if (gm == null) yield break;

        // MapCreaterが生成し終わってから、Mapルート配下の敵を拾う
        const int maxFrames = 120;
        for (int i = 0; i < maxFrames; i++)
        {
            if (TryCacheEnemies()) yield break;
            yield return null;
        }
    }

    private bool TryCacheEnemies()
    {
        var mcObj = GameObject.Find("MapCreater");
        if (mcObj == null) return false;
        var mc = mcObj.GetComponent<MapCreater>();
        if (mc == null || mc.Map == null) return false;

        mapHeight = mc.Map.GetLength(0);
        if (mapHeight <= 0) return false;

        var mapRoot = GameObject.Find("Map");
        if (mapRoot == null) return false;

        swordsmen.Clear();
        archers.Clear();

        foreach (Transform child in mapRoot.transform)
        {
            Vector2Int cell = WorldToCell(child.position);
            int v = gm.GetMasValue(cell.y, cell.x);
            if (v == swordsmanValue) swordsmen.Add(child);
            if (v == archerValue) archers.Add(child);
        }

        return true;
    }

    private void Update()
    {
        if (gm == null) return;
        if (mapHeight <= 0) return;

        if (player == null)
        {
            var p = FindAnyObjectByType<Player>();
            if (p == null) return;
            player = p.transform;
        }

        int[] pos = gm.GetPlayerPos();
        if (pos == null || pos.Length < 2) return;
        var playerCell = new Vector2Int(pos[1], pos[0]);

        if (IsDetectedBySwordsman(playerCell) || IsDetectedByArcher(playerCell))
        {
            KillPlayer();
        }
    }

    private bool IsDetectedBySwordsman(Vector2Int playerCell)
    {
        int rr = swordsmanRangeCells * swordsmanRangeCells;
        for (int i = 0; i < swordsmen.Count; i++)
        {
            Transform enemy = swordsmen[i];
            if (enemy == null) continue;

            Vector2Int e = WorldToCell(enemy.position);
            int dx = playerCell.x - e.x;
            int dy = playerCell.y - e.y;
            if (dx * dx + dy * dy <= rr) return true;
        }
        return false;
    }

    private bool IsDetectedByArcher(Vector2Int playerCell)
    {
        for (int i = 0; i < archers.Count; i++)
        {
            Transform enemy = archers[i];
            if (enemy == null) continue;

            Vector2Int e = WorldToCell(enemy.position);
            Vector2Int step = RotationToStep(enemy.eulerAngles.z);
            if (step == Vector2Int.zero) continue;

            if (RayToPlayer(e, step, archerRangeCells, playerCell)) return true;
        }
        return false;
    }

    private bool RayToPlayer(Vector2Int enemy, Vector2Int step, int range, Vector2Int playerCell)
    {
        for (int i = 1; i <= range; i++)
        {
            var c = new Vector2Int(enemy.x + step.x * i, enemy.y + step.y * i);
            int v = gm.GetMasValue(c.y, c.x);
            if (v == wallValue) return false;
            if (c == playerCell) return true;
        }
        return false;
    }

    private Vector2Int WorldToCell(Vector3 world)
    {
        int x = Mathf.RoundToInt(world.x - startCreatePos.x);
        int yFromBottom = Mathf.RoundToInt(world.y - startCreatePos.y);
        int y = (mapHeight - 1) - yFromBottom;
        return new Vector2Int(x, y);
    }

    private static Vector2Int RotationToStep(float z)
    {
        // Player.csの回転に合わせる: Up=0, Left=90, Down=180, Right=270(=-90)
        float angle = Mathf.Repeat(z, 360f);
        int snapped = Mathf.RoundToInt(angle / 90f) * 90;
        snapped = (snapped % 360 + 360) % 360;

        return snapped switch
        {
            0 => Vector2Int.up,
            90 => Vector2Int.left,
            180 => Vector2Int.down,
            270 => Vector2Int.right,
            _ => Vector2Int.zero,
        };
    }

    private void KillPlayer()
    {
        if (player == null) return;
        var rb = player.GetComponent<Rigidbody2D>();
        Destroy(rb != null ? rb.gameObject : player.gameObject);
        player = null;
    }

    public int GetMasValue(int y, int x)
    {
        return gm.GetMasValue(y, x);
    }
}