using UnityEngine;

public static class Session
{
    public static string Name { get; private set; } = "";
    public static string Pin { get; private set; } = "";
    public static int BestStageNo { get; private set; } = 0;

    public static bool IsLoggedIn => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Pin);

    public static void SetLogin(string name, string pin)
    {
        if (name == null)
        {
            Name = "";
        }
        else
        {
            Name = name;
        }

        if (pin == null)
        {
            Pin = "";
        }
        else
        {
            Pin = pin;
        }

        BestStageNo = 0;
    }

    public static void SetBestStage(int bestStageNo)
    {
        BestStageNo = Mathf.Max(BestStageNo, bestStageNo);
    }

    public static void Clear()
    {
        Name = "";
        Pin = "";
        BestStageNo = 0;
    }
}
