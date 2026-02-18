using System;
using UnityEngine;

public sealed class GridOcclusionMap : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2 origin = Vector2.zero;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private bool invertY = true;

    private int width;
    private int height;
    private byte[] blocked; // 1 = blocked, 0 = empty

    public bool IsInitialized => blocked != null && blocked.Length == width * height && width > 0 && height > 0 && cellSize > 0f;

    public void Initialize(int gridWidth, int gridHeight, Vector2 gridOrigin, float gridCellSize, ReadOnlySpan<bool> isBlocked)
    {
        if (gridWidth <= 0) throw new ArgumentOutOfRangeException(nameof(gridWidth));
        if (gridHeight <= 0) throw new ArgumentOutOfRangeException(nameof(gridHeight));
        if (gridCellSize <= 0f) throw new ArgumentOutOfRangeException(nameof(gridCellSize));
        if (isBlocked.Length != gridWidth * gridHeight) throw new ArgumentException("isBlocked length must be width*height", nameof(isBlocked));

        width = gridWidth;
        height = gridHeight;
        origin = gridOrigin;
        cellSize = gridCellSize;

        blocked = new byte[width * height];
        for (int i = 0; i < blocked.Length; i++)
        {
            blocked[i] = (byte)(isBlocked[i] ? 1 : 0);
        }
    }

    public void Initialize(int gridWidth, int gridHeight, Vector2 gridOrigin, float gridCellSize, bool[] isBlocked)
    {
        if (isBlocked == null) throw new ArgumentNullException(nameof(isBlocked));
        Initialize(gridWidth, gridHeight, gridOrigin, gridCellSize, isBlocked.AsSpan());
    }

    public Vector2Int WorldToCell(Vector2 worldPosition)
    {
        if (cellSize <= 0f) return Vector2Int.zero;
        Vector2 local = worldPosition - origin;
        // DOTweenなどで 0.999999 / 1.000001 のような誤差が出ても
        // 意図したセルに安定して入るように Round を採用する
        int x = Mathf.RoundToInt(local.x / cellSize);
        int yFromBottom = Mathf.RoundToInt(local.y / cellSize);
        int y = invertY && height > 0
            ? (height - 1) - yFromBottom
            : yFromBottom;
        return new Vector2Int(x, y);
    }

    public bool IsBlocked(Vector2Int cell)
    {
        if (!IsInitialized) return false;
        if ((uint)cell.x >= (uint)width || (uint)cell.y >= (uint)height) return true;

        int index = cell.y * width + cell.x;
        return blocked[index] != 0;
    }

    public bool HasLineOfSight(Vector2 fromWorld, Vector2 toWorld)
    {
        if (!IsInitialized) return true;

        Vector2Int a = WorldToCell(fromWorld);
        Vector2Int b = WorldToCell(toWorld);

        bool first = true;
        foreach (var cell in EnumerateLineCells(a, b))
        {
            if (first)
            {
                first = false;
                continue;
            }

            if (cell == b) break;
            if (IsBlocked(cell)) return false;
        }

        return true;
    }

    private static System.Collections.Generic.IEnumerable<Vector2Int> EnumerateLineCells(Vector2Int a, Vector2Int b)
    {
        int x0 = a.x;
        int y0 = a.y;
        int x1 = b.x;
        int y1 = b.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            yield return new Vector2Int(x0, y0);
            if (x0 == x1 && y0 == y1) yield break;

            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
