using System.Collections.Generic;
using Script.Grid;
using UnityEngine;

namespace Script
{
    public class test : MonoBehaviour
    {
        public GameObject GridManger1;
        public GameObject GridManger2;
        public GameObject inf1;
        public GameObject inf2;

        public void Save()
        {
            GridManger1.GetComponent<DivisionManger>().SaveDivision("");
        }

        public void Refresh()
        {
            List<GameObject> checkObjects = new List<GameObject>();
            var objects = GridManger2.GetComponent<DivisionManger>().Grid.Battalions;
            for (var i0 = 0; i0 < objects.GetLength(0); i0++)
            for (var i1 = 0; i1 < objects.GetLength(1); i1++)
            {
                var VARIABLE = objects[i0, i1];
                if (VARIABLE is null) continue;
                GridManger2.GetComponent<DivisionManger>().Grid.DebugTextArray[i0, i1].color = Color.red;
                if (checkObjects.Count == 0)
                {
                    checkObjects.Add(VARIABLE);
                    GameObject gameObject =
                        Instantiate(VARIABLE, GridManger2.transform.parent.Find("BattalionList"), false);
                    gameObject.GetComponent<Battalion.Battalion>().BattalionData.Dir =
                        VARIABLE.GetComponent<Battalion.Battalion>().BattalionData.Dir;
                    gameObject.GetComponent<Battalion.Battalion>().BattalionData.LoadBattalion(1,
                        VARIABLE.GetComponent<Battalion.Battalion>().BattalionData.BattalionType);
                }

                bool b = true;
                foreach (var checkObject in checkObjects)
                {
                    if (VARIABLE == checkObject)
                    {
                        b = false;
                    }
                }

                if (!b) continue;
                checkObjects.Add(VARIABLE);
                GameObject go = Instantiate(VARIABLE, GridManger2.transform.parent.Find("BattalionList"),
                    false);
                go.GetComponent<Battalion.Battalion>().BattalionData.Dir = VARIABLE.GetComponent<Battalion.Battalion>().BattalionData.Dir;
                go.GetComponent<Battalion.Battalion>().BattalionData.LoadBattalion(1,
                    VARIABLE.GetComponent<Battalion.Battalion>().BattalionData.BattalionType);
            }
        }

        public void Load()
        {
            GridData gridData = Functions.Functions.LoadByJson("填线宝宝");
            GridManger2.GetComponent<DivisionManger>().LoadDivision(gridData);
        }
    }
}