using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonGenerate : MonoBehaviour
{
    #region 構造体・内部クラス
    [System.Serializable]
    public class StageData
    {
        public string stageName;   //シーン管理上の名前
        public string buttonLabel; //ボタンに表示するテキスト(例:第1話)
        public string buttonName;  //生成されたオブジェクトの名前
    }
    #endregion

    #region 変数定義
    [Header("ステージデータの設定")]
    [SerializeField] StageData[] stages;

    [Header("UIパーツの参照")]
    [SerializeField] GameObject stageButton;        //ボタンのプレハブ
    [SerializeField] private RectTransform content; //ボタンを並べる親オブジェクト

    private GameObject objctName;                   //クリックされたオブジェクトの一時保存用

    public string selectStage;
    public string selectStageName;
    #endregion

    private void Start()
    {
        //シーン開始時にボタンを動的に生成する
        //CreateSelectButton();
    }

    /// <summary>
    /// 外部から呼ぶボタン生成
    /// </summary>
    public void RefreshButtons()
    {
        //一度生成されたボタンをすべて消す
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        //最新の番号を参照してボタンを生成
        CreateSelectButton();
    }

    /// <summary>
    /// GASから取得した「最高到達ステージ」を元にボタンを生成・制御
    /// </summary>
    void CreateSelectButton()
    {
        //GAS経由でSessionに保存された最高クリアステージ番号を取得
        int clearedStage = Session.BestStageNo;
        //Debug.Log("判定に使用するクリアステージ数:" + clearedStage);

        for (int i = 0; i < stages.Length; i++)
        {
            var stage = stages[i];
            //インデックス0をチュートリアルとして扱う、iをそのままステージ番号にする
            int currentStageNum = i;

            //ボタンを生成してScrollViewのContent内に配置
            GameObject buttonObj = Instantiate(stageButton, content);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            //ボタン名とラベルを設定
            buttonObj.name = stage.buttonName;
            buttonText.text = stage.buttonLabel;

            //ステージ解放ロジック
            //クリア済みのステージ+1(次の未クリアステージ)までを有効化する
            bool isUnlocked = currentStageNum <= (clearedStage + 1);

            if (isUnlocked)
            {
                //解放済み：ボタンを有効にし、クリック時の遷移処理を登録
                button.interactable = true;
                button.onClick.AddListener(() =>
                {
                    //シングルトンにステージ番号を保存
                    StageIndex.Instance.SetIndex(currentStageNum);
                    GameStart();
                });
            }
            else
            {
                //未開放：ボタンを無効化し、見た目を「ロック状態」に変更
                button.interactable = false;
                buttonText.text = "???";
                buttonText.color = Color.white;
            }
        }
    }

    #region シーン遷移処理
    private void GameStart()
    {
        //現在クリックされたボタンのオブジェクトを取得
        objctName = EventSystem.current.currentSelectedGameObject;
        if (objctName == null) return;

        string name = objctName.name;
        //ボタンの名前が"Stage"で始まる場合、その後の数値を抽出
        if (name.StartsWith("Stage"))
        {
            string numberPart = name.Replace("Stage", "");
            if (int.TryParse(numberPart, out int number))
            {
                //抽出した番号をセットしてコルーチンでロード開始
                StageIndex.Instance.SetIndex(number);
                StartCoroutine(StageLoad());
            }
        }
    }

    IEnumerator StageLoad()
    {
        //0.5秒待機(SEなどの再生猶予)
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }
    #endregion
}