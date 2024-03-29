using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class BattalionData
    {
        private const double HpLow = 10;
        private const double HpNormal = 15;
        private const double HpHigh = 20;

        private const double OpLow = 10;
        private const double OpNormal = 15;
        private const double OpHigh = 20;

        private const double ReOpLow = .10;
        private const double ReOpNormal = .15;
        private const double ReOpHigh = .20;

        private const double AttLow = 10;
        private const double AttNormal = 15;
        private const double AttHigh = 20;

        private const double DefLow = 10;
        private const double DefNormal = 15;
        private const double DefHigh = 20;

        public int[,] Ints { get; set; } = new int[3, 4];

        /// <summary>
        /// 放置方向
        /// </summary>
        public enum Dirs
        {
            Up,
            Right,
            Down,
            Left
        }

        private Dirs dir = Dirs.Up;

        public Dirs Dir
        {
            get => dir;
            set => dir = value;
        }

        private int id; //名称代码 
        private string battalionName; //名称

        /// <summary>
        /// Infantry,
        /// Artillery,
        /// Armor
        /// </summary>
        public enum BattalionTypes
        {
            Infantry,
            Artillery,
            Armor,
            Default
        }

        private BattalionTypes battalionType;

        private Sprite image;

        [Header("属性")] [Tooltip("血量")] [SerializeField]
        private double HP;

        [Tooltip("组织度")] [SerializeField] private double OP;

        [Tooltip("组织度恢复度")] [SerializeField] private double ReOP;

        [Tooltip("攻击力")] [SerializeField] private double ATT;
        [Tooltip("防御力")] [SerializeField] private double DEF;
        private List<Buff> _buffList = new List<Buff>();

        public GameObject GameObject { get; set; }

        public string BattalionName
        {
            get => battalionName;
            set => battalionName = value;
        }

        public BattalionTypes BattalionType
        {
            get => battalionType;
            set => battalionType = value;
        }

        public Sprite Image
        {
            get => image;
            set => image = value;
        }

        public double Hp
        {
            get => HP;
            set => HP = value;
        }

        public double Op
        {
            get => OP;
            set => OP = value;
        }

        public double ReOp
        {
            get => ReOP;
            set => ReOP = value;
        }

        public double Att
        {
            get => ATT;
            set => ATT = value;
        }

        public double Def
        {
            get => DEF;
            set => DEF = value;
        }

        public List<Buff> BuffList
        {
            get => _buffList;
            set => _buffList = value;
        }

        public BattalionData(GameObject gameObject, int id, BattalionTypes battalionType, Dirs dir)
        {
            this.dir = dir;
            this.id = id;
            this.battalionType = battalionType;
            GameObject = gameObject;
            LoadBattalion(this.id, this.battalionType);
        }

        public void LoadBattalion(int id, BattalionTypes battalionType)
        {
            this.id = id;
            battalionName = id.ToString() + "th." + battalionType.ToString();
            this.battalionType = battalionType;
            switch (battalionType.GetHashCode())
            {
                case 0:
                    image = Resources.Load<Sprite>("BattalionsImage/步兵营");
                    Hp = HpHigh;
                    Op = OpHigh;
                    ReOP = OpHigh;
                    Att = AttLow;
                    Def = DefNormal;
                    Ints = new int[3, 2]
                    {
                        { 1, 1 },
                        { 1, 1 },
                        { 1, 1 }
                    };
                    break;
                case 1:
                    image = Resources.Load<Sprite>("BattalionsImage/炮兵营");
                    Hp = HpLow;
                    Op = OpLow;
                    ReOp = ReOpLow;
                    Att = AttNormal;
                    Def = DefHigh;
                    Ints = new int[3, 4]
                    {
                        { 0, 1, 1, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 1, 1, 0 }
                    };
                    break;
                case 2:
                    image = Resources.Load<Sprite>("BattalionsImage/装甲营");
                    Hp = HpNormal;
                    Op = OpNormal;
                    ReOp = ReOpNormal;
                    Att = AttHigh;
                    Def = DefNormal;
                    Ints = new int[3, 4]
                    {
                        { 1, 0, 0, 1 },
                        { 1, 1, 1, 1 },
                        { 0, 1, 1, 0 }
                    };
                    break;
                default:
                    image = Resources.Load<Sprite>("BattalionsImage/空选中");
                    break;
            }

            GameObject.GetComponent<Image>().sprite = image;
        }
    }
}