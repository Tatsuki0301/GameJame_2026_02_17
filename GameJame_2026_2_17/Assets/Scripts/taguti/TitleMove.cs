using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMove: MonoBehaviour
{
    [SerializeField] Gas gas;
    [SerializeField] string titleSceneName = "Title";

    public void OnClickBackToTitle()
    {
        gas.SendBestStageAndLoadTitle(titleSceneName);
    }
}
