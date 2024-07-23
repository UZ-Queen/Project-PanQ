using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class Utilities
{
    static public string[] indexToEnglishNumerals = {
    "Zero", "One", "Two", "Three", "Four", "Five",
    "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
    "Twelve", "Thirteen", "Fourteen", "Fifteen",
    "Sixteen", "Seventeen", "Eighteen", "Nineteen", "Twenty"
    };
    static public string[] indexToRomanNumerals = {
        "N", "I", "II", "III", "IV", "V",
        "VI", "VII", "VIII", "IX", "X",
        "XI", "XII", "XIII", "XIV", "XV",
        "XVI", "XVII", "XVIII", "XIX", "XX"
    };
    // Start is called before the first frame update
    void Start()
    {

    }




    static public T[] ShuffleArray<T>(T[] array, int seed)
    {
        Random prng = new Random(seed);
        for (int i = 0; i < array.Length - 1; i++)
        {
            Swap(ref array[i], ref array[prng.Next(i, array.Length - 1)]);
        }

        return array;


    }
    static public void Swap<T>(ref T a, ref T b)
    {
        (b, a) = (a, b); //이런게 되나요?
    }
    static public UnityEngine.Vector3 GetReciprocalVector(UnityEngine.Vector3 vector, bool regardInfinityAsZero = false)
    {

        float infinityOrZero = regardInfinityAsZero ? float.PositiveInfinity : 0;

        float x = vector.x != 0 ? 1 / vector.x : infinityOrZero;
        float y = vector.y != 0 ? 1 / vector.y : infinityOrZero;
        float z = vector.z != 0 ? 1 / vector.z : infinityOrZero;

        return new UnityEngine.Vector3(x, y, z);
    }
}
