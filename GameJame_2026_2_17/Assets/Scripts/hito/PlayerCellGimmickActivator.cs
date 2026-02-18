using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider2D/Trigger を使わず、プレイヤーのセル移動を監視して
/// そのセルに配置されたギミック（NoiseFloor / Spikefloor）を発火させる。
/// </summary>
[DefaultExecutionOrder(2000)]
public sealed class PlayerCellGimmickActivator : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GridConverterFromMapCreater converter;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private Transform player;

    private readonly Dictionary<Vector2Int, Spikefloor> spikeByCell = new();
    private readonly Dictionary<Vector2Int, NoiseFloor> noiseByCell = new();

    private bool indexed;
    private Vector2Int lastCell;
    private bool hasLastCell;

    private void Awake()
    {
        if (converter == null)
        {
            converter = FindAnyObjectByType<GridConverterFromMapCreater>();
        }
    }

    private IEnumerator Start()
    {
        // MapCreater.Start で生成される床/ギミックを拾うため、1フレ待つ
        yield return null;
        RebuildIndex();
    }

    private void Update()
    {
        if (converter == null) return;

        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj == null) return;
            player = playerObj.transform;
        }

        if (!indexed)
        {
            RebuildIndex();
            if (!indexed) return;
        }

        Vector2Int cell = converter.WorldToCell(player.position);
        if (hasLastCell && cell == lastCell) return;

        lastCell = cell;
        hasLastCell = true;

        if (spikeByCell.TryGetValue(cell, out var spike) && spike != null)
        {
            spike.Activate(player.gameObject);
            return;
        }

        if (noiseByCell.TryGetValue(cell, out var noise) && noise != null)
        {
            noise.ActivateIfDanger(player.gameObject);
        }
    }

    private void RebuildIndex()
    {
        spikeByCell.Clear();
        noiseByCell.Clear();

        var spikes = FindObjectsByType<Spikefloor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var noises = FindObjectsByType<NoiseFloor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        for (int i = 0; i < spikes.Length; i++)
        {
            var s = spikes[i];
            if (s == null) continue;
            var cell = converter.WorldToCell(s.transform.position);
            if (!spikeByCell.ContainsKey(cell)) spikeByCell.Add(cell, s);
        }

        for (int i = 0; i < noises.Length; i++)
        {
            var n = noises[i];
            if (n == null) continue;
            var cell = converter.WorldToCell(n.transform.position);
            if (!noiseByCell.ContainsKey(cell)) noiseByCell.Add(cell, n);
        }

        indexed = spikeByCell.Count > 0 || noiseByCell.Count > 0;
    }
}
