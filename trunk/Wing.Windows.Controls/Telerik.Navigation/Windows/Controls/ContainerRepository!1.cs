namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;

    internal class ContainerRepository<T> where T: FrameworkElement, new()
    {
        private int collectedCount;
        private Queue<WeakReference> containers;
        private int hitCount;
        private int recycleCount;

        internal ContainerRepository()
        {
            this.containers = new Queue<WeakReference>(0x40);
            this.IsContainerReuseEnabled = true;
        }

        internal T GetContainer()
        {
            T result = default(T);
            while (this.containers.Count > 0)
            {
                WeakReference reference = this.containers.Dequeue();
                if (reference.IsAlive)
                {
                    T target = reference.Target as T;
                    if ((bool) target.GetValue(ContainerRepositoryRegister.IsStoredProperty))
                    {
                        result = reference.Target as T;
                        result.SetValue(ContainerRepositoryRegister.IsStoredProperty, false);
                        this.hitCount++;
                        break;
                    }
                }
                this.collectedCount++;
            }
            if (result != null)
            {
                return result;
            }
            return Activator.CreateInstance<T>();
        }

        internal bool RecycleContainer(T container)
        {
            if (!this.IsContainerReuseEnabled)
            {
                return false;
            }
            if ((bool) container.GetValue(ContainerRepositoryRegister.IsStoredProperty))
            {
                return false;
            }
            container.SetValue(ContainerRepositoryRegister.IsStoredProperty, true);
            this.recycleCount++;
            this.containers.Enqueue(new WeakReference(container));
            return true;
        }

        internal void ResetCounters()
        {
            this.recycleCount = 0;
            this.hitCount = 0;
        }

        internal bool IsContainerReuseEnabled{get;set;}

        internal int RecycledContainersCount
        {
            get
            {
                return this.recycleCount;
            }
        }

        internal int ReusedContainersCount
        {
            get
            {
                return this.hitCount;
            }
        }

        internal int UnavailableContainersCount
        {
            get
            {
                return this.collectedCount;
            }
        }
    }
}

