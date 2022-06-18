using Leopotam.EcsLite;
using UnityEngine;

namespace Leopotam.EcsLite
{
	/// <summary>
	/// Represents an observing GameObject
	/// </summary>
	public interface IViewController
	{
		Vector2 Position {get; set;}
		Vector2 Scale {get; set;}
		bool Active {get; set;}
		void InitializeView(IServiceContainer services, EcsPackedEntityWithWorld entity);
		void DestroyView();
	}
}