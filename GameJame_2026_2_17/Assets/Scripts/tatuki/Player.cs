using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.ComponentModel;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Sprite[] ps;

    public bool end = false;

    public enum PlayerDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    private int px, py;
    private int startX, startY;
    public int PX
    {
        get { return px; }
        set { px = value; }
    }

    public int PY
    {
        get { return py; }
        set { py = value; }
    }

    private Vector2 startPos;

    private PlayerManager pm;
    private PlayerDirection myDir;
    private PlayerDirection startDir;
    private Vector3[] addPos =
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0),
    };

    private SpriteRenderer sr;

    private Queue<GameObject> gameObjects = new Queue<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayerStart(PlayerManager _pm,  PlayerDirection dir, int x, int y)
    {
        pm = _pm;
        myDir = dir;
        startDir = dir;
        px = x;
        py = y;
        startPos = transform.position;
        startX = x;
        startY = y;

        Debug.Log($"{px}, {py}");

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = ps[(int)myDir];

        //初期角度設定
        switch (myDir)
        {
            case PlayerDirection.Right:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case PlayerDirection.Down:
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case PlayerDirection.Left:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
        }

    }

    public void ResetPlayer()
    {
        transform.position = startPos;
        px = startX;
        py = startY;
        myDir = startDir;

        //初期角度設定
        switch (myDir)
        {
            case PlayerDirection.Right:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case PlayerDirection.Down:
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            case PlayerDirection.Left:
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;
        }

        sr.sprite = ps[(int)myDir];
    }

    public async void StartAction(Queue<int> _actions, Queue<GameObject> _objQueue)
    {
        for(int i = _actions.Count; i > 0; i--)
        {
            if (end) break;
            switch (_actions.Dequeue())
            {
                case 0:
                    await WalkAction();
                    pm.SearchPlayer();
                    break;
                case 1:
                    RotationAction(1);
                    break;
                case 2:
                    RotationAction(-1);
                    break;
                case 3:
                    AtackAction();
                    break;
            }
            GameObject obj = _objQueue.Dequeue();
            Destroy(obj);
        }
        pm.ResetMap();
    }

    private async UniTask WalkAction()
    {
        int ax = System.Convert.ToInt32(addPos[(int)myDir].x);
        int ay = System.Convert.ToInt32(addPos[(int)myDir].y);

        int moveMasValue = pm.GetMasValue(py - ay, px + ax);

        //前に進めるかの判定&プレイヤーの2次元配列上の位置更新
        if (moveMasValue == 3) return;

        py -= ay;
        px += ax;

        //移動先の座標
        Vector2 movePos = new Vector2(transform.position.x + addPos[(int)myDir].x, transform.position.y + addPos[(int)myDir].y);
        await transform.DOMove(movePos, 1f).ToUniTask();

        if(moveMasValue == 5)
        {
            pm.AlertSound(py, px);
        }
        else if(moveMasValue == 6)
        {
            pm.EndMainGame();
        }
        else if(moveMasValue == 7)
        {
            print("クリア");
            GameState.Instance.SetState(true);
            SceneManager.LoadScene("ResultScene");
        }
    }

    private void RotationAction(int rotateDir)
    {
        ChengeDirection(rotateDir);
        sr.sprite = ps[(int)myDir];
    }

    /// <summary>
    /// プレイヤーの方向を変更
    /// 正なら右、負なら左回転 * 90
    /// </summary>
    /// <param name="_rotateDir"></param>
    private void ChengeDirection(int _rotateDir)
    {
        switch(myDir)
        {
            case PlayerDirection.Up:
                myDir = (_rotateDir > 0) ? PlayerDirection.Right : PlayerDirection.Left;
                break;
            case PlayerDirection.Right:
                myDir = (_rotateDir > 0) ? PlayerDirection.Down : PlayerDirection.Up;
                break;
            case PlayerDirection.Down:
                myDir = (_rotateDir > 0) ? PlayerDirection.Left : PlayerDirection.Right;
                break;
            case PlayerDirection.Left:
                myDir = (_rotateDir > 0) ? PlayerDirection.Up : PlayerDirection.Down;
                break;
        }
    }

    private void AtackAction()
    {
        Debug.Log(1);
        int ax = System.Convert.ToInt32(addPos[(int)myDir].x);
        int ay = System.Convert.ToInt32(addPos[(int)myDir].y);

        if(pm.GetMasValue(py - ay, px + ax) == 4)
        {
            Debug.Log(2);
            pm.EnemyDestroy(py - ay, px + ax);
        }
    }
}
