using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HouseType
{
    [SerializeField]
    private GameObject[] prefabs;
    public int sizeOfStructure; // Keeps how big a structure is 4 example: 2 wide or 1 wide
    public int totaltLotsWanted;// Number of lots avaible for placing structures on
    public int placedLots;      // Number of lots already placed

    public GameObject GetPrefab()
    {
        placedLots++;

        if (prefabs != null && prefabs.Length > 0)
        {
            var rand = UnityEngine.Random.Range(0, prefabs.Length);
            return prefabs[rand];
        }
        
        Debug.LogError("Prefabs array is empty!"); // Handle the case when prefabs array is empty or null
        return prefabs[0];  // Change later to set to a defult prefab?
    }

    // Bool to check if more houses are needed to be placed or not
    public bool IsHouseAvailable()
    {
        return placedLots < totaltLotsWanted;
    }

    public void Reset() { placedLots = 0; }

}
