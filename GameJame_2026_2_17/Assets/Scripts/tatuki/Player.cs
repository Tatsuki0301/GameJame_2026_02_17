using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Player : MonoBehaviour
{
    private enum PlayerDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    private PlayerDirection myDir;
    private Vector3[] addPos =
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0),
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myDir = 0;
        WalkAction();
    }

    public void StartAction(Queue<int> _actions)
    {
        for(int i = _actions.Count; i >= 0; i--)
        {
            switch (_actions.Dequeue())
            {
                case 0:
                    break;
            }
        }
    }

    private void WalkAction()
    {
        Vector2 movePos = new Vector2(transform.position.x + addPos[(int)myDir].x, transform.position.y + addPos[(int)myDir].y);
        transform.DOMove((movePos), 10f);
        Debug.Log("a");
    }
}
