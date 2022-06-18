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
	
	/// <summary>
	/// A class that listens to one or more events on one entity.
	/// </summary>
	public abstract class MonoEventListener : MonoBehaviour, IEventListener
	{
		/// <summary>
		/// Whether this listener is bound to an entity.
		/// </summary>
		public bool IsBound { get; private set; }
		
		/// <summary>
		/// The cached unpacked world.  ONLY SAFE FOR USE INSIDE OF OnSubscribe AND EVENT CALLBACKS!  Otherwise, use PackedEntity.
		/// </summary>
		protected EcsWorld UnsafeWorld { get; private set; }
		
		/// <summary>
		/// The cached unpacked entity.  ONLY SAFE FOR USE INSIDE OF OnSubscribe AND EVENT CALLBACKS!  Otherwise, use PackedEntity.
		/// </summary>
		protected int UnsafeEntity { get; private set; }
		
		/// <summary>
		/// The packed entity that this listener is bound to, which should always be safe.  When inside
		/// of an event callback, it is faster to use the cached Unsafe versions, as it is only possible to get the
		/// callback if that entity is still alive.  Outside of the callback and OnSubscribe, however, you must use the PackedEntity
		/// version.
		/// </summary>
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