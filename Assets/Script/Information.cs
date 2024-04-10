using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class Information : MonoBehaviour
{
    private GameObject BattalionList;
    private int ListLength;

    private void Start()
    {
        BattalionList = gameObject.transform.parent.Find("BattalionList").gameObject;
    }

    private void Update()
    {
        if (ListLength == BattalionList.transform.childCount)
        {
            return;
        }

        Double HP = 0;
        Double OP = 0;
        Double ReOP = 0;
        Double ATT = 0;
        Double DEF = 0;

        foreach (Transform o in BattalionList.transform)
        {
            Battalion b = o.GetComponent<Battalion>();
            b.LoadBuff();
            BattalionData bd = b.BattalionData;
            bd.InitializeData();

            //执行buff效果
            foreach (var nBuff in bd.BuffList)
            {
                nBuff.Run(new[] { bd });
            }

            HP += bd.Hp *  bd._Hp;
            OP += bd.Op *  bd._Op;
            ReOP += bd.ReOp *  bd._ReOp;
            ATT += bd.Att *  bd._Att;
            DEF += bd.Def * bd._Def;

            ListLength = BattalionList.transform.childCount;
        }

        this.GetComponent<Text>().text = "HP:" + HP + ";OP:" + OP + ";ReOP:" + ReOP + ";ATT:" + ATT + ";DEF:" + DEF;
    }
}