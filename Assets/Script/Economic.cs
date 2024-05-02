using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class Inventory
    {
        private int value;
        public Text _text;

        public int Value
        {
            get => value;
            set=> this.value = value;
                
            
        }

        public Inventory(Transform transform)
        {
            value = 0;
            _text = transform.Find("value").GetComponent<Text>();
        }

        private void RefreshText()
        {
            _text.text = Value.ToString();
        }
    }

    public class Factory
    {
        public enum EquipmemntType
        {
            Infantry,
            Artillery,
            Armor,
            Manpower
        }

        public Inventory Inventory;
        public EquipmemntType Type;
        public int IC = 500;
        public int value;
        public Text _text;
        public Button increase;
        public Button reduce;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform">this</param>
        /// <param name="name">Inf Art Arm Manpower</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Factory(Transform transform, string name)
        {
            value = 0;
            var find = transform.Find("Factory").Find("Data").Find(name + "F");
            _text = find.Find("value").GetComponent<Text>();
            increase = find.Find("increase").GetComponent<Button>();
            reduce = find.Find("reduce").GetComponent<Button>();
            Inventory = new Inventory(transform.Find("Inventory").Find("Data").Find(name + "I"));
            switch (name)
            {
                case "Inf":
                    Type = EquipmemntType.Infantry;
                    break;
                case "Art":
                    Type = EquipmemntType.Artillery;
                    break;
                case "Arm":
                    Type = EquipmemntType.Armor;
                    break;
                case "Manpower":
                    Type = EquipmemntType.Manpower;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transform.name));
            }

            RefreshText();
        }

        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;
                RefreshText(); // 当值改变时自动更新文本
            }
        }

        // 新的方法来处理重复逻辑
        private int CalculateProduction()
        {
            return Type switch
            {
                EquipmemntType.Infantry => (int)(Value * IC / 0.5),
                EquipmemntType.Artillery => (int)(Value * IC / 5),
                EquipmemntType.Armor => (int)(Value * IC / 10),
                EquipmemntType.Manpower => (int)(Value * IC / 0.25),
                _ => 0
            };
        }

        public void NextTurn()
        {
            Inventory.Value += CalculateProduction();
            RefreshText();
        }

        public void RefreshText()
        {
            _text.text = Value.ToString();
            Inventory._text.text = $"{Inventory.Value}+{CalculateProduction()}";
        }
    }

    public class Economic : MonoBehaviour
    {
        [SerializeField] private int factoryNumber;
        [SerializeField] private int IC;
        [SerializeField] private int manpower;
        [SerializeField] private bool isDisplay;
        [SerializeField] private Vector3 nonDisplayVector3;

        public Factory[] factories;


        public int InfInventory
        {
            get => factories[0].Inventory.Value;
            set => factories[0].Inventory.Value = value;
        }

        public int ArtInventory
        {
            get => factories[1].Inventory.Value;
            set => factories[1].Inventory.Value = value;
        }

        public int ArmInventory
        {
            get => factories[2].Inventory.Value;
            set => factories[2].Inventory.Value = value;
        }

        public int ManpowerInventory
        {
            get => factories[3].Inventory.Value;
            set => factories[3].Inventory.Value = value;
        }


        private void Awake()
        {
            var thisTransform = transform;
            // 初始化工厂对象的数组
            factories = new[]
            {
                new Factory(thisTransform, "Inf"),
                new Factory(thisTransform, "Art"),
                new Factory(thisTransform, "Arm"),
                new Factory(thisTransform, "Manpower")
            };

            // 使用循环来注册按钮事件
            foreach (var factory in factories)
            {
                factory.increase.onClick.AddListener(delegate { Increase(factory); });
                factory.reduce.onClick.AddListener(delegate { Reduce(factory); });
            }

            RefreshFactoryNumber();
        }

        public void NextTurn()
        {
            foreach (var factory in factories)
            {
                Debug.Log("Debug:下一回合");
                factory.NextTurn();
            }
        }

        public void Increase(Factory factory)
        {
            if (factoryNumber == 0) return;

            factoryNumber--;
            factory.value++;
            factory.RefreshText();

            RefreshFactoryNumber();
        }


        public void Reduce(Factory factory)
        {
            if (factory.value == 0) return;
            factoryNumber++;
            factory.value--;
            factory.RefreshText();

            RefreshFactoryNumber();
        }

        private void RefreshFactoryNumber()
        {
            var factoryText = transform.Find("Factory").Find("Number").GetComponent<Text>();
            factoryText.text = $"空闲{factoryNumber}个生产单元";
        }

        public void RefreshInventory()
        {
            foreach (var varFactory in factories)
            {
                varFactory.RefreshText();
            }
        }

        public void SwitchDisplayStatus()
        {
            if (isDisplay)
            {
                transform.DOMove(nonDisplayVector3, 0.5f);
                isDisplay = !isDisplay;
            }
            else
            {
                transform.DOMove(new Vector3(0, 0, 0), 0.5f);
                isDisplay = !isDisplay;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextTurn();
            }
        }
    }
}