using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using Script.Functions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class test : MonoBehaviour
{
    public GameObject myPrefab;
    [SerializeField] private GameObject parent;
    public Tilemap Tilemap;

    private Script.Grid.Grid grid;
   public void addBattalion()
   {
       TileBase tile = Tilemap.GetTile(new Vector3Int(1, 0, 0));
        gameObject.GetComponent<BattalionData>().LoadBattalion(111,Enum.Parse<BattalionData.BattalionTypes>("Artillery"));
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        
   }
}
