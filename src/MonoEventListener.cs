using System.ComponentModel.Design;
using UnityEngine;

namespace Leopotam.EcsLite
{
	/// <summary>
	/// A handy class for less extreme performance scenarios, or when subscribing to large numbers of events dynamically.
	/// It allows you to use the SubscribeTo method and .DisposeWith(_disposings) the results, and automatically have the
	/// events disposed of.
	/// </summary>
	public abstract class MonoEventListener_WithTracking : MonoEventListener
	{
		protected DisposableContainer _disposings = new DisposableContainer();

		protected override void OnUnsubscribe()
		{
			base.OnUnsubscribe();
			_disposings.Dispose();
		}
	}
	
	
	public abstract class MonoEventListener : MonoBehaviour, IEventListener
	{
		public bool IsBound { get; private set; }
		protected EcsWorld UnsafeWorld { get; private set; }
		protected int UnsafeEntity { get; private set; }
		protected EcsPackedEntityWithWorld PackedEntity { get; private set; }
		
		public IServiceContainer Services { get; private set;  }


		
		
		public void RegisterListeners(IServiceContainer container, EcsPackedEntityWithWorld packed)
		{
			Services = container;

			PackedEntity = packed;
			if (PackedEntity.Unpack(out var world, out var entity))
			{
				UnsafeWorld = world;
				UnsafeEntity = entity;

				IsBound = true;
				
				OnSubscribe();
			}
		}


		private void OnEnable()
		{
			if (IsBound)
				OnSubscribe();
		}


		protected virtual void OnDisable()
		{
			if (IsBound)
				OnUnsubscribe();
		}



		protected abstract void OnSubscribe();
		
		protected virtual void OnUnsubscribe()
		{
		
		}


		public void ReleaseListeners()
		{
			PackedEntity = default;
			UnsafeWorld = default;
			UnsafeEntity = default;
			IsBound = false;
		}
	}
}