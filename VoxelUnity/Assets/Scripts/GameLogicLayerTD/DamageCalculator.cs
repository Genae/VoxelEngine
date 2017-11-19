using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.GameLogicLayerTD
{
    public static class DamageCalculator
    {

        //TODO graph solution? this seems very ugly

        public static float Calc(float dmg, ElementType attacker, ElementType defender)
        {
            var multiplier = 1f;

            if (attacker == defender) multiplier = 1;

            if (attacker == ElementType.Water && defender == ElementType.Fire)
            {
                multiplier = 2;
            }
            else if (attacker == ElementType.Fire && defender == ElementType.Water)
            {
                multiplier = 0.5f;
            }
            //to ugly to cont, Graph might be best option
            return dmg * multiplier;
        }
    }

    public enum ElementType
    {
        Fire,
        Water
    }

}


