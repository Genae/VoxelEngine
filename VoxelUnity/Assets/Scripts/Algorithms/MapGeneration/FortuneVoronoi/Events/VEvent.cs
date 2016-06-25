using System;

namespace Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.Events
{
    internal abstract class VEvent : IComparable
    {
        public abstract double Y {get;}
        protected abstract double X {get;}
        #region IComparable Members

        public int CompareTo(object obj)
        {
            if(!(obj is VEvent))
                throw new ArgumentException("obj not VEvent!");
            var i = Y.CompareTo(((VEvent)obj).Y);
            if(i!=0)
                return i;
            return X.CompareTo(((VEvent)obj).X);
        }

        #endregion
    }
}