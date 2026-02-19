using System.Collections.Generic;
using UnityEngine;

public class EnemyManager_T : MonoBehaviour
{
    private Dictionary<int, EnemyBase> enemyDic;
    private GameManager_T gm;

    public void EMStart(GameManager_T _gm)
    {
        gm = _gm;
        enemyDic = new Dictionary<int, EnemyBase>();
    }

    public void SetEnemyData(int ey, int ex, float _dir, EnemyBase _enemy)
    {
        int key = gm.GetKeyValue(ey, ex);
        EnemyBase.EnemyDir enemyDir;

        int dir = System.Convert.ToInt32(_dir);

        switch (dir)
        {
            case 0:
                enemyDir = EnemyBase.EnemyDir.Up;
                break;
            case 270:
                enemyDir = EnemyBase.EnemyDir.Right;
                break;
            case 180:
                enemyDir = EnemyBase.EnemyDir.Down;
                break;
            case 90:
                enemyDir = EnemyBase.EnemyDir.Left;
                break;
            default:
                enemyDir = EnemyBase.EnemyDir.Up;
                break;
        }

        enemyDic.Add(key, _enemy);
        _enemy.SetMyTransform(ey, ex, enemyDir, this);
    }

    public void SearchPlayer()
    {
        foreach(var e in enemyDic)
        {
            e.Value.SearchPlayer();
        }
    }

    public int[] GetPlayerPos()
    {
        return gm.GetPlayerPos();
    }

    public void EndMainGame()
    {
        gm.EndMainGame();
    }

    public int GetMasValue(int y, int x)
    {
        return gm.GetMasValue(y, x);
    }

    public void EnemyDestroy(int y, int x)
    {
        int key = gm.GetKeyValue(y, x);
        enemyDic[key].DeadEnemy();
        enemyDic.Remove(key);
    }
}
