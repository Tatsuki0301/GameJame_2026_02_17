using UnityEngine;

public class AlertFloor : MonoBehaviour
{
    public void AlertSound(GimickManager gimM, int _y, int _x)
    {
        int cy;
        int cx;

        for(int y = 0; y < 5; y++)
        {
            for(int x = 0; x < 5; x++)
            {
                cy = _y + (-2 + y);
                cx = _x + (-2 + x);
                if(gimM.GetMasValue(cy, cx) == 4)
                {
                    gimM.EndMainGame();
                }
            }
        }
    }
}
