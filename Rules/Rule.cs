using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ProcedualTown/Rule")]

public class Rule : ScriptableObject
{
    public string letter;

    [SerializeField] 
    private string[] result = null;

    [SerializeField]
    private bool randResult = false;

    public string GetResult()
    {   
        if (randResult) 
        {
            int randomIndex = UnityEngine.Random.Range(0, result.Length);
            return result[randomIndex];
        }
        return result[0];
    }

}
