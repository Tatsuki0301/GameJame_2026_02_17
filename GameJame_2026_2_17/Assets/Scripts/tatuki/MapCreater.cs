using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

//x 2.5 y -2

public class MapCreater : MonoBehaviour
{
    [Header("生成するマップのファイル名")]
    [SerializeField]
    private string mapFileName;

    [Header("生成するオブジェクト※順番関係あるため注意")]
    [SerializeField]
    private GameObject[] createObjects;

    [SerializeField]
    private GameObject playerObj;

    private string mapDataPath = Application.dataPath + "/MapData/";

    private int[,] map;

    private Vector2 startCreatePos = new Vector2(-8.39f, -4.50f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMapData(mapFileName);
        MapCreate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMapData(string _mapFileName)
    {
        string paht = mapDataPath + _mapFileName + ".csv";
        if(File.Exists(paht))
        {
            string[] lines = File.ReadLines(paht).ToArray(); //ファイル内データを行に分割
            int len = lines.Length - 1;
            map = new int[len, lines[0].Split(',').Length];

            for (int i = 0; i < len; i++)
            {
                string[] cells = lines[i].Split(","); //行の中身を分解
                for (int j = 0; j < cells.Length; j++)
                {
                    map[i, j] = int.Parse(cells[j]);
                }
            }
        }
        else
        {
            Debug.Log($"{_mapFileName}ファイルが見つかりませんでした。");
            Debug.Log($"{paht}ここには存在しません。");
            return;
        }
    }

    private void MapCreate()
    {
        GameObject parent = new GameObject("Map");
        GameObject createObj;
        Vector2 createPos;

        for (int y = map.GetLength(0) - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                Debug.Log($"{y}, {x}");
                if (map[y, x] == 0) continue;

                createPos = startCreatePos + new Vector2(1 * x, 1 * ((map.GetLength(0) - 1) - y));

                if (map[y, x] == 1)
                {
                    Instantiate(playerObj, createPos, Quaternion.identity);
                    createObj = Instantiate(createObjects[0], createPos, Quaternion.identity);
                    createObj.transform.parent = parent.transform;
                }
                else
                {
                    createObj = Instantiate(createObjects[map[y, x] - 2], createPos, Quaternion.identity);
                    createObj.transform.parent = parent.transform;
                }
            }
        }
    }
}
