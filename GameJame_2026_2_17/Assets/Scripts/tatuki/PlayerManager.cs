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

    public void StartAction(Queue<int> _actions, Queue<GameObject> objQueue)
    {
        player.StartAction(_actions, objQueue);
    }

    public int GetMasValue(int y, int x)
    {
        return gm.GetMasValue(y, x);
    }

    public void SearchPlayer()
    {
        gm.SearchPlayer();
    }

    public int[] GetPlayerPos()
    {
        int[] playerPos =
        {
            player.PY,
            player.PX,
        };

        return playerPos;
    }

    public void AlertSound(int _y, int _x)
    {
        gm.AlertSound(_y, _x);
    }

    public void EndMainGame()
    {
        gm.EndMainGame();
    }

    public void EnemyDestroy(int y, int x)
    {
        gm.EnemyDestroy(y, x);
    }

    public void PlayerReset()
    {
        player.ResetPlayer();
    }

    public void ResetMap()
    {
        gm.ResetMap();
    }

    public void EndPlayer()
    {
        player.end = true;
    }
}
