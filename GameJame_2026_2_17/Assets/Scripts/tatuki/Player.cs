using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Player : MonoBehaviour
{
    public enum PlayerDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    private int px, py;
    private PlayerManager pm;
    private PlayerDirection myDir;
    private Vector3[] addPos =
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0),
    };

    private Queue<GameObject> gameObjects = new Queue<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayerStart(PlayerManager _pm,  PlayerDirection dir, int x, int y)
    {
        pm = _pm;
        myDir = dir;
        px = x;
        py = y;

        Debug.Log($"{px}, {py}");

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

    public async void StartAction(Queue<int> _actions, Queue<GameObject> _objQueue)
    {
        for(int i = _actions.Count; i > 0; i--)
        {
            switch (_actions.Dequeue())
            {
                case 0:
                    await WalkAction();
                    break;
                case 1:
                    await RotationAction(1);
                    break;
                case 2:
                    await RotationAction(-1);
                    break;
            }
            GameObject obj = _objQueue.Dequeue();
            Destroy(obj);
        }
    }

    private async UniTask WalkAction()
    {
        //前に進めるかの判定&プレイヤーの2次元配列上の位置更新
        switch (myDir)
        {
            case PlayerDirection.Up:
                if (pm.GetMasValue(py - 1, px) == 3) return;
                Debug.Log(pm.GetMasValue(py - 1, px) + $" {py}, {px}");
                py--;
                break;
            case PlayerDirection.Right:
                if (pm.GetMasValue(py, px + 1) == 3) return;
                px++;
                break;
            case PlayerDirection.Down:
                if (pm.GetMasValue(py + 1, px) == 3) return;
                py++;
                break;
            case PlayerDirection.Left:
                if (pm.GetMasValue(py, px - 1) == 3) return;
                px--;
                break;
        }

        //移動先の座標
        Vector2 movePos = new Vector2(transform.position.x + addPos[(int)myDir].x, transform.position.y + addPos[(int)myDir].y);
        await transform.DOMove(movePos, 1f).ToUniTask();
    }

    private async UniTask RotationAction(int rotateDir)
    {
        ChengeDirection(rotateDir);
        await transform.DORotate(new Vector3(0f, 0f, 90f * -rotateDir), 0.5f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .ToUniTask();
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
}
