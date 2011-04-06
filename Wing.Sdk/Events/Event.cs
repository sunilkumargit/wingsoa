using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Events
{
    public class Event<TPayload> : EventBase
    {
        public virtual SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive, Predicate<TPayload> filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference;
            if (filter != null)
            {
                filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);
            }
            else
            {
                filterReference = new DelegateReference(new Predicate<TPayload>(delegate { return true; }), true);
            }

            EventSubscription<TPayload> subscription = new EventSubscription<TPayload>(actionReference, filterReference);
            return base.InternalSubscribe(subscription);
        }

        public virtual SubscriptionToken Subscribe(Action<TPayload> action, Predicate<TPayload> filter)
        {
            return Subscribe(action, true, filter);
        }

        public virtual SubscriptionToken Subscribe(Action<TPayload> action, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(action, keepSubscriberReferenceAlive, null);
        }

        /// <summary>
        /// Publishes the <see cref="CompositePresentationEvent{TPayload}"/>.
        /// </summary>
        /// <param name="payload">Message to pass to the subscribers.</param>
        public virtual void Publish(TPayload payload)
        {
            base.InternalPublish(payload);
        }

        /// <summary>
        /// Removes the first subscriber matching <seealso cref="Action{TPayload}"/> from the subscribers' list.
        /// </summary>
        /// <param name="subscriber">The <see cref="Action{TPayload}"/> used when subscribing to the event.</param>
        public virtual void Unsubscribe(Action<TPayload> subscriber)
        {
            lock (Subscriptions)
            {
                IEventSubscription eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if there is a subscriber matching <seealso cref="Action{TPayload}"/>.
        /// </summary>
        /// <param name="subscriber">The <see cref="Action{TPayload}"/> used when subscribing to the event.</param>
        /// <returns><see langword="true"/> if there is an <seealso cref="Action{TPayload}"/> that matches; otherwise <see langword="false"/>.</returns>
        public virtual bool Contains(Action<TPayload> subscriber)
        {
            IEventSubscription eventSubscription;
            lock (Subscriptions)
            {
                eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            }
            return eventSubscription != null;
        }

    }
}
