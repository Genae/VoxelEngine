namespace Assets.Scripts.Logic.Unit
{
    public class Unit
    {
        public BaseValues BaseValues;
        public int Race;

        public Unit(BaseValues values, int race)
        {
            BaseValues = values;
            Race = race;
        }


    }

    public struct BaseValues
    {
        public float Hitpoints;
        public float Condition;
        public float Strength;
        public float Dexterity;
        public float Intelligence;
        public float Wisdom;
    }
}

