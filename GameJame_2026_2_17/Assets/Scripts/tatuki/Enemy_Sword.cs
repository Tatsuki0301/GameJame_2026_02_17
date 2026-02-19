using UnityEngine;

public class Enemy_Sword : EnemyBase
{
    public override void SearchPlayer()
    {
        base.SearchPlayer();

        int difY = myPos[0] - playerPos[0];
        int difX = myPos[1] - playerPos[1];

        int hitLenghMaX;
        int hitLenghMin;

        switch (myDir)
        {
            case EnemyDir.Up:
                hitLenghMaX = 1;
                hitLenghMin = 0;
                SearchVer(difY, difX, hitLenghMaX, hitLenghMin);
                break;
            case EnemyDir.Right:
                hitLenghMaX = 1;
                hitLenghMin = 0;
                SearchHor(difY, difX, hitLenghMaX, hitLenghMin);
                break;
            case EnemyDir.Down:
                hitLenghMaX =  0;
                hitLenghMin = -1;
                SearchVer(difY, difX, hitLenghMaX, hitLenghMin);
                break;
            case EnemyDir.Left:
                hitLenghMaX =  0;
                hitLenghMin = -1;
                SearchHor(difY, difX, hitLenghMaX, hitLenghMin);
                break;
            default:
                break;
        }
    }

    private void SearchHor(int _difY, int _difX, int _hitLenghtMax, int _hitLenghtMin)
    {
        _difY = Mathf.Abs(_difY);
        if (_difY < 2 && _hitLenghtMin <= _difX && _difX <= _hitLenghtMax)
        {
            HitPlayer();
        }
    }

    private void SearchVer(int _difY, int _difX, int _hitLenghtMax, int _hitLenghtMin)
    {
        _difX = Mathf.Abs(_difX);
        if (_hitLenghtMin <= _difY && _difY <= _hitLenghtMax && _difX < 2)
        {
            HitPlayer();
        }
    }
}
