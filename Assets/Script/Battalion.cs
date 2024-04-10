using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using Script.Functions;
using UnityEngine;
using Types = Script.BattalionData.BattalionTypes;

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
        string str = "坐标如下：\n";
        foreach (var XYs in BattalionXY)
        {
            str = str + XYs.X + "," + XYs.Y + "\n";
        }

        return str;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string debugtext = "属性：\n";
            debugtext += "Hp:" + BattalionData.Hp + " + " + BattalionData.Hp * (BattalionData._Hp - 1) + "\n";
            debugtext += "Op:" + BattalionData.Op + " + " + BattalionData.Op * (BattalionData._Op - 1) + "\n";
            debugtext += "ReOp:" + BattalionData.ReOp + " + " + BattalionData.ReOp * (BattalionData._ReOp - 1) + "\n";
            debugtext += "Att:" + BattalionData.Att + " + " + BattalionData.Att * (BattalionData._Att - 1) + "\n";
            debugtext += "Def:" + BattalionData.Def + " + " + BattalionData.Def * (BattalionData._Def - 1) + "\n";
            debugtext += "BuffList:\n";
            foreach (var variablBuff in BattalionData.BuffList)
            {
                debugtext += variablBuff.funName + "\n";
            }

            Functions.CreateTip(debugtext, gameObject.transform.position, 5);
        }
    }

    /// <summary>
    /// 检测并获取自身可获得的动态buff
    /// </summary>
    public void LoadBuff()
    {
        string debugtext = "buff来源：\n";
        List<GameObject> gameObjects =  gameObject.transform.parent.parent.Find("GridManger").GetComponent<GridManger>().DetectAdjacentCellProperties(gameObject);
        BattalionData.BuffList.Clear();
        foreach (var o in gameObjects)
        {
            string Buff = "Buff_";
            switch (BattalionData.BattalionType)
            {
                case Types.Infantry:
                    Buff += "Infantry_";
                    break;
                case Types.Artillery:
                    Buff += "Artillery_";
                    break;
                case Types.Armor:
                    Buff += "Armor_";
                    break;
            }

            switch (o.GetComponent<Battalion>().BattalionData.BattalionType)
            {
                case Types.Infantry:
                    Buff += "Infantry";
                    break;
                case Types.Artillery:
                    Buff += "Artillery";
                    break;
                case Types.Armor:
                    Buff += "Armor";
                    break;
            }

            BattalionData.BuffList.Add(new Buff(Buff));
            debugtext += "由" + o.GetComponent<Battalion>().BattalionData.BattalionName + "获得" + Buff + "\n";
        }

        Functions.CreateTip(debugtext, gameObject.transform.position, 5);
    }
}