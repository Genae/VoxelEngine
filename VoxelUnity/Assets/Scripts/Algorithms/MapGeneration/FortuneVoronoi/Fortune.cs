using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.Events;
using Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.Nodes;
using Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi.VoroniGraph;
using Assets.Scripts.Algorithms.MapGeneration.Graph;
using UnityEngine;

namespace Assets.Scripts.Algorithms.MapGeneration.FortuneVoronoi
{
    public abstract class Fortune
	{
        internal static double ParabolicCut(double x1, double y1, double x2, double y2, double ys)
		{
			if(Math.Abs(x1-x2)<1e-10 && Math.Abs(y1-y2)<1e-10)
			{
				throw new Exception("Identical datapoints are not allowed!");
			}

			if(Math.Abs(y1-ys)<1e-10 && Math.Abs(y2-ys)<1e-10)
				return (x1+x2)/2;
			if(Math.Abs(y1-ys)<1e-10)
				return x1;
			if(Math.Abs(y2-ys)<1e-10)
				return x2;
			double a1 = 1/(2*(y1-ys));
			double a2 = 1/(2*(y2-ys));
			if(Math.Abs(a1-a2)<1e-10)
				return (x1+x2)/2;
            double xs1 = 0.5f / (2 * a1 - 2 * a2) * (4 * a1 * x1 - 4 * a2 * x2 + 2 * Math.Sqrt(-8 * a1 * x1 * a2 * x2 - 2 * a1 * y1 + 2 * a1 * y2 + 4 * a1 * a2 * x2 * x2 + 2 * a2 * y1 + 4 * a2 * a1 * x1 * x1 - 2 * a2 * y2));
            double xs2 = 0.5f / (2 * a1 - 2 * a2) * (4 * a1 * x1 - 4 * a2 * x2 - 2 * Math.Sqrt(-8 * a1 * x1 * a2 * x2 - 2 * a1 * y1 + 2 * a1 * y2 + 4 * a1 * a2 * x2 * x2 + 2 * a2 * y1 + 4 * a2 * a1 * x1 * x1 - 2 * a2 * y2));
			if(xs1>xs2)
			{
				double h = xs1;
				xs1=xs2;
				xs2=h;
			}
			if(y1>=y2)
				return xs2;
			return xs1;
		}
		internal static double[] CircumCircleCenter(Vector2 a, Vector2 b, Vector2 c)
		{
			if(a.Equals(b) || b.Equals(c) || a.Equals(c))
				throw new Exception("Need three different points!");
			var tx = (a[0] + c[0])/2;
			var ty = (a[1] + c[1])/2;

			var vx = (b[0] + c[0])/2;
			var vy = (b[1] + c[1])/2;

			double ux,uy,wx,wy;
			
			if(a[0].Equals(c[0]))
			{
				ux = 1;
				uy = 0;
			}
			else
			{
				ux = (c[1] - a[1])/(a[0] - c[0]);
				uy = 1;
			}

			if(b[0].Equals(c[0]))
			{
				wx = -1;
				wy = 0;
			}
			else
			{
				wx = (b[1] - c[1])/(b[0] - c[0]);
				wy = -1;
			}

			var alpha = (wy*(vx-tx)-wx*(vy - ty))/(ux*wy-wx*uy);

            return new []{(tx + alpha * ux), (ty + alpha * uy)};
		}	
		public static VoronoiGraph ComputeVoronoiGraph(IEnumerable<Vector2> datapoints)
		{
            var pq = new List<VEvent>();
			var currentCircles = new Hashtable();
			var vg = new VoronoiGraph();
			VNode rootNode = null;
			foreach(var v in datapoints)
			{
				pq.Add(new VDataEvent(v));
			}
            pq.Sort();
			while(pq.Count>0)
			{
				var ve = pq.First();
                pq.Remove(ve);
			    if (ve == null) return null;
				VDataNode[] circleCheckList;
				if(ve is VDataEvent)
				{
					rootNode = VNode.ProcessDataEvent(ve as VDataEvent,rootNode,vg,ve.Y,out circleCheckList);
				}
				else if(ve is VCircleEvent)
				{
					currentCircles.Remove(((VCircleEvent)ve).NodeN);
					if(!((VCircleEvent)ve).Valid)
						continue;
					rootNode = VNode.ProcessCircleEvent(ve as VCircleEvent,rootNode,vg,out circleCheckList);
				}
				else throw new Exception("Got event of type "+ve.GetType()+"!");
				foreach(var vd in circleCheckList)
				{
					if(currentCircles.ContainsKey(vd))
					{
						((VCircleEvent)currentCircles[vd]).Valid=false;
						currentCircles.Remove(vd);
					}
					var vce = VNode.CircleCheckDataNode(vd,ve.Y);
					if(vce!=null)
					{
						pq.Add(vce);
                        pq.Sort();
						currentCircles[vd]=vce;
					}
				}
				if(ve is VDataEvent)
				{
					var dp = ((VDataEvent)ve).DataPoint;
					foreach(VCircleEvent vce in currentCircles.Values)
					{
						if(Dist(dp[0],dp[1],vce.CenterX,vce.CenterY)<vce.Y-vce.CenterY && Math.Abs(Dist(dp[0],dp[1],vce.CenterX,vce.CenterY)-(vce.Y-vce.CenterY))>1e-10)
							vce.Valid = false;
					}
				}
			}
			VNode.CleanUpTree(rootNode);
			foreach(var ve in vg.GetEdges().Select(edge => edge as VoronoiEdge))
			{
                if (Edge.IsUnknownVertex(ve.V2))
				{
					ve.AddVertex(Edge.GetInfiniteVertex());
					if(Math.Abs(ve.LeftData[1]-ve.RightData[1])<1e-10 && ve.LeftData[0]<ve.RightData[0])
					{
						var T = ve.LeftData;
						ve.LeftData = ve.RightData;
						ve.RightData = T;
					}
				}
			}
			
			var minuteEdges = new ArrayList();
			foreach(var ve in vg.GetEdges())
			{
				if(!ve.IsPartlyInfinite && ve.V1.Equals(ve.V2))
				{
					minuteEdges.Add(ve);
					foreach(var ve2 in vg.GetEdges())
					{
						if(ve2.V1.Equals(ve.V1))
							ve2.V1 = ve.V1;
						if(ve2.V2.Equals(ve.V1))
							ve2.V2 = ve.V1;
					}
				}
			}
			foreach(VoronoiEdge ve in minuteEdges)
				vg.RemoveEdge(ve, false);

			return vg;
		}
		public static VoronoiGraph FilterVg(VoronoiGraph vg, double minLeftRightDist)
		{
			var vgErg = new VoronoiGraph();
			foreach(var ve in vg.GetEdges().Select(edge => edge as VoronoiEdge))
			{
				if((ve.LeftData-ve.RightData).magnitude>=minLeftRightDist)
					vgErg.AddEdge(ve);
			}
			foreach(var ve in vgErg.GetEdges())
			{
				vgErg.AddVertex(ve.V1);
                vgErg.AddVertex(ve.V2);
			}
			return vgErg;
		}
        


	    public static double Dist(double x1, double y1, double x2, double y2)
	    {
	        return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
	    }

	    public static int Ccw(double p0X, double p0Y, double p1X, double p1Y, double p2X, double p2Y, bool plusOneOnZeroDegrees)
	    {
	        var dx1 = p1X - p0X; 
            var dy1 = p1Y - p0Y;
	        var dx2 = p2X - p0X; 
            var dy2 = p2Y - p0Y;
	        if (dx1 * dy2 > dy1 * dx2) return +1;
	        if (dx1 * dy2 < dy1 * dx2) return -1;
	        if ((dx1 * dx2 < 0) || (dy1 * dy2 < 0)) return -1;
	        if ((dx1 * dx1 + dy1 * dy1) < (dx2 * dx2 + dy2 * dy2) && plusOneOnZeroDegrees)
	            return +1;
	        return 0;
	    }
	}
}
