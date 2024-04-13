using System;
using System.Collections.Generic;
using Script.Battalion;
using Script.Grid;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private GameObject friendGridPanel;
        [SerializeField] private GameObject enemyGridPanel;
        [SerializeField] private GameObject pfTacticCard;
        [Tooltip("缴获系数")] [SerializeField] private double captureCoefficient;

        private CombatTactic fCombatTactic;
        private CombatTactic eCombatTactic;

        private int fIntelligenceValue;
        private int eIntelligenceValue;
        private List<BattalionData> _lostBattalionDatas = new List<BattalionData>();
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
            
        }

        /// <summary>
        /// 战斗开始
        /// </summary>
        public void BattleStart()
        {
            fIntelligenceValue = CalculateIntelligenceValue(false);
            eIntelligenceValue = CalculateIntelligenceValue(true);
            CreatTacticCard(fIntelligenceValue);
            Debug.Log("我方情报值：" + fIntelligenceValue + "\n" + "敌方情报值：" + eIntelligenceValue);
        }

        /// <summary>
        /// 设置战术卡效果
        /// </summary>
        /// <param name="tacticname">战术卡名字</param>
        /// <param name="cost">花费</param>
        /// <param name="itself">战术卡游戏对象自身</param>
        public void SetfTacticCard(string tacticname, int cost, GameObject itself)
        {
            fIntelligenceValue -= cost;
            Debug.Log("cost:-" + cost);
            fCombatTactic = new CombatTactic(tacticname);
            CreatTacticCard(fIntelligenceValue);
        }

        /// <summary>
        /// 创建战术卡
        /// </summary>
        /// <param name="intelligenceValue">情报值</param>
        private void CreatTacticCard(int intelligenceValue)
        {
            Transform tfTacticCardList = gameObject.transform.Find("TacticCardList").transform;
            foreach (Transform chilren in tfTacticCardList.transform)
            {
                Destroy(chilren.gameObject);
            }

            Vector3 offset = new Vector3(0, 0, 0);
            List<(string name, int cost)> Tactics = new List<(string name, int cost)>();
            Tactics.Add(("Defend", 1));
            Tactics.Add(("Attack", 2));
            Tactics.Add(("Guerrilla", 3));
            for (int i = 0; i < Tactics.Count; i++)
            {
                GameObject pf = Instantiate(pfTacticCard, tfTacticCardList);
                pf.transform.position += offset;
                offset += new Vector3(pf.GetComponent<RectTransform>().rect.width, 0, 0);
                if (Tactics[i].cost > intelligenceValue)
                {
                    Destroy(pf);
                    continue;
                }

                string tempname = Tactics[i].name;
                var tempcost = Tactics[i].cost;
                pf.GetComponent<Button>().onClick
                    .AddListener(delegate { SetfTacticCard(tempname, tempcost, pf); });
                pf.transform.Find("TacticName").GetComponent<Text>().text =
                    tempname + "\n花费：" + tempcost;
            }
        }

        /// <summary>
        /// 进行一次战斗
        /// </summary>
        /// <param name="friendTactic"></param>
        /// <param name="enemyTactic"></param>
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

        public void BattleEnd()
        {
            string message = "打扫战场：修复或缴获如下\n";
            int inf = 0;
            int art = 0;
            int arm = 0;
            foreach (var lost in _lostBattalionDatas)
            {
                inf += (int)(lost.InfantryEquipment * captureCoefficient);
                art += (int)(lost.ArtilleryEquipment * captureCoefficient);
                arm += (int)(lost.ArmorEquipment * captureCoefficient);
            }

            DivisionManger gm = friendGridPanel.transform.Find("DivisionManger").GetComponent<DivisionManger>();
            gm.InfantryEquipment += inf;
            gm.ArtilleryEquipment += art;
            gm.ArmorEquipment += arm;

            message += "步枪装备：" + inf + "\n";
            message += "火炮：" + art + "\n";
            message += "装甲：" + arm + "\n";
            Debug.Log(message);
        }


        /// <summary>
        /// 计算情报值
        /// </summary>
        /// <param name="isEnemy">获取敌方的填 true，友方填 false</param>
        /// <returns>情报值</returns>
        private int CalculateIntelligenceValue(bool isEnemy)
        {
            var fbattalionDatas =
                friendGridPanel.transform.Find("Information").GetComponent<Information>().BattalionDatas;
            var ebattalionDatas =
                enemyGridPanel.transform.Find("Information").GetComponent<Information>().BattalionDatas;
            int IntelligenceValue;
            if (isEnemy)
            {
                // 基础情报
                IntelligenceValue = fbattalionDatas.Count * CalculateReconnaissanceValue(ebattalionDatas) /
                                    ebattalionDatas.Count;
                // 情报反制
                IntelligenceValue -= Random.Range(0, CalculateReconnaissanceValue(fbattalionDatas));
            }
            else
            {
                IntelligenceValue = ebattalionDatas.Count * CalculateReconnaissanceValue(fbattalionDatas) /
                                    fbattalionDatas.Count;
                // 情报反制
                IntelligenceValue -= Random.Range(0, CalculateReconnaissanceValue(ebattalionDatas));
            }

            return IntelligenceValue < 0 ? 0 : IntelligenceValue;
        }

        /// <summary>
        /// 计算侦察值
        /// </summary>
        /// <param name="battalionDatas">营列表</param>
        /// <returns>侦察值</returns>
        private int CalculateReconnaissanceValue(List<BattalionData> battalionDatas)
        {
            int ReconnaissanceValue = 0;
            foreach (var battalion in battalionDatas)
            {
                switch (battalion.BattalionType)
                {
                    case BattalionData.BattalionTypes.Infantry:
                        ReconnaissanceValue += 3;
                        break;
                    case BattalionData.BattalionTypes.Artillery:
                        ReconnaissanceValue += 1;
                        break;
                    case BattalionData.BattalionTypes.Armor:
                        ReconnaissanceValue += 5;
                        break;
                }
            }

            return ReconnaissanceValue / battalionDatas.Count;
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

        public List<BattalionData> LostBattalionDatas
        {
            get => _lostBattalionDatas;
            set => _lostBattalionDatas = value;
        }
    }
}