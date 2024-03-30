using System;
using System.Collections.Generic;
using UnityEngine;

public class StructureHelper : MonoBehaviour
{
    public HouseType[] HouseTypes;
    public GameObject[] trees;
    public bool randomTreePlacement = false;
    [Range(0, 1)]
    public float randomTreeThreshold = 0.3f; 
    public Dictionary<Vector3Int, GameObject> structureDic = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, GameObject> treeDic = new Dictionary<Vector3Int, GameObject>();

    public void PlaceStructures(List<Vector3Int> roadPos)
    {
        Dictionary<Vector3Int, Direction> freeHousingSpots = FindEmptyHousingLots(roadPos);
        List<Vector3Int> blockedLots = new List<Vector3Int>(); // to make 2 "tiled" houses possible

        foreach (var freeLot in freeHousingSpots) // Add the prefab to the spaces
        {
            if (blockedLots.Contains(freeLot.Key)) { continue; }

            var rot = Quaternion.identity;
            switch (freeLot.Value) // Value of the freeLot var is the direction, every house points to the left in the beginning 
            {
                case Direction.up:
                    rot = Quaternion.Euler(0, 90, 0);
                    break;
                case Direction.down:
                    rot = Quaternion.Euler(0, -90, 0);
                    break;
                case Direction.right:
                    rot = Quaternion.Euler(0, 180, 0);
                    break;
                default:
                    break;
            }
            for (var i = 0; i < HouseTypes.Length; i++) // placing the bigger houses first (looking at sizeOfStructure) to the smaller
            {
                if (HouseTypes[i].totaltLotsWanted == -1)
                {
                    if (randomTreePlacement)
                    {
                        var rand = UnityEngine.Random.value;
                        if(rand < randomTreeThreshold)
                        {
                            var treePrefab = SpawnPrefab(trees[UnityEngine.Random.Range(0, trees.Length)], freeLot.Key, rot);
                            treeDic.Add(freeLot.Key, treePrefab);
                            break;
                        }
                    }
                    var house = SpawnPrefab(HouseTypes[i].GetPrefab(), freeLot.Key, rot);
                    structureDic.Add(freeLot.Key, house);
                    break;
                }
                if (HouseTypes[i].IsHouseAvailable())
                {
                    if (HouseTypes[i].sizeOfStructure > 1) // houses which requires more than 1 "tile"
                    {
                        var halfHouseSize = Mathf.FloorToInt(HouseTypes[i].sizeOfStructure / 2.0f);
                        List<Vector3Int> tempBlockLots = new List<Vector3Int>(); // Will be send as a reference to func

                        if (DoesHouseFit(halfHouseSize, freeHousingSpots, freeLot, blockedLots,ref tempBlockLots))
                        {
                            var house = SpawnPrefab(HouseTypes[i].GetPrefab(), freeLot.Key, rot);
                            blockedLots.AddRange(tempBlockLots); // Does not place two structures in the same pos
                            structureDic.Add(freeLot.Key, house);

                            foreach (var pos in tempBlockLots)
                            {
                                structureDic.Add(pos, house);
                            }
                        }
                    }
                    else
                    {
                        var house = SpawnPrefab(HouseTypes[i].GetPrefab(), freeLot.Key, rot);
                        structureDic.Add(freeLot.Key, house);
                    }
                    break;
                }
            }
        }
    }

    private bool DoesHouseFit(int halfSize,
            Dictionary<Vector3Int, Direction> freeEstateSpots,
            KeyValuePair<Vector3Int, Direction> freeSpot,
            List<Vector3Int> blockedPos,
            ref List<Vector3Int> tempPositionsBlocked)
    {
        Vector3Int direction = Vector3Int.zero;
        if (freeSpot.Value == Direction.down || freeSpot.Value == Direction.up)
        {
            direction = Vector3Int.right;
        }
        else
        {
            direction = new Vector3Int(0, 0, 1);
        }
        for (int i = 1; i <= halfSize; i++)
        {
            var pos1 = freeSpot.Key + direction * i;
            var pos2 = freeSpot.Key - direction * i;
            if (!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2) || blockedPos.Contains(pos1) || blockedPos.Contains(pos2))
            {
                return false;
            }
            tempPositionsBlocked.Add(pos1);
            tempPositionsBlocked.Add(pos2);
        }
        return true;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3Int pos, Quaternion rot)
    {
        var newHouse = Instantiate(prefab, pos, rot, transform);
        return newHouse;
    }

    // Function to find the free spaces around the roads
    private Dictionary<Vector3Int, Direction> FindEmptyHousingLots(List<Vector3Int> roadPos)
    {
        Dictionary<Vector3Int, Direction> freeLots = new Dictionary<Vector3Int, Direction>();

        foreach (var pos in roadPos)
        {
            var neighDir = PlacementHelper.FindRoadNeigh(pos, roadPos);
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                if (neighDir.Contains(dir) == false)
                {
                    var newPos = pos + PlacementHelper.GetOffsetFromDir(dir);
                    if (freeLots.ContainsKey(newPos)) // check if it already has this position
                    {
                        continue;
                    }
                    freeLots.Add(newPos, PlacementHelper.GetReverseDir(dir));
                }
            }
        }
        return freeLots;
    }

    // To regenerate the town
    public void Reset()
    {
        foreach(var i in structureDic.Values)
        {
            Destroy(i);
        }
        structureDic.Clear();

        foreach (var t in treeDic.Values)
        {
            Destroy(t);
        }
        treeDic.Clear();

        foreach(var h in HouseTypes)
        {
            h.Reset();
        }
    }
}
