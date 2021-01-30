using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

namespace Initiation {



	public class StateMachine:NetworkBehaviour {
		private Dictionary<Type,BaseState> states;
		public event Action<BaseState> OnStateChanged;

		[SerializeField] string CurrentStateName;
		public BaseState CurrentState { get; private set; }

		public void SetStates(Dictionary<Type,BaseState> states)
		{
			this.states = states;
		}

		private void Update()
		{
			if(CurrentState == null) {
				Type stateType = states.Keys.First();
				SwitchToNewState(stateType);
			}

			if(CurrentState == null) {
				return;
			}

			var nextState = CurrentState.Tick();

			if(nextState != null &&
				nextState != CurrentState.GetType()) {
				SwitchToNewState(nextState);
			}
		}

		void SwitchToNewState(Type nextState)
		{
			if(CurrentState != null) {
				CurrentState.Finish();
			}
			CurrentState = states[nextState];
			CurrentState.Start();
			CurrentStateName = CurrentState.GetType().ToString();
			OnStateChanged?.Invoke(CurrentState);
		}

	}
}