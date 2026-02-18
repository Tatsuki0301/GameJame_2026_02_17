using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("プレイヤーの初期角度")]
    [SerializeField]
    private Player.PlayerDirection direction;

    private GameManager_T gm;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PMStart(GameManager_T _gm, Player _player, int x, int y)
    {
        gm = _gm;
        player = _player;

        player.PlayerStart(this, direction, x, y);
    }

    public void StartAction(Queue<int> _actions)
    {
        player.StartAction(_actions);
    }

    public int GetMasValue(int y, int x)
    {
        return gm.GetMasValue(y, x);
    }
}
