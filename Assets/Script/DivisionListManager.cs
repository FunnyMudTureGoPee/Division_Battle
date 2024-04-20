using System.Collections.Generic;
using System.IO;
using Script.Battalion;
using Script.Grid;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class DivisionListManger : MonoBehaviour
    {
        public string folderPath = "Assets/SaveFiles/user";
        [SerializeField] private GameObject pfDivisionItem;
        [SerializeField] private GameObject friendPanel;
        [SerializeField] private float offset;
        private Transform context;
        private List<GridData> GridDatas = new List<GridData>();

        void Start()
        {
            ExploreDirectory(folderPath);
            context = gameObject.GetComponent<ScrollRect>().content;
            CreatItem();
        }

        private void CreatItem()
        {
            foreach (Transform o in context)
            {
                Destroy(o);
            }

            Vector3 offset = new Vector3(0, -this.offset, 0);
            foreach (var gridData in GridDatas)
            {
                GameObject item = Instantiate(pfDivisionItem, context);
                item.transform.position += offset;
                offset += new Vector3(0, -item.GetComponent<RectTransform>().rect.height - this.offset, 0);
                switch (gridData.Type)
                {
                    case BattalionData.BattalionTypes.Infantry:
                        item.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Inf");
                        break;
                    case BattalionData.BattalionTypes.Artillery:
                        item.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Art");
                        break;
                    case BattalionData.BattalionTypes.Armor:
                        item.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Arm");
                        break;
                }

                item.transform.Find("Name").GetComponent<Text>().text = gridData.name;
                GridData gd = gridData;
                Debug.Log(gd.name);
                item.transform.Find("Buttons").Find("Edit").GetComponent<Button>().onClick.AddListener(delegate
                {
                    LoadDivision(gd);
                });
                string s = gridData.name;
                item.transform.Find("Buttons").Find("Delete").GetComponent<Button>().onClick.AddListener(delegate
                {
                    DeleteDivision(s, item);
                });
            }
        }

        private void LoadDivision(GridData gridData)
        {
            friendPanel.transform.Find("DivisionManger").GetComponent<DivisionManger>().LoadDivision(gridData);
        }
        private void DeleteDivision(string name, GameObject itself)
        {
            string filePath = folderPath + "/" + name;
            // 检查文件是否存在
            if (File.Exists(filePath))
            {
                // 如果文件存在，删除它
                File.Delete(filePath);
            }
            else
            {
                Debug.LogError("文件不存在: " + filePath);
            }

            Destroy(itself);
            CreatItem();
        }

        void ExploreDirectory(string path)
        {
            // 确保路径存在
            if (Directory.Exists(path))
            {
                // 获取指定路径下的所有文件
                FileInfo[] files = new DirectoryInfo(path).GetFiles("*", SearchOption.TopDirectoryOnly);

                // 遍历所有文件
                foreach (FileInfo file in files)
                {
                    if (file.Name.EndsWith(".meta"))continue;
                    // 打印文件的路径
                    Debug.Log("文件名: " + file.Name);
                    
                    GridDatas.Add(Functions.Functions.LoadByJson("user/" + file.Name));
                }
            }
            else
            {
                Debug.LogError("目录不存在: " + path);
            }
        }
    }
}