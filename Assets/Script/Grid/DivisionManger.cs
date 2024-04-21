using System.Collections.Generic;
using Script.Battalion;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Types = Script.Battalion.BattalionData.BattalionTypes;

namespace Script.Grid
{
    public class DivisionManger : MonoBehaviour
    {
        [SerializeField] private GameObject pfGameObject;

        [SerializeField] private int width;
        [SerializeField] private int heigth;
        [SerializeField] private float cellsize;
        [SerializeField] private bool isEditor; //是否可编辑
        [SerializeField] private int infantryEquipment;
        [SerializeField] private int artilleryEquipment;
        [SerializeField] private int armorEquipment;
        [SerializeField] private int manpower;
        [SerializeField] private int IC;

        [Tooltip("目标对象")] [SerializeField] private GameObject aimPanel; //目标对象

        private Transform aimTransform;

        [Tooltip("开启后，将根据目标对象的大小，自动设置网格的长宽")] [SerializeField]
        private bool isAutoSet;

        public Grid Grid { get; set; }

        public int InfantryEquipment
        {
            get => infantryEquipment;
            set => infantryEquipment = value;
        }

        public int ArtilleryEquipment
        {
            get => artilleryEquipment;
            set => artilleryEquipment = value;
        }

        public int ArmorEquipment
        {
            get => armorEquipment;
            set => armorEquipment = value;
        }

        public int Manpower
        {
            get => manpower;
            set => manpower = value;
        }

        public int Ic
        {
            get => IC;
            set => IC = value;
        }

        public float Cellsize
        {
            get => cellsize;
            set => cellsize = value;
        }

        public BattalionData.BattalionTypes Types { get; set; } = BattalionData.BattalionTypes.Infantry;

        public BattalionData.Dirs Dirs { get; set; } = BattalionData.Dirs.Up;

        public float GetRotationAngle(BattalionData.Dirs dirs)
        {
            switch (dirs)
            {
                default:
                case BattalionData.Dirs.Up: return 0f;
                case BattalionData.Dirs.Right: return 270f;
                case BattalionData.Dirs.Down: return 180f;
                case BattalionData.Dirs.Left: return 90f;
            }
        }

        public Vector2Int GetRotationOffset(BattalionData.Dirs dirs)
        {
            switch (dirs)
            {
                default:
                case BattalionData.Dirs.Up: return new Vector2Int(0, 0);
                case BattalionData.Dirs.Right: return new Vector2Int(0, 1);
                case BattalionData.Dirs.Down: return new Vector2Int(1, 1);
                case BattalionData.Dirs.Left: return new Vector2Int(1, 0);
            }
        }

        /// <summary>
        /// 相邻检测
        /// </summary>
        /// <param name="go">被检测的 battalion</param>
        /// <returns></returns>
        public List<GameObject> DetectAdjacentCellProperties(GameObject go)
        {
            BattalionData.Dirs godirs = go.GetComponent<Battalion.Battalion>().BattalionData.Dir;
            Vector3 goVector3 = go.transform.position;
            Vector2Int rotationOffset = GetRotationOffset(godirs);
            Vector3 placeObjectWorldPosition = goVector3 -
                                               new Vector3(rotationOffset.x, rotationOffset.y, 0) * (cellsize * 2)
                                               + new Vector3(cellsize, cellsize, 0);
            if (godirs == BattalionData.Dirs.Down)
            {
                placeObjectWorldPosition += new Vector3(-cellsize, -cellsize);
            }

            Grid.GetXY(placeObjectWorldPosition, out int x, out int y);
            //debug文本内容
            string Debugtext = "相邻如下：\n";
            //相邻的对象
            List<GameObject> gameObjectList = new List<GameObject>();
            //
            GameObject gameObjectSelf = Grid.GetBattalion(x, y);
            List<(int X, int Y)> BattalionXY;
            if (gameObjectSelf is not null)
            {
                BattalionXY = gameObjectSelf.GetComponent<Battalion.Battalion>().BattalionXY;
            }
            else
            {
                BattalionXY = new List<(int X, int Y)>();
                Debug.Log(x + "," + y + "gameObjectSelf is null");
            }

            GameObject checkGameobject;
            foreach (var XYs in BattalionXY)
            {
                List<GameObject> gameObjects = new List<GameObject>();
                checkGameobject = Grid.GetBattalion(XYs.X + 1, XYs.Y);

                if (checkGameobject != gameObjectSelf && checkGameobject is not null)
                {
                    if (gameObjects.Count == 0)
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);

                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                     XYs.X + "," +
                                     XYs.Y + "\n";
                    }

                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        if (checkGameobject == gameObjects[i]) continue;
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                     XYs.X + "," +
                                     XYs.Y + "\n";
                    }
                }

                checkGameobject = Grid.GetBattalion(XYs.X - 1, XYs.Y);
                if (checkGameobject != gameObjectSelf && checkGameobject is not null)
                {
                    if (gameObjects.Count == 0)
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                     XYs.X + "," +
                                     XYs.Y + "\n";
                    }

                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        if (checkGameobject != gameObjects[i])
                        {
                            gameObjects.Add(checkGameobject);
                            gameObjectList.Add(checkGameobject);
                            //debug文本内容
                            Debugtext +=
                                checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                XYs.X +
                                "," +
                                XYs.Y + "\n";
                        }
                    }
                }

                checkGameobject = Grid.GetBattalion(XYs.X, XYs.Y - 1);
                if (checkGameobject != gameObjectSelf && checkGameobject is not null)
                {
                    if (gameObjects.Count == 0)
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                     XYs.X + "," +
                                     XYs.Y + "\n";
                    }

                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        if (checkGameobject != gameObjects[i])
                        {
                            gameObjects.Add(checkGameobject);
                            gameObjectList.Add(checkGameobject);
                            //debug文本内容
                            Debugtext +=
                                checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                XYs.X +
                                "," +
                                XYs.Y + "\n";
                        }
                    }
                }

                checkGameobject = Grid.GetBattalion(XYs.X, XYs.Y + 1);
                if (checkGameobject != gameObjectSelf && checkGameobject is not null)
                {
                    if (gameObjects.Count == 0)
                    {
                        gameObjects.Add(checkGameobject);
                        gameObjectList.Add(checkGameobject);
                        //debug文本内容
                        Debugtext += checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                     XYs.X + "," +
                                     XYs.Y + "\n";
                    }

                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        if (checkGameobject != gameObjects[i])
                        {
                            gameObjects.Add(checkGameobject);
                            gameObjectList.Add(checkGameobject);
                            //debug文本内容
                            Debugtext +=
                                checkGameobject.GetComponent<Battalion.Battalion>().BattalionData.BattalionName +
                                XYs.X +
                                "," +
                                XYs.Y + "\n";
                        }
                    }
                }
            }

            return gameObjectList;
        }

        private void Awake()
        {
            aimTransform = aimPanel.transform.Find("BattalionList");
            if (aimPanel)
            {
                if (isAutoSet)
                {
                    width = (int)(aimPanel.GetComponent<RectTransform>().rect.width / cellsize);
                    heigth = (int)(aimPanel.GetComponent<RectTransform>().rect.height / cellsize);
                }

                Grid = new Script.Grid.Grid(width, heigth, cellsize, aimPanel,
                    new Vector3(-aimPanel.GetComponent<RectTransform>().rect.width / 2,
                        -aimPanel.GetComponent<RectTransform>().rect.height / 2, 0));
            }
            else
            {
                Grid = new Script.Grid.Grid(width, heigth, cellsize, aimPanel,
                    new Vector3(-aimPanel.GetComponent<RectTransform>().rect.width / 2,
                        -aimPanel.GetComponent<RectTransform>().rect.height / 2, 0));
            }
        }

        private void Update()
        {
            if (isEditor)
            {
                Editor();
            }
        }

        public void SwitchEditingStatus()
        {
            isEditor = !isEditor;
        }

        /// <summary>
        /// 编辑部队编成
        /// </summary>
        private void Editor()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Grid.IsOnGrid(Functions.Functions.GetMouseWorldPosition()) is false)
                {
                    return;
                }

                Grid.GetXY(Functions.Functions.GetMouseWorldPosition(), out int x, out int y);

                CreatBattalion(x, y, Dirs, Types);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (Grid.IsOnGrid(Functions.Functions.GetMouseWorldPosition()) is false)
                {
                    return;
                }

                switch (Types.GetHashCode())
                {
                    case 0:
                        Types = BattalionData.BattalionTypes.Artillery;
                        break;
                    case 1:
                        Types = BattalionData.BattalionTypes.Armor;
                        break;
                    case 2:
                        Types = BattalionData.BattalionTypes.Infantry;
                        break;
                }

                Functions.Functions.CreateTip(Types.ToString(), Functions.Functions.GetMouseWorldPosition(), 1f);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                switch (Dirs.GetHashCode())
                {
                    case 0:
                        Dirs = BattalionData.Dirs.Left;
                        Functions.Functions.CreateTip("Left", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                    case 1:
                        Dirs = BattalionData.Dirs.Up;
                        Functions.Functions.CreateTip("Up", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                    case 2:
                        Dirs = BattalionData.Dirs.Right;
                        Functions.Functions.CreateTip("Right", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                    case 3:
                        Dirs = BattalionData.Dirs.Down;
                        Functions.Functions.CreateTip("Down", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (Dirs.GetHashCode())
                {
                    case 0:
                        Dirs = BattalionData.Dirs.Right;
                        Functions.Functions.CreateTip("Right", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                    case 1:
                        Dirs = BattalionData.Dirs.Down;
                        Functions.Functions.CreateTip("Down", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                    case 2:
                        Dirs = BattalionData.Dirs.Left;
                        Functions.Functions.CreateTip("Left", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                    case 3:
                        Dirs = BattalionData.Dirs.Up;
                        Functions.Functions.CreateTip("Up", Functions.Functions.GetMouseWorldPosition(), 1f);
                        break;
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                if (Grid.IsOnGrid(Functions.Functions.GetMouseWorldPosition()) is false)
                {
                    return;
                }

                RemoveBattalion(Functions.Functions.GetMouseWorldPosition());
            }
        }

        /// <summary>
        /// 删除一个营
        /// </summary>
        /// <param name="MouseWorldPosition">世界坐标</param>
        public void RemoveBattalion(Vector3 MouseWorldPosition)
        {
            Grid.GetXY(MouseWorldPosition, out int x, out int y);
            GameObject g = Grid.GetBattalion(x, y);
            if (g is not null)
            {
                Battalion.Battalion battalion = g.GetComponent<Battalion.Battalion>();
                infantryEquipment -= battalion.BattalionData.InfantryEquipment;
                artilleryEquipment -= battalion.BattalionData.ArtilleryEquipment;
                armorEquipment -= battalion.BattalionData.ArmorEquipment;
                manpower -= battalion.BattalionData.Manpower;
                Ic -= Functions.Functions.CalculateIC(g.GetComponent<Battalion.Battalion>().BattalionData
                    .BattalionType);
                Grid.RemoveBattalion(g);
            }

            Destroy(g);
        }

        /// <summary>
        /// 创建一个营
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="dirs">方向</param>
        /// <param name="types">类型</param>
        public void CreatBattalion(int x, int y, BattalionData.Dirs dirs, Types types)
        {
            Vector2Int rotationOffset = GetRotationOffset(dirs);
            Vector3 placeObjectWorldPosition = Grid.GetWorldPosition(x, y) +
                                               new Vector3(rotationOffset.x, rotationOffset.y, 0) * cellsize;
            GameObject gameObject = Instantiate(
                pfGameObject,
                placeObjectWorldPosition,
                Quaternion.Euler(0, 0, GetRotationAngle(dirs)),
                aimTransform);
            gameObject.GetComponent<Battalion.Battalion>().BattalionData = new BattalionData(gameObject, 123,
                types, dirs);


            // 获取一次组件引用并存储在局部变量中
            Battalion.Battalion battalion = gameObject.GetComponent<Battalion.Battalion>();
            
            infantryEquipment += battalion.BattalionData.InfantryEquipment;
            artilleryEquipment += battalion.BattalionData.ArtilleryEquipment;
            armorEquipment += battalion.BattalionData.ArmorEquipment;
            manpower += battalion.BattalionData.Manpower;
            Ic += Functions.Functions.CalculateIC(battalion.BattalionData
                .BattalionType);


            // 检查是否可以在网格上添加营
            if (!Grid.AddBattalion(x, y, gameObject, out List<(int X, int Y)> list))
            {
                // 如果不可以，则撤销之前的资源扣除
                infantryEquipment -= battalion.BattalionData.InfantryEquipment;
                artilleryEquipment -= battalion.BattalionData.ArtilleryEquipment;
                armorEquipment -= battalion.BattalionData.ArmorEquipment;
                manpower -= battalion.BattalionData.Manpower;
                Ic -= Functions.Functions.CalculateIC(battalion.BattalionData
                    .BattalionType);
                Destroy(gameObject);
                Functions.Functions.CreateTip("无法放置", Functions.Functions.GetMouseWorldPosition(), 2f);
            }


            if (list is not null)
            {
                gameObject.GetComponent<Battalion.Battalion>().BattalionXY = list;
            }
            RefreshCost();
        }

        public void Type2Infantry()
        {
            SwitchType(Types.Infantry);
        }

        public void Type2Artillery()
        {
            SwitchType(Types.Artillery);
        }

        public void Type2Armor()
        {
            SwitchType(Types.Armor);
        }

        public void SwitchType(Types types)
        {
            switch (types.GetHashCode())
            {
                case 0:
                    Types = BattalionData.BattalionTypes.Infantry;
                    break;
                case 1:
                    Types = BattalionData.BattalionTypes.Artillery;
                    break;
                case 2:
                    Types = BattalionData.BattalionTypes.Armor;
                    break;
            }
        }

        public void SaveDivision()
        {
            string dividsonName = aimPanel.transform.Find("Information").GetComponent<Information>().DivisionName;
            int level = aimPanel.transform.Find("Information").GetComponent<Information>().ListLength;
            GridData gridData = new GridData(dividsonName, level, Grid.Battalions);
            Functions.Functions.SaveByJson("user/" + dividsonName, gridData);
        }


        public void LoadDivision(GridData gridData)
        {
            InitDivision();

            gameObject.transform.parent.Find("Information").Find("Name").GetComponent<InputField>().text =
                gridData.name;
            foreach (var varBattalionData in gridData.battalionDatas)
            {
                CreatBattalion(varBattalionData.x, varBattalionData.y,
                    varBattalionData.dirs, varBattalionData.types);
            }
            RefreshCost();
        }

        public void RefreshCost()
        {
            Transform cost = transform.Find("Cost");
            cost.Find("InfE").Find("value").GetComponent<Text>().text = InfantryEquipment+"";
            cost.Find("ArtE").Find("value").GetComponent<Text>().text = ArtilleryEquipment+"";
            cost.Find("ArmE").Find("value").GetComponent<Text>().text = ArmorEquipment+"";
            cost.Find("Manpower").Find("value").GetComponent<Text>().text = Manpower+"";
            cost.Find("IC").Find("value").GetComponent<Text>().text = Ic+"";

        }

        public void InitDivision()
        {
            foreach (Transform o in gameObject.transform.parent.Find("BattalionList"))
            {
                o.GetComponent<Battalion.Battalion>().Die();
                infantryEquipment -= o.GetComponent<Battalion.Battalion>().BattalionData.InfantryEquipment;
                artilleryEquipment -= o.GetComponent<Battalion.Battalion>().BattalionData.ArtilleryEquipment;
                armorEquipment -= o.GetComponent<Battalion.Battalion>().BattalionData.ArmorEquipment;
                manpower -= o.GetComponent<Battalion.Battalion>().BattalionData.Manpower;
                Ic -= Functions.Functions.CalculateIC(o.GetComponent<Battalion.Battalion>().BattalionData
                    .BattalionType);
            }
            RefreshCost();
        }
    }
}