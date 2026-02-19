using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyDir
    {
        Up,
        Right,
        Down,
        Left,
    }

    protected int[] myPos = new int[2];
    protected int[] playerPos;
    protected EnemyDir myDir;
    protected EnemyManager_T em;

    public void SetMyTransform(int _y, int _x, EnemyDir _dir, EnemyManager_T _em)
    {
        em = _em;
        myPos[0] = _y;
        myPos[1] = _x;
        myDir = _dir;
    }

    public void DeadEnemy()
    {
        Debug.Log(4);
        Destroy(gameObject);
    }

    virtual public void SearchPlayer()
    {
        playerPos = em.GetPlayerPos();
    }

    protected void HitPlayer()
    {
        em.EndMainGame();
    }
}
