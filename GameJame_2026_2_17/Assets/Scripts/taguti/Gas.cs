using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class Gas : MonoBehaviour
{
    #region 定数
    //PlayerPrefsで使用するキー名（打ち間違い防止用）
    private const string KEY_LOGGED_IN = "IsLoggedIn";
    private const string KEY_NAME = "SavedName";
    private const string KEY_PIN = "SavedPin";
    private const string KEY_BEST_STAGE = "SavedBestStage";

    #endregion

    #region private変数
    [SerializeField] string gasUrl;

    [SerializeField] InputField nameInput;
    [SerializeField] InputField pinInput;
    [SerializeField] Button loginButton;
    [SerializeField] Button registerButton;
    [SerializeField] Text messageText;
    [SerializeField] GameObject titlePanel;

    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject selectPanel;

    [SerializeField] string sceneName;

    #endregion

    #region Unityイベント関数
    void Start()
    {
        if (messageText) messageText.text = "";

        if (pinInput)
        {
            pinInput.contentType = InputField.ContentType.IntegerNumber;
            pinInput.characterLimit = 4;
        }

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(() => StartCoroutine(OnClickRegister()));

        //自動ログインチェック
        CheckAutoLogin();
    }

    #endregion

    /// <summary>
    /// 保存されたログイン情報があれば自動でステージ選択へ飛ばす
    /// </summary>
    void CheckAutoLogin()
    {
        if (PlayerPrefs.GetInt(KEY_LOGGED_IN, 0) == 1)
        {
            string savedName = PlayerPrefs.GetString(KEY_NAME, "");
            string savedPin = PlayerPrefs.GetString(KEY_PIN, "");
            int savedBest = PlayerPrefs.GetInt(KEY_BEST_STAGE, 0);

            //セッションにデータを復元
            Session.SetLogin(savedName, savedPin);
            Session.SetBestStage(savedBest);

            //UIをステージ選択画面に切り替え
            titlePanel.SetActive(false);
            loginPanel.SetActive(false);
            selectPanel.SetActive(true);

            //ボタンの表示更新
            ButtonGenerate bg = FindFirstObjectByType<ButtonGenerate>();
            if (bg != null) bg.RefreshButtons();

            Debug.Log($"自動ログイン完了: {savedName}");
        }
    }

    /// <summary>
    /// ログアウト処理（ログアウトボタンにこれを割り当てる）
    /// </summary>
    public void Logout()
    {
        SoundManager.Instance.SePlay(1);

        //保存データの削除
        PlayerPrefs.SetInt(KEY_LOGGED_IN, 0);
        PlayerPrefs.DeleteKey(KEY_NAME);
        PlayerPrefs.DeleteKey(KEY_PIN);
        PlayerPrefs.DeleteKey(KEY_BEST_STAGE);
        PlayerPrefs.Save();

        //ボタンを入力可能に
        SetInteractable(true);

        //入力されているテキストを削除
        nameInput.text = "";
        pinInput.text = "";
        messageText.text = "";

        //UIをタイトルに戻す
        selectPanel.SetActive(false);
        titlePanel.SetActive(true);
        //if (messageText) messageText.text = "ログアウトしました";

        //Debug.Log("ログアウト完了");
    }

    //Title画面のボタンをクリックしたらパネルを表示する
    public void ShowLoginPanel()
    {
        SoundManager.Instance.SePlay(0);

        titlePanel.SetActive(false);

        if (loginPanel)
        {
            loginPanel.SetActive(true);
        }
    }

    /// <summary>
    /// タイトルへ戻る
    /// </summary>
    public void BackTitle()
    {
        SoundManager.Instance.SePlay(1);

        titlePanel.SetActive(true);
        loginPanel.SetActive(false);
        selectPanel.SetActive(false);
    }

    /// <summary>
    /// 中継用のメソッドを作る
    /// </summary>
    public void OnLoginButtonClicked()
    {
        StartCoroutine(OnClickLogin());
    }

    /// <summary>
    /// 通信開始時に呼ぶアニメーション
    /// </summary>
    /// <param name="targetText">アニメーションさせたいテキスト</param>
    /// <param name="baseMessage">表示したい固定文字（例：ログイン中）</param>
    void StartLoadingAnim(Text targetText, string baseMessage)
    {
        //最初に最大幅（...込み）をセットしてリッチテキストを有効化
        targetText.supportRichText = true;

        int dotCount = 0;

        //DOVirtual.DelayedCall を使って 0.5秒おきに呼び出し
        //最後の引数(false)をtrueにすると無限ループ
        Sequence seq = DOTween.Sequence().SetId("LoadingDots"); //IDをセット

        //0.5秒待ってからドットを更新する処理をループさせる
        seq.AppendCallback(() => {
            dotCount = (dotCount + 1) % 4; // 0, 1, 2, 3 の繰り返し

            string visibleDots = new string('.', dotCount);
            string invisibleDots = new string('.', 3 - dotCount);

            //透明なドットを混ぜて全体の幅を維持
            targetText.text = $"{baseMessage}{visibleDots}<color=#00000000>{invisibleDots}</color>";
        });
        seq.AppendInterval(0.5f);
        seq.SetLoops(-1); //無限ループ
    }

    /// <summary>
    /// 通信終了時に止める
    /// </summary>
    void StopLoadingAnim()
    {
        //IDで指定してKill
        DOTween.Kill("LoadingDots");
    }


    /// <summary>
    /// 端末にログイン情報を保存する
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="pin">パスワード</param>
    /// <param name="bestScore">最高解放ステージ</param>
    void SaveLoginInfo(string name, string pin, int bestScore)
    {
        PlayerPrefs.SetInt(KEY_LOGGED_IN, 1);
        PlayerPrefs.SetString(KEY_NAME, name);
        PlayerPrefs.SetString(KEY_PIN, pin);
        PlayerPrefs.SetInt(KEY_BEST_STAGE, bestScore);
        PlayerPrefs.Save(); //確実に書き込む
    }


    IEnumerator OnClickLogin()
    {
        var (name, pin, ok) = ReadInputs();
        if (!ok) yield break;

        SetInteractable(false); //通信中はボタン連打を防止
        
        //ログイン中のアニメーション開始
        StartLoadingAnim(messageText, "ログイン中");

        //ログイン情報をJSON形式に変換
        string json = JsonUtility.ToJson(new LoginRequest { name = name, pin = pin });

        //GASへPOST送信
        yield return PostJson(json, (respText) =>
        {
            StopLoadingAnim(); //レスポンスが来たら止める

            if (string.IsNullOrEmpty(respText))
            {
                SetMessage("通信エラー");
                SetInteractable(true);
                return;
            }

            //サーバーからの返答(JSON)をクラスにデコード
            var res = JsonUtility.FromJson<LoginResponse>(respText);
            Debug.Log("サーバーから受信したクリアステージ数: " + res.bestScore);

            if (res.result == "OK" && res.status == "LOGGED_IN")
            {
                //ログイン成功時に情報を保存
                SaveLoginInfo(name, pin, res.bestScore);

                //サーバーから受け取った「最高クリアステージ」をSessionに保存
                Session.SetLogin(name, pin);
                Session.SetBestStage(res.bestScore);

                //ButtonGenerateスクリプトを探す
                ButtonGenerate bg = FindFirstObjectByType<ButtonGenerate>();

                if (bg != null)
                {
                    //ログイン後の最新データを使ってボタンを生成・更新する
                    bg.RefreshButtons();
                }

                selectPanel.SetActive(true); //ステージ選択画面を表示
                loginPanel.SetActive(false); //ログイン画面を隠す
            }
            else
            {
                SetMessage(res.message);
                SetInteractable(true);
            }
        });
    }

    IEnumerator OnClickRegister()
    {
        var (name, pin, ok) = ReadInputs();
        if (!ok) yield break;

        SetInteractable(false);

        //登録中のアニメーション開始
        StartLoadingAnim(messageText, "登録中");

        string json = JsonUtility.ToJson(new LoginRequest { name = name, pin = pin });

        yield return PostJson(json, (respText) =>
        {
            StopLoadingAnim(); //レスポンスが来たら止める

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
                //登録成功時も情報を保存
                SaveLoginInfo(name, pin, 0);

                Session.SetLogin(name, pin);
                Session.SetBestStage(0); // 登録直後は0想定

                //ButtonGenerateスクリプトを探す
                ButtonGenerate bg = FindFirstObjectByType<ButtonGenerate>();

                if (bg != null)
                {
                    //ログイン後の最新データを使ってボタンを生成・更新する
                    bg.RefreshButtons();
                }

                selectPanel.SetActive(true);
                loginPanel.SetActive(false);
                //SceneManager.LoadScene(sceneName);
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
