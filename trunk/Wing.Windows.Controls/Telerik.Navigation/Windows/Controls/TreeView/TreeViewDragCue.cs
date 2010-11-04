namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    [TemplateVisualState(GroupName="DropStates", Name="DropImpossible"), TemplateVisualState(GroupName="DropStates", Name="DropPossible")]
    public class TreeViewDragCue : Telerik.Windows.Controls.ItemsControl
    {
        public static readonly DependencyProperty DragActionContentProperty = DependencyProperty.Register("DragActionContent", typeof(object), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DragActionContentTemplateProperty = DependencyProperty.Register("DragActionContentTemplate", typeof(DataTemplate), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DragPreviewVisibilityProperty = DependencyProperty.Register("DragPreviewVisibility", typeof(Visibility), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DragTooltipContentProperty = DependencyProperty.Register("DragTooltipContent", typeof(object), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DragTooltipContentTemplateProperty = DependencyProperty.Register("DragTooltipContentTemplate", typeof(DataTemplate), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DragTooltipVisibilityProperty = DependencyProperty.Register("DragTooltipVisibility", typeof(Visibility), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DropImpossibleIconProperty = DependencyProperty.Register("DropImpossibleIcon", typeof(object), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DropImpossibleIconTemplateProperty = DependencyProperty.Register("DropImpossibleIconTemplate", typeof(DataTemplate), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DropPossibleIconProperty = DependencyProperty.Register("DropPossibleIcon", typeof(object), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty DropPossibleIconTemplateProperty = DependencyProperty.Register("DropPossibleIconTemplate", typeof(DataTemplate), typeof(TreeViewDragCue), null);
        public static readonly DependencyProperty IsDropPossibleProperty = DependencyProperty.Register("IsDropPossible", typeof(bool), typeof(TreeViewDragCue), new PropertyMetadata(new PropertyChangedCallback(TreeViewDragCue.OnIsDragPossibleChanged)));

        public TreeViewDragCue()
        {
            base.DefaultStyleKey = typeof(TreeViewDragCue);
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.IsDropPossible)
            {
                VisualStateManager.GoToState(this, "DropPossible", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DropImpossible", true);
            }
        }

        public static object GetNonVisualRepresentation(object item)
        {
            if (item == null)
            {
                return null;
            }
            if (!(item is DependencyObject))
            {
                return item;
            }
            HeaderedItemsControl headeredItemsControl = item as HeaderedItemsControl;
            if (headeredItemsControl != null)
            {
                return GetNonVisualRepresentation(headeredItemsControl.Header);
            }
            HeaderedContentControl headeredContentControl = item as HeaderedContentControl;
            if (headeredContentControl != null)
            {
                return GetNonVisualRepresentation(headeredContentControl.Header);
            }
            ContentControl contentControl = item as ContentControl;
            if (contentControl != null)
            {
                return GetNonVisualRepresentation(contentControl.Content);
            }
            ContentPresenter contentPresenter = item as ContentPresenter;
            if (contentPresenter != null)
            {
                return GetNonVisualRepresentation(contentPresenter.Content);
            }
            FrameworkElement visualItem = item as FrameworkElement;
            if (visualItem == null)
            {
                return item;
            }
            if ((visualItem.DataContext != null) && (visualItem.DataContext.GetType() == typeof(object)))
            {
                return GetNonVisualRepresentation(visualItem.DataContext);
            }
            if (TextSearch.GetText(visualItem) != null)
            {
                return TextSearch.GetText(visualItem);
            }
            return visualItem.ToString();
        }

        private static void OnIsDragPossibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TreeViewDragCue).ChangeVisualState();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        public void SetSafeItemsSource(IEnumerable itemsSource)
        {
            if (itemsSource != null)
            {
                IEnumerable<object> filteredSource = itemsSource.Cast<object>().Select<object, object>(new Func<object, object>(TreeViewDragCue.GetNonVisualRepresentation));
                int sourceCount = filteredSource.Count<object>();
                if ((base.ItemsSource == null) || (sourceCount != base.ItemsSource.Cast<object>().Union<object>(filteredSource).Count<object>()))
                {
                    base.ItemsSource = filteredSource;
                }
            }
        }

        public object DragActionContent
        {
            get
            {
                return base.GetValue(DragActionContentProperty);
            }
            set
            {
                base.SetValue(DragActionContentProperty, value);
            }
        }

        public DataTemplate DragActionContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(DragActionContentTemplateProperty);
            }
            set
            {
                base.SetValue(DragActionContentTemplateProperty, value);
            }
        }

        public Visibility DragPreviewVisibility
        {
            get
            {
                return (Visibility) base.GetValue(DragPreviewVisibilityProperty);
            }
            set
            {
                base.SetValue(DragPreviewVisibilityProperty, value);
            }
        }

        public object DragTooltipContent
        {
            get
            {
                return base.GetValue(DragTooltipContentProperty);
            }
            set
            {
                base.SetValue(DragTooltipContentProperty, value);
            }
        }

        public DataTemplate DragTooltipContentTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(DragTooltipContentTemplateProperty);
            }
            set
            {
                base.SetValue(DragTooltipContentTemplateProperty, value);
            }
        }

        public Visibility DragTooltipVisibility
        {
            get
            {
                return (Visibility) base.GetValue(DragTooltipVisibilityProperty);
            }
            set
            {
                base.SetValue(DragTooltipVisibilityProperty, value);
            }
        }

        public object DropImpossibleIcon
        {
            get
            {
                return base.GetValue(DropImpossibleIconProperty);
            }
            set
            {
                base.SetValue(DropImpossibleIconProperty, value);
            }
        }

        public DataTemplate DropImpossibleIconTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(DropImpossibleIconTemplateProperty);
            }
            set
            {
                base.SetValue(DropImpossibleIconTemplateProperty, value);
            }
        }

        public object DropPossibleIcon
        {
            get
            {
                return base.GetValue(DropPossibleIconProperty);
            }
            set
            {
                base.SetValue(DropPossibleIconProperty, value);
            }
        }

        public DataTemplate DropPossibleIconTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(DropPossibleIconTemplateProperty);
            }
            set
            {
                base.SetValue(DropPossibleIconTemplateProperty, value);
            }
        }

        public bool IsDropPossible
        {
            get
            {
                return (bool) base.GetValue(IsDropPossibleProperty);
            }
            set
            {
                base.SetValue(IsDropPossibleProperty, value);
            }
        }
    }
}

