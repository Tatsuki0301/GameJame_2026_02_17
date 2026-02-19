using UnityEngine;

public class GameState : MonoBehaviour
{

    #region シングルトン（他のスクリプトからInstanceでアクセスできるようにする）
    public static GameState Instance { get; private set; }
    #endregion

    #region private変数
    private bool state;
    #endregion

    #region Set関数

    public void SetState(bool s)
    {
        state = s;
    }

    #endregion

    public bool GetState()
    {
        return state;
    }

    #region Unityイベント関数
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Start呼び出し関数

    #region 初期化
    void Init()
    {

    }
    #endregion

    #endregion

    #region Update呼び出し関数

    #endregion
}
