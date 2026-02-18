using UnityEngine;

public sealed class EnemyCellStatic : MonoBehaviour
{
    private Vector2Int cell;

    private void Start()
    {
        if (EnemyCellTracker.I == null) return;

        cell = EnemyCellTracker.I.WorldToCell(transform.position);
        EnemyCellTracker.I.Register(cell);
    }

    private void OnDestroy()
    {
        if (EnemyCellTracker.I == null) return;
        EnemyCellTracker.I.Unregister(cell);
    }
}
