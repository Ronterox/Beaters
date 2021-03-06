#define EVENTROUTER_THROWEXCEPTIONS
#if EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plugins.Tools
{
    /// <summary>
    /// MMGameEvents are used throughout the game for general match events
    /// </summary>
    public struct MMGameEvent
    {
        public const string PAUSE = "Pause";
        public const string UNPAUSE = "Unpause";
        public const string LOAD = "Load";

        private string eventName;

        public MMGameEvent(string newName) => eventName = newName;
    }

    /// <summary>
    /// Sound events
    /// </summary>
    public struct MMSfxEvent
    {
        private AudioClip clipToPlay;

        public MMSfxEvent(AudioClip clipToPlay) => this.clipToPlay = clipToPlay;
    }

    /// <summary>
    /// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
    /// Events are structs, you can define any kind of events you want. This manager comes with MMGameEvents, which are 
    /// basically just made of a string, but you can work with more complex ones if you want.
    /// 
    /// To trigger a new event, from anywhere, just call MMEventManager.TriggerEvent(YOUR_EVENT);
    /// For example : MMEventManager.TriggerEvent(new MMGameEvent("GameStart")); will broadcast an MMGameEvent named GameStart to all listeners.
    ///
    /// To start listening to an event from any class, there are 3 things you must do : 
    ///
    /// 1 - tell that your class implements the MMEventListener interface for that kind of event.
    /// For example: public class GUIManager : Singleton<GUIManager>, MMEventListener<MMGameEvent>
    /// You can have more than one of these (one per event type).
    ///
    /// 2 - On Enable and Disable, respectively start and stop listening to the event :
    /// void OnEnable()
    /// {
    /// 	this.MMEventStartListening<MMGameEvent>();
    /// }
    /// void OnDisable()
    /// {
    /// 	this.MMEventStopListening<MMGameEvent>();
    /// }
    /// 
    /// 3 - Implement the MMEventListener interface for that event. For example :
    /// public void OnMMEvent(MMGameEvent gameEvent)
    /// {
    /// 	if (gameEvent.eventName == "GameOver")
    ///		{
    ///			// DO SOMETHING
    ///		}
    /// } 
    /// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
    ///</summary>
    [ExecuteInEditMode]
    public static class MMEventManager
    {
        private static readonly Dictionary<Type, List<MMEventListenerBase>> m_SubscribersList;

        static MMEventManager() => m_SubscribersList = new Dictionary<Type, List<MMEventListenerBase>>();

        /// <summary>
        /// Adds a new subscriber to a certain event.
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="MMEvent">The event type.</typeparam>
        public static void AddListener<MMEvent>(MMEventListener<MMEvent> listener) where MMEvent : struct
        {
            Type eventType = typeof(MMEvent);

            if (!m_SubscribersList.ContainsKey(eventType))
                m_SubscribersList[eventType] = new List<MMEventListenerBase>();

            if (!SubscriptionExists(eventType, listener))
                m_SubscribersList[eventType].Add(listener);
        }

        /// <summary>
        /// Removes a subscriber from a certain event.
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="MMEvent">The event type.</typeparam>
        public static void RemoveListener<MMEvent>(MMEventListener<MMEvent> listener) where MMEvent : struct
        {
            Type eventType = typeof(MMEvent);

            if (!m_SubscribersList.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
                throw new ArgumentException($"Removing listener \"{listener}\", but the event type \"{eventType}\" isn't registered.");
#else
                return;
#endif
            }

            List<MMEventListenerBase> subscriberList = m_SubscribersList[eventType].ToList();
            var listenerFound = false;

            if (listenerFound) { }

            for (var i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] != listener) continue;
                subscriberList.Remove(subscriberList[i]);
                listenerFound = true;

                if (subscriberList.Count == 0)
                    m_SubscribersList.Remove(eventType);

                return;
            }

#if EVENTROUTER_THROWEXCEPTIONS
            if (!listenerFound)
            {
                throw new ArgumentException($"Removing listener, but the supplied receiver isn't subscribed to event type \"{eventType}\".");
            }
#endif
        }

        /// <summary>
        /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
        /// </summary>
        /// <param name="newEvent">The event to trigger.</param>
        /// <typeparam name="MMEvent">The 1st type parameter.</typeparam>
        public static void TriggerEvent<MMEvent>(MMEvent newEvent) where MMEvent : struct
        {
            if (!m_SubscribersList.TryGetValue(typeof(MMEvent), out List<MMEventListenerBase> list))
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            foreach (MMEventListenerBase t in list) { (t as MMEventListener<MMEvent>)?.OnMMEvent(newEvent); }
        }

        /// <summary>
        /// Checks if there are subscribers for a certain type of events
        /// </summary>
        /// <returns><c>true</c>, if exists was suscriptioned, <c>false</c> otherwise.</returns>
        /// <param name="type">type.</param>
        /// <param name="receiver">Receiver.</param>
        private static bool SubscriptionExists(Type type, MMEventListenerBase receiver) => m_SubscribersList.TryGetValue(type, out List<MMEventListenerBase> receivers) && receivers.Any(t => t == receiver);
    }

    /// <summary>
    /// Static class that allows any class to start or stop listening to events
    /// </summary>
    public static class EventRegister
    {
        public delegate void Delegate<in T>(T eventType);

        public static void MMEventStartListening<EventType>(this MMEventListener<EventType> caller) where EventType : struct => MMEventManager.AddListener<EventType>(caller);

        public static void MMEventStopListening<EventType>(this MMEventListener<EventType> caller) where EventType : struct => MMEventManager.RemoveListener<EventType>(caller);
    }

    /// <summary>
    /// Event listener basic interface
    /// </summary>
    public interface MMEventListenerBase { };

    /// <summary>
    /// A public interface you'll need to implement for each type of event you want to listen to.
    /// </summary>
    public interface MMEventListener<in T> : MMEventListenerBase
    {
        void OnMMEvent(T eventType);
    }
}
