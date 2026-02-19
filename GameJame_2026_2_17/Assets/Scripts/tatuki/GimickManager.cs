using System.Collections.Generic;
using UnityEngine;

public class GimickManager : MonoBehaviour
{
    private GameManager_T gm;
    private Dictionary<int, AlertFloor> gimDIc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GimMStart(GameManager_T _gm)
    {
        gm = _gm;
        gimDIc = new Dictionary<int, AlertFloor>();
    }

    public void SetGimickData(int key, AlertFloor alertFloor)
    {
        gimDIc.Add(key, alertFloor);
    }

    public int GetMasValue(int y, int x)
    {
        return gm.GetMasValue(y, x);
    }

    public void EndMainGame()
    {
        gm.EndMainGame();
    }

    public void AlertSound(int _y, int _x)
    {
        int key = gm.GetKeyValue(_y, _x);
        gimDIc[key].AlertSound(this, _y, _x);
    }
}
