using System;
using System.Collections.Generic;
using Script.Battalion;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public struct OutputData
    {
        public Double HP;
        public Double OP;
        public Double ReOP;
        public Double ATT;
        public Double DEF;

        public OutputData(double hp, double op, double reOp, double att, double def)
        {
            HP = hp;
            OP = op;
            ReOP = reOp;
            ATT = att;
            DEF = def;
        }

        public void SaveData(double hp, double op, double reOp, double att, double def)
        {
            HP = hp;
            OP = op;
            ReOP = reOp;
            ATT = att;
            DEF = def;
        }
    }

    public class Information : MonoBehaviour
    {
        private GameObject BattalionList;
        private int ListLength;
        public List<BattalionData> BattalionDatas { get; set; }
        public OutputData outputData { get; set; }


        private void Start()
        {
            BattalionList = gameObject.transform.parent.Find("BattalionList").gameObject;
            BattalionDatas = new List<BattalionData>();
        }

        private void Update()
        {
            if (ListLength == BattalionList.transform.childCount)
            {
                return;
            }
            Refresh();
        }

        public void Refresh()
        {
            double HP = 0;
            double OP = 0;
            double ReOP = 0;
            double ATT = 0;
            double DEF = 0;
            BattalionDatas.Clear();
            foreach (Transform o in BattalionList.transform)
            {
                Battalion.Battalion b = o.GetComponent<Battalion.Battalion>();
                b.LoadBuff();
                BattalionData bd = b.BattalionData;
                bd.InitializeData();

                //执行buff效果
                foreach (var nBuff in bd.BuffList)
                {
                    nBuff.Run(new[] { bd });
                }
                
                
                BattalionDatas.Add(bd);
                
                HP += bd.Hp *  bd._Hp;
                OP += bd.Op *  bd._Op;
                ReOP += bd.ReOp *  bd._ReOp;
                ATT += bd.Att *  bd._Att;
                DEF += bd.Def * bd._Def;
                

                
                ListLength = BattalionList.transform.childCount;
            }

            outputData = new OutputData(HP, OP, ReOP, ATT, DEF);
            
            this.GetComponent<Text>().text = "HP:" + HP.ToString("F2") + ";OP:" + OP.ToString("F2") + ";ReOP:" + ReOP + ";ATT:" + ATT + ";DEF:" + DEF;

        }
    }
}