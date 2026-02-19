using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region シングルトン
    public static SoundManager Instance { get; private set; }
    #endregion

    #region private変数

    #region BGM
    [Header("BGM用オーディオソースセット")]
    [SerializeField] private AudioSource bgmSource;
    [Header("BGM用オーディオ（本体）セット")]
    [SerializeField] private AudioClip[] bgmClip;
    #endregion

    #region SE
    [Header("SE用オーディオソースセット")]
    [SerializeField] private AudioSource seSource;
    [Header("SE用オーディオ（本体）セット")]
    [SerializeField] private AudioClip[] seClip;
    #endregion

    private int number;

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

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    #endregion

    #region Start呼び出し関数
    void Init()
    {
        //number = StageIndex.Instance.GetIndex();
        //number = 1;
        //BgmPlay(number);
    }
    #endregion

    public void BgmPlay(int number)
    {
        //bgmClip（配列名）が null じゃないか、指定された番号が範囲内かを確認
        if (bgmClip == null || number < 0 || number >= bgmClip.Length)
        {
            Debug.LogWarning($"BGM番号 {number} は範囲外か、リストが設定されていません！");
            return;
        }

        //再生処理
        bgmSource.clip = bgmClip[number];
        bgmSource.Play();
    }

    public void BgmStop()
    {
        bgmSource.Stop();
    }

    public void SePlay(int number)
    {
        seSource.PlayOneShot(seClip[number]);
    }
}
