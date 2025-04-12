using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A class for calling Logs for debugging in the unity engine
//These calls won't be made in the exported game for players
public class GameLog : MonoBehaviour
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Message(string text)
    {
        Debug.Log(text);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Warning(string text)
    {
        Debug.LogWarning(text);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Error(string text)
    {
        Debug.LogError(text);
    }

}
