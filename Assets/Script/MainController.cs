using System.Collections.Generic;
using Script.Functions;
using Script.Grid;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Svlist = new List<GameObject>();
    [SerializeField] private List<GameObject> Fvlist = new List<GameObject>();
    [SerializeField] private DivisionManger fdm;
    [SerializeField] private DivisionManger edm;


    public void Fight(GridData fGridData,GridData eGridData)
    {
        fdm.InitDivision();
        edm.InitDivision();
        fdm.LoadDivision(fGridData);
        edm.LoadDivision(eGridData);
        SwitchDisplay();
    }
    public void SwitchDisplay()
    {
        foreach (var go in Svlist)
        {
            go.SetActive(!go.activeSelf);
        }

        foreach (var go in Fvlist)
        {
            go.SetActive(!go.activeSelf);
        }
    }
    
    
}