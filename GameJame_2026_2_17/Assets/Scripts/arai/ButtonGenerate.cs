using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonGenerate : MonoBehaviour
{
    #region 構造体
    [System.Serializable]
    public class StageData
    {
        public string stageName;
        public string buttonLabel;
        public string buttonName;
    }

    #endregion

    #region private変数
    [Header("ステージデータ")]
    [SerializeField] StageData[] stages;
    [Header("UI参照")]
    [SerializeField] GameObject stageButton;
    [SerializeField] private RectTransform content;

    private GameObject objctName; //オブジェクト名

    // 押された結果を保存する変数（どこからでも参照できる）
    public string selectStage;
    public string selectStageName;

    #endregion

    #region Unityイベント関数
    private void Start()
    {
        CreateSelectButton();
    }

    #endregion

    #region Start呼び出し関数
    /// <summary>
    /// ボタン生成処理とシーンロード
    /// </summary>
    void CreateSelectButton()
    {
        foreach (var stage in stages)
        {
            GameObject buttonObj = Instantiate(stageButton, content);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonObj.name = stage.buttonName;
            buttonText.text = stage.buttonLabel;
            // ボタンが押されたときの処理を追加
            button.onClick.AddListener(() =>
            {
                /*                selectStage = stage.stageName;
                                selectStageName = stage.buttonLabel;
                                Debug.Log("選択されたステージ: " + selectStage);
                                Debug.Log("選択されたステージ名：" + selectStageName);*/

                GameStart();
            });
        }
    }

    #region タイトルでステージの何かが押された時
    /// <summary>
    /// ステージ番号を保存してゲームシーンをロード
    /// </summary>
    private void GameStart()
    {
        objctName = EventSystem.current.currentSelectedGameObject;
        string name = objctName.name;

        //ステージ番号に変換
        if (name.StartsWith("Stage"))
        {
            string numberPart = name.Replace("Stage", "");

            //TryParseで安全に整数に変換
            if (int.TryParse(numberPart, out int number))
            {
                StageIndex.Instance.SetIndex(number); //選択されたステージ番号を保存
                StartCoroutine(StageLoad());          //シーンロード
            }
            else
            {
                //Debug.LogWarning("ステージ名に数値が含まれていません: " + name);

                StartCoroutine(TextCountDown());
            }
        }
    }

    /// <summary>
    /// ゲームシーン読み込み
    /// </summary>
    /// <returns></returns>
    IEnumerator StageLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator TextCountDown()
    {
        yield return new WaitForSeconds(1.0f); //1秒待つ
    }
    #endregion

    #endregion
}
