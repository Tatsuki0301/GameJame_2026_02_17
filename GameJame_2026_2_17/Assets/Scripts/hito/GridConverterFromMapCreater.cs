using UnityEngine;

public sealed class GridConverterFromMapCreater : MonoBehaviour
{
    [SerializeField] private MapCreater mapCreater;


    [SerializeField] private Vector2 startCreatePos = new Vector2(-8.39f, -4.50f);


    [SerializeField] private float cellSize = 1f;

    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - startCreatePos.x) / cellSize);

        int yFromBottom = Mathf.RoundToInt((worldPos.y - startCreatePos.y) / cellSize);
        int h = mapCreater.Map.GetLength(0);
        int y = (h - 1) - yFromBottom;

        return new Vector2Int(x, y);
    }
}
