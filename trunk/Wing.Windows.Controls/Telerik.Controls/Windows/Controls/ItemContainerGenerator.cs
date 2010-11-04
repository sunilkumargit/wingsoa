namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Telerik.Windows.Controls.Primitives;

    [ScriptableType]
    public sealed class ItemContainerGenerator : IRecyclingItemContainerGenerator, IItemContainerGenerator
    {
        private int generatedItemsCount;
        private System.Windows.Controls.ItemContainerGenerator originalGenerator;
        private IItemContainerGenerator originalIGenerator;
        private GeneratorStatus status;

        public event ItemsChangedEventHandler ItemsChanged;

        [ScriptableMember]
        public event EventHandler StatusChanged;

        public ItemContainerGenerator(System.Windows.Controls.ItemContainerGenerator originalGenerator)
        {
            this.originalGenerator = originalGenerator;
            this.originalIGenerator = this.originalIGenerator;
            this.originalGenerator.ItemsChanged += new ItemsChangedEventHandler(this.OnOriginalGeneratorItemsChanged);
        }

        [ScriptableMember]
        public DependencyObject ContainerFromIndex(int index)
        {
            return this.originalGenerator.ContainerFromIndex(index);
        }

        [ScriptableMember]
        public DependencyObject ContainerFromItem(object item)
        {
            return this.originalGenerator.ContainerFromItem(item);
        }

        public GeneratorPosition GeneratorPositionFromIndex(int itemIndex)
        {
            return this.originalGenerator.GeneratorPositionFromIndex(itemIndex);
        }

        [ScriptableMember]
        public int IndexFromContainer(DependencyObject container)
        {
            return this.originalGenerator.IndexFromContainer(container);
        }

        public int IndexFromGeneratorPosition(GeneratorPosition position)
        {
            return this.originalGenerator.IndexFromGeneratorPosition(position);
        }

        [ScriptableMember]
        public object ItemFromContainer(DependencyObject container)
        {
            return this.originalGenerator.ItemFromContainer(container);
        }

        internal void NotifyBeginPrepareContainer()
        {
            this.SetStatus(GeneratorStatus.GeneratingContainers);
        }

        internal void NotifyEndPrepareContainer(int itemsCount)
        {
            this.generatedItemsCount++;
            if (this.generatedItemsCount == itemsCount)
            {
                this.SetStatus(GeneratorStatus.ContainersGenerated);
            }
        }

        private void OnOriginalGeneratorItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (this.Status == GeneratorStatus.ContainersGenerated)
                    {
                        this.generatedItemsCount--;
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.generatedItemsCount = 0;
                    break;
            }
            if (this.ItemsChanged != null)
            {
                this.ItemsChanged(this, e);
            }
        }

        private void SetStatus(GeneratorStatus value)
        {
            if (value != this.status)
            {
                this.status = value;
                if (this.StatusChanged != null)
                {
                    this.StatusChanged(this, EventArgs.Empty);
                }
            }
        }

        DependencyObject IItemContainerGenerator.GenerateNext(out bool isNewlyRealized)
        {
            return this.originalIGenerator.GenerateNext(out isNewlyRealized);
        }

        System.Windows.Controls.ItemContainerGenerator IItemContainerGenerator.GetItemContainerGeneratorForPanel(Panel panel)
        {
            return this.originalIGenerator.GetItemContainerGeneratorForPanel(panel);
        }

        void IItemContainerGenerator.PrepareItemContainer(DependencyObject container)
        {
            this.originalIGenerator.PrepareItemContainer(container);
        }

        void IItemContainerGenerator.Remove(GeneratorPosition position, int count)
        {
            this.originalIGenerator.Remove(position, count);
        }

        void IItemContainerGenerator.RemoveAll()
        {
            this.originalIGenerator.RemoveAll();
        }

        IDisposable IItemContainerGenerator.StartAt(GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem)
        {
            return this.originalIGenerator.StartAt(position, direction, allowStartAtRealizedItem);
        }

        void IRecyclingItemContainerGenerator.Recycle(GeneratorPosition position, int count)
        {
            (this.originalIGenerator as IRecyclingItemContainerGenerator).Recycle(position, count);
        }

        [ScriptableMember]
        public GeneratorStatus Status
        {
            get
            {
                return this.status;
            }
        }
    }
}

