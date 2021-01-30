using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Initiation {


	public abstract class BaseState {
		public event Action<BaseState> OnStateEnter;
		public event Action<BaseState> OnStateExit;

		public GameObject gameObject { get; private set; }

		public BaseState(GameObject obj)
		{
			this.gameObject = obj;
		}

		public virtual void Start()
		{
			OnStateEnter?.Invoke(this);
		}


		public virtual void Finish()
		{
			OnStateExit?.Invoke(this);
		}


		public abstract Type Tick();
	}
}