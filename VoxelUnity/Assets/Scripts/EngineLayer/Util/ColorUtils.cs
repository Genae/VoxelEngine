using UnityEngine;

namespace EngineLayer.Util
{
    public class ColorUtils
    {
        private static float ReferenceX = 100;
        private static float ReferenceY = 100;
        private static float ReferenceZ = 100;


        public static float ColorDistance(Color c1, Color c2)
        {
            //return ColorDistanceCIELab(new Vector3(c1.r, c1.g, c1.b), new Vector3(c2.r, c2.g, c2.b));
            return ColorDistanceCIELab(XYZtoCIELab(RGBToXYZ(c1)), XYZtoCIELab(RGBToXYZ(c2)));
        }

        protected static Vector3 RGBToXYZ(Color c)
        {
            var varR = c.r;
            var varG = c.g;
            var varB = c.b;
            if (varR > 0.04045) varR = Mathf.Pow((varR + 0.055f) / 1.055f, 2.4f);
            else varR = varR / 12.92f;
            if (varG > 0.04045) varG = Mathf.Pow((varG + 0.055f) / 1.055f, 2.4f);
            else varG = varG / 12.92f;
            if (varB > 0.04045) varB = Mathf.Pow((varB + 0.055f) / 1.055f, 2.4f);
            else varB = varB / 12.92f;

            varR = varR * 100;
            varG = varG * 100;
            varB = varB * 100;

            var x = varR * 0.4124f + varG * 0.3576f + varB * 0.1805f;
            var y = varR * 0.2126f + varG * 0.7152f + varB * 0.0722f;
            var z = varR * 0.0193f + varG * 0.1192f + varB * 0.9505f;
            return new Vector3(x, y, z);
        }

        protected static Vector3 XYZtoCIELab(Vector3 c)
        {
            var var_X = c.x / ReferenceX;
            var var_Y = c.y / ReferenceY;
            var var_Z = c.z / ReferenceZ;

            if (var_X > 0.008856f) var_X = Mathf.Pow(var_X, 1f / 3);
            else var_X = (7.787f * var_X) + (16f/116);
            if (var_Y > 0.008856f) var_Y = Mathf.Pow(var_Y, 1f / 3);
            else var_Y = (7.787f * var_Y) + (16f / 116);
            if (var_Z > 0.008856f) var_Z = Mathf.Pow(var_Z, 1f / 3);
            else var_Z = (7.787f * var_Z) + (16f / 116);

            var CIEL = (116 * var_Y) - 16;
            var CIEa = 500 * (var_X - var_Y);
            var CIEb = 200 * (var_Y - var_Z);
            return new Vector3(CIEL, CIEa, CIEb);
        }

        protected static float ColorDistanceCIELab(Vector3 c, Vector3 c2)
        {
            return (c - c2).magnitude;
        }
    }
}
