using UnityEngine;
using System.Collections;
using Assets.Scripts.Data.Importer;
using System.Collections.Generic;

namespace Assets.Scripts.Logic.Unit
{
    public class UnitCreator
    {
        private BaseValueRange BaseValueRange;
        private List<ClassValueModificators> ClassModList;

        public UnitCreator(string baseValuePath, string classModPath)
        {
            BaseValueRange = ConfigImporter.GetConfig<BaseValueRange>(baseValuePath)[0];
            ClassModList = new List<ClassValueModificators>();
            ClassModList = ConfigImporter.GetConfig<ClassValueModificators>(classModPath);
        }

        public Unit GenerateUnit(int race)
        {
            BaseValues v = new BaseValues();

            v.Hitpoints = Random.Range(BaseValueRange.Hitpoints[0], BaseValueRange.Hitpoints[1]);
            v.Condition = Random.Range(BaseValueRange.Condition[0], BaseValueRange.Condition[1]);
            v.Strength = Random.Range(BaseValueRange.Strength[0], BaseValueRange.Strength[1]);
            v.Dexterity = Random.Range(BaseValueRange.Dexterity[0], BaseValueRange.Dexterity[1]);
            v.Intelligence = Random.Range(BaseValueRange.Intelligence[0], BaseValueRange.Intelligence[1]);
            v.Wisdom = Random.Range(BaseValueRange.Wisdom[0], BaseValueRange.Wisdom[1]);

            //hacky af, may not work! renamed class files to get them in specific order
            var cvm = ClassModList[race];

            v.Hitpoints *= cvm.HitpointMod;
            v.Condition *= cvm.ConditionMod;
            v.Strength *= cvm.StrengthMod;
            v.Dexterity *= cvm.DexterityMod;
            v.Intelligence *= cvm.IntelligenceMod;
            v.Wisdom *= cvm.WisdomMod;

            Unit u = new Unit(v, race);

            return u;
        }



    }

    public struct BaseValueRange
    {
        public float[] Hitpoints;
        public float[] Condition;
        public float[] Strength;
        public float[] Dexterity;
        public float[] Intelligence;
        public float[] Wisdom;
    }

    public struct ClassValueModificators
    {
        public int ClassNumber;
        public float HitpointMod;
        public float ConditionMod;
        public float StrengthMod;
        public float DexterityMod;
        public float IntelligenceMod;
        public float WisdomMod;
        public float pFighting;
        public float mFighting;
        public float rFighting;
        public float Mining;
        public float Carrying;
        public float WoodChopping;
        public float Building;
    }
}

