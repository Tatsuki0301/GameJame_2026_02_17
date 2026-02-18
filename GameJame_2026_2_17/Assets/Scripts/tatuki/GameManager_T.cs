using System.Collections.Generic;
using UnityEngine;

public class GameManager_T : MonoBehaviour
{
    private static GameManager_T gmInstance;

    private MapCreater mc;
    private PlayerManager pm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mc = GameObject.Find("MapCreater").gameObject.GetComponent<MapCreater>();
        pm = GameObject.Find("PlayerManager").gameObject.GetComponent<PlayerManager>();
    }

    public void StartAction(Queue<int> _actions)
    {
        pm.StartAction(_actions);
    }

    public void PMStart(Player player, int x, int y)
    {
        pm.PMStart(this, player, x, y);
    }

    public int GetMasValue(int y, int x)
    {
        return mc.Map[y, x];
    }
}
