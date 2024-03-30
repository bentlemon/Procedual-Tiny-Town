using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadHelper : MonoBehaviour
{
    public GameObject rStraight, rConor, r3way, r4way, rEnd; // Road pieces
    Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>(); // Need to compare ints and not float, would return false most of the time
    // Hashset to make sure no duplicates is obtained
    HashSet<Vector3Int> fixRoadPieces = new HashSet<Vector3Int>(); // To fix the pieces laid out to match the next iteration of road pieces

    // Housing placement
    public List<Vector3Int> getRoadPos() { return roadDictionary.Keys.ToList(); }

    public void PlaceRoadPiece(Vector3 startPos, Vector3Int dir, int length)
    {
        var rRotation = Quaternion.identity;
        if (dir.x == 0)
        {
            rRotation = Quaternion.Euler(0, 90, 0); // Euler(x, y, z)roatates the road if following in the z-direction 
        }

        for (int i = 0; i < length; i++)
        {
            var pos = Vector3Int.RoundToInt(startPos + dir * i);
            if (roadDictionary.ContainsKey(pos)) // check so that a road not get placed on the same position if rotation made from seed
            {
                continue;
            }
            var road = Instantiate(rStraight, pos, rRotation, transform);
            roadDictionary.Add(pos, road);

            // check and add pieces which has connecting road parts
            if (i == 0 || i == (length - 1))
            {
                fixRoadPieces.Add(pos);
            }
        }
    }

    public void FixRoad()
    {
        foreach (var pos in fixRoadPieces)
        {
            List<Direction> roadNeighDir = PlacementHelper.FindRoadNeigh(pos, roadDictionary.Keys);

            Quaternion rot = Quaternion.identity;

            // Debugging output to print positions and detected neighbors
            Debug.Log($"Position: {pos}, Neighbors: {string.Join(", ", roadNeighDir)}");


            /* Unity has a left hand side coordinate system mening that +y is upwards (not like up in the code), +z is the depth in and +x is to the right. 
                     Roation is counterclockwise!
            
                                 +y    
                                 ^   ^ +z (up)
                                 |  /                
                                 | /
               (left) -x <_______|/_______> +x (right)
                                 /
                                /
                               /
                              v -z (down)
            */

            if (roadNeighDir.Count == 1) // Just one neighour = road end!
            {
                Destroy(roadDictionary[pos]);
                if (roadNeighDir.Contains(Direction.down))
                {
                    rot = Quaternion.Euler(0, 90, 0);
                }
                else if (roadNeighDir.Contains(Direction.left))
                {
                    rot = Quaternion.Euler(0, 180, 0);
                }
                else if (roadNeighDir.Contains(Direction.up))
                {
                    rot = Quaternion.Euler(0, -90, 0);
                }
                roadDictionary[pos] = Instantiate(rEnd, pos, rot, transform);
            }
            else if (roadNeighDir.Count == 2) // Straight road or curved road piece
            {
                if (roadNeighDir.Contains(Direction.up) && roadNeighDir.Contains(Direction.down) ||
                    roadNeighDir.Contains(Direction.left) && roadNeighDir.Contains(Direction.right))
                {
                    continue; // Defult placement is a stright road, no need for change 
                }
                else
                {
                    Destroy(roadDictionary[pos]);

                    if (roadNeighDir.Contains(Direction.up) && roadNeighDir.Contains(Direction.right))
                    {
                        rot = Quaternion.Euler(0, 90, 0);
                    }
                    else if (roadNeighDir.Contains(Direction.down) && roadNeighDir.Contains(Direction.right))
                    {
                        rot = Quaternion.Euler(0, 180, 0);

                    }
                    else if (roadNeighDir.Contains(Direction.down) && roadNeighDir.Contains(Direction.left))
                    {
                        rot = Quaternion.Euler(0, -90, 0);
                    }

                    roadDictionary[pos] = Instantiate(rConor, pos, rot, transform);
                }

            }
            else if (roadNeighDir.Count == 3) // 3Way piece - somethings wrong here --> check later
            {
                Destroy(roadDictionary[pos]);

                if (roadNeighDir.Contains(Direction.down) && roadNeighDir.Contains(Direction.right)
                    && roadNeighDir.Contains(Direction.left))
                {
                    rot = Quaternion.Euler(0, 90, 0);
                }
                else if (roadNeighDir.Contains(Direction.down) && roadNeighDir.Contains(Direction.up)
                   && roadNeighDir.Contains(Direction.left))
                {
                    rot = Quaternion.Euler(0, 180, 0);

                }
                else if (roadNeighDir.Contains(Direction.up) && roadNeighDir.Contains(Direction.left)
                    && roadNeighDir.Contains(Direction.right))
                {
                    rot = Quaternion.Euler(0, -90, 0);
                }

                roadDictionary[pos] = Instantiate(r3way, pos, rot, transform);
            }
            else // 4way piece
            {
                // Remove the instanciation of the straight piece and replace with 4way piece
                Destroy(roadDictionary[pos]);
                roadDictionary[pos] = Instantiate(r4way, pos, rot, transform);
            }

        }
    }

    public void Reset()
    {
        foreach (var road in roadDictionary.Values)
        {
            Destroy(road);
        }
        roadDictionary.Clear();
        fixRoadPieces = new HashSet<Vector3Int>();
    }
}
