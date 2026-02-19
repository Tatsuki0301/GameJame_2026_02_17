using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnemyBase;

[DefaultExecutionOrder(-5)]
public class GameManager_T : MonoBehaviour
{
    private MapCreater mc;
    private PlayerManager pm;
    private EnemyManager_T em;
    private GimickManager gimm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mc = GameObject.Find("MapCreater").GetComponent<MapCreater>();
        pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        em = GameObject.Find("EnemyManager").GetComponent<EnemyManager_T>();
        gimm = GameObject.Find("GimickManager").GetComponent<GimickManager>();

        em.EMStart(this);
        gimm.GimMStart(this);
        mc.MapStart(this);
    }

    public void StartAction(Queue<int> _actions, Queue<GameObject> objQueue)
    {
        pm.StartAction(_actions, objQueue);
    }

    public void PMStart(Player player, int x, int y)
    {
        pm.PMStart(this, player, x, y);
    }

    public int GetMasValue(int y, int x)
    {
        if (y < mc.Map.GetLength(0) && y >= 0
            && x < mc.Map.GetLength(1) && x >= 0)
        {
            return mc.Map[y, x];
        }
        else return 3;
    }

    public int[] GetPlayerPos()
    {
        return pm.GetPlayerPos();
    }

    public int GetKeyValue(int _y, int _x)
    {
        int key = _y * mc.Map.GetLength(1) + _x;
        return key;
    }

    public void SetEnemyData(int ey, int ex, float _dir, EnemyBase _enemy)
    {
        em.SetEnemyData(ey, ex, _dir, _enemy);
    }

    public void SetGimickData(int key, AlertFloor alertFloor)
    {
        gimm.SetGimickData(key, alertFloor);
    }

    public void SearchPlayer()
    {
        em.SearchPlayer();
    }

    public void EndMainGame()
    {
        print("‚¨‚í‚¨‚í‚è");
        GameState.Instance.SetState(false);
        SceneManager.LoadScene("ResultScene");
    }

    public void AlertSound(int _y, int _x)
    {
        gimm.AlertSound(_y, _x);
    }

    public void EnemyDestroy(int y, int x)
    {
        em.EnemyDestroy(y, x);
    }
}
