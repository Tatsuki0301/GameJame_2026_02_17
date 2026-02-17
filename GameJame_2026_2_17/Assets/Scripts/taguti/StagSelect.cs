using UnityEngine;
using UnityEngine.UI;

public class StagSelect : MonoBehaviour
{

    [System.Serializable]
    public class StageData
    {
        public string stageName;
        public string buttonLabel;

    }
    [Header("ステージデータ")]
    [SerializeField] StageData[] stages;
    [Header("UI参照")]
    [SerializeField] GameObject stageButton;
    [SerializeField] private RectTransform content;

    // 押された結果を保存する変数（どこからでも参照できる）
    public string selectStage;
    public string selectStageName;
    private void Start()
    {
        CreateSelectButton();
    }
    void CreateSelectButton()
    {
        foreach (var stage in stages)
        {
            GameObject buttonObj = Instantiate(stageButton, content);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = stage.buttonLabel;
            // ボタンが押されたときの処理を追加
            button.onClick.AddListener(() =>
            {
                selectStage = stage.stageName;
                selectStageName = stage.buttonLabel;
                Debug.Log("選択されたステージ: " + selectStage);
                Debug.Log("選択されたステージ名：" + selectStageName);
            });
        }
    }


}
