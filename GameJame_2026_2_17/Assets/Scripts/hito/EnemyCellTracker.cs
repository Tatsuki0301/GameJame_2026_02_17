using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyCellTracker : MonoBehaviour
{
    public static EnemyCellTracker I { get; private set; }

    [SerializeField] private GridConverterFromMapCreater converter;

    private readonly Dictionary<Vector2Int, int> enemyCounts = new();

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public Vector2Int WorldToCell(Vector2 pos) => converter.WorldToCell(pos);

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
