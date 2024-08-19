using System;
using System.Collections.Generic;
using System.Linq;
using Script.Battalion;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Script.Grid
{
    public enum Ownership
    {
        Friend,
        Enemy,
        Null
    }

    public enum State
    {
        Move,
        Attack,
        Default
    }

    public class FrontGrid : MonoBehaviour
    {
        public Economic Economic;
        public GameObject hexPrefab;
        public int width = 10;
        public int height = 10;
        private float pfwidth;
        private float pfheight;
        private GameObject[,] hexes;

        private GameObject selectedHex = null;
        private HexCoordinates selectedHexCoordinates;
        private List<HexCoordinates> aimHexes = new List<HexCoordinates>();
        private HexCoordinates aimHexCoordinates;
        private bool IsEnableAimHex;
        private State state = State.Default;

        private DivisionObject[,] Divisions;
        private List<(HexCoordinates, int)> aimCoordinatesList = new List<(HexCoordinates, int)>(); // 敌军坚守目标(目标，目标等级)

        private GridData selectedGridData;
        private bool isReadyArrange;

        // todo 记得测试完了删掉
        private HexCoordinates f;
        private HexCoordinates e;

        public GridData SelectedGridData
        {
            get => selectedGridData;
            set => selectedGridData = value;
        }

        void Start()
        {
            hexes = new GameObject[width, height];
            // hexesOwnerships = new Ownership[width, height];
            Divisions = new DivisionObject[width, height];

            //GridDatas = new GridData[width, height];
            pfwidth = hexPrefab.GetComponent<RectTransform>().rect.width;
            pfheight = hexPrefab.GetComponent<RectTransform>().rect.height;
            Economic = GameObject.Find("MainController").transform.Find("EconomicPanel").GetComponent<Economic>();
            CreateHexMap();
            EnemyInit(Random.Range(99999, 999999)); // 配置敌军
            ResetHex();
        }

        void CreateHexMap()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float xPos = x * 0.76f;
                    if (y % 2 == 1)
                    {
                        xPos += 0.38f;
                    }

                    GameObject hex_go = Instantiate(hexPrefab, transform);
                    hex_go.transform.position += new Vector3(xPos * pfwidth, y * 0.66f * pfheight, 0);
                    hex_go.name = "Hex_" + x + "_" + y;
                    hex_go.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", x, y);
                    hexes[x, y] = hex_go;
                    Divisions[x, y] = new DivisionObject(Ownership.Null);
                }
            }
        }

        void Update()
        {
            // 进攻或移动
            if (IsEnableAimHex) //选择目标单元格
            {
                if (Input.GetMouseButtonDown(2))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                    if (hit.collider is not null)
                    {
                        Transform objectHit = hit.transform;
                        HexCoordinates coordinates = GetHexCoordinates(objectHit.position);
                        if (aimHexes.Any(hex => hex.IsEqual(coordinates)))
                        {
                            aimHexCoordinates = coordinates;
                            switch (state)
                            {
                                case State.Move:
                                    Move();
                                    break;
                                case State.Attack:
                                    SelectFight(coordinates);
                                    break;
                                default: break;
                            }

                            return;
                        }

                        Functions.Functions.CreateTip("超出移动范围", Functions.Functions.GetMouseWorldPosition(), 2f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    Functions.Functions.CreateTip("取消命令", Functions.Functions.GetMouseWorldPosition(), 2f);
                    IsEnableAimHex = false;
                    selectedHexCoordinates = new HexCoordinates();
                    aimHexes.Clear();
                    ResetHex();
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                if (Camera.main != null)
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                    if (hit.collider is not null)
                    {
                        Transform objectHit = hit.transform;
                        SelectHex(GetHexCoordinates(objectHit.position));
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider is not null)
                {
                    Transform objectHit = hit.transform;
                    Image image = hit.transform.Find("image").GetComponent<Image>();

                    HexCoordinates coordinates = GetHexCoordinates(objectHit.position);
                    Arrange(coordinates);

                    if (image is not null)
                    {
                        if (selectedHex is not null)
                        {
                            // Reset the previous selection
                            ResetHex();
                        }


                        selectedHex = objectHit.gameObject;
                        image.color = Color.green;
                    }
                }
            }

            // if (Input.GetMouseButtonDown(2))
            // {
            //     Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            //
            //     if (hit.collider is not null)
            //     {
            //         Transform objectHit = hit.transform;
            //         Image image = hit.transform.Find("image").GetComponent<Image>();
            //
            //         HexCoordinates coordinates = GetHexCoordinates(objectHit.position);
            //
            //         if (image is not null)
            //         {
            //             if (selectedHex is not null)
            //             {
            //                 // Reset the previous selection
            //                 ResetHex();
            //             }
            //
            //             selectedHex = objectHit.gameObject;
            //             image.color = Color.cyan;
            //             ChangeAdjacentHexesColor(GetMovAbleHexes(coordinates), Color.blue);
            //         }
            //     }
            // }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextTurn();
            }

            // 测试方法
            //todo 测完记得删了
            if (Input.GetKeyDown(KeyCode.T))
            {
                e = new HexCoordinates(1, 3);
                f = new HexCoordinates(7, 6);
                Divisions[1, 3] = new DivisionObject(Ownership.Enemy, Functions.Functions.LoadByJson("装甲掷弹兵"));
                Divisions[7, 6] = new DivisionObject(Ownership.Friend, Functions.Functions.LoadByJson("填线宝宝"));
                ResetHex();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                List<HexCoordinates> list = new List<HexCoordinates>();
                for (var x = 0; x < hexes.GetLength(0); x++)
                {
                    for (var y = 0; y < hexes.GetLength(1); y++)
                    {
                        if (Divisions[x, y].HexesOwnership == Ownership.Enemy)
                        {
                            list.Add(new HexCoordinates(x, y));
                        }
                    }
                }

                for (var index = 0; index < list.Count; index++)
                {
                    var item = list[index];
                    HexCoordinates temp = item;
                    item = FindPath(item, f);
                    Swap(temp, item);
                    ResetHex();
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                List<HexCoordinates> list = new List<HexCoordinates>();
                for (var x = 0; x < hexes.GetLength(0); x++)
                {
                    for (var y = 0; y < hexes.GetLength(1); y++)
                    {
                        if (Divisions[x, y].HexesOwnership == Ownership.Enemy)
                        {
                            list.Add(new HexCoordinates(x, y));
                        }
                    }
                }

                foreach (var item in list)
                {
                    List<HexCoordinates> aimCoordinatesList =  GetAttackAbleHexes(item);
                    foreach (var aim in aimCoordinatesList)
                    {
                        if (Divisions[item.X, item.Y].GridData.IC >= Divisions[aim.X, aim.Y].GridData.IC)
                        {
                            selectedHexCoordinates = item;
                            aimHexCoordinates = aim;
                            AutoFight();
                            break;
                        }
                    }
                }
            }
        }

        private void MoveAbleAimHex()
        {
            aimHexes = GetMovAbleHexes(selectedHexCoordinates,
                Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y].HexesOwnership);
            if (aimHexes is null) return;
            IsEnableAimHex = true;
            ChangeAdjacentHexesColor(aimHexes, Color.blue);
        }

        public void Move()
        {
            Swap(aimHexCoordinates, selectedHexCoordinates);

            IsEnableAimHex = false;
            selectedHexCoordinates = new HexCoordinates();
            aimHexes.Clear();
            ResetHex();
            state = State.Default;
        }

        private void Swap(HexCoordinates hexCoordinate1, HexCoordinates hexCoordinate2)
        {
            Divisions[hexCoordinate1.X, hexCoordinate1.Y].Movepoint -= 1;
            Divisions[hexCoordinate2.X, hexCoordinate2.Y].Movepoint -= 1;
            Debug.Log(
                hexCoordinate1.X + "," + hexCoordinate1.Y + "与" + hexCoordinate2.X + "," + hexCoordinate2.Y + "交换");
            (Divisions[hexCoordinate1.X, hexCoordinate1.Y], Divisions[hexCoordinate2.X, hexCoordinate2.Y]) =
                (Divisions[hexCoordinate2.X, hexCoordinate2.Y],
                    Divisions[hexCoordinate1.X, hexCoordinate1.Y]);
        }

        /// <summary>
        /// 部署友方单位
        /// </summary>
        /// <param name="coordinates"></param>
        public void Arrange(HexCoordinates coordinates)
        {
            if (selectedGridData is null) return;

            if (Divisions[coordinates.X, coordinates.Y].HexesOwnership != Ownership.Null) return;

            if (Economic.InfInventory < selectedGridData.infantryEquipment ||
                Economic.ArtInventory < selectedGridData.artilleryEquipment ||
                Economic.ArmInventory < selectedGridData.armorEquipment ||
                Economic.ManpowerInventory < selectedGridData.Manpower)
            {
                Debug.Log("资源不足");
                return;
            }


            Divisions[coordinates.X, coordinates.Y] = new DivisionObject(Ownership.Friend, selectedGridData);
            ResetHex();
            //资源扣除
            Economic.InfInventory -= selectedGridData.infantryEquipment;
            Economic.ArtInventory -= selectedGridData.artilleryEquipment;
            Economic.ArmInventory -= selectedGridData.armorEquipment;
            Economic.ManpowerInventory -= selectedGridData.Manpower;
            Economic.RefreshInventory();
        }

        /// <summary>
        /// 撤回编制
        /// </summary>
        /// <param name="coordinates"></param>
        private void Withdraw(HexCoordinates coordinates)
        {
            Economic.InfInventory += Divisions[coordinates.X, coordinates.Y].GridData.infantryEquipment;
            Economic.ArtInventory += Divisions[coordinates.X, coordinates.Y].GridData.artilleryEquipment;
            Economic.ArmInventory += Divisions[coordinates.X, coordinates.Y].GridData.armorEquipment;
            Economic.ManpowerInventory += Divisions[coordinates.X, coordinates.Y].GridData.Manpower;
            Economic.RefreshInventory();

            Divisions[coordinates.X, coordinates.Y] = new DivisionObject(Ownership.Null);
            ResetHex();
        }

        private void AttackAbleAimHex()
        {
            IsEnableAimHex = true;
            aimHexes = GetAttackAbleHexes(selectedHexCoordinates);
            ChangeAdjacentHexesColor(aimHexes, Color.yellow);
        }

        private void SelectFight(HexCoordinates coordinates)
        {
            GameObject buttonPanel = Instantiate(Resources.Load<GameObject>("ButtonPanel"),
                Functions.Functions.GetMouseWorldPosition() - new Vector3(-pfwidth, 0, 5), Quaternion.identity);
            buttonPanel.GetComponent<ButtonPanel>().Initialize(Divisions[coordinates.X, coordinates.Y].GridData.name,
                new List<(string name, Action action)>()
                {
                    ("自动战斗", () =>
                    {
                        AutoFight();
                        Destroy(buttonPanel);
                    }),
                    ("手动战斗", () =>
                    {
                        ManualFight();
                        Destroy(buttonPanel);
                    }),
                    ("取消", () => { Destroy(buttonPanel); })
                });
            ;
        }

        private void ManualFight()
        {
            GameObject.Find("MainController").GetComponent<MainController>().Fight(
                Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y].GridData,
                Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].GridData);
        }

        private void AutoFight()
        {
            int attIC = Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y].GridData.IC,
                attMP = Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y].GridData.Manpower;
            int defIC = Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].GridData.IC,
                defMP = Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].GridData.Manpower;
            float attQ = (float)attIC / attMP;
            float defQ = (float)defIC / defMP;
            float attBuff = attQ / defQ;
            float defBuff = defQ / attQ;
            // todo 平衡保底伤害系数
            int attIcDamage = (int)((attBuff - defBuff + attQ * 3) * attIC);
            int attMpDamage = (int)((attBuff - defBuff + attQ * 3) * attMP);
            int defIcDamage = (int)((attBuff - defBuff + defQ * 3) * defIC);
            int defMpDamage = (int)((attBuff - defBuff + defQ * 3) * defMP);


            float deflostIC = attIcDamage / defIC >= 1 ? 1 : attIcDamage / defIC;
            float attlostIC = defIcDamage / attIC >= 1 ? 1 : defIcDamage / attIC;


            int captureInf = (int)(deflostIC * Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].GridData
                .infantryEquipment + (int)attlostIC * Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y]
                .GridData.infantryEquipment);
            int captureArt = (int)(deflostIC * Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].GridData
                .artilleryEquipment + (int)attlostIC * Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y]
                .GridData.artilleryEquipment);
            int captureArm = (int)(deflostIC * Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].GridData
                .armorEquipment + (int)attlostIC * Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y]
                .GridData.armorEquipment);

            if (attIcDamage >= defIC || attMpDamage >= defMP) // 击败敌军
            {
                Victory_S();
            }
            else if (attIcDamage * 2 >= defIC || attMpDamage * 2 >= defMP) // 击退敌军
            {
                Victory_A();
            }
            else if (defIcDamage >= attIC || defMpDamage >= attMP) //被击败
            {
                Defeat();
            }

            FightEnd();
        }

        public void FightEnd()
        {
            Debug.Log("打完了");
            // 更新数据
            IsEnableAimHex = false;
            selectedHexCoordinates = new HexCoordinates();
            aimHexes.Clear();
            ResetHex();
            state = State.Default;
        }

        public void Defeat()
        {
            Debug.Log("我军败了");
            Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y] = new DivisionObject(Ownership.Null);
        }


        public void Victory_A()
        {
            Debug.Log("击退敌军");

            List<HexCoordinates> retractableHexs = new List<HexCoordinates>();
            retractableHexs = GetMovAbleHexes(aimHexCoordinates,
                Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].HexesOwnership);
            if (retractableHexs is not null)
            {
                foreach (var coor in retractableHexs)
                {
                    Swap(aimHexCoordinates, coor);
                    if (Divisions[aimHexCoordinates.X, aimHexCoordinates.Y].HexesOwnership == Ownership.Null)
                    {
                        Divisions[aimHexCoordinates.X, aimHexCoordinates.Y] =
                            Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y];
                        Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y] =
                            new DivisionObject(Ownership.Null);
                    }

                    break;
                }
            }
            else
            {
                Debug.Log("无路可走");
                Divisions[aimHexCoordinates.X, aimHexCoordinates.Y] =
                    Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y];
                Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y] = new DivisionObject(Ownership.Null);
            }
        }

        public void Victory_S()
        {
            Debug.Log("大败敌军");
            Divisions[aimHexCoordinates.X, aimHexCoordinates.Y] =
                Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y];
            Divisions[selectedHexCoordinates.X, selectedHexCoordinates.Y] = new DivisionObject(Ownership.Null);
        }

        private void Victory_A(HexCoordinates attHexCoordinates, HexCoordinates defHexCoordinates)
        {
            Debug.Log("击退敌军");

            List<HexCoordinates> retractableHexs = new List<HexCoordinates>();
            retractableHexs = GetMovAbleHexes(defHexCoordinates,
                Divisions[defHexCoordinates.X, defHexCoordinates.Y].HexesOwnership);
            if (retractableHexs is not null)
            {
                foreach (var coor in retractableHexs)
                {
                    Swap(defHexCoordinates, coor);
                    if (Divisions[defHexCoordinates.X, defHexCoordinates.Y].HexesOwnership == Ownership.Null)
                    {
                        Divisions[defHexCoordinates.X, defHexCoordinates.Y] =
                            Divisions[attHexCoordinates.X, attHexCoordinates.Y];
                        Divisions[attHexCoordinates.X, attHexCoordinates.Y] =
                            new DivisionObject(Ownership.Null);
                    }

                    break;
                }
            }
            else
            {
                Debug.Log("无路可走");
                Divisions[defHexCoordinates.X, defHexCoordinates.Y] =
                    Divisions[attHexCoordinates.X, attHexCoordinates.Y];
                Divisions[attHexCoordinates.X, attHexCoordinates.Y] = new DivisionObject(Ownership.Null);
            }
        }

        private void Victory_S(HexCoordinates attHexCoordinates, HexCoordinates defHexCoordinates)
        {
            Debug.Log("大败敌军");
            Divisions[attHexCoordinates.X, attHexCoordinates.Y] =
                Divisions[defHexCoordinates.X, defHexCoordinates.Y];
            Divisions[defHexCoordinates.X, defHexCoordinates.Y] = new DivisionObject(Ownership.Null);
        }

        /// <summary>
        /// 选中单元格后出现命令菜单
        /// </summary>
        /// <param name="coordinates">单元格</param>
        private void SelectHex(HexCoordinates coordinates)
        {
            switch (Divisions[coordinates.X, coordinates.Y].HexesOwnership)
            {
                case Ownership.Friend:

                    GameObject buttonPanel = Instantiate(Resources.Load<GameObject>("ButtonPanel"),
                        Functions.Functions.GetMouseWorldPosition() - new Vector3(-pfwidth, 0, 5), Quaternion.identity);
                    selectedHexCoordinates = coordinates;
                    buttonPanel.GetComponent<ButtonPanel>().Initialize(
                        Divisions[coordinates.X, coordinates.Y].GridData.name,
                        new List<(string name, Action action)>()
                        {
                            ("移动", () =>
                            {
                                state = State.Move;
                                MoveAbleAimHex();
                                Destroy(buttonPanel);
                            }),
                            ("进攻", () =>
                            {
                                state = State.Attack;
                                AttackAbleAimHex();
                                Destroy(buttonPanel);
                            }),
                            ("撤编", () =>
                            {
                                Withdraw(coordinates);
                                Destroy(buttonPanel);
                            }),
                            ("取消", () => { Destroy(buttonPanel); })
                        });
                    break;
            }
        }


        void ChangeAdjacentHexesColor(List<HexCoordinates> adjacentCoords, Color color)
        {
            foreach (var adjacentCoord in adjacentCoords)
            {
                if (adjacentCoord.X >= 0 && adjacentCoord.X < width && adjacentCoord.Y >= 0 && adjacentCoord.Y < height)
                {
                    GameObject adjacentHex = hexes[adjacentCoord.X, adjacentCoord.Y];
                    Image adjacentImage = adjacentHex.GetComponentInChildren<Image>();
                    if (adjacentImage is not null)
                    {
                        hexes[adjacentCoord.X, adjacentCoord.Y].GetComponentInChildren<Image>().color = color;
                    }
                }
            }
        }


        List<HexCoordinates> GetAdjacentHexes(HexCoordinates coords)
        {
            List<HexCoordinates> adjacentHexes = new List<HexCoordinates>();

            // Add all possible adjacent hex coordinates
            adjacentHexes.Add(new HexCoordinates(coords.X - 1, coords.Y));
            adjacentHexes.Add(new HexCoordinates(coords.X + 1, coords.Y));
            adjacentHexes.Add(new HexCoordinates(coords.X, coords.Y - 1));
            adjacentHexes.Add(new HexCoordinates(coords.X, coords.Y + 1));
            adjacentHexes.Add(new HexCoordinates(coords.X + (coords.Y % 2 == 0 ? -1 : 1), coords.Y + 1));
            adjacentHexes.Add(new HexCoordinates(coords.X + (coords.Y % 2 == 0 ? -1 : 1), coords.Y - 1));

            return adjacentHexes;
        }


        /// <summary>
        /// 获取可移动的单元格
        /// </summary>
        /// <param name="coords">中心单元格</param>
        /// <param name="ownership">中心单元格所有者</param>
        /// <returns>可移动的单元格列表</returns>
        public List<HexCoordinates> GetMovAbleHexes(HexCoordinates coords, Ownership ownership)
        {
            List<HexCoordinates> adjacentHexes = new List<HexCoordinates>();
            if (Divisions[coords.X, coords.Y].Movepoint == 0)
            {
                Functions.Functions.CreateTip("没有行动点", GetWorldPostion(coords.X, coords.Y), 2f);
                return null;
            }

            int xOffset = coords.Y % 2 == 0 ? -1 : 1;

            // 定义六个可能的相邻六边形方向
            (int x, int y)[] directions =
            {
                (-1, 0), // 左
                (1, 0), // 右
                (0, -1), // 下
                (0, 1), // 上
                (xOffset, 1), // 根据Y坐标的奇偶性决定是上右或上左
                (xOffset, -1) // 根据Y坐标的奇偶性决定是下右或下左
            };

            // 计算并添加符合条件的相邻六边形坐标
            foreach (var (dx, dy) in directions)
            {
                HexCoordinates newCoords = new HexCoordinates(coords.X + dx, coords.Y + dy);
                // 检查边界和所有权
                if (IsWithinBounds(newCoords, Divisions.GetLength(0), Divisions.GetLength(1)))
                {
                    switch (Divisions[newCoords.X, newCoords.Y].HexesOwnership)
                    {
                        case Ownership.Enemy:
                            if (ownership == Ownership.Enemy)
                            {
                                adjacentHexes.Add(newCoords);
                            }

                            break;
                        case Ownership.Friend:
                            if (ownership == Ownership.Friend)
                            {
                                adjacentHexes.Add(newCoords);
                            }

                            break;
                        case Ownership.Null:
                            adjacentHexes.Insert(0, newCoords);
                            break;
                    }
                }
            }

            return adjacentHexes;
        }


        /// <summary>
        /// 获取可攻击的单元格
        /// </summary>
        /// <param name="coords">中心单元格</param>
        /// <returns></returns>
        public List<HexCoordinates> GetAttackAbleHexes(HexCoordinates coords)
        {
            List<HexCoordinates> adjacentHexes = new List<HexCoordinates>();
            int xOffset = coords.Y % 2 == 0 ? -1 : 1;

            // Define the six possible directions for adjacent hexes
            (int x, int y)[] directions =
            {
                (-1, 0), // Left
                (1, 0), // Right
                (0, -1), // Bottom
                (0, 1), // Top
                (xOffset, 1), // Top-right or Top-left based on parity
                (xOffset, -1) // Bottom-right or Bottom-left based on parity
            };
            Ownership ownership = Divisions[coords.X, coords.Y].HexesOwnership;
            // Calculate and add the adjacent hex coordinates if they meet the condition
            foreach (var (dx, dy) in directions)
            {
                HexCoordinates newCoords = new HexCoordinates(coords.X + dx, coords.Y + dy);
                // Check bounds and ownership before adding
                if (IsWithinBounds(newCoords, Divisions.GetLength(0), Divisions.GetLength(1)))
                {
                    switch (Divisions[newCoords.X, newCoords.Y].HexesOwnership)
                    {
                        case Ownership.Enemy:
                            if (ownership == Ownership.Friend)
                            {
                                adjacentHexes.Add(newCoords);
                            }

                            break;
                        case Ownership.Friend:
                            if (ownership == Ownership.Enemy)
                            {
                                adjacentHexes.Add(newCoords);
                            }

                            break;
                        case Ownership.Null:
                            break;
                    }
                }
            }

            return adjacentHexes;
        }

        private bool IsWithinBounds(HexCoordinates coords, int width, int height)
        {
            return coords.X >= 0 && coords.X < width && coords.Y >= 0 && coords.Y < height;
        }


        void ResetHex()
        {
            for (var x = 0; x < hexes.GetLength(0); x++)
            {
                for (var y = 0; y < hexes.GetLength(1); y++)
                {
                    SetHexColorAndIcon(x, y);
                }
            }
        }

        void SetHexColorAndIcon(int x, int y)
        {
            var hex = hexes[x, y];
            Image image = hex.GetComponentInChildren<Image>();
            Image icon = hex.transform.Find("icon").GetComponent<Image>();

            // 设置颜色和图标可见性
            var ownership = Divisions[x, y].HexesOwnership;
            image.color = GetColorByOwnership(ownership);
            icon.enabled = (ownership != Ownership.Null);

            // 如果不是 Null 所有权，设置相应的图标
            if (ownership != Ownership.Null)
            {
                icon.sprite = GetIconByBattalionType(Divisions[x, y].GridData);
            }
        }

        Color GetColorByOwnership(Ownership ownership)
        {
            switch (ownership)
            {
                case Ownership.Enemy: return Color.red;
                case Ownership.Friend: return Color.green;
                default: return Color.white;
            }
        }

        Sprite GetIconByBattalionType(GridData gridData)
        {
            var iconPathDictionary = new Dictionary<BattalionData.BattalionTypes, string>
            {
                { BattalionData.BattalionTypes.Infantry, "Icon/Inf" },
                { BattalionData.BattalionTypes.Artillery, "Icon/Art" },
                { BattalionData.BattalionTypes.Armor, "Icon/Arm" }
            };

            if (iconPathDictionary.TryGetValue(gridData.Type, out var path))
            {
                path += gridData.level switch
                {
                    1 => "_1",
                    2 => "_2",
                    _ => "_3"
                };
                return Resources.Load<Sprite>(path);
            }

            return null; // 或者返回一个默认的图标
        }


        HexCoordinates GetHexCoordinates(Vector2 worldPos)
        {
            // 将世界坐标转换为本地坐标
            Vector2 gridPos = transform.InverseTransformPoint(new Vector3(worldPos.x, worldPos.y, 0));

            // 考虑预制体的长宽来计算六边形的X和Y坐标
            int y = Mathf.RoundToInt(gridPos.y / (0.66f * pfheight));
            int x = Mathf.RoundToInt((gridPos.x / pfwidth - (y % 2 == 1 ? 0.38f : 0)) / 0.76f);

            return new HexCoordinates(x, y);
        }

        public Vector3 GetWorldPostion(int x, int y)
        {
            return hexes[x, y].transform.position;
        }

        /// <summary>
        /// 初始化敌军部署位置
        /// </summary>
        /// <param name="seed"></param>
        private void EnemyInit(int seed)
        {
            int y = 7 - seed % 4;
            int x = 0;
            int xHQ = seed % 10;
            int yHQ = height - 1;
            while (x < width)
            {
                // 放置敌军
                Divisions[x, y] = new DivisionObject(Ownership.Enemy, Functions.Functions.LoadByJson("填线宝宝"));
                aimCoordinatesList.Add((new HexCoordinates(x, y), 1));
                int t = seed % 3;
                if (t == 1)
                {
                    x += 1;
                }
                else
                {
                    if (y % 2 == 1) x += 1;
                    switch (t)
                    {
                        case 0 when y < height - 1:
                            y += 1;
                            break;
                        case 2 when y > 0:
                            y -= 1;
                            break;
                    }
                }

                seed = seed / 7 * 5;
            }

            Divisions[xHQ, yHQ] = new DivisionObject(Ownership.Enemy, Functions.Functions.LoadByJson("Hq"));
            if (xHQ - 1 > 0)
            {
                Divisions[xHQ - 1, yHQ] = new DivisionObject(Ownership.Enemy, Functions.Functions.LoadByJson("装甲掷弹兵"));
            }

            if (xHQ + 1 < width)
            {
                Divisions[xHQ + 1, yHQ] = new DivisionObject(Ownership.Enemy, Functions.Functions.LoadByJson("装甲掷弹兵"));
            }
        }

        public void NextTurn()
        {
            for (var x = 0; x < hexes.GetLength(0); x++)
            {
                for (var y = 0; y < hexes.GetLength(1); y++)
                {
                    SetHexColorAndIcon(x, y);
                    if (Divisions[x, y].HexesOwnership == Ownership.Null)
                    {
                        Divisions[x, y].Movepoint = -1;
                    }
                    else
                    {
                        Divisions[x, y].Movepoint = 1;
                    }
                }
            }
        }


        private HexCoordinates FindPath(HexCoordinates originCoordinates, HexCoordinates targetCoordinates)
        {
            int distance = GetDistance(originCoordinates, targetCoordinates);
            Debug.Log("distance:" + distance);
            HexCoordinates moveto = originCoordinates;
            List<HexCoordinates> list = GetMovAbleHexes(originCoordinates,
                Divisions[originCoordinates.X, originCoordinates.Y].HexesOwnership);
            foreach (var coor in list)
            {
                if (distance <= GetDistance(coor, targetCoordinates)) continue;
                distance = GetDistance(coor, targetCoordinates);
                Debug.Log("new distance:" + distance);
                moveto = coor;
            }

            return moveto;
        }

        // private int GetDistance(HexCoordinates originCoordinates, HexCoordinates targetCoordinates)
        // {
        //     int dx = Math.Abs(targetCoordinates.X - originCoordinates.X);
        //     int dy = Math.Abs(targetCoordinates.Y - originCoordinates.Y);
        //     int penalty =
        //         ((originCoordinates.Y % 2 != 0 && targetCoordinates.Y % 2 == 0 &&
        //           originCoordinates.X < targetCoordinates.X) || (targetCoordinates.Y % 2 != 0 &&
        //                                                          originCoordinates.Y % 2 == 0 &&
        //                                                          targetCoordinates.X < originCoordinates.X))
        //             ? 1
        //             : 0;
        //     Debug.Log("dx:"+dx+",dy:"+dy+",penalty:"+penalty);
        //     int result = Math.Max(dy, dx + penalty*(dy/2));
        //
        //     return result;
        // }
        private int GetDistance(HexCoordinates originCoordinates, HexCoordinates targetCoordinates)
        {
            Vector3Int o = evenr_to_cube(new Vector2Int(originCoordinates.X, originCoordinates.Y));
            Vector3Int t = evenr_to_cube(new Vector2Int(targetCoordinates.X, targetCoordinates.Y));
            return cube_distance(o, t);
        }

        private Vector2Int cube_to_evenr(Vector3Int v3)
        {
            var col = v3.x + (v3.z - (v3.z & 1)) / 2;
            var row = v3.z;
            return new Vector2Int(col, row);
        }


        private Vector3Int evenr_to_cube(Vector2Int v2)
        {
            var x = v2.x - (v2.y - (v2.y & 1)) / 2;
            var z = v2.y;
            var y = -x - z;
            return new Vector3Int(x, y, z);
        }

        private int cube_distance(Vector3Int a, Vector3Int b)
        {
            return Math.Max(Math.Max(Math.Abs(a.x - b.x), Math.Abs(a.y - b.y)), Math.Abs(a.z - b.z));
        }
    }


    public struct DivisionObject
    {
        public Ownership HexesOwnership;
        public GridData GridData;
        public int Movepoint;
        public HexCoordinates AimCoordinates;


        public DivisionObject(Ownership hexesOwnership, GridData gridData) : this()
        {
            this.HexesOwnership = hexesOwnership;
            GridData = gridData;
            Movepoint = 1;
        }

        public DivisionObject(Ownership ownership = Ownership.Null)
        {
            HexesOwnership = ownership;
            GridData = null;
            Movepoint = 0;
            AimCoordinates = new HexCoordinates();
        }
    }

    public struct HexCoordinates
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public HexCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsEqual(HexCoordinates hexCoordinates)
        {
            return X == hexCoordinates.X && Y == hexCoordinates.Y;
        }
    }
}