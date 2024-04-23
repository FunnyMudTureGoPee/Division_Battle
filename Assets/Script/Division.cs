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
            switch (gridData.Type)
            {
                case BattalionData.BattalionTypes.Infantry:
                    transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Inf");
                    break;
                case BattalionData.BattalionTypes.Artillery:
                    transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Art");
                    break;
                case BattalionData.BattalionTypes.Armor:
                    transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Arm");
                    break;
            }

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