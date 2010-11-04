namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Controls;
    using Telerik.Windows;

    internal class SelectionChanger<T> : ObservableCollection<T>
    {
        private List<T> addToSelection;
        private Func<System.Windows.Controls.SelectionChangedEventArgs, bool> canChangeSelection;
        private Func<T, bool> canSelectItem;
        private Func<T, bool> canUnselectItem;
        private Func<T, T> coerceItem;
        private List<T> internalSelection;
        private bool isActive;
        private WeakReferenceList<T> removeFromSelection;
        private System.Windows.Controls.SelectionChangedEventHandler _selectionChanged;

        public event System.Windows.Controls.SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                System.Windows.Controls.SelectionChangedEventHandler handler2;
                System.Windows.Controls.SelectionChangedEventHandler selectionChanged = this._selectionChanged;
                do
                {
                    handler2 = selectionChanged;
                    System.Windows.Controls.SelectionChangedEventHandler handler3 = (System.Windows.Controls.SelectionChangedEventHandler) Delegate.Combine(handler2, value);
                    selectionChanged = Interlocked.CompareExchange<System.Windows.Controls.SelectionChangedEventHandler>(ref this._selectionChanged, handler3, handler2);
                }
                while (selectionChanged != handler2);
            }
            remove
            {
                System.Windows.Controls.SelectionChangedEventHandler handler2;
                System.Windows.Controls.SelectionChangedEventHandler selectionChanged = this._selectionChanged;
                do
                {
                    handler2 = selectionChanged;
                    System.Windows.Controls.SelectionChangedEventHandler handler3 = (System.Windows.Controls.SelectionChangedEventHandler) Delegate.Remove(handler2, value);
                    selectionChanged = Interlocked.CompareExchange<System.Windows.Controls.SelectionChangedEventHandler>(ref this._selectionChanged, handler3, handler2);
                }
                while (selectionChanged != handler2);
            }
        }

        public SelectionChanger()
        {
            this.addToSelection = new List<T>(1);
            this.removeFromSelection = new WeakReferenceList<T>();
            this.internalSelection = new List<T>(1);
            this.InitFlags();
        }

        public SelectionChanger(Func<T, bool> isItemValidForSelection) : this()
        {
            this.canSelectItem = isItemValidForSelection;
        }

        public SelectionChanger(Func<T, bool> isItemValidForSelection, Func<T, bool> isItemValidForUnselection, Func<System.Windows.Controls.SelectionChangedEventArgs, bool> isSelectionChangePossible) : this(isItemValidForSelection)
        {
            this.canChangeSelection = isSelectionChangePossible;
            this.canUnselectItem = isItemValidForUnselection;
        }

        internal void AddJustThis(T item)
        {
            this.Begin();
            int count = base.Items.Count;
            for (int i = 0; i < count; i++)
            {
                T itemInCollection = base.Items[i];
                if (!itemInCollection.Equals(item))
                {
                    base.Remove(itemInCollection);
                }
            }
            if (!base.Items.Contains(item))
            {
                base.Add(item);
            }
            this.End();
        }

        private void AddToSelection(T item)
        {
            bool result = true;
            T coercedItem = default(T);
            if (this.CoerceItemCallback != null)
            {
                coercedItem = this.CoerceItemCallback(item);
            }
            if (this.canSelectItem != null)
            {
                result = this.canSelectItem((this.CoerceItemCallback != null) ? coercedItem : item);
            }
            if (result)
            {
                T newItem = (this.CoerceItemCallback != null) ? coercedItem : item;
                if (newItem != null)
                {
                    this.addToSelection.Add(newItem);
                }
            }
        }

        internal void Begin()
        {
            this.isActive = true;
            this.addToSelection.Clear();
            this.removeFromSelection.Clear();
        }

        internal System.Windows.Controls.SelectionChangedEventArgs BuildSelectionChangedEventArgs()
        {
            return new System.Windows.Controls.SelectionChangedEventArgs(this.removeFromSelection.ToList<T>(), this.addToSelection);
        }

        internal void Cancel()
        {
            this.InitFlags();
            if (this.addToSelection.Count > 0)
            {
                this.addToSelection.Clear();
            }
            if (this.removeFromSelection.Count > 0)
            {
                this.removeFromSelection.Clear();
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            this.addToSelection.Clear();
            this.removeFromSelection = new WeakReferenceList<T>();
            if (this.internalSelection != null)
            {
                foreach (T item in this.internalSelection)
                {
                    this.removeFromSelection.Add(item);
                }
            }
            this.internalSelection.Clear();
            if (!this.isActive)
            {
                this.InvokeSelectionChangedEvent();
            }
        }

        internal void End()
        {
            bool isRealSelectionChangePending = (this.removeFromSelection.Count > 0) || (this.addToSelection.Count > 0);
            System.Windows.Controls.SelectionChangedEventArgs e = this.BuildSelectionChangedEventArgs();
            if ((isRealSelectionChangePending && (this.canChangeSelection != null)) && this.canChangeSelection(e))
            {
                this.Cancel();
            }
            else
            {
                this.InitFlags();
                this.SynchronizeInternalSelection();
                if (isRealSelectionChangePending)
                {
                    this.InvokeSelectionChangedEvent(e);
                }
            }
        }

        private void InitFlags()
        {
            this.isActive = false;
        }

        protected override void InsertItem(int index, T item)
        {
            if (this.isActive)
            {
                this.Select(item);
            }
            else
            {
                this.Begin();
                this.Select(item);
                this.End();
            }
        }

        private void InvokeSelectionChangedEvent()
        {
            this.InvokeSelectionChangedEvent(this.BuildSelectionChangedEventArgs());
        }

        private void InvokeSelectionChangedEvent(System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this._selectionChanged != null)
            {
                this._selectionChanged(this, e);
            }
        }

        private void RemoveFromSelection(T item)
        {
            bool result = true;
            if (this.canUnselectItem != null)
            {
                result = this.canUnselectItem(item);
            }
            if (result && (item != null))
            {
                this.removeFromSelection.Add(item);
            }
        }

        protected override void RemoveItem(int index)
        {
            T changedItem = base[index];
            if (this.isActive)
            {
                this.Unselect(changedItem);
            }
            else
            {
                this.Begin();
                this.Unselect(changedItem);
                this.End();
            }
        }

        private void Select(T item)
        {
            if ((!this.removeFromSelection.Remove(item) && !this.addToSelection.Contains(item)) && !this.internalSelection.Contains(item))
            {
                this.AddToSelection(item);
            }
        }

        private void SelectJustThisItem(T item)
        {
            this.Begin();
            if (base.Items.Count > 0)
            {
                base.Clear();
            }
            try
            {
                if (item != null)
                {
                    if (this.removeFromSelection.Contains(item))
                    {
                        this.removeFromSelection.Remove(item);
                    }
                    else
                    {
                        this.Select(item);
                    }
                }
            }
            finally
            {
                this.End();
            }
        }

        protected override void SetItem(int index, T item)
        {
            this.Begin();
            T oldItem = base[index];
            this.removeFromSelection.Add(oldItem);
            this.addToSelection.Add(item);
            base.SetItem(index, item);
            this.InvokeSelectionChangedEvent();
            this.Cancel();
        }

        private void SynchronizeInternalSelection()
        {
            foreach (T item in this.removeFromSelection)
            {
                int index = base.IndexOf(item);
                if (index > -1)
                {
                    base.RemoveItem(index);
                }
                this.internalSelection.Remove(item);
            }
            foreach (T item in this.addToSelection)
            {
                base.InsertItem(base.Items.Count, item);
                this.internalSelection.Add(item);
            }
        }

        private void Unselect(T item)
        {
            if ((!this.addToSelection.Remove(item) && !this.removeFromSelection.Contains(item)) && this.internalSelection.Contains(item))
            {
                this.RemoveFromSelection(item);
            }
        }

        internal Func<T, T> CoerceItemCallback
        {
            get
            {
                return this.coerceItem;
            }
            set
            {
                this.coerceItem = value;
            }
        }

        private bool IsActive
        {
            get
            {
                return this.isActive;
            }
        }
    }
}

