using UnityEngine;
using System.Linq;

public class Randomizator
{
    public static int GetRandomIndexByChances(float[] chances)
    {
        var randomNumber = Random.Range(0f, chances.Sum());
        var currentNumber = 0f;
        for (var i = 0; i < chances.Length; ++i)
        {
            currentNumber += chances[i];
            if (randomNumber <= currentNumber)
                return i;
        }
        return 0;
    }
}
