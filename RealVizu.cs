using System.Collections.Generic;
using UnityEngine;

public class RealVizu : MonoBehaviour
{
    public LsystemGen lSystem;
    public RoadHelper rHelper;
    public StructureHelper structHelper;

    public int roadLenght = 8;
    private int length = 8;
    private float angle = 90;

    public int Length
    {
        get
        {
            if (length > 0) { return length; }
            else
            {
                return 1;
            }

        }
        set => length = value;
    }

    private void Start() { CreateTown(); }

    public void CreateTown()
    {
        length = roadLenght;
        rHelper.Reset();
        structHelper.Reset();

        var sequence = lSystem.GenerateSentence();
        VisualizeSeq(sequence);
    }

    // visulize the implementation with FIFO
    private void VisualizeSeq(string sequence)
    {
        Stack<AgentPara> savePoints = new Stack<AgentPara>();
        var currentPos = Vector3.zero;

        Vector3 direction = Vector3.forward; // Z axiz
        Vector3 tempPos = Vector3.zero;

        foreach (var letter in sequence)
        {
            SimpleVisu.EncodeLetters encoding = (SimpleVisu.EncodeLetters)letter; // gets only the letters which match with the encoding
            switch (encoding)
            {
                case SimpleVisu.EncodeLetters.save:
                    savePoints.Push(new AgentPara
                    {
                        position = currentPos,
                        direction = direction,
                        length = Length
                    });
                    break;
                case SimpleVisu.EncodeLetters.load:
                    if (savePoints.Count > 0)
                    {
                        var AgentPara = savePoints.Pop();
                        currentPos = AgentPara.position;
                        direction = AgentPara.direction;
                        Length = AgentPara.length;
                    }
                    else { throw new System.Exception("Opsie! No saved points in the stack :( "); }
                    break;
                case SimpleVisu.EncodeLetters.draw:
                    tempPos = currentPos;
                    currentPos += direction * length;

                    rHelper.PlaceRoadPiece(tempPos, Vector3Int.RoundToInt(direction), length);

                    Length -= 2; // shorten the line
                    break;
                case SimpleVisu.EncodeLetters.turnR:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction; // Vector3.up, where up is the y-axis
                    break;
                case SimpleVisu.EncodeLetters.turnL:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
                default:
                    break;
            }

        }
        rHelper.FixRoad(); // Fixing the roads from RoadHelper class
        structHelper.PlaceStructures(rHelper.getRoadPos());
    }
}
