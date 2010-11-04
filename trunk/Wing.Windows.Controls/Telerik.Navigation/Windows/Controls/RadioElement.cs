namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    internal class RadioElement : OptionElement
    {
        public override void PropagateItemState(RadTreeViewItem itemContainer)
        {
            if (itemContainer != null)
            {
                RadTreeView treeView = itemContainer.ParentTreeView;
                if (((treeView != null) && treeView.IsOptionElementsEnabled) && (itemContainer.CheckState != ToggleState.Off))
                {
                    ItemCollection items = itemContainer.Owner.Items;
                    RadTreeView parentTreeView = itemContainer.ParentTreeView;
                    Telerik.Windows.Controls.ItemsControl owner = itemContainer.Owner;
                    RadTreeViewItem childItemContainer = null;
                    object item = 0;
                    int itemsCount = owner.Items.Count;
                    if ((parentTreeView != null) && (owner != null))
                    {
                        for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
                        {
                            item = owner.Items[itemIndex];
                            childItemContainer = (owner.ItemContainerGenerator.ContainerFromIndex(itemIndex) as RadTreeViewItem) ?? (item as RadTreeViewItem);
                            if (childItemContainer == null)
                            {
                                if (((OptionListType) parentTreeView.ItemStorage.GetValue<OptionListType>(item, RadTreeViewItem.OptionTypeProperty, OptionListType.CheckList)) == OptionListType.OptionList)
                                {
                                    parentTreeView.StoreCheckState(null, item, ToggleState.Off);
                                }
                            }
                            else if ((childItemContainer != itemContainer) && (childItemContainer.OptionType == OptionListType.OptionList))
                            {
                                bool oldState = childItemContainer.IsInCheckBoxPropagateStateMode;
                                childItemContainer.IsInCheckBoxPropagateStateMode = true;
                                childItemContainer.CheckState = ToggleState.Off;
                                childItemContainer.IsInCheckBoxPropagateStateMode = oldState;
                            }
                        }
                    }
                }
            }
        }

        public override void Render(ToggleButton toggleElement, ToggleState state)
        {
            this.Show(toggleElement);
            if (toggleElement != null)
            {
                toggleElement.IsChecked = new bool?(state == ToggleState.On);
            }
        }
    }
}

