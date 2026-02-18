using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static UnityEngine.Mesh;

public class TitleManager : MonoBehaviour
{
    #region シングルトン（他のスクリプトからInstanceでアクセスできるようにする）
    public static TitleManager Instance { get; private set; }
    #endregion

    #region private変数

    #endregion


    #region Unityイベント関数
    void Awake()
    {
        //シングルトン管理
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //既にInstanceがあれば自分を破棄
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
