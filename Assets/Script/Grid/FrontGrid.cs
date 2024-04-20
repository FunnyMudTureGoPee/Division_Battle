using System.Collections.Generic;
using UnityEngine;

namespace Script.Grid
{
    public class FrontGrid : MonoBehaviour
    {
        public GameObject hexPrefab;
        public int width = 10;
        public int height = 10;
        private float pfwidth;
        private float pfheight;
        private GameObject[,] hexes;
        private GameObject selectedHex = null;

        void Start()
        {
            hexes = new GameObject[width, height];
            pfwidth = hexPrefab.GetComponent<RectTransform>().rect.width;
            pfheight = hexPrefab.GetComponent<RectTransform>().rect.height;
            CreateHexMap();
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
                }
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    Transform objectHit = hit.transform;
                    TextMesh textMesh = objectHit.GetComponentInChildren<TextMesh>();
                    if (textMesh != null)
                    {
                        if (selectedHex != null)
                        {
                            // Reset the previous selection
                            ResetHexColors();
                        }

                        selectedHex = objectHit.gameObject;
                        textMesh.color = Color.red;
                        ChangeAdjacentHexesColor(objectHit.position, Color.yellow);
                    }
                }
            }
        }

        void ChangeAdjacentHexesColor(Vector2 position, Color color)
        {
            HexCoordinates coords = GetHexCoordinates(position);
            Debug.Log("Changing colors for Hex at: " + coords.X + "," + coords.Y);

            List<HexCoordinates> adjacentCoords = GetAdjacentHexes(coords);
            foreach (var adjacentCoord in adjacentCoords)
            {
                if (adjacentCoord.X >= 0 && adjacentCoord.X < width && adjacentCoord.Y >= 0 && adjacentCoord.Y < height)
                {
                    GameObject adjacentHex = hexes[adjacentCoord.X, adjacentCoord.Y];
                    TextMesh adjacentTextMesh = adjacentHex.GetComponentInChildren<TextMesh>();
                    if (adjacentTextMesh != null)
                    {
                        Debug.Log("Changing color of Hex at: " + adjacentCoord.X + "," + adjacentCoord.Y);
                        hexes[adjacentCoord.X, adjacentCoord.Y].GetComponentInChildren<TextMesh>().color = color;
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


        void ResetHexColors()
        {
            foreach (GameObject hex in hexes)
            {
                TextMesh textMesh = hex.GetComponentInChildren<TextMesh>();
                if (textMesh != null)
                {
                    textMesh.color = Color.white;
                }
            }
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
    }
}