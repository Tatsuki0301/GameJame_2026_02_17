using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject content;

    [SerializeField]
    private GameObject test;

    private Queue<int> actionQueue = new Queue<int>();
    private GameManager_T gm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager_T>();
    }

    public void PushActionButton(int actionNumber)
    {
        actionQueue.Enqueue(actionNumber);
        GameObject createObj = Instantiate(test);
        createObj.transform.parent = content.transform;
        createObj.transform.localPosition = new Vector3(createObj.transform.localPosition.x, createObj.transform.localPosition.y, 0);
        createObj.transform.localScale = Vector3.one;
        if(actionNumber == 0)
        {
            createObj.GetComponent<Image>().color = Color.red;
        }
        else if(actionNumber == 1)
        {
            createObj.GetComponent<Image>().color = Color.blue;
        }
    }

    public void PushStartButton()
    {
        gm.StartAction(actionQueue);
    }

    public void PushActionResetButton()
    {
        actionQueue = new Queue<int>();
    }
}
