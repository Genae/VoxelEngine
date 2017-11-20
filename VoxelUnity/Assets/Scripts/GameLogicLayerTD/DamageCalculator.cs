using System.Collections.Generic;


namespace Assets.Scripts.GameLogicLayerTD
{
    public static class DamageCalculator
    {
        public static float[,] ElementTable= {
            {  1f, 0.5f,   1f,   2f,   1f },
            {  2f,   1f,   2f, 0.5f,   1f },
            {  2f, 0.5f,   1f,   0f,   2f },
            {0.5f,   1f,   2f,   2f,   1f },
            {0.5f,   1f,   1f, 0.5f,   2f },
        };

        public static float Calc(float dmg, List<ElementType> aList, List<ElementType> dList)
        {

            if (aList.Count == 0 || dList.Count == 0) return dmg; //incase something has no element? temp workaround TODO

            var multiplier = 1f;
            foreach (var elementA in aList)
            {
                foreach (var elementD in dList)
                {
                    multiplier *= ElementTable[(int)elementA, (int)elementD];
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


