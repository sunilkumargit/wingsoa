namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Windows;
    using Telerik.Windows.Controls;

    public abstract class SaveLoadLayoutHelper
    {
        protected SaveLoadLayoutHelper()
        {
        }

        internal void AttachEvents(RadDocking docking)
        {
            docking.ElementSaved += new EventHandler<LayoutSerializationEventArgs>(this.OnElementSaved);
            docking.ElementSaving += new EventHandler<LayoutSerializationEventArgs>(this.OnElementSaving);
            docking.ElementLoading += new EventHandler<LayoutSerializationLoadingEventArgs>(this.OnElementLoading);
            docking.ElementLoaded += new EventHandler<LayoutSerializationEventArgs>(this.OnElementLoaded);
            docking.ElementCleaning += new EventHandler<LayoutSerializationEventArgs>(this.OnElementCleaning);
            docking.ElementCleaned += new EventHandler<LayoutSerializationEventArgs>(this.OnElementCleaned);
            this.AttachEventsOverride(docking);
        }

        protected virtual void AttachEventsOverride(RadDocking docking)
        {
        }

        internal void DetachEvents(RadDocking docking)
        {
            this.DetachEventsOverride(docking);
            docking.ElementSaved -= new EventHandler<LayoutSerializationEventArgs>(this.OnElementSaved);
            docking.ElementSaving -= new EventHandler<LayoutSerializationEventArgs>(this.OnElementSaving);
            docking.ElementLoading -= new EventHandler<LayoutSerializationLoadingEventArgs>(this.OnElementLoading);
            docking.ElementLoaded -= new EventHandler<LayoutSerializationEventArgs>(this.OnElementLoaded);
            docking.ElementCleaning -= new EventHandler<LayoutSerializationEventArgs>(this.OnElementCleaning);
            docking.ElementCleaned -= new EventHandler<LayoutSerializationEventArgs>(this.OnElementCleaned);
        }

        protected virtual void DetachEventsOverride(RadDocking docking)
        {
        }

        protected virtual void ElementCleanedOverride(string serializationTag, DependencyObject element)
        {
        }

        protected virtual void ElementCleaningOverride(string serializationTag, DependencyObject element)
        {
        }

        protected virtual void ElementLoadedOverride(string serializationTag, DependencyObject element)
        {
        }

        protected virtual DependencyObject ElementLoadingOverride(string serializationTag)
        {
            return null;
        }

        protected virtual void ElementSavedOverride(string serializationTag, DependencyObject element)
        {
        }

        protected virtual void ElementSavingOverride(string serializationTag, DependencyObject element)
        {
        }

        private void OnElementCleaned(object sender, LayoutSerializationEventArgs e)
        {
            this.ElementCleanedOverride(e.AffectedElementSerializationTag, e.AffectedElement);
        }

        private void OnElementCleaning(object sender, LayoutSerializationEventArgs e)
        {
            this.ElementCleaningOverride(e.AffectedElementSerializationTag, e.AffectedElement);
        }

        private void OnElementLoaded(object sender, LayoutSerializationEventArgs e)
        {
            this.ElementLoadedOverride(e.AffectedElementSerializationTag, e.AffectedElement);
        }

        private void OnElementLoading(object sender, LayoutSerializationLoadingEventArgs e)
        {
            e.SetAffectedElement(this.ElementLoadingOverride(e.AffectedElementSerializationTag));
        }

        private void OnElementSaved(object sender, LayoutSerializationEventArgs e)
        {
            this.ElementSavedOverride(e.AffectedElementSerializationTag, e.AffectedElement);
        }

        private void OnElementSaving(object sender, LayoutSerializationEventArgs e)
        {
            this.ElementSavingOverride(e.AffectedElementSerializationTag, e.AffectedElement);
        }
    }
}

