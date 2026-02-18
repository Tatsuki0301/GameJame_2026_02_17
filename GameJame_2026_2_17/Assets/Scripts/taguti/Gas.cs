using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;


public class Gas : MonoBehaviour
{
    [SerializeField] string gasUrl;

    [SerializeField] InputField nameInput;
    [SerializeField] InputField pinInput;
    [SerializeField] Button loginButton;
    [SerializeField] Button registerButton;
    [SerializeField] Text messageText;

    [SerializeField] GameObject loginPanel;

    [SerializeField] string sceneName;

    void Start()
    {
        if (messageText) messageText.text = "";

        if (pinInput)
        {
            pinInput.contentType = InputField.ContentType.IntegerNumber;
            pinInput.characterLimit = 4;
        }

        loginButton.onClick.AddListener(() => StartCoroutine(OnClickLogin()));
        registerButton.onClick.AddListener(() => StartCoroutine(OnClickRegister()));
    }
    //Title画面のボタンをクリックしたらパネルを表示する
    public void ShowLoginPanel()
    {
        if (loginPanel) loginPanel.SetActive(true);
    }

    IEnumerator OnClickLogin()
    {
        var (name, pin, ok) = ReadInputs();
        if (!ok) yield break;

        SetInteractable(false);
        SetMessage("ログイン中...");

        string json = JsonUtility.ToJson(new LoginRequest { name = name, pin = pin });

        yield return PostJson(json, (respText) =>
        {
            if (string.IsNullOrEmpty(respText))
            {
                SetMessage("通信エラー");
                SetInteractable(true);
                return;
            }

            var res = JsonUtility.FromJson<LoginResponse>(respText);

            if (res.result != "OK")
            {
                SetMessage(res.message);
                SetInteractable(true);
                return;
            }

            if (res.status == "LOGGED_IN")
            {
                // ★ユーザー切替安全：SetLoginでリセットしてから反映
                Session.SetLogin(name, pin);
                Session.SetBestStage(res.bestScore);

                SceneManager.LoadScene(sceneName);
            }
            else if (res.status == "REGISTERED")
            {
                SetMessage("名前もしくはパスワードが間違っています。\n" +
                    "確認してください");
                SetInteractable(true);
            }
            else
            {
                SetMessage("不明なレスポンス");
                SetInteractable(true);
            }
        });
    }

    IEnumerator OnClickRegister()
    {
        var (name, pin, ok) = ReadInputs();
        if (!ok) yield break;

        SetInteractable(false);
        SetMessage("登録中...");

        string json = JsonUtility.ToJson(new LoginRequest { name = name, pin = pin });

        yield return PostJson(json, (respText) =>
        {
            if (string.IsNullOrEmpty(respText))
            {
                SetMessage("通信エラー");
                SetInteractable(true);
                return;
            }

            var res = JsonUtility.FromJson<LoginResponse>(respText);

            if (res.result != "OK")
            {
                if (res.message == "PIN mismatch")
                    SetMessage("この名前は既に使われてます");
                else
                    SetMessage("登録に失敗しました");

                SetInteractable(true);
                return;
            }

            if (res.status == "REGISTERED")
            {
                Session.SetLogin(name, pin);
                Session.SetBestStage(0); // 登録直後は0想定

                SceneManager.LoadScene(sceneName);
                // TODO: パネル非表示、ゲーム開始へ
            }
            else if (res.status == "LOGGED_IN")
            {
                SetMessage("登録済みです。『ログイン』を押してください。");
                SetInteractable(true);
            }
            else
            {
                SetMessage("不明なレスポンス");
                SetInteractable(true);
            }
        });
    }

    (string name, string pin, bool ok) ReadInputs()
    {
        string name = (nameInput ? nameInput.text : "").Trim();
        string pin = (pinInput ? pinInput.text : "").Trim();

        if (string.IsNullOrEmpty(name))
        {
            SetMessage("名前を入力してください");
            return (null, null, false);
        }

        if (!IsPin4(pin))
        {
            SetMessage("パスワードは4桁の数字にしてください");
            return (null, null, false);
        }

        return (name, pin, true);
    }

    bool IsPin4(string pin)
    {
        if (pin.Length != 4) return false;
        for (int i = 0; i < 4; i++)
            if (pin[i] < '0' || pin[i] > '9') return false;
        return true;
    }

    void SetMessage(string msg)
    {
        if (messageText) messageText.text = msg;
        Debug.Log(msg);
    }

    void SetInteractable(bool on)
    {
        if (loginButton) loginButton.interactable = on;
        if (registerButton) registerButton.interactable = on;
    }
    public void SendBestStageAndLoadTitle(string titleSceneName)
    {
        StartCoroutine(SendBestStageCoroutine(titleSceneName));
    }

    IEnumerator SendBestStageCoroutine(string titleSceneName)
    {
        // ログインしてなければ送らない
        if (!Session.IsLoggedIn)
        {
            SceneManager.LoadScene(titleSceneName);
            yield break;
        }

        int bestStageNo = Session.BestStageNo;

        // 送る必要がないならそのまま戻る
        if (bestStageNo <= 0)
        {
            SceneManager.LoadScene(titleSceneName);
            yield break;
        }

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
                Debug.LogError("bestStage送信失敗");
            }
            else
            {
                Debug.Log("bestStage送信完了: " + respText);
            }

            // ★送信完了後にタイトルへ戻る（ここ重要）
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
