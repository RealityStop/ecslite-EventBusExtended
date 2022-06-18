# EcsLite EventBus Extended

💡 EventBus Extended is an extension for [RealityStop/ecslite-EventBus] that adds a few helpful utilities, particularly for Unity.

⚠️ Unlike the core EventBus, this extension requires the [RealityStop/ecslite-ServiceContainer](https://github.com/RealityStop/ecslite-ServiceContainer) extension.  This is in order to provide an in-scene Hosted EventBus service, so that Unity scripts can wire listeners before the ECS system has started.

# General Features

## DisposableContainer
Offers a simple container for IDisposable objects, with bulk disposal.  This is particularly handy when subscribing to many events or when subscribing to events from many entities.

```C#
var disposings = new DisposableContainer();

//subscribing and storing them in the container
_eventBus.EntityEvents.SubscribeTo<HealthChanged>(packed, OnHealthChanged).DisposeWith(disposings);
_eventBus.EntityEvents.SubscribeTo<PositionChanged>(packed, OnPositionChanged).DisposeWith(disposings);
_eventBus.FlagComponents.SubscribeTo<Invincible>(packed, OnInvincibility).DisposeWith(disposings);

//later
disposings.Dispose();  //All event subscriptions are terminated and released.
```

## Disposable
A class that makes it easy to create disposables from methods, which you can then use in `using` statements, or store them in a `DisposableContainer`.

```C#
	private void DoSomething()
	{
		//Do something cool
	}
	
	Disposable.Create(DoSomthing).DisposeWith(disposings);
```

# Unity Features
The Extended package has a few more tricks that make working in Unity a bit easier.

## HostedEventBus
A Hostable Service (see [RealityStop/ecslite-ServiceContainer](https://github.com/RealityStop/ecslite-ServiceContainer) ), so that you can add an Event Bus to your scene (rather than from script), to make it accessible and subscribable before the ECS system even starts up.  

## ViewController and IViewController
A handy pattern for use with Events that provides a framework for establishing a GameObject view and handling setting up/tearing down the listeners on the view.  When using the ViewController pattern,  one ViewController should be present on the GameObject representing the Entity, and then any number of  listeners attached as IEventListener implementing MonoBehaviours.

![test](../assets/images/viewcontroller.PNG)

This makes it simple for the ECS to request a view from some service that can locate and instantiate the appropriate view prefab, and then request the ViewController `.InitializeView(services, entity)`, which will automatically start up any attached listeners.

## MonoEventListener and IEventListener
When using the ViewController pattern, you need MonoBehaviours that implement IEventListener.  MonoEventListener is a simple base implementation that provides a framework to write your own listeners on.

Here is a simple listener from the performance demo of the EventBus.  This example uses ListenTo to subscribe and overrides `OnUnsubscribe` to manually remove the listener.  `OnSubscribe` and `OnUnsubscribe` are automatically called as a combination of the EventListener being bound and unbound, and as it is enabled and disabled, such that `OnSubscribe` happens on the transition from (Bound and Enabled) and `OnUnsubscribe` happens when transitioning away.

```C#
public class UnityHealthListener_EntityEvent : MonoEventListener  
{  
		private IEventBus _eventBus;  

	protected override void OnSubscribe()  
	{
		_eventBus ??= Services.Get<IEventBus>();  
		_eventBus.EntityEvents.ListenTo<HealthChangedEvent>(PackedEntity, OnHealthChanged);  
	}  

	private void OnHealthChanged(EcsPackedEntityWithWorld packed, ref HealthChangedEvent item)  
	{
		//perform some action
	}  

	protected override void OnUnsubscribe()  
	{ 
		_eventBus.EntityEvents.RemoveListener<HealthChangedEvent>(PackedEntity, OnHealthChanged);  
	}
 }
 ```

There is a second variant of MonoEventListener: MonoEventListener_WithTracking, that includes a Disposable container that it automatically cleans up when unsubscribing.  This makes it quite simple to subscribe to multiple events without needing to track how to unsubscribe.

```C#
public class UnityHealthListener_Tracking : MonoEventListener_WithTracking  
{  
	protected override void OnSubscribe()  
	{
		var eventBus = Services.Get<IEventBus>();  
		eventBus.EntityEvents.SubscribeTo<HealthChangedEvent>(PackedEntity, OnHealthChanged).DisposeWith(_disposings);    
		eventBus.EntityEvents.SubscribeTo<PositionChangedEvent>(PackedEntity, OnPositionChanged).DisposeWith(_disposings);  
		eventBus.FlagComponents.SubscribeTo<Invincible>(PackedEntity, OnInvincibility).DisposeWith(disposings);
	}  

	private void OnHealthChanged(EcsPackedEntityWithWorld packed, ref HealthChangedEvent item) {}

	private void OnPositionChanged(EcsPackedEntityWithWorld packed, ref PositionChangedEvent item) {}

	private void OnInvincibility(EcsPackedEntityWithWorld obj) {}
```



## License
All code in this repository is covered by the [Mozilla Public License](https://www.mozilla.org/en-US/MPL/), which states that you are free to use the code for any purpose, commercial or otherwise, for any type of application or purpose, and that you are free to release your works under whatever license you choose.  However, regardless of application or method, this code remains under the MPL license, and all modifications or portions of it must also remain under the MPL license and be made available, but this is limited to the covered code and modifications to it.  It is NOT viral, nor does it enforce the MPL license on any other portion of your code, as in strong copyleft licenses like GPL and its derivatives.   The intent is that this code is MPL, shall always be MPL regardless of author, and that it and all modified versions should be public and available to all.

Simple guidelines:
| Use| Modify |
|--|--|
| Put a text file in your distribution that states OSS usage, with a link to this repository among any others. | Same as **Use** and make modifications public under the MPL by either issuing a pull request to this repository, forking it, or hosting your own. |

However, these are only guidelines, please see the actual license and [Additional license FAQ](https://www.mozilla.org/en-US/MPL/2.0/FAQ/) for actual terms and conditions.
