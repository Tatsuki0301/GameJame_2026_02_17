using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] string SelectSceneName;
    [SerializeField]string GameSceneName;

    [SerializeField] Text text;
    public void OnClickBackToSelect()
    {
        SceneManager.LoadScene(SelectSceneName);
    }

    public void OnClickRetry()
    {
        //リトライしたいのでStage情報の書いてね。はーと.
        SceneManager.LoadScene(GameSceneName);
    }
    //クリアかゲームオーバーかテキスト変えるよー.
    public void SetResultText(bool isClear)
    {
        if (isClear)
        {
            text.text = "クリア！";
        }
        else
        {
            text.text = "ゲームオーバー";
        }
    }
}
