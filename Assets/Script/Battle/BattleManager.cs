using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject pfBattalInformation;
        [Tooltip("缴获系数")] [SerializeField] private double captureCoefficient;

        private CombatTactic fCombatTactic;
        private CombatTactic eCombatTactic;

        List<(string name, int cost)> fTactics = new List<(string name, int cost)>();
        List<(string name, int cost)> eTactics = new List<(string name, int cost)>();


        private int fIntelligenceValue;
        private int eIntelligenceValue;
        private List<BattalionData> _lostBattalionDatas = new List<BattalionData>();
        private int turn;


        private void Start()
        {
            SetTactics(false,
                new List<(string name, int cost)>() { ("Default", 0), ("Defend", 1), ("Attack", 2), ("Guerrilla", 3) });
            SetTactics(true,
                new List<(string name, int cost)>() { ("Default", 0), ("Defend", 1), ("Attack", 2), ("Guerrilla", 3) });

            InitTurn();
        }

        public void InitTurn()
        {
            turn = 0;
        }

        public void test1()
        {
            InitTurn();
            BattleStart();
        }

        public void test2()
        {
            InitTacticCard();
            Battle(fCombatTactic, eCombatTactic);
        }

        public void SetTactics(bool isEnemy, List<(string name, int cost)> list)
        {
            if (isEnemy)
            {
                eTactics = list;
            }
            else
            {
                fTactics = list;
            }
        }

        private void InitTacticCard()
        {
            eTactics = eTactics.OrderByDescending(t => t.cost).ToList();
            foreach (var tactic in eTactics)
            {
                if (eIntelligenceValue >= tactic.cost)
                {
                    eCombatTactic = new CombatTactic(tactic.name);
                    eIntelligenceValue -= tactic.cost;
                    break;
                }
                else
                {
                    eCombatTactic = new CombatTactic("Default");
                }
            }
        }

        public void Button2Next(GameObject gameObject)
        {
            if (friendGridPanel.transform.Find("Information").GetComponent<Information>()
                    .ListLength == 0 ||
                enemyGridPanel.transform.Find("Information").GetComponent<Information>()
                    .ListLength == 0)
            {
                BattleEnd();
                Destroy(gameObject);
                return;
            }

            if (fIntelligenceValue == 0 && eIntelligenceValue == 0)
            {
                BattleStart();
            }
            else
            {
                CreatTacticCard(fIntelligenceValue);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// 战斗开始
        /// </summary>
        public void BattleStart()
        {
            turn++;

            foreach (var variaBattalionData in friendGridPanel.transform.Find("Information").GetComponent<Information>()
                         .BattalionDatas)
            {
                if (variaBattalionData.Op >= variaBattalionData.MaxOp) continue;
                variaBattalionData.Op += variaBattalionData.ReOp;
            }

            foreach (var variaBattalionData in enemyGridPanel.transform.Find("Information").GetComponent<Information>()
                         .BattalionDatas)
            {
                if (variaBattalionData.Op >= variaBattalionData.MaxOp) continue;
                variaBattalionData.Op += variaBattalionData.ReOp;
            }

            friendGridPanel.transform.Find("Information").GetComponent<Information>().Refresh();
            enemyGridPanel.transform.Find("Information").GetComponent<Information>().Refresh();

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
            // Debug.Log("cost:-" + cost);
            fCombatTactic = new CombatTactic(tacticname);
            CreatTacticCard(fIntelligenceValue);
            // 选择战术后直接开战
            test2();
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

            for (int i = 0; i < fTactics.Count; i++)
            {
                GameObject pf = Instantiate(pfTacticCard, tfTacticCardList);
                pf.transform.position += offset;
                offset += new Vector3(pf.GetComponent<RectTransform>().rect.width, 0, 0);
                if (fTactics[i].cost > intelligenceValue)
                {
                    Destroy(pf);
                    continue;
                }

                string tempname = fTactics[i].name;
                var tempcost = fTactics[i].cost;
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
            var fInformation = friendGridPanel.transform.Find("Information").GetComponent<Information>();
            var eInformation = enemyGridPanel.transform.Find("Information").GetComponent<Information>();

            var fData = fInformation.outputData;
            var eData = eInformation.outputData;
            var fBattalionListCount = fInformation
                .BattalionDatas.Count;
            var eBattalionListCount = eInformation
                .BattalionDatas.Count;
            var fBattalionList = fInformation
                .BattalionDatas;
            var eBattalionList = eInformation
                .BattalionDatas;
            // 集火率
            var ConcentrationRate = 0.2;

            var fhits = friendTactic.Run(true, new object[] { fData });
            var ehits = enemyTactic.Run(true, new object[] { eData });

            var fDataTemp = fData;
            var eDataTemp = eData;

            double fHpdamge = 0;
            double fOpdamge = 0;
            double eHpdamge = 0;
            double eOpdamge = 0;
            for (int i = 0; i < (int)ehits; i++)
            {
                var lackyBattalion = Random.Range(0, fBattalionListCount - 1);
                for (int j = 0; j < fBattalionListCount; j++)
                {
                    var hpDamge = HpDamge(eDataTemp.ATT, fDataTemp.DEF);
                    var opDamage = OpDamage(eDataTemp.ATT, fDataTemp.DEF);

                    fBattalionList[j].Hp -= hpDamge / eBattalionListCount *
                                            (1 - ConcentrationRate);
                    fBattalionList[j].Op -= opDamage / eBattalionListCount *
                                            (1 - ConcentrationRate);

                    // Debug.Log(fBattalionList[j].BattalionName + "受攻击了,hp收到：" + HpDamge(fDataTemp.ATT, eDataTemp.DEF) /
                    //     fBattalionListCount *
                    //     (1 - ConcentrationRate) + "点伤害，Op受到：" + OpDamage(fDataTemp.ATT, eDataTemp.DEF) /
                    //     fBattalionListCount *
                    //     (1 - ConcentrationRate) + "点伤害");
                    if (j == lackyBattalion)
                    {
                        fHpdamge += hpDamge / eBattalionListCount *
                                    ConcentrationRate * 10;
                        fOpdamge += opDamage / eBattalionListCount *
                                    ConcentrationRate * 10;

                        fBattalionList[j].Hp -= hpDamge / eBattalionListCount *
                                                ConcentrationRate * 10;
                        fBattalionList[j].Op -= opDamage / eBattalionListCount *
                                                ConcentrationRate * 10;
                        // Debug.Log(fBattalionList[j].BattalionName + "\n aieeeeee，受暴击了,hp收到：" +
                        //           HpDamge(eDataTemp.ATT, fDataTemp.DEF) / eBattalionListCount *
                        //           ConcentrationRate * 10 + "点伤害，Op受到：" + OpDamage(eDataTemp.ATT, fDataTemp.DEF) /
                        //           eBattalionListCount *
                        //           ConcentrationRate * 10 + "点伤害");
                    }


                    fHpdamge += hpDamge / eBattalionListCount *
                                (1 - ConcentrationRate);
                    fOpdamge += opDamage / eBattalionListCount *
                                (1 - ConcentrationRate);
                }
            }

            for (int i = 0; i < (int)fhits; i++)
            {
                var lackyBattalion = Random.Range(0, eBattalionListCount - 1);
                for (int j = 0; j < eBattalionListCount; j++)
                {
                    var hpDamge = HpDamge(fDataTemp.ATT, eDataTemp.DEF);
                    var opDamage = OpDamage(fDataTemp.ATT, eDataTemp.DEF);
                    eBattalionList[j].Hp -= hpDamge / fBattalionListCount *
                                            (1 - ConcentrationRate);

                    eBattalionList[j].Op -= opDamage / fBattalionListCount *
                                            (1 - ConcentrationRate);

                    if (j == lackyBattalion)
                    {
                        eBattalionList[j].Hp -= hpDamge / fBattalionListCount *
                                                ConcentrationRate * 10;
                        eBattalionList[j].Op -= opDamage / fBattalionListCount *
                                                ConcentrationRate * 10;
                        eHpdamge += hpDamge / fBattalionListCount *
                                    (1 - ConcentrationRate);
                        eOpdamge += opDamage / fBattalionListCount *
                                    (1 - ConcentrationRate);
                    }

                    eHpdamge += hpDamge / fBattalionListCount *
                                (1 - ConcentrationRate);
                    eOpdamge += opDamage / fBattalionListCount *
                                (1 - ConcentrationRate);
                }
            }


            fInformation.Refresh();
            eInformation.Refresh();
            GameObject nextButton =
                Instantiate(pfBattalInformation, gameObject.transform.Find("BattleInformation"));
            nextButton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(delegate
            {
                Button2Next(nextButton);
            });
            nextButton.transform.Find("Information").GetComponent<Text>().text = turn + "\n" + "我方情报：" +
                fIntelligenceValue + "敌方情报" + eIntelligenceValue +
                "我方采取" + friendTactic.funName + "战术\n" + "敌方采取" + enemyTactic.funName + "战术\n" + "此战我方兵力损失：" +
                fHpdamge.ToString("F2") + "士气损失：" + fOpdamge.ToString("F2") +
                "对敌方造成兵力损失：" + eHpdamge.ToString("F2") + "士气损失：" + eOpdamge.ToString("F2");
            if (fOpdamge <= eOpdamge || fHpdamge <= eHpdamge)
            {
                nextButton.transform.Find("Button").Find("ButtonText").GetComponent<Text>().text = "优势在我";
            }
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

            Economic ec = GameObject.Find("MainController").transform.Find("EconomicPanel").GetComponent<Economic>();
            ec.factories[0].Inventory.Value += inf;
            ec.factories[1].Inventory.Value += art;
            ec.factories[2].Inventory.Value += arm;

            
            message += "步枪装备：" + inf + "\n";
            message += "火炮：" + art + "\n";
            message += "装甲：" + arm + "\n";
            Debug.LogError(message);
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
                damage += temp * att / def;
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