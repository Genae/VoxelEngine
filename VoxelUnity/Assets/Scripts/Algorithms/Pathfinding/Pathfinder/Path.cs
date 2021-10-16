using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Algorithms.Pathfinding.Agents;
using Algorithms.Pathfinding.Graphs;
using Algorithms.Pathfinding.Utils;
using UnityEngine;

namespace Algorithms.Pathfinding.Pathfinder
{
    public class Path : Promise, IDisposable
    {
        public List<Node> Nodes;
        public Node Start;
        public List<Node> Targets;
        public virtual float Length { get; set; }
        public bool IsT0;
        public PathState State;
        private int _currentNode;
        private readonly PathRegistry _registry;

        public Path(Node start, List<Node> targets, PathRegistry registry)
        {
            IsT0 = false;
            Start = start;
            Targets = targets;
            State = PathState.InitialCalulation;
            _registry = registry;
            _registry.ActivePaths.Add(this);
        }


        public Node GetNode(int i)
        {
            if (State.Equals(PathState.Invalid) || State.Equals(PathState.Recalculation) && i > _currentNode)
                throw new Exception("Path State is " + State);
            if (Nodes == null)
                return null;
            if (i > Nodes.Count - 1)
            {
                Dispose();
                return null;
            }
            _currentNode = i;
            return Nodes[i];
        }

        public static Path Calculate(VoxelGraph graph, Vector3I from, Vector3I to, bool forceOptimal = false)
        {
            return Calculate(graph, from, new List<Vector3I> {to}, forceOptimal);
        }


        public static Path Calculate(VoxelGraph graph, Vector3I from, List<Vector3I> to, bool forceOptimal = false)
        {
            if (to.Any(t => (from - (Vector3)t).magnitude < 200 || forceOptimal))
            {
                return CalculateLowlevelPath(graph, from, to);
            }
            return CalculateHighlevelPath(graph, from, to);
        }

        private static Path CalculateHighlevelPath(VoxelGraph graph, Vector3I from, List<Vector3I> to)
        {
            var start = graph.GetNode(from) ?? graph.GetClosestNode(from, 5);
            if (start == null)
                return null;
            var targets = to.Select(graph.GetNode).Where(t => t != null).ToList();
            if (targets.Count == 0)
                return null;
            var path = new HighLevelPath(start, targets, graph);
            path.Thread = new Thread(() =>
            {
                path = (HighLevelPath)AStar.GetPath(start.SuperNodes.ToDictionary(n => n.Key as Node, n => n.Value.Length), targets.Select(t => t.GetClosestSuperNode()).ToList(), path);
                path.Finished = true;
            });
            path.Thread.Start();
            return path;
        }

        private static Path CalculateLowlevelPath(VoxelGraph graph, Vector3I from, List<Vector3I> to)
        {
            var start = graph.GetNode(from) ?? graph.GetClosestNode(from, 5);
            if (start == null)
                return null;
            var targets = to.Select(graph.GetNode).Where(t => t != null).ToList();
            if (targets.Count == 0)
                return null;
            var path = new Path(start, targets, graph.GetPathRegistry());
            path.Thread = new Thread(() =>
            {
                path = AStar.GetPath(start, targets, path);
                path.Finished = true;
                path.State = PathState.Ready;
            });
            path.Thread.Start();
            return path;
        }

        public void Visualize(Color color, int fromNode = -1)
        {
            if (Nodes == null)
            {
                return;
            }
            for (int i = fromNode + 1; i < Nodes.Count - 1; i++)
            {
                if (fromNode == -1)
                    Debug.DrawLine(Nodes[i].Position, Nodes[i + 1].Position, color, 60000, true);
                else
                    Debug.DrawLine(Nodes[i].Position, Nodes[i + 1].Position, color);

            }
        }

        public void Recalculate(Node removedNode, VoxelGraph graph)
        {
            if (State.Equals(PathState.Recalculation))
                Thread.Abort();
            if (Nodes[Nodes.Count-1].Equals(removedNode))
            {
                State = PathState.Invalid;
            }
            else
            {
                State = PathState.Recalculation;
                var currentNode = GetNode(_currentNode);
                if (currentNode == null)
                {
                    State = PathState.Invalid;
                }
                else
                {
                    var p2 = Calculate(graph, currentNode.Position, Targets.Select(t => t.Position).ToList());
                    if (p2 == null)
                    {
                        State = PathState.Invalid;
                    }
                    else
                    {
                        Thread = p2.Thread;
                        p2.OnFinish = () =>
                        {
                            Nodes = p2.Nodes;
                            Start = p2.Start;
                            Length = p2.Length;
                            IsT0 = p2.IsT0;
                            State = p2.State;
                            p2.Dispose();
                        };
                    }
                }
            }
        }

        public void Dispose()
        {
            _registry.ActivePaths.Remove(this);
        }
    }
    public enum PathState
    {
        InitialCalulation,
        Recalculation,
        Invalid,
        Ready
    }

    public class HighLevelPath : Path
    {
        private readonly VoxelGraph _graph;
        private Path _drilledDownPath;
        public Path ExactPath
        {
            get
            {
                if (Nodes == null || Nodes.Count == 0)
                    return null;
                return _drilledDownPath ?? (_drilledDownPath = CreateDrilledDownPath());
            }
        }

        private Path CreateDrilledDownPath()
        {
            var ag = new MovingAgent();
            return ag.FollowPath(this, _graph);
        }

        public override float Length
        {
            get { return ExactPath != null ? ExactPath.Length : 0; }
            set { }
        }



        public HighLevelPath(Node start, List<Node> target, VoxelGraph graph) : base(start, target, graph.GetPathRegistry())
        {
            _graph = graph;
        }
    }
}