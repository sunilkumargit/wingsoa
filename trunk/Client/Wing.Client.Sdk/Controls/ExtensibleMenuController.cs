using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.Client.Sdk.Controls
{
    public abstract class ExtensibleMenuController
    {
        private List<ExtensibleMenuController> _items = new List<ExtensibleMenuController>();
        private ReadOnlyCollection<ExtensibleMenuController> _itemsReadOnly;
        private List<IExtensibleMenuItem> _visualItems = new List<IExtensibleMenuItem>();
        private ReadOnlyCollection<IExtensibleMenuItem> _visualItemsReadOnly;

        protected abstract ExtensibleMenuController CreateItemInstance(String id, String caption);
        protected abstract void RemoveItemInstance(ExtensibleMenuController item);

        public ExtensibleMenuController(IExtensibleMenuItem item, String id)
        {
            _itemsReadOnly = new ReadOnlyCollection<ExtensibleMenuController>(_items);
            _visualItemsReadOnly = new ReadOnlyCollection<IExtensibleMenuItem>(_visualItems);
            ItemId = id;
            VisualElement = item;
        }

        public IExtensibleMenuItem VisualElement { get; private set; }
        public ExtensibleMenuController Parent { get; internal set; }
        public String ItemId { get; private set; }

        #region IExtensibleMenu Members

        public ExtensibleMenuController CreateItem(string id, string caption, string parentId)
        {
            if (parentId.HasValue())
            {
                var parent = FindItem(parentId, true);
                if (parent == null)
                    throw new Exception(String.Format("O item com id {0} não existem", parentId));
                return parent.CreateItem(id, caption, null);
            }
            else
            {
                return VisualContext.Sync<ExtensibleMenuController>(() =>
                {
                    var existing = FindItem(id, true);
                    if (existing != null)
                        throw new Exception(String.Format("O item com o id {0} já existe no menu", id));
                    existing = CreateItemInstance(id, caption);
                    existing.Parent = this;
                    _items.Add(existing);
                    _visualItems.Add(existing.VisualElement);
                    return existing;
                });
            }
        }

        public ExtensibleMenuController FindItem(string id, bool searchInChilds)
        {
            var result = _items.FirstOrDefault(i => i.ItemId.Equals(id));
            if (result == null && searchInChilds)
            {
                foreach (var item in _items)
                {
                    result = item.FindItem(id, true);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        public void RemoveItem(string id)
        {
            var item = FindItem(id, true);
            if (item != null)
                item.Parent.RemoveItemInstanceInternal(item);
        }

        internal void RemoveItemInstanceInternal(ExtensibleMenuController item)
        {
            VisualContext.Sync(() =>
            {
                _visualItems.Remove(item.VisualElement);
                _items.Remove(item);
                RemoveItemInstance(item);
            });
        }

        public ReadOnlyCollection<ExtensibleMenuController> Items
        {
            get { return _itemsReadOnly; }
        }

        public ReadOnlyCollection<IExtensibleMenuItem> VisualItems
        {
            get { return _visualItemsReadOnly; }
        }

        #endregion
    }
}
