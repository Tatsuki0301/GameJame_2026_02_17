using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

// 0 50 +34 34
// 0 30 +14 14

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private float cameraMoveSpeed;

    private Vector2 minPos;

    private Vector2 maxPos;

    public void CameraStart(int xLenght, int yLenght)
    {
        float xMax, yMax;
        minPos = transform.position;
        if(yLenght < 9)
        {
            yMax = transform.position.y;
        }
        else
        {
            yMax = yLenght - 8;
        }

        if(xLenght < 17)
        {
            xMax = transform.position.x;
        }
        else
        {
            xMax = xLenght - 16 + 0.8f;
        }
        maxPos = new Vector2(xMax, yMax);
    }

    private void Update()
    {
        // 現在のゲームパッド情報
        var current = Gamepad.current;

        // ゲームパッド接続チェック
        if (current == null)
            return;

        // 左スティック入力取得
        var leftStickValue = (current.leftStick.ReadValue() * cameraMoveSpeed) * Time.deltaTime;
        transform.position = transform.position + new Vector3(leftStickValue.x, leftStickValue.y);
    }
}
