using UnityEngine;

public sealed class GridConverterFromMapCreater : MonoBehaviour
{
    [SerializeField] private MapCreater mapCreater;

    // MapCreater の startCreatePos と同じ値を入れる（Inspectorで設定）
    [SerializeField] private Vector2 startCreatePos = new Vector2(-8.39f, -4.50f);

    // セルサイズ（あなたの生成が 1*x, 1*y なので基本1）
    [SerializeField] private float cellSize = 1f;

    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        // x
        int x = Mathf.RoundToInt((worldPos.x - startCreatePos.x) / cellSize);

        // y（MapCreaterは生成で上下反転しているので map高さが必要）
        int yFromBottom = Mathf.RoundToInt((worldPos.y - startCreatePos.y) / cellSize);
        int h = mapCreater.Map.GetLength(0);
        int y = (h - 1) - yFromBottom;

        return new Vector2Int(x, y);
    }
}
