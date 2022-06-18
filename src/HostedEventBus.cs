using UnityEngine;

namespace Leopotam.EcsLite
{
	public class HostedEventBus : MonoBehaviour, IHostedService, IEventBus
	{
		IEventBus _bus = new EventsBus();


		public EcsWorld GetEventsWorld() => _bus.GetEventsWorld();


		public void Destroy() => _bus.Destroy();

		public IEventBus_Uniques UniqueEvents => _bus.UniqueEvents;
		public IEventBus_Globals GlobalEvents => _bus.GlobalEvents;
		public IEventBus_EntityEvents EntityEvents => _bus.EntityEvents;

		public IEventBus_FlagComponents FlagComponents => _bus.FlagComponents;
		
		
		public IEcsSystem AllEventsProcessor() => _bus.AllEventsProcessor();
	}
}