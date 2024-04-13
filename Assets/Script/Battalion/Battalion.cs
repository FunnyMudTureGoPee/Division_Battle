using System.Collections.Generic;
using Script.Grid;
using UnityEngine;
using Types = Script.Battalion.BattalionData.BattalionTypes;

namespace Script.Battalion
{
    public class Battalion : MonoBehaviour
    {
        public BattalionData BattalionData { get; set; }
        public List<(int X, int Y)> BattalionXY { get; set; }
        private bool isLowOp = false;

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
            if (BattalionData.Hp <= 0)
            {
                gameObject.transform.parent.parent.Find("DivisionManger").GetComponent<DivisionManger>()
                    .Grid.RemoveBattalion(gameObject);
                GameObject.Find("BattleManager").GetComponent<BattleManager>().LostBattalionDatas.Add(BattalionData);
                Destroy(gameObject);
            }

            if (BattalionData.Op<=5&&isLowOp is false)
            {
                BattalionData.BuffList.Add(new Buff("Buff_LowOp"));
                isLowOp = true;
                if (BattalionData.Op<=0)
                {
                    BattalionData.Op = 0;
                }
            }

            if (isLowOp)
            {
               var index= BattalionData.BuffList.FindIndex(e => e.funName == "Buff_LowOp");
               BattalionData.BuffList.RemoveAt(index);
               isLowOp = false;
            }
          

            if (Input.GetKeyDown(KeyCode.Space))
            {
                string debugtext = "属性：\n";
                debugtext += "Hp:" + BattalionData.Hp + " + " + BattalionData.Hp * (BattalionData._Hp - 1) + "\n";
                debugtext += "Op:" + BattalionData.Op + " + " + BattalionData.Op * (BattalionData._Op - 1) + "\n";
                debugtext += "ReOp:" + BattalionData.ReOp + " + " + BattalionData.ReOp * (BattalionData._ReOp - 1) +
                             "\n";
                debugtext += "Att:" + BattalionData.Att + " + " + BattalionData.Att * (BattalionData._Att - 1) + "\n";
                debugtext += "Def:" + BattalionData.Def + " + " + BattalionData.Def * (BattalionData._Def - 1) + "\n";
                debugtext += "BuffList:\n";
                foreach (var variablBuff in BattalionData.BuffList)
                {
                    debugtext += variablBuff.funName + "\n";
                }

                Functions.Functions.CreateTip(debugtext, gameObject.transform.position, 5);
            }
        }

        /// <summary>
        /// 检测并获取自身可获得的动态buff
        /// </summary>
        public void LoadBuff()
        {
            string debugtext = "buff来源：\n";
            List<GameObject> gameObjects = gameObject.transform.parent.parent.Find("DivisionManger")
                .GetComponent<DivisionManger>().DetectAdjacentCellProperties(gameObject);
            BattalionData.BuffList.Clear();
            if (BattalionData.Op<=5&&isLowOp is false)
            {
                BattalionData.BuffList.Add(new Buff("Buff_LowOp"));
                isLowOp = true;
            }
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

            Functions.Functions.CreateTip(debugtext, gameObject.transform.position, 5);
        }
    }
}