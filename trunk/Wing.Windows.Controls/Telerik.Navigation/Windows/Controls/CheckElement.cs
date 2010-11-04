namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Automation;
    using System.Windows.Controls.Primitives;

    internal class CheckElement : OptionElement
    {
        private void CheckItem(RadTreeViewItem itemContainer, ToggleState state)
        {
            if (itemContainer.OptionElement is CheckElement)
            {
                itemContainer.CheckState = state;
                RadTreeView parentTreeView = itemContainer.ParentTreeView;
                if (parentTreeView == null)
                {
                    parentTreeView = itemContainer.SearchForParentTreeView();
                }
                RadTreeViewItem childItemContainer = null;
                object item = 0;
                int itemsCount = itemContainer.Items.Count;
                for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
                {
                    item = itemContainer.Items[itemIndex];
                    childItemContainer = (itemContainer.ItemContainerGenerator.ContainerFromIndex(itemIndex) as RadTreeViewItem) ?? (item as RadTreeViewItem);
                    if (childItemContainer == null)
                    {
                        if (((OptionListType) parentTreeView.ItemStorage.GetValue<OptionListType>(item, RadTreeViewItem.OptionTypeProperty, OptionListType.Default)) == OptionListType.CheckList)
                        {
                            parentTreeView.ItemStorage.SetValue(item, RadTreeViewItem.IsInCheckBoxPropagateStateModePropery, true);
                            parentTreeView.StoreCheckState(null, item, state);
                        }
                    }
                    else
                    {
                        OptionListType optionType = childItemContainer.OptionType;
                        switch (optionType)
                        {
                            case OptionListType.Default:
                                parentTreeView.SetItemOptionElementType(childItemContainer);
                                optionType = childItemContainer.OptionType;
                                break;

                            case OptionListType.CheckList:
                            {
                                bool oldState = childItemContainer.IsInCheckBoxPropagateStateMode;
                                childItemContainer.IsInCheckBoxPropagateStateMode = true;
                                this.CheckItem(childItemContainer, state);
                                childItemContainer.IsInCheckBoxPropagateStateMode = oldState;
                                goto Label_00EA;
                            }
                        }
                    Label_00EA:;
                    }
                }
            }
        }

        public override void PropagateItemState(RadTreeViewItem itemContainer)
        {
            if (itemContainer != null)
            {
                RadTreeView parentTreeView = itemContainer.ParentTreeView;
                if (parentTreeView != null)
                {
                    ToggleState state = itemContainer.CheckState;
                    if ((state == ToggleState.Indeterminate) && !parentTreeView.IsTriStateMode)
                    {
                        itemContainer.CheckState = ToggleState.Off;
                    }
                    if (parentTreeView.IsOptionElementsEnabled && parentTreeView.IsTriStateMode)
                    {
                        RadTreeViewItem childItemContainer = null;
                        object item = 0;
                        int itemsCount = itemContainer.Items.Count;
                        ToggleState checkState = itemContainer.CheckState;
                        for (int itemIndex = 0; itemIndex < itemsCount; itemIndex++)
                        {
                            item = itemContainer.Items[itemIndex];
                            childItemContainer = (itemContainer.ItemContainerGenerator.ContainerFromIndex(itemIndex) as RadTreeViewItem) ?? (item as RadTreeViewItem);
                            if (childItemContainer == null)
                            {
                                switch (parentTreeView.ItemStorage.GetValue<OptionListType>(item, RadTreeViewItem.OptionTypeProperty, OptionListType.Default))
                                {
                                    case OptionListType.CheckList:
                                    case OptionListType.Default:
                                        parentTreeView.ItemStorage.SetValue(item, RadTreeViewItem.IsInCheckBoxPropagateStateModePropery, true);
                                        parentTreeView.StoreCheckState(null, item, state);
                                        break;
                                }
                            }
                            else
                            {
                                OptionListType optionType = childItemContainer.OptionType;
                                switch (optionType)
                                {
                                    case OptionListType.Default:
                                        parentTreeView.SetItemOptionElementType(childItemContainer);
                                        optionType = childItemContainer.OptionType;
                                        break;

                                    case OptionListType.CheckList:
                                        childItemContainer.SetCheckStateWithNoPropagation(state);
                                        this.CheckItem(childItemContainer, checkState);
                                        goto Label_0102;
                                }
                            Label_0102:;
                            }
                        }
                        this.PropagateStateOnParentItem(itemContainer.ParentItem);
                    }
                }
            }
        }

        internal void PropagateStateOnParentItem(RadTreeViewItem item)
        {
            if ((item != null) && (item.OptionElement is CheckElement))
            {
                ToggleState state = RadTreeViewItem.CalculateItemCheckState(item);
                item.SetCheckStateWithNoPropagation(state);
                item = item.ParentItem;
                while ((item != null) && (item.ItemsOptionListType == OptionListType.Default))
                {
                    this.PropagateStateOnParentItem(item);
                    item = item.ParentItem;
                }
            }
        }

        public override void Render(ToggleButton toggleElement, ToggleState state)
        {
            this.Show(toggleElement);
            bool? isChecked = false;
            if (state == ToggleState.On)
            {
                isChecked = true;
            }
            else if (state == ToggleState.Indeterminate)
            {
                isChecked = null;
            }
            if (toggleElement != null)
            {
                toggleElement.IsChecked = isChecked;
            }
        }
    }
}

