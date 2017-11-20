using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.GameLogicLayerTD
{
    public static class DamageCalculator
    {
        public static float[,] elementTable= new float[5,5]
        {
            {  1f, 0.5f,   1f,   2f,   1f },
            {  2f,   1f,   2f, 0.5f,   1f },
            {  2f, 0.5f,   1f,   0f,   2f },
            {0.5f,   1f,   2f,   2f,   1f },
            {0.5f,   1f,   1f, 0.5f,   2f },
        };

        public static float Calc(float dmg, List<ElementType> AList, List<ElementType> DList)
        {

            if (AList.Count == 0 || DList.Count == 0) return dmg; //incase something has no element? temp workaround TODO

            var multiplier = 1f;
            foreach (var elementA in AList)
            {
                foreach (var elementD in DList)
                {
                    multiplier *= elementTable[(int)elementA, (int)elementD];
                }
            }
            return dmg * multiplier;
        }
    }
}

public enum ElementType
{
    Fire,
    Water,
    Earth,
    Air,
    Light
}


