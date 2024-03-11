namespace RSLib.Framework.FSM
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class FSMState
    {
        protected Dictionary<FSMTransition, FSMStateId> _map = new Dictionary<FSMTransition, FSMStateId>(new FSMTransitionComparer());

        public FSMStateId Id { get; protected set; }

        public void AddTransition(FSMTransition transition, FSMStateId id)
        {
            UnityEngine.Assertions.Assert.IsTrue(transition != FSMTransition.NONE, $"{transition} FSMTransition is not allowed to add a transition.");
            UnityEngine.Assertions.Assert.IsTrue(id != FSMStateId.NONE, $"{id} FSMStateId is not allowed to add a transition.");
            UnityEngine.Assertions.Assert.IsFalse(_map.ContainsKey(transition), $"A FSMTransition for Id {id} already exists in the map.");

            _map.Add(transition, id);
        }

        public void RemoveTransition(FSMTransition transition)
        {
            UnityEngine.Assertions.Assert.IsTrue(transition != FSMTransition.NONE, $"{transition} FSMTransition is not allowed to remove a transition.");
            UnityEngine.Assertions.Assert.IsTrue(_map.ContainsKey(transition), $"Map does not contain {transition} FSMTransition.");

            _map.Remove(transition);
        }

        public FSMStateId GetTransitionOutputState(FSMTransition transition)
        {
            return _map.TryGetValue(transition, out FSMStateId id) ? id : FSMStateId.NONE;
        }

        public virtual void OnStateEntered() { }
        public virtual void OnStateExit() { }

        /// <summary>
        /// Method used for the FSM owner to check if it should transition to another state.
        /// GameObject parameter type and parameters can be changed if needed.
        /// </summary>
        /// <param name="player">Player's gameObject.</param>
        public abstract void Reason(GameObject player);

        /// <summary>
        /// Method used for the FSM owner to behave according to its current state.
        /// GameObject parameter type and parameters can be changed if needed.
        /// </summary>
        /// <param name="player">Player's gameObject.</param>
        public abstract void Act(GameObject player);
    }
}