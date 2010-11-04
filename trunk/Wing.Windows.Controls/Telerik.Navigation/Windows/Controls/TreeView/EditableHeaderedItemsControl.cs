namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Telerik.Windows;
    using Telerik.Windows.Controls;

    [TemplateVisualState(Name="Display", GroupName="EditStates"), TemplateVisualState(Name="Edit", GroupName="EditStates"), TemplatePart(Name="EditHeaderElement", Type=typeof(ContentPresenter))]
    public abstract class EditableHeaderedItemsControl : HeaderedItemsControl
    {
        private bool beginEditReentrancyCheck;
        public static readonly Telerik.Windows.RoutedEvent EditCanceledEvent = EventManager.RegisterRoutedEvent("EditCanceled", RoutingStrategy.Bubble, typeof(RadTreeViewItemEditedEventHandler), typeof(EditableHeaderedItemsControl));
        public static readonly Telerik.Windows.RoutedEvent EditedEvent = EventManager.RegisterRoutedEvent("Edited", RoutingStrategy.Bubble, typeof(RadTreeViewItemEditedEventHandler), typeof(EditableHeaderedItemsControl));
        public static readonly Telerik.Windows.RoutedEvent EditorPrepareEvent = EventManager.RegisterRoutedEvent("EditorPrepare", RoutingStrategy.Bubble, typeof(EditorPrepareEventHandler), typeof(EditableHeaderedItemsControl));
        public static readonly Telerik.Windows.RoutedEvent EditStartedEvent = EventManager.RegisterRoutedEvent("EditStarted", RoutingStrategy.Bubble, typeof(RadTreeViewItemEditedEventHandler), typeof(EditableHeaderedItemsControl));
        private ContentPresenter headerEditElement;
        public static readonly DependencyProperty HeaderEditTemplateProperty = DependencyProperty.Register("HeaderEditTemplate", typeof(DataTemplate), typeof(EditableHeaderedItemsControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(EditableHeaderedItemsControl.OnHeaderEditTemplateChanged)));
        public static readonly DependencyProperty HeaderEditTemplateSelectorProperty = DependencyProperty.Register("HeaderEditTemplateSelector", typeof(DataTemplateSelector), typeof(EditableHeaderedItemsControl), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(EditableHeaderedItemsControl.OnHeaderEditTemplateSelectorChanged)));
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableHeaderedItemsControl), new Telerik.Windows.PropertyMetadata(true));
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableHeaderedItemsControl), new Telerik.Windows.PropertyMetadata(false, new PropertyChangedCallback(EditableHeaderedItemsControl.OnIsInEditModeChanged), null));
        private bool isInEditModeReentrancyCheck;
        private object oldEditValue;
        private Dictionary<TextBox, string> originaEditValuesStorage = new Dictionary<TextBox, string>(4);
        public static readonly Telerik.Windows.RoutedEvent PreviewEditCanceledEvent = EventManager.RegisterRoutedEvent("PreviewEditCanceled", RoutingStrategy.Tunnel, typeof(RadTreeViewItemEditedEventHandler), typeof(EditableHeaderedItemsControl));
        public static readonly Telerik.Windows.RoutedEvent PreviewEditedEvent = EventManager.RegisterRoutedEvent("PreviewEdited", RoutingStrategy.Tunnel, typeof(RadTreeViewItemEditedEventHandler), typeof(EditableHeaderedItemsControl));
        public static readonly Telerik.Windows.RoutedEvent PreviewEditorPrepareEvent = EventManager.RegisterRoutedEvent("PreviewEditorPrepare", RoutingStrategy.Tunnel, typeof(EditorPrepareEventHandler), typeof(EditableHeaderedItemsControl));
        public static readonly Telerik.Windows.RoutedEvent PreviewEditStartedEvent = EventManager.RegisterRoutedEvent("PreviewEditStarted", RoutingStrategy.Tunnel, typeof(RadTreeViewItemEditedEventHandler), typeof(EditableHeaderedItemsControl));

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler EditCanceled
        {
            add
            {
                this.AddHandler(EditCanceledEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditCanceledEvent, value);
            }
        }

        [Category("Behavior"), Description("Occurs when a item has been edited."), ScriptableMember]
        public event RadTreeViewItemEditedEventHandler Edited
        {
            add
            {
                this.AddHandler(EditedEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditedEvent, value);
            }
        }

        [Category("Behavior"), ScriptableMember]
        public event EditorPrepareEventHandler EditorPrepare
        {
            add
            {
                AddEditorPrepareHandler(this, value);
            }
            remove
            {
                RemoveEditorPrepareHandler(this, value);
            }
        }

        [Category("Behavior"), ScriptableMember]
        public event RadTreeViewItemEditedEventHandler EditStarted
        {
            add
            {
                this.AddHandler(EditStartedEvent, value);
            }
            remove
            {
                this.RemoveHandler(EditStartedEvent, value);
            }
        }

        [ScriptableMember]
        public event RadTreeViewItemEditedEventHandler PreviewEditCanceled
        {
            add
            {
                this.AddHandler(PreviewEditCanceledEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewEditCanceledEvent, value);
            }
        }

        [ScriptableMember, Category("Behavior"), Description("Occurs before the item to accept the new edit text.")]
        public event RadTreeViewItemEditedEventHandler PreviewEdited
        {
            add
            {
                this.AddHandler(PreviewEditedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewEditedEvent, value);
            }
        }

        [Category("Behavior"), ScriptableMember]
        public event EditorPrepareEventHandler PreviewEditorPrepare
        {
            add
            {
                AddPreviewEditorPrepareHandler(this, value);
            }
            remove
            {
                RemovePreviewEditorPrepareHandler(this, value);
            }
        }

        [ScriptableMember, Category("Behavior")]
        public event RadTreeViewItemEditedEventHandler PreviewEditStarted
        {
            add
            {
                this.AddHandler(PreviewEditStartedEvent, value);
            }
            remove
            {
                this.RemoveHandler(PreviewEditStartedEvent, value);
            }
        }

        protected EditableHeaderedItemsControl()
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddEditCanceledHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.AddHandler(EditCanceledEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddEditedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.AddHandler(EditedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddEditorPrepareHandler(UIElement target, EditorPrepareEventHandler handler)
        {
            target.AddHandler(EditorPrepareEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddEditStartedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.AddHandler(EditStartedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddPreviewEditedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.AddHandler(PreviewEditedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddPreviewEditorPrepareHandler(UIElement target, EditorPrepareEventHandler handler)
        {
            target.AddHandler(PreviewEditorPrepareEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void AddPreviewEditStartedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.AddHandler(PreviewEditStartedEvent, handler);
        }

        public virtual bool BeginEdit()
        {
            if (!this.IsEditable || this.beginEditReentrancyCheck)
            {
                return false;
            }
            this.beginEditReentrancyCheck = true;
            try
            {
                if (((this.HeaderEditTemplate == null) && (this.HeaderEditTemplateSelector == null)) || (this.headerEditElement == null))
                {
                    return false;
                }
                this.UpdateEditContentTemplates();
                this.oldEditValue = this.GetEditValue();
                RadTreeViewItemEditedEventArgs previewBegin = new RadTreeViewItemEditedEventArgs(null, this.oldEditValue, PreviewEditStartedEvent, this);
                if (this.OnPreviewEditStarted(previewBegin))
                {
                    this.oldEditValue = null;
                    return false;
                }
                this.oldEditValue = previewBegin.OldValue;
                this.isInEditModeReentrancyCheck = true;
                try
                {
                    this.IsInEditMode = true;
                }
                finally
                {
                    this.isInEditModeReentrancyCheck = false;
                }
                this.ChangeVisualState();
                RadTreeViewItemEditedEventArgs begin = new RadTreeViewItemEditedEventArgs(null, this.oldEditValue, EditStartedEvent, this);
                this.OnEditStarted(begin);
            }
            finally
            {
                this.beginEditReentrancyCheck = false;
            }
            return true;
        }

        public virtual bool CancelEdit()
        {
            RadTreeViewItemEditedEventArgs cancel;
            if (this.IsInEditMode || this.isInEditModeReentrancyCheck)
            {
                object newEditValue = this.GetEditValue();
                RadTreeViewItemEditedEventArgs previewCancel = new RadTreeViewItemEditedEventArgs(newEditValue, this.oldEditValue, PreviewEditCanceledEvent, this);
                if (this.OnPreviewEditCanceled(previewCancel))
                {
                    return false;
                }
                this.IsInEditMode = false;
                this.RestoreOriginalTextBoxTexts();
                this.UpdateTextBoxesInBeforeEditEnd();
                this.ChangeVisualState();
                this.SetEditValue(this.oldEditValue);
                cancel = new RadTreeViewItemEditedEventArgs(newEditValue, this.oldEditValue, EditCanceledEvent, this);
                base.Dispatcher.BeginInvoke(delegate {
                    this.OnEditCanceled(cancel);
                });
            }
            return true;
        }

        protected internal override void ChangeVisualState(bool useTransitions)
        {
            base.ChangeVisualState(useTransitions);
            if (this.IsInEditMode)
            {
                this.GoToState(useTransitions, new string[] { "Edit" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Display" });
            }
        }

        public virtual bool CommitEdit()
        {
            if (this.IsInEditMode || this.isInEditModeReentrancyCheck)
            {
                this.UpdateTextBoxesInBeforeEditEnd();
                object newEditValue = this.GetEditValue();
                if (this.HasInvalidEditElements())
                {
                    return false;
                }
                RadTreeViewItemEditedEventArgs previewCommit = new RadTreeViewItemEditedEventArgs(newEditValue, this.oldEditValue, PreviewEditedEvent, this);
                if (this.OnPreviewEdited(previewCommit))
                {
                    return false;
                }
                newEditValue = previewCommit.NewValue;
                this.IsInEditMode = false;
                this.ChangeVisualState();
                this.SetEditValue(newEditValue);
                RadTreeViewItemEditedEventArgs commit = new RadTreeViewItemEditedEventArgs(newEditValue, this.oldEditValue, EditedEvent, this);
                this.OnEdited(commit);
            }
            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="This may be an expensive operation, better leave it as a method")]
        protected virtual object GetEditValue()
        {
            if (this.IsInEditMode)
            {
                TextBox focusableTextBox = this.headerEditElement.ChildrenOfType<Control>().FirstOrDefault<Control>() as TextBox;
                if ((focusableTextBox != null) && (focusableTextBox.GetBindingExpression(TextBox.TextProperty) == null))
                {
                    return focusableTextBox.Text;
                }
                return base.Header;
            }
            if (!(base.Header is UIElement))
            {
                return base.Header;
            }
            Telerik.Windows.Controls.ItemsControl owner = Telerik.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this);
            if (owner != null)
            {
                return TextSearch.GetPrimaryTextFromItem(owner, this);
            }
            return null;
        }

        private bool HasInvalidEditElements()
        {
            if (this.headerEditElement == null)
            {
                return false;
            }
            return ((from c in this.headerEditElement.ChildrenOfType<Control>()
                where Validation.GetHasError(c)
                select c).FirstOrDefault<Control>() != null);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.headerEditElement = base.GetTemplateChild("EditHeaderElement") as ContentPresenter;
        }

        protected virtual bool OnEditCanceled(RadTreeViewItemEditedEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        protected virtual void OnEdited(RadTreeViewItemEditedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnEditorPrepare(EditorPrepareEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnEditStarted(RadTreeViewItemEditedEventArgs e)
        {
            if (this.headerEditElement != null)
            {
                this.headerEditElement.Content = e.OldValue;
            }
            base.Dispatcher.BeginInvoke(delegate {
                this.PrepareEditor(e);
            });
            this.RaiseEvent(e);
        }

        protected virtual void OnHeaderEditTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
        }

        private static void OnHeaderEditTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EditableHeaderedItemsControl).OnHeaderEditTemplateChanged(e.OldValue as DataTemplate, e.NewValue as DataTemplate);
        }

        private static void OnHeaderEditTemplateSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EditableHeaderedItemsControl).OnHeaderEditTemplateSelectorChanged(e.OldValue as DataTemplateSelector, e.NewValue as DataTemplateSelector);
        }

        protected virtual void OnHeaderEditTemplateSelectorChanged(DataTemplateSelector oldTemplateSelector, DataTemplateSelector newTemplateSelector)
        {
        }

        protected virtual void OnIsInEditModeChanged(bool oldValue, bool newValue)
        {
        }

        private static void OnIsInEditModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EditableHeaderedItemsControl editableHeaderedItemsControl = sender as EditableHeaderedItemsControl;
            bool newValue = Convert.ToBoolean(e.NewValue, CultureInfo.InvariantCulture);
            editableHeaderedItemsControl.isInEditModeReentrancyCheck = true;
            try
            {
                editableHeaderedItemsControl.OnIsInEditModeChanged(!newValue, newValue);
            }
            finally
            {
                editableHeaderedItemsControl.isInEditModeReentrancyCheck = false;
            }
            KeyboardNavigationMode navMode = newValue ? KeyboardNavigationMode.Local : KeyboardNavigationMode.Once;
            editableHeaderedItemsControl.TabNavigation = navMode;
        }

        protected virtual bool OnPreviewEditCanceled(RadTreeViewItemEditedEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        protected virtual bool OnPreviewEdited(RadTreeViewItemEditedEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        protected virtual void OnPreviewEditorPrepare(EditorPrepareEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual bool OnPreviewEditStarted(RadTreeViewItemEditedEventArgs e)
        {
            this.RaiseEvent(e);
            return e.Handled;
        }

        protected virtual void PrepareEditor(RadTreeViewItemEditedEventArgs e)
        {
            if (((this.headerEditElement != null) && this.IsInEditMode) && (VisualTreeHelper.GetChildrenCount(this.headerEditElement) > 0))
            {
                FrameworkElement editor = VisualTreeHelper.GetChild(this.headerEditElement, 0) as FrameworkElement;
                EditorPrepareEventArgs previewPrepare = new EditorPrepareEventArgs(PreviewEditorPrepareEvent, this, editor);
                this.OnPreviewEditorPrepare(previewPrepare);
                if (!previewPrepare.Handled)
                {
                    Control focusable = (from c in this.headerEditElement.ChildrenOfType<Control>()
                        where c.IsEnabled
                        select c).FirstOrDefault<Control>();
                    if (focusable != null)
                    {
                        TextBox editBox = focusable as TextBox;
                        if (editBox != null)
                        {
                            if (editBox.GetBindingExpression(TextBox.TextProperty) == null)
                            {
                                editBox.Text = this.ToString();
                            }
                            editBox.SelectAll();
                        }
                        focusable.Focus();
                    }
                    this.SaveTextBoxExitorTexts();
                    EditorPrepareEventArgs prepare = new EditorPrepareEventArgs(EditorPrepareEvent, this, editor);
                    this.OnEditorPrepare(prepare);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemoveEditCanceledHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.RemoveHandler(EditCanceledEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemoveEditedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.RemoveHandler(EditedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemoveEditorPrepareHandler(UIElement target, EditorPrepareEventHandler handler)
        {
            target.RemoveHandler(EditorPrepareEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemoveEditStartedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.RemoveHandler(EditStartedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemovePreviewEditedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.RemoveHandler(PreviewEditedEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemovePreviewEditorPrepareHandler(UIElement target, EditorPrepareEventHandler handler)
        {
            target.RemoveHandler(PreviewEditorPrepareEvent, handler);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void RemovePreviewEditStartedHandler(UIElement target, RadTreeViewItemEditedEventHandler handler)
        {
            target.RemoveHandler(PreviewEditStartedEvent, handler);
        }

        private void RestoreOriginalTextBoxTexts()
        {
            if (this.headerEditElement != null)
            {
                foreach (TextBox box in this.headerEditElement.ChildrenOfType<TextBox>())
                {
                    if (this.originaEditValuesStorage.ContainsKey(box))
                    {
                        box.Text = this.originaEditValuesStorage[box];
                    }
                }
            }
            this.originaEditValuesStorage.Clear();
        }

        private void SaveTextBoxExitorTexts()
        {
            if (this.headerEditElement != null)
            {
                foreach (TextBox box in this.headerEditElement.ChildrenOfType<TextBox>())
                {
                    this.originaEditValuesStorage[box] = box.Text;
                }
            }
        }

        protected virtual void SetEditValue(object editValue)
        {
            base.Header = editValue;
        }

        private void UpdateEditContentTemplates()
        {
            if (this.headerEditElement != null)
            {
                DataTemplate newContentTemplate = this.HeaderEditTemplate;
                if ((newContentTemplate == null) && (this.HeaderEditTemplateSelector != null))
                {
                    newContentTemplate = this.HeaderEditTemplateSelector.SelectTemplate(base.Header, this);
                    this.HeaderEditTemplate = newContentTemplate;
                }
                this.headerEditElement.ContentTemplate = newContentTemplate;
            }
        }

        private void UpdateTextBoxesInBeforeEditEnd()
        {
            if (this.headerEditElement != null)
            {
                foreach (TextBox textBox in this.headerEditElement.ChildrenOfType<TextBox>())
                {
                    BindingExpression bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                    }
                }
            }
        }

        protected ContentPresenter HeaderEditPresenterElement
        {
            get
            {
                return this.headerEditElement;
            }
        }

        public DataTemplate HeaderEditTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(HeaderEditTemplateProperty);
            }
            set
            {
                base.SetValue(HeaderEditTemplateProperty, value);
            }
        }

        public DataTemplateSelector HeaderEditTemplateSelector
        {
            get
            {
                return (DataTemplateSelector) base.GetValue(HeaderEditTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(HeaderEditTemplateSelectorProperty, value);
            }
        }

        public bool IsEditable
        {
            get
            {
                return (bool) base.GetValue(IsEditableProperty);
            }
            set
            {
                base.SetValue(IsEditableProperty, value);
            }
        }

        public bool IsInEditMode
        {
            get
            {
                return (bool) base.GetValue(IsInEditModeProperty);
            }
            set
            {
                base.SetValue(IsInEditModeProperty, value);
            }
        }
    }
}

