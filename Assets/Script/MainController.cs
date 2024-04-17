using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private List<GameObject> list1 = new List<GameObject>();
    [SerializeField] private List<GameObject> list2 = new List<GameObject>();

    public void SwitchDisplay()
    {
        
            foreach (var go in list1)
            {
                go.SetActive(!go.activeSelf);
            }
        
            foreach (var go in list2)
            {
                go.SetActive(!go.activeSelf);
            }
            
    }
}