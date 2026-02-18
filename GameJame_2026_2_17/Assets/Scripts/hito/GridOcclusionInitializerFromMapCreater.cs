using UnityEngine;

[DefaultExecutionOrder(1000)]
public sealed class GridOcclusionInitializerFromMapCreater : MonoBehaviour
{
    [SerializeField] private MapCreater mapCreater;
    [SerializeField] private GridOcclusionMap gridOcclusion;


    [SerializeField] private Vector2 startCreatePos = new Vector2(-8.39f, -4.50f);


    [SerializeField] private float cellSize = 1f;

    [SerializeField] private int wallValue = 3;

    private void Start()
    {
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (mapCreater == null) return;
        if (gridOcclusion == null) return;

        var map = mapCreater.Map;
        if (map == null) return;

        int h = map.GetLength(0);
        int w = map.GetLength(1);
        if (h <= 0 || w <= 0) return;

        var isBlocked = new bool[w * h];
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            isBlocked[y * w + x] = map[y, x] == wallValue;
        }

        gridOcclusion.Initialize(w, h, startCreatePos, cellSize, isBlocked);
    }
}
