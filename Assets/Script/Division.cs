using System;
using Script.Battalion;
using Script.Grid;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class Division : MonoBehaviour
    {
        private GridData gridData;
        private DivisionListManger DLM;
        private FrontGrid FG;

        private void Awake()
        {
            DLM = GameObject.Find("DivisionList").GetComponent<DivisionListManger>();
            FG = GameObject.Find("FrontGrid").GetComponent<FrontGrid>();
        }

        public void LoadData()
        {
            string path = gridData.Type switch
            {
                BattalionData.BattalionTypes.Infantry => "Icon/Inf",
                BattalionData.BattalionTypes.Artillery => ("Icon/Art"),
                _ => ("Icon/Arm")
            };

            path += gridData.level switch
            {
                1 => "_1",
                2 => "_2",
                _ => "_3"
            };
            transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(path);

                transform.Find("Name").GetComponent<Text>().text = gridData.name;
            transform.Find("Cost").Find("manpower").Find("value").GetComponent<Text>().text =
                gridData.Manpower + "";
            transform.Find("Cost").Find("ic").Find("value").GetComponent<Text>().text =
                gridData.IC + "";
            GridData gd = gridData;
            transform.Find("Buttons").Find("Edit").GetComponent<Button>().onClick.AddListener(delegate
            {
                DLM.LoadDivision(gd);
            });
            string s = gridData.name;
            transform.Find("Buttons").Find("Delete").GetComponent<Button>().onClick.AddListener(delegate
            {
                DLM.DeleteDivision(s, gameObject);
            });
            transform.GetComponent<Button>().onClick.AddListener(delegate
            {
                FG.SelectedGridData = gridData;
            });
        }

        public GridData GridData
        {
            get => gridData;
            set => gridData = value;
        }
    }
}