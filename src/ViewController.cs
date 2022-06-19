using UnityEngine;

namespace Leopotam.EcsLite
{
	/// <summary>
	/// A handy pattern for use with Events that provides a framework for establishing a GameObject view and
	/// handling setting up/tearing down the listeners on the view.  When using the ViewController pattern,
	/// one ViewController should be present on the GameObject representing the Entity, and then any number of
	/// listeners attached as IEventListener implementing MonoBehaviours.
	/// </summary>
	public class ViewController : MonoBehaviour, IViewController
	{
		protected EcsPackedEntityWithWorld Entity;

		public Vector2 Position
		{
			get { return transform.position; }
			set { transform.position = value; }
		}

		public Vector2 Scale 
		{
			get { return transform.localScale; }
			set { transform.localScale = value; }
		}

		public bool Active
		{
			get { return gameObject.activeSelf; }
			set { gameObject.SetActive(value); }
		}


		public void InitializeView(IServiceContainer services, EcsPackedEntityWithWorld entity)
		{
			Entity = entity;
			
			OnPreInitializeListeners(services);

			var eventListeners = GetComponentsInChildren<IEventListener>();
			foreach (var eventListener in eventListeners)
			{
				if (eventListener is MonoBehaviour mono)
					if (mono.isActiveAndEnabled)
						eventListener.RegisterListeners(services, entity);
			}
			
			OnPostInitializeListeners(services);
		}


		protected virtual void OnPreInitializeListeners(IServiceContainer serviceContainer)
		{
		}
		
		
		protected virtual void OnPostInitializeListeners(IServiceContainer serviceContainer)
		{
		}
		

		public virtual void DestroyView()
		{
			OnPreDestroyListeners();
			
			var eventListeners = GetComponentsInChildren<IEventListener>();
			foreach (var eventListener in eventListeners)
			{
				if (eventListener is MonoBehaviour mono)
					eventListener.ReleaseListeners();
			}
			
			OnPostDestroyListeners();

			Entity = default;
			OnViewDestroyed();
		}

		
		protected virtual void OnPreDestroyListeners()
		{
		}
		
		
		protected virtual void OnPostDestroyListeners()
		{
		}

		

		/// <summary>
		/// Controls how the object is destroyed.  By default, the gameobject is simply destroyed.  Override this
		/// to provide a pooling implementation, etc.
		/// </summary>
		public virtual void OnViewDestroyed()
		{
			Object.Destroy(this);
		}
	}
}