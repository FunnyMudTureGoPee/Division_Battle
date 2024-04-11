using System;
using Script.Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private GameObject friendGridPanel;
        [SerializeField] private GameObject enemyGridPanel;
        private int turn;

        private void Start()
        {
            friendGridPanel = GameObject.Find("FriendGridPanel");
            enemyGridPanel = GameObject.Find("EnemyGridPanel");
            InitTurn();
        }

        public void InitTurn()
        {
            turn = 0;
        }

        public void test()
        {
            Battle(new CombatTactic("Guerrilla"), new CombatTactic("Defend"));
        }

        public void Battle(CombatTactic friendTactic, CombatTactic enemyTactic)
        {
            var fData = friendGridPanel.transform.Find("Information").GetComponent<Information>().outputData;
            var eData = enemyGridPanel.transform.Find("Information").GetComponent<Information>().outputData;
            var fBattalionListCount = friendGridPanel.transform.Find("Information").GetComponent<Information>()
                .BattalionDatas.Count;
            var eBattalionListCount = enemyGridPanel.transform.Find("Information").GetComponent<Information>()
                .BattalionDatas.Count;
            var fBattalionList = friendGridPanel.transform.Find("Information").GetComponent<Information>()
                .BattalionDatas;
            var eBattalionList = enemyGridPanel.transform.Find("Information").GetComponent<Information>()
                .BattalionDatas;
            // 集火率
            var ConcentrationRate = 0.2;

            var fhits = friendTactic.Run(true, new object[] { fData });
            var ehits = enemyTactic.Run(true, new object[] { eData });

            var fDataTemp = fData;
            var eDataTemp = eData;

            for (int i = 0; i < (int)ehits; i++)
            {
                var lackyBattalion = Random.Range(0, fBattalionListCount - 1);
                Debug.Log(lackyBattalion);
                for (int j = 0; j < fBattalionListCount; j++)
                {
                    fBattalionList[j].Hp -= HpDamge(eDataTemp.ATT, fDataTemp.DEF) / eBattalionListCount *
                                            (1 - ConcentrationRate);
                    fBattalionList[j].Op -= OpDamage(eDataTemp.ATT, fDataTemp.DEF) / eBattalionListCount *
                                            (1 - ConcentrationRate);
                    Debug.Log(fBattalionList[j].BattalionName + "受攻击了,hp收到：" + HpDamge(fDataTemp.ATT, eDataTemp.DEF) /
                        fBattalionListCount *
                        (1 - ConcentrationRate) + "点伤害，Op受到：" + OpDamage(fDataTemp.ATT, eDataTemp.DEF) /
                        fBattalionListCount *
                        (1 - ConcentrationRate) + "点伤害");
                    if (j == lackyBattalion)
                    {
                        fBattalionList[j].Hp -= HpDamge(eDataTemp.ATT, fDataTemp.DEF) / eBattalionListCount *
                                                ConcentrationRate * 10;
                        fBattalionList[j].Op -= OpDamage(eDataTemp.ATT, fDataTemp.DEF) / eBattalionListCount *
                                                ConcentrationRate * 10;
                        Debug.Log(fBattalionList[j].BattalionName + "\n aieeeeee，受暴击了,hp收到：" +
                                  HpDamge(eDataTemp.ATT, fDataTemp.DEF) / eBattalionListCount *
                                  ConcentrationRate * 10 + "点伤害，Op受到：" + OpDamage(eDataTemp.ATT, fDataTemp.DEF) /
                                  eBattalionListCount *
                                  ConcentrationRate * 10 + "点伤害");
                    }
                }
            }

            for (int i = 0; i < (int)fhits; i++)
            {
                var lackyBattalion = Random.Range(0, eBattalionListCount - 1);
                for (int j = 0; j < eBattalionListCount; j++)
                {
                    eBattalionList[j].Hp -= HpDamge(fDataTemp.ATT, eDataTemp.DEF) / fBattalionListCount *
                                            (1 - ConcentrationRate);
                    eBattalionList[j].Op -= OpDamage(fDataTemp.ATT, eDataTemp.DEF) / fBattalionListCount *
                                            (1 - ConcentrationRate);

                    if (j == lackyBattalion)
                    {
                        eBattalionList[j].Hp -= HpDamge(fDataTemp.ATT, eDataTemp.DEF) / fBattalionListCount *
                                                ConcentrationRate * 10;
                        eBattalionList[j].Op -= OpDamage(fDataTemp.ATT, eDataTemp.DEF) / fBattalionListCount *
                                                ConcentrationRate * 10;
                    }
                }
            }

            foreach (var variaBattalionData in fBattalionList)
            {
                if (variaBattalionData.Op >= variaBattalionData.MaxOp) continue;
                variaBattalionData.Op += variaBattalionData.ReOp;
            }

            foreach (var variaBattalionData in eBattalionList)
            {
                if (variaBattalionData.Op >= variaBattalionData.MaxOp) continue;
                variaBattalionData.Op += variaBattalionData.ReOp;
            }

            turn++;
        }

        private double HpDamge(double att, double def)
        {
            var damage = (att - def) / (def / att);
            // 随机伤害
            damage += Random.Range(-5, 5);
            //保底伤害
            if (damage < 0)
            {
                damage = 1;
            }

            return damage;
        }

        private double OpDamage(double att, double def)
        {
            //保底伤害
            var damage = att / def;
            var temp = att - def;
            // 破防伤害
            if (temp >= 0)
            {
                damage += temp * 4;
            }

            // 随机伤害+保底伤害
            damage += Random.Range(0, (int)(att / def * 10));
            return damage;
        }
    }
}