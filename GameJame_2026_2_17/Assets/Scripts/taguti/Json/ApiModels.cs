using System;

[System.Serializable]
public class ApiBaseResponse
{
    public string result;   // "OK" or "NG"
    public string message;  // NGの時に入ることが多い
}

[System.Serializable]
public class LoginResponse : ApiBaseResponse
{
    public string status;   // "REGISTERED" or "LOGGED_IN"
    public string name;
    public int bestScore;   // LOGGED_INのとき入る（未登録なら0でもOK）
}

[System.Serializable]
public class LoginRequest
{
    public string action = "registerOrLogin";
    public string name;
    public string pin;
}
[System.Serializable]
public class SaveBestStageRequest
{
    public string action = "saveBestStage";
    public string name;
    public string pin;
    public int bestStageNo;
}

[System.Serializable]
public class SaveBestStageResponse : ApiBaseResponse
{
    public bool updated;
    public int bestStageNo; // サーバ側に保存された値を返したいなら
}
