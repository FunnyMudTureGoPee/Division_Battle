using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ButtonPanelFactory
    {
        public static Transform CreateButtonPanel(Transform parentTransform, Vector2 buttonSize, float buttonSpacing,
            List<Action> actions,
            GameObject buttonPrefab)
        {
            // GridLayoutGroup并设置属性

            GridLayoutGroup gridLayout = parentTransform.gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = buttonSize;
            gridLayout.spacing = new Vector2(0, buttonSpacing); // 假设按钮间距为10

            // 创建并添加按钮
            foreach (var action in actions)
            {
                GameObject buttonObject = UnityEngine.Object.Instantiate(buttonPrefab, parentTransform);
                buttonObject.GetComponent<Button>().onClick.AddListener(() => action());
            }

            return parentTransform;
        }

        public static Transform CreateButtonPanel(Transform parentTransform, Vector2 buttonSize, float buttonSpacing,
            List<(string name, Action action)> actions,
            GameObject buttonPrefab)
        {
            // GridLayoutGroup并设置属性

            GridLayoutGroup gridLayout = parentTransform.gameObject.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = buttonSize;
            gridLayout.spacing = new Vector2(0, buttonSpacing); // 假设按钮间距为10

            // 创建并添加按钮
            foreach (var action in actions)
            {
                GameObject buttonObject = UnityEngine.Object.Instantiate(buttonPrefab, parentTransform);
                buttonObject.GetComponent<Button>().onClick.AddListener(() => action.action());
                buttonObject.transform.Find("text").GetComponent<Text>().text = action.name;
            }

            return parentTransform;
        }
    }

    public class ButtonPanel : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab; // 按钮预制体
        [SerializeField] private Vector2 buttonSize;
        [SerializeField] private float buttonSpacing;
        private Transform buttonList;
        private Text title;


        private void Awake()
        {
            buttonList = transform.Find("ButtonList");
            title = transform.Find("Title").GetComponent<Text>();
        }

        public void Initialize(List<Action> actions)
        {
            Vector2 panelSize = new Vector2(buttonSize.x + 10 * 2, actions.Count * (buttonSize.y + buttonSpacing) + 10);
            buttonList =
                ButtonPanelFactory.CreateButtonPanel(buttonList, buttonSize, buttonSpacing, actions, buttonPrefab);

            transform.GetComponent<RectTransform>().sizeDelta = panelSize +
                                                                new Vector2(0,
                                                                    transform.Find("Title")
                                                                        .GetComponent<RectTransform>().rect.height);
        }
        
        public void Initialize( List<(string name, Action action)> actions)
        {
            Vector2 panelSize = new Vector2(buttonSize.x + 10 * 2, actions.Count * (buttonSize.y + buttonSpacing) + 10);
            buttonList =
                ButtonPanelFactory.CreateButtonPanel(buttonList, buttonSize, buttonSpacing, actions, buttonPrefab);

            transform.GetComponent<RectTransform>().sizeDelta = panelSize +
                                                                new Vector2(0,
                                                                    transform.Find("Title")
                                                                        .GetComponent<RectTransform>().rect.height);
        }

        public void Test()
        {
            Initialize(new List<Action>()
            {
                () =>
                {
                    Debug.Log("第1个按钮被点击");
                    Destroy(gameObject);
                },
                () =>
                {
                    Debug.Log("第2个按钮被点击");
                    Destroy(gameObject);
                },
                () =>
                {
                    Debug.Log("第2个按钮被点击");
                    Destroy(gameObject);
                },
            });
        }
    }
}