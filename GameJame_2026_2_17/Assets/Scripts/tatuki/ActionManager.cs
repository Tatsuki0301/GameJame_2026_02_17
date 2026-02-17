using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private Queue<int> actionQueue = new Queue<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void PushActionButton(int actionNumber)
    {
        actionQueue.Enqueue(actionNumber);
    }

    public void PushActionResetButton()
    {
        actionQueue = new Queue<int>();
    }
}
