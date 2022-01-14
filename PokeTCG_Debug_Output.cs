/*
using System;
using System.Collections;
using System.Collections.Generic;
using Editor;
using UnityEditor;
using UnityEngine;

public class PokeTCG_Debug_Output : EditorWindow
{

    
    private static PokeTCG_Debug_Output _outWin;
    private string logStr = "";
    private bool isUpdate = false;
    
    
    //[MenuItem("Window/pokeGET/dLog")]
    public static  PokeTCG_Debug_Output Init()
    {
       //GetCallerEditor.ReqLogEvent += ReportEventReceived;

        // Get existing open window or if none, make a new one:
        _outWin = (PokeTCG_Debug_Output) EditorWindow.GetWindow(typeof(PokeTCG_Debug_Output));
        _outWin.Show();
        return _outWin;
    }

    static void logMessageReceived()
    {
        
    }
    private void OnGUI()
    {
        GUILayout.TextArea(logStr, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
          
    }

    public void Log(string str)
    {
        logStr += str;
        isUpdate = true;
    }
}
*/


