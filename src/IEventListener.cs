#if UNITY_5_3_OR_NEWER

namespace Leopotam.EcsLite
{
	public interface IEventListener
	{
		void RegisterListeners(IServiceContainer container, EcsPackedEntityWithWorld entity);
		void ReleaseListeners();
	}
}

#endif