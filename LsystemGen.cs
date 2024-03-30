using System;
using System.Text;
using UnityEngine;

public class LsystemGen : MonoBehaviour
{
    // --- Varibles --- 
    public Rule[] rules;
    public string rootSenctence;
    [Range(0, 10)]
    public int iterLim = 1;

    public bool randIgnoreRuleMod = true;
    [Range(0, 1)]
    public float chanceToIgnoreRule = 0.3f;
    // -----------------

    // Debugging to see the sentence 
    private void Start()
    {
        Debug.Log(GenerateSentence());
    }

    // ------------------ Functions ------------------
    public string GenerateSentence(string word = null)
    {
        if (word == null)
        {
            word = rootSenctence;
        }
        return GrowRecursive(word);
    }

    private string GrowRecursive(string word, int iterIndex = 0)
    {
        if (iterLim <= iterIndex) // Base case
        {
            return word;
        }

        StringBuilder newWord = new StringBuilder(); // Modify the existing string and dosen't create a new one --> reduces overhead (Class from .Net framework) 

        foreach (var c in word)
        {
            newWord.Append(c); // Adding or attach one string/char to another stringBuilder instance (From StringBuilder)
            ProcessRulesRecurs(newWord, c, iterIndex);
        }

        return newWord.ToString();
    }

    private void ProcessRulesRecurs(StringBuilder newWord, char c, int iterIndex)
    {
        foreach (var rule in rules)
        {
            if (rule.letter == c.ToString())
            {
                if (rule.GetResult() != rule.letter) // Prevent infinite loop
                {
                    if (randIgnoreRuleMod && iterIndex > 1) // Don't want the first branch to be ignored for better looking result
                    {
                        if (UnityEngine.Random.value < chanceToIgnoreRule) // Not the best solution but gives more randomness to the L system
                        {
                            return;
                        }
                    }
                    newWord.Append(GrowRecursive(rule.GetResult(), iterIndex + 1));

                }

            }
        }
    }
}
