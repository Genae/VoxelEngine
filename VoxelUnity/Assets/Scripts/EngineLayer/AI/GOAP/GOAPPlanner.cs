using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Algorithms;
using UnityEngine;

namespace EngineLayer.AI.GOAP
{
    public class GOAPPlanner
    {
        public GOAPlan Plan(GameObject agent, HashSet<GOAPAction> availableActions, Dictionary<string, object> worldState, Dictionary<string, object> goal)
        {
            var plan = new GOAPlan(goal);
            plan.Thread = new Thread(() =>
            {
                // reset the actions so we can start fresh with them
                foreach (var a in availableActions)
                {
                    a.DoReset();
                }

                // check what actions can run using their checkProceduralPrecondition
                var usableActions = new HashSet<GOAPAction>();
                foreach (var a in availableActions)
                {
                    if (a.CheckProceduralPrecondition(agent))
                        usableActions.Add(a);
                }
                // we now have all actions that can run, stored in usableActions

                // build up the tree and record the leaf nodes that provide a solution to the goal.
                var leaves = new List<Node>();

                // build graph
                var start = new Node(null, 0, worldState, null);
                var success = BuildGraph(start, leaves, usableActions, goal);
                plan.Success = success;
                if (!success)
                {
                    return;
                }

                // get the cheapest leaf
                var cheapest = leaves.OrderBy(l => l.RunningCost).First();

                // get its node and work back through the parents
                var result = new List<GOAPAction>();
                var n = cheapest;
                while (n != null)
                {
                    if (n.Action != null)
                    {
                        result.Insert(0, n.Action); // insert the action in the front
                    }
                    n = n.Parent;
                }
                // we now have this action list in correct order

                var queue = new Queue<GOAPAction>();
                foreach (var a in result)
                {
                    queue.Enqueue(a);
                }
                plan.Actions = queue;
            });
            
            plan.Thread.Start();
            // hooray we have a plan!
            return plan;
        }

        /**
         * Returns true if at least one solution was found.
         * The possible paths are stored in the leaves list. Each leaf has a
         * 'runningCost' value where the lowest cost will be the best action
         * sequence.
         */
        protected bool BuildGraph(Node parent, List<Node> leaves, HashSet<GOAPAction> usableActions, Dictionary<string, object> goal)
        {
            var foundOne = false;

            // go through each action available at this node and see if we can use it here
            foreach (var action in usableActions)
            {

                // if the parent state has the conditions for this action's preconditions, we can use it here
                if (InState(action.Preconditions, parent.State))
                {

                    // apply the action's effects to the parent state
                    var currentState = PopulateState(parent.State, action.Effects);
                    //Debug.Log(GoapAgent.prettyPrint(currentState));
                    var node = new Node(parent, parent.RunningCost + action.Cost, currentState, action);

                    if (GoalInState(goal, currentState))
                    {
                        // we found a solution!
                        leaves.Add(node);
                        foundOne = true;
                    }
                    else
                    {
                        // test all the remaining actions and branch out the tree
                        var subset = ActionSubset(usableActions, action);
                        var found = BuildGraph(node, leaves, subset, goal);
                        if (found)
                            foundOne = true;
                    }


                }
            }

            return foundOne;
        }

        /**
         * Create a subset of the actions excluding the removeMe one. Creates a new set.
         */
        protected HashSet<GOAPAction> ActionSubset(HashSet<GOAPAction> actions, GOAPAction removeMe)
        {
            var subset = new HashSet<GOAPAction>();
            foreach (var a in actions)
            {
                if (!a.Equals(removeMe))
                    subset.Add(a);
            }
            return subset;
        }

        /*
         * Checks if at least one goal is met. 
         * to-do: Create a system for weighting towards paths that fulfill more goals
         */
        protected bool GoalInState(Dictionary<string, object> test, Dictionary<string, object> state)
        {
            var match = false;
            foreach (var t in test)
            {
                foreach (var s in state)
                {
                    if (s.Equals(t))
                    {
                        match = true;
                        break;
                    }
                }
            }
            return match;
        }

        /*
         * Check that all items in 'test' are in 'state'. If just one does not match or is not there
         * then this returns false.
         */
        protected bool InState(Dictionary<string, object> test, Dictionary<string, object> state)
        {
            var allMatch = true;
            foreach (var t in test)
            {
                var match = false;
                foreach (var s in state)
                {
                    if (s.Equals(t))
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                    allMatch = false;
            }
            return allMatch;
        }

        /**
         * Apply the stateChange to the currentState
         */
        protected Dictionary<string, object> PopulateState(Dictionary<string, object> currentState, Dictionary<string, object> stateChange)
        {
            var state = new Dictionary<string, object>();
            // copy the KVPs over as new objects
            foreach (var s in currentState)
            {
                state.Add(s.Key, s.Value);
            }

            foreach (var change in stateChange)
            {
                state[change.Key] = change.Value;
            }
            return state;
        }

        /**
         * Used for building up the graph and holding the running costs of actions.
         */
        protected class Node
        {
            public Node Parent;
            public float RunningCost;
            public Dictionary<string, object> State;
            public GOAPAction Action;

            public Node(Node parent, float runningCost, Dictionary<string, object> state, GOAPAction action)
            {
                Parent = parent;
                RunningCost = runningCost;
                State = state;
                Action = action;
            }
        }

    }

    public class GOAPlan: Promise
    {
        public readonly Dictionary<string, object> Goal;

        public GOAPlan(Dictionary<string, object> goal)
        {
            Goal = goal;
        }

        public bool Success { get; set; }
        public Queue<GOAPAction> Actions { get; set; }
    }
}
