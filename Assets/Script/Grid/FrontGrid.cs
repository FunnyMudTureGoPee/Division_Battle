using System;
using System.Collections.Generic;
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

    public class FrontGrid : MonoBehaviour
    {
        public Economic Economic;
        public GameObject hexPrefab;
        public int width = 10;
        public int height = 10;
        private float pfwidth;
        private float pfheight;
        private GameObject[,] hexes;
        private Ownership[,] hexesOwnerships;
        private GameObject selectedHex = null;
        private HexCoordinates selectedHexCoordinates;
        private List<HexCoordinates> aimHexes = new List<HexCoordinates>();
        private HexCoordinates aimHexCoordinates;
        private bool IsEnableAimHex;
        private GridData[,] GridDatas;
        private GridData selectedGridData;
        private bool isReadyArrange;

        public GridData SelectedGridData
        {
            get => selectedGridData;
            set => selectedGridData = value;
        }

        void Start()
        {
            hexes = new GameObject[width, height];
            hexesOwnerships = new Ownership[width, height];
            GridDatas = new GridData[width, height];
            pfwidth = hexPrefab.GetComponent<RectTransform>().rect.width;
            pfheight = hexPrefab.GetComponent<RectTransform>().rect.height;
            Economic = GameObject.Find("MainController").transform.Find("EconomicPanel").GetComponent<Economic>();
            CreateHexMap();
            EnemyInit(Random.Range(99999, 999999));
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
                    hexesOwnerships[x, y] = Ownership.Null;
                }
            }
        }

        void Update()
        {
            if (IsEnableAimHex)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                    if (hit.collider is not null)
                    {
                        Transform objectHit = hit.transform;
                        HexCoordinates coordinates = GetHexCoordinates(objectHit.position);
                        foreach (var hex in aimHexes)
                        {
                            if (hex.IsEqual(coordinates))
                            {
                                aimHexCoordinates = coordinates;
                                Move();
                                return;
                            }
                        }

                        Functions.Functions.CreateTip("超出移动范围", Functions.Functions.GetMouseWorldPosition(), 2f);
                    }
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider is not null)
                {
                    Transform objectHit = hit.transform;
                    SelectHex(GetHexCoordinates(objectHit.position));
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
                        ChangeAdjacentHexesColor(GetMovAbleHexes(coordinates), Color.yellow);
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
                ResetHex();
            }
        }

        private void EnableAimHex()
        {
            IsEnableAimHex = true;
            aimHexes = GetMovAbleHexes(selectedHexCoordinates);
            ChangeAdjacentHexesColor(aimHexes, Color.blue);
        }

        public void Move()
        {
            Swap(aimHexCoordinates,selectedHexCoordinates);
            IsEnableAimHex = false;
            selectedHexCoordinates = new HexCoordinates();
            aimHexes.Clear();
            ResetHex();
        }

        private void Swap(HexCoordinates hexCoordinate1, HexCoordinates hexCoordinate2)
        {
            (hexesOwnerships[hexCoordinate1.X, hexCoordinate1.Y], hexesOwnerships[hexCoordinate2.X, hexCoordinate2.Y]) =
                (hexesOwnerships[hexCoordinate2.X, hexCoordinate2.Y],
                    hexesOwnerships[hexCoordinate1.X, hexCoordinate1.Y]);
            (GridDatas[hexCoordinate1.X, hexCoordinate1.Y], GridDatas[hexCoordinate2.X, hexCoordinate2.Y]) =
                (GridDatas[hexCoordinate2.X, hexCoordinate2.Y],
                    GridDatas[hexCoordinate1.X, hexCoordinate1.Y]);
        }

        /// <summary>
        /// 部署友方单位
        /// </summary>
        /// <param name="coordinates"></param>
        public void Arrange(HexCoordinates coordinates)
        {
            if (selectedGridData is null)
            {
                Debug.Log("空");
                return;
            }

            if (Economic.InfInventory < selectedGridData.infantryEquipment ||
                Economic.ArtInventory < selectedGridData.artilleryEquipment ||
                Economic.ArmInventory < selectedGridData.armorEquipment ||
                Economic.ManpowerInventory < selectedGridData.Manpower)
            {
                Debug.Log("资源不足");
                return;
            }

            GridDatas[coordinates.X, coordinates.Y] = selectedGridData;
            hexesOwnerships[coordinates.X, coordinates.Y] = Ownership.Friend;
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
            Economic.InfInventory += GridDatas[coordinates.X, coordinates.Y].infantryEquipment;
            Economic.ArtInventory += GridDatas[coordinates.X, coordinates.Y].artilleryEquipment;
            Economic.ArmInventory += GridDatas[coordinates.X, coordinates.Y].armorEquipment;
            Economic.ManpowerInventory += GridDatas[coordinates.X, coordinates.Y].Manpower;
            Economic.RefreshInventory();
            GridDatas[coordinates.X, coordinates.Y] = null;
            hexesOwnerships[coordinates.X, coordinates.Y] = Ownership.Null;
            ResetHex();
        }

        /// <summary>
        /// 选中单元格后出现命令菜单
        /// </summary>
        /// <param name="coordinates">单元格</param>
        private void SelectHex(HexCoordinates coordinates)
        {
            switch (hexesOwnerships[coordinates.X, coordinates.Y])
            {
                case Ownership.Friend:

                    GameObject buttonPanel = Instantiate(Resources.Load<GameObject>("ButtonPanel"),
                        Functions.Functions.GetMouseWorldPosition() - new Vector3(-pfwidth, 0, 5), Quaternion.identity);
                    selectedHexCoordinates = coordinates;
                    buttonPanel.GetComponent<ButtonPanel>().Initialize(GridDatas[coordinates.X, coordinates.Y].name,
                        new List<(string name, Action action)>()
                        {
                            ("移动", () =>
                            {
                                EnableAimHex();
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
        /// <returns></returns>
        public List<HexCoordinates> GetMovAbleHexes(HexCoordinates coords)
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

            // Calculate and add the adjacent hex coordinates if they meet the condition
            foreach (var (dx, dy) in directions)
            {
                HexCoordinates newCoords = new HexCoordinates(coords.X + dx, coords.Y + dy);
                // Check bounds and ownership before adding
                if (IsWithinBounds(newCoords, hexesOwnerships.GetLength(0), hexesOwnerships.GetLength(1)))
                {
                    switch (hexesOwnerships[newCoords.X, newCoords.Y])
                    {
                        case Ownership.Enemy:
                            break;
                        case Ownership.Friend:
                            adjacentHexes.Add(newCoords); 
                            break;
                        case Ownership.Null:
                            adjacentHexes.Add(newCoords);
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
            var ownership = hexesOwnerships[x, y];
            image.color = GetColorByOwnership(ownership);
            icon.enabled = (ownership != Ownership.Null);

            // 如果不是 Null 所有权，设置相应的图标
            if (ownership != Ownership.Null)
            {
                icon.sprite = GetIconByBattalionType(GridDatas[x, y]);
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

        private void EnemyInit(int seed)
        {
            int y = 7 - seed % 4;
            int x = 0;
            while (x < width)
            {
                // 放置敌军
                hexesOwnerships[x, y] = Ownership.Enemy;
                GridDatas[x, y] = Functions.Functions.LoadByJson("填线宝宝");
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

                seed /= 7;
            }
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