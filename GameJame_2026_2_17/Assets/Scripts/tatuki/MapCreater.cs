using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

//x 2.5 y -2

public class MapCreater : MonoBehaviour
{
    [Header("生成するマップのファイル名")]
    [SerializeField]
    private string[] mapFileName;

    [Header("生成するオブジェクト※順番関係あるため注意")]
    [SerializeField]
    private GameObject[] createObjects;

    [SerializeField]
    private GameObject playerObj;

    private GameManager_T gm;
    private string mapDataPath = Application.dataPath + "/MapData/";

    private int[,] map;
    public int[,] Map
    {
        get { return map; }
        set { map = value; }
    }

    private Dictionary<int, ObjectDataStorage.ObjectData> objDic = new Dictionary<int, ObjectDataStorage.ObjectData>();
    private Vector2 startCreatePos = new Vector2(-8.39f, -4.50f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MapStart(GameManager_T _gm)
    {
        gm = _gm;
        SetMapData(3);
        MapCreate();
    }

    public void SetMapData(int mapNumber)
    {
        string paht = mapDataPath + mapFileName[mapNumber] + ".csv";
        if(File.Exists(paht))
        {
            string[] lines = File.ReadLines(paht).ToArray(); //ファイル内データを行に分割
            int len = lines.Length;
            map = new int[len, lines[0].Split(',').Length];

            for (int i = 0; i < len; i++)
            {
                string[] cells = lines[i].Split(","); //行の中身を分解
                for (int j = 0; j < cells.Length; j++)
                {
                    map[i, j] = int.Parse(cells[j]);
                }
            }

            string objPath = "Assets/Scripts/tatuki/ObjectDataStorage.asset"; //パス指定
            ObjectDataStorage obj = AssetDatabase.LoadAssetAtPath<ObjectDataStorage>(objPath);
            for (int i = 0; i < obj.objDatas.Count; i++)
            {
                objDic.Add(i + 2, obj.objDatas[i]);
            }
        }
        else
        {
            Debug.Log($"{mapFileName[mapNumber]}ファイルが見つかりませんでした。");
            Debug.Log($"{paht}ここには存在しません。");
            return;
        }
    }

    private void MapCreate()
    {
        ObjectDataStorage.ObjectData objData; //辞書からオブジェクトを取得

        GameObject parent = new GameObject("Map");
        GameObject createObj;
        Vector2 createPos;

        for (int y = map.GetLength(0) - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 0) continue;

                createPos = startCreatePos + new Vector2(1 * x, 1 * ((map.GetLength(0) - 1) - y));

                if (map[y, x] == 1)
                {
                    objData = objDic[2];
                    createObj = Instantiate(playerObj, createPos, Quaternion.identity);
                    gm.PMStart(createObj.GetComponent<Player>(), x, y);
                    createObj = Instantiate(objData.obj, createPos, objData.qua);
                    createObj.transform.parent = parent.transform;
                }
                else
                {
                    objData = objDic[map[y, x]];
                    createObj = Instantiate(objData.obj, createPos, objData.qua);
                    createObj.transform.parent = parent.transform;

                    switch (objData.value)
                    {
                        case 4:
                            gm.SetEnemyData(y, x, objData.qua.eulerAngles.z, createObj, objData.obj);
                            createObj = Instantiate(objDic[2].obj, createPos, Quaternion.identity);
                            createObj.transform.parent = parent.transform;
                            break;
                        case 5:
                            int key = gm.GetKeyValue(y, x);
                            gm.SetGimickData(key, createObj.GetComponent<AlertFloor>());
                            break;
                        default:
                            break;
                    }
                }

                map[y, x] = objData.value;
            }
        }

        Log(map);
    }

    public static void Log(int[,] map)
    {
        if (map == null)
        {
            Debug.Log("map is null");
            return;
        }

        int h = map.GetLength(0);
        int w = map.GetLength(1);

        var sb = new StringBuilder();

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                sb.Append(map[y, x]);
                if (x < w - 1) sb.Append(",");
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
}
