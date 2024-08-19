using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using LitJson;
using Script.Battalion;
using Script.Grid;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Script.Functions
{
    public class Functions
    {
        // Create a Sprite in the World, no parent
        public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale,
            int sortingOrder, Color color)
        {
            return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
        }

        // Create a Sprite in the World
        public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition,
            Vector3 localScale, int sortingOrder, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = color;
            return gameObject;
        }

        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }

        public static TextMesh CreateWorldText(string text, Transform parent = null,
            Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null,
            TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left,
            int sortingOrder = default)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment,
                sortingOrder);
        }

        // Create Text in the World
        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
            Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        public static GameObject CreateWorldTextReturnGameObject(string text, Transform parent = null,
            Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null,
            TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left,
            int sortingOrder = default)
        {
            if (color == null) color = Color.white;
            return CreateWorldTextReturnGameObject(parent, text, localPosition, fontSize, (Color)color, textAnchor,
                textAlignment, sortingOrder);
        }

        public static GameObject CreateWorldTextReturnGameObject(Transform parent, string text, Vector3 localPosition,
            int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return gameObject;
        }

        //用Json方法对数据进行保存和读取
        public static void SaveByJson(string filename, GridData gridData)
        {
            string datapath = Application.dataPath + "/SaveFiles" + "/" + filename + ".json";
            string dateStr = JsonMapper.ToJson(gridData); //利用JsonMapper将date转换成字符串
            StreamWriter sw = new StreamWriter(datapath); //创建一个写入流
            sw.Write(dateStr); //将dateStr写入
            sw.Close(); //关闭流
        }

        public static GridData LoadByJson(string filename)
        {
            string datePath;
            if (filename.EndsWith(".json"))
            {
                 datePath = Application.dataPath + "/SaveFiles" + "/" + filename;
            }
            else
            {
                datePath = Application.dataPath + "/SaveFiles" + "/" + filename + ".json";

            }
            if (File.Exists(datePath)) //判断这个路径里面是否为空
            {
                StreamReader sr = new StreamReader(datePath); //创建读取流;
                string jsonStr = sr.ReadToEnd(); //使用方法ReadToEnd（）遍历的到保存的内容
                sr.Close();
                GridData date = JsonMapper.ToObject<GridData>(jsonStr); //使用JsonMapper将遍历得到的jsonStr转换成Date对象
                return date;
            }
            else
            {
                Debug.Log("------未找到文件------");
                return null;
            }
        }

        public static void CreateTip(string text, Vector3 vector3, float time)
        {
            vector3.z = -2;
            GameObject gameObject = CreateWorldTextReturnGameObject(text, null, vector3, 100, Color.red);
            gameObject.AddComponent<MonoStub>();
            gameObject.GetComponent<MonoStub>().StartCoroutine(Timer(gameObject, time));
        }

        /// <summary>
        /// 创建一次性提示按钮
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="parent">父对象</param>
        /// <param name="localPosition">本地坐标</param>
        /// <param name="actions">行为列表（最后摧毁自身）</param>
        /// <returns>提示按钮对象</returns>
        public static GameObject CreateButtonTip(string text, Transform parent,
            Vector3 localPosition, List<Action> actions)
        {
            GameObject gameObject = new GameObject
            {
                name = "Button Tip",
                transform =
                {
                    parent = parent,
                    position = localPosition
                }
            };
            gameObject.AddComponent<Image>();
            gameObject.AddComponent<Button>();
            foreach (var action in actions)
            {
                gameObject.GetComponent<Button>().onClick.AddListener(()=>action());
            }
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                Object.Destroy(gameObject);
            });
            GameObject textobject = new GameObject("Text", typeof(Text))
            {
                transform = { parent = gameObject.transform }
            };
            textobject.GetComponent<Text>().text = text;
            textobject.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textobject.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            textobject.GetComponent<Text>().color = Color.black;
            textobject.GetComponent<Text>().fontSize = 80;
            textobject.GetComponent<Text>().resizeTextForBestFit = true;

            return gameObject;
        }

        public static int CalculateIC(BattalionData.BattalionTypes type)
        {
            switch (type)
            {
                case BattalionData.BattalionTypes.Infantry:
                    return (int)(100 * 0.5);
                    break;
                case BattalionData.BattalionTypes.Artillery:
                    return (int)(36 * 5);
                    break;
                case BattalionData.BattalionTypes.Armor:
                    return (int)(50 * 10);
                    break;
            }

            return 0;
        }
        public static int CalculateMapower(BattalionData.BattalionTypes type)
        {
            switch (type)
            {
                case BattalionData.BattalionTypes.Infantry:
                    return 1000;
                    break;
                case BattalionData.BattalionTypes.Artillery:
                    return 500;
                    break;
                case BattalionData.BattalionTypes.Armor:
                    return 500;
                    break;
            }

            return 0;
        }


        private class MonoStub : MonoBehaviour
        {
        }

        /// <summary>
        /// 计时销毁
        /// </summary>
        /// <param name="gameObject">被销毁的对象</param>
        /// <param name="time">计时时间</param>
        /// <returns></returns>
        private static IEnumerator Timer(GameObject gameObject, float time)
        {
            yield return new WaitForSeconds(time);
            GameObject.Destroy(gameObject);
        }
    }
}