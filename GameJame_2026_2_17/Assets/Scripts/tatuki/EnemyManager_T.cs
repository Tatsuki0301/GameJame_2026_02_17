using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class EnemyManager_T : MonoBehaviour
{
    private struct EnemySettings
    {
        public int y, x;
        public GameObject obj;
        public Vector2 pos;
        public Quaternion qua;
    }

    private List<int> keys = new List<int>();

    private Dictionary<int, EnemyBase> enemyDic;
    private Dictionary<int, EnemySettings> startDic;
    private GameManager_T gm;
    

    public void EMStart(GameManager_T _gm)
    {
        gm = _gm;
        enemyDic = new Dictionary<int, EnemyBase>();
        startDic = new Dictionary<int, EnemySettings>();
    }

    public void SetEnemyData(int ey, int ex, float _dir, GameObject obj, GameObject createObj)
    {
        EnemyBase enemy = obj.GetComponent<EnemyBase>();
        int key = gm.GetKeyValue(ey, ex);
        EnemySettings setting = new EnemySettings()
        {
            y = ey,
            x = ex,
            obj = createObj,
            pos = obj.transform.position,
            qua = obj.transform.rotation
        };
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

        keys.Add(key);
        enemyDic.Add(key, enemy);
        startDic.Add(key, setting);
        enemy.SetMyTransform(ey, ex, enemyDir, this);
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

    public void ResetEnemy()
    {
        foreach (var key in keys)
        {
            if(!enemyDic.ContainsKey(key))
            {
                GameObject obj = Instantiate(startDic[key].obj, startDic[key].pos, startDic[key].qua);
                EnemyBase enemy = obj.GetComponent<EnemyBase>();

                int dir = System.Convert.ToInt32(startDic[key].qua.eulerAngles.z);
                EnemyDir enemyDir;

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

                enemyDic.Add(key, enemy);
                enemy.SetMyTransform(startDic[key].y, startDic[key].x, enemyDir, this);
            }
        }
    }
}
