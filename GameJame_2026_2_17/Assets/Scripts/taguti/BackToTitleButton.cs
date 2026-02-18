using UnityEngine;

public class BackToTitleButton : MonoBehaviour
{
    [SerializeField] Gas gas;
    [SerializeField] string titleSceneName = "Title";

    public void OnClickBackToTitle()
    {
        gas.SendBestStageAndLoadTitle(titleSceneName);
    }
}
