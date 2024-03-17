using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using Script.Functions;
using UnityEngine;

public class Battalion : MonoBehaviour
{
    public BattalionData BattalionData { get; set; }
    public List<(int X, int Y)> BattalionXY { get; set; }

    private void Awake()
    {
        if (BattalionData is null)
        {
            BattalionData =
                new BattalionData(gameObject, 111, BattalionData.BattalionTypes.Infantry, BattalionData.Dirs.Up);
        }
    }

    private string test()
    {
        string str="坐标如下：\n";
        foreach (var XYs in BattalionXY)
        {
            str = str+XYs.X + "," + XYs.Y + "\n";
        }

        return str;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Functions.CreateTip(test(),gameObject.transform.position,5);
        }
        
    }
    
}
