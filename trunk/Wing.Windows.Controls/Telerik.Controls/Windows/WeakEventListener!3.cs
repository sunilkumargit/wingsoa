namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;

    internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        private WeakReference weakInstance;

        public WeakEventListener(TInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            this.weakInstance = new WeakReference(instance);
        }

        public void Detach()
        {
            if (this.OnDetachAction != null)
            {
                this.OnDetachAction((WeakEventListener<TInstance, TSource, TEventArgs>)this);
                this.OnDetachAction = null;
            }
        }

        public void OnEvent(TSource source, TEventArgs eventArgs)
        {
            TInstance target = (TInstance)this.weakInstance.Target;
            if (target != null)
            {
                if (this.OnEventAction != null)
                {
                    this.OnEventAction(target, source, eventArgs);
                }
            }
            else
            {
                this.Detach();
            }
        }

        public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

        public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }
    }
}

