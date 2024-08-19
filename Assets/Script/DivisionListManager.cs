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
            context = gameObject.GetComponent<ScrollRect>().content;
            Refresh();
        }

        public void Refresh()
        {
            ExploreDirectory(folderPath);

            CreatItem();
        }

        private void CreatItem()
        {
            foreach (Transform o in context)
            {
                Destroy(o.gameObject);
            }

            Vector3 offset = new Vector3(0, -this.offset, 0);
            foreach (var gridData in GridDatas)
            {
                GameObject item = Instantiate(pfDivisionItem, context);
                item.transform.position += offset;
                offset += new Vector3(0, -item.GetComponent<RectTransform>().rect.height - this.offset, 0);
                item.GetComponent<Division>().GridData = gridData;
                item.GetComponent<Division>().LoadData();
            }
        }

        public void LoadDivision(GridData gridData)
        {
            friendPanel.SetActive(true);
            friendPanel.transform.Find("DivisionManger").GetComponent<DivisionManger>().LoadDivision(gridData);
        }

        public void DeleteDivision(string name, GameObject itself)
        {
            string filePath = folderPath + "/" + name+".json";
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
            Refresh();
        }

        void ExploreDirectory(string path)
        {
            GridDatas.Clear();
            // 确保路径存在
            if (Directory.Exists(path))
            {
                // 获取指定路径下的所有文件
                FileInfo[] files = new DirectoryInfo(path).GetFiles("*", SearchOption.TopDirectoryOnly);

                // 遍历所有文件
                foreach (FileInfo file in files)
                {
                    if (file.Name.EndsWith(".meta")) continue;

                    GridDatas.Add(Functions.Functions.LoadByJson("user/" + file.Name));
                }
            }
            else
            {
                Debug.LogError("目录不存在: " + path);
            }
        }

        public void CreateNewDivision()
        {
            friendPanel.SetActive(true);
            friendPanel.transform.Find("DivisionManger").GetComponent<DivisionManger>().InitDivision();
        }
    }
}