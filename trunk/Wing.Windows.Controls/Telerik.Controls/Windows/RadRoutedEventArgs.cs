namespace Telerik.Windows
{
    using System;
    using System.Windows;
    using Telerik.Windows.Controls;

    public class RadRoutedEventArgs : RoutedEventArgs
    {
        private bool handled;
        private bool invokingHandler;
        private object originalSource;
        private Telerik.Windows.RoutedEvent routedEvent;
        private object source;

        public RadRoutedEventArgs()
        {
        }

        public RadRoutedEventArgs(Telerik.Windows.RoutedEvent routedEvent) : this(routedEvent, null)
        {
        }

        public RadRoutedEventArgs(Telerik.Windows.RoutedEvent routedEvent, object source)
        {
            this.routedEvent = routedEvent;
            this.source = this.originalSource = source;
        }

        protected virtual void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            if (genericHandler == null)
            {
                throw new ArgumentNullException("genericHandler");
            }
            if (genericTarget == null)
            {
                throw new ArgumentNullException("genericTarget");
            }
            if (this.RoutedEvent == null)
            {
                throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("RoutedEventArgsMustHaveRoutedEvent", new object[0]));
            }
            RadRoutedEventHandler routedEventHandler = genericHandler as RadRoutedEventHandler;
            if (routedEventHandler != null)
            {
                routedEventHandler(genericTarget, this);
            }
            else
            {
                genericHandler.DynamicInvoke(new object[] { genericTarget, this });
            }
        }

        internal void InvokeHandler(Delegate handler, object target)
        {
            this.InvokingHandler = true;
            try
            {
                this.InvokeEventHandler(handler, target);
            }
            finally
            {
                this.InvokingHandler = false;
            }
        }

        protected virtual void OnSetSource(object newSource)
        {
        }

        internal void OverrideRoutedEvent(Telerik.Windows.RoutedEvent newRoutedEvent)
        {
            this.routedEvent = newRoutedEvent;
        }

        internal void OverrideSource(object newSource)
        {
            this.source = newSource;
        }

        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                if (this.routedEvent == null)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("RoutedEventArgsMustHaveRoutedEvent", new object[0]));
                }
                this.handled = value;
            }
        }

        private bool InvokingHandler
        {
            get
            {
                return this.invokingHandler;
            }
            set
            {
                this.invokingHandler = value;
            }
        }

        public object OriginalSource
        {
            get
            {
                return this.originalSource;
            }
        }

        public Telerik.Windows.RoutedEvent RoutedEvent
        {
            get
            {
                return this.routedEvent;
            }
            set
            {
                if (this.InvokingHandler)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("RoutedEventCannotChangeWhileRouting"));
                }
                this.routedEvent = value;
            }
        }

        public object Source
        {
            get
            {
                return this.source;
            }
            set
            {
                if (this.InvokingHandler)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("RoutedEventCannotChangeWhileRouting"));
                }
                if (this.routedEvent == null)
                {
                    throw new InvalidOperationException(Telerik.Windows.Controls.SR.Get("RoutedEventArgsMustHaveRoutedEvent"));
                }
                object newSource = value;
                if ((this.source == null) && (this.originalSource == null))
                {
                    this.source = this.originalSource = newSource;
                    this.OnSetSource(this.source);
                }
                else if (this.source != newSource)
                {
                    this.source = newSource;
                    this.OnSetSource(this.source);
                }
            }
        }
    }
}

