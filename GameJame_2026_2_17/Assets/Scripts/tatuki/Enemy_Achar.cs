using UnityEngine;

public class Enemy_Achar : EnemyBase
{
    public override void SearchPlayer()
    {
        base.SearchPlayer();

        int indexDir;

        switch (myDir)
        {
            case EnemyDir.Up:
                indexDir = -1;
                SearchVer(indexDir);
                break;
            case EnemyDir.Right:
                indexDir = 1;
                SearchHor(indexDir);
                break;
            case EnemyDir.Down:
                indexDir = 1;
                SearchVer(indexDir);
                break;
            case EnemyDir.Left:
                indexDir = -1;
                SearchHor(indexDir);
                break;
            default:
                break;
        }
    }

    private void SearchHor(int _indexDir)
    {
        if (playerPos[0] != myPos[0]) return;
        int curMas = myPos[1] + _indexDir;
        for(int i = 0; i < 4; i++)
        {
            if (em.GetMasValue(myPos[0], curMas) == 3) break;
            else if (playerPos[1] == curMas)
            {
                HitPlayer();
                break;
            }
            curMas += _indexDir;
        }
    }

    private void SearchVer(int _indexDir)
    {
        if (playerPos[1] != myPos[1]) return;
        int curMas = myPos[0] + _indexDir;
        for (int i = 0; i < 4; i++)
        {
            if (em.GetMasValue(curMas, myPos[1]) == 3) break;
            else if (playerPos[0] == curMas)
            {
                HitPlayer();
                break;
            }
            curMas += _indexDir;
        }
    }
}
