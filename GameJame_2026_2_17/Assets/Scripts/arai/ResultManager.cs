using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Text state;
    [SerializeField] string gasUrl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool isClear = GameState.Instance.GetState(); // クリアしたかどうか
        int currentStage = StageIndex.Instance.GetIndex(); // 今プレイしたステージ番号
        int bestStage = Session.BestStageNo; // これまでの最高記録

        if (isClear)
        {
            state.text = "ゲームクリア";

            //最高記録を更新しているかチェック
            if (currentStage > bestStage)
            {
                Debug.Log($"最高記録更新！ {bestStage} -> {currentStage}");

                //セッション（メモリ上）を更新
                Session.SetBestStage(currentStage);

                //端末（PlayerPrefs）を更新
                PlayerPrefs.SetInt("SavedBestStage", currentStage);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log("no");
            }
        }
        else
        {
            state.text = "ゲームオーバー";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushTitle(string titleSceneName)
    {
        StartCoroutine(SendBestStageCoroutine(titleSceneName));
    }

    IEnumerator SendBestStageCoroutine(string titleSceneName)
    {
        Debug.Log($"[Debug] 送信確認 - ログイン状態: {Session.IsLoggedIn}, 送信スコア: {Session.BestStageNo}");

        if (!Session.IsLoggedIn)
        {
            SceneManager.LoadScene(titleSceneName);
            yield break;
        }

        int bestStageNo = Session.BestStageNo;

        // デバッグログで今の値を確認してみる
        Debug.Log($"送信試行: Name={Session.Name}, Stage={bestStageNo}");


        if (bestStageNo <= 0)
        {
            Debug.Log("スコアが0以下のため送信をスキップします");
            SceneManager.LoadScene(titleSceneName);
            yield break;
        }

        //サーバーに送る前に、まず端末(PlayerPrefs)に保存しておく（確実性のため）
        PlayerPrefs.SetInt("SavedBestStage", bestStageNo);
        PlayerPrefs.Save();

        string json = JsonUtility.ToJson(new SaveBestStageRequest
        {
            name = Session.Name,
            pin = Session.Pin,
            bestStageNo = bestStageNo
        });

        yield return PostJson(json, (respText) =>
        {
            if (string.IsNullOrEmpty(respText))
            {
                Debug.LogError("bestStage送信失敗（レスポンス空）");
            }
            else
            {
                Debug.Log("bestStage送信完了: " + respText);
            }

            SceneManager.LoadScene(titleSceneName);
        });
    }

    IEnumerator PostJson(string json, System.Action<string> onDone)
    {
        var req = new UnityWebRequest(gasUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.error);
            onDone?.Invoke(null);
        }
        else
        {
            onDone?.Invoke(req.downloadHandler.text);
        }
    }
}

