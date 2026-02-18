using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyCellTracker : MonoBehaviour
{
    public static EnemyCellTracker I { get; private set; }

    [SerializeField] private GridConverterFromMapCreater converter;

    public bool IsReady => converter != null;

    private readonly Dictionary<Vector2Int, int> enemyCounts = new();

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public bool TryWorldToCell(Vector2 pos, out Vector2Int cell)
    {
        if (converter == null)
        {
            cell = default;
            return false;
        }

        cell = converter.WorldToCell(pos);
        return true;
    }

    public Vector2Int WorldToCell(Vector2 pos)
    {
        return TryWorldToCell(pos, out var cell) ? cell : default;
    }

    public void Register(Vector2Int cell)
    {
        enemyCounts.TryGetValue(cell, out int c);
        enemyCounts[cell] = c + 1;
    }

    public void Unregister(Vector2Int cell)
    {
        if (!enemyCounts.TryGetValue(cell, out int c)) return;
        c--;
        if (c <= 0) enemyCounts.Remove(cell);
        else enemyCounts[cell] = c;
    }

    public bool HasEnemy(Vector2Int cell) => enemyCounts.ContainsKey(cell);
}
