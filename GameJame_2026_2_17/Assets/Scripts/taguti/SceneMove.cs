using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    //Scene移動用のスクリプト
    [SerializeField]
    private string[] sceneNames;

    //ClickされたらsceneNames[0]に移動する
    public void MoveTitleOnClick()
    {
       //タイトルシーンに移動する
        SceneManager.LoadScene(sceneNames[0]);
    }
    //Clickされたらタイトルシーンに移動する
    public void MoveOnClick()
    {
        //ゲームシーンに移動する
        SceneManager.LoadScene(sceneNames[1]);
    }
}
