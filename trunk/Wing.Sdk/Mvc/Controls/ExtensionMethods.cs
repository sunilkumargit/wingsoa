using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public static class HtmlObjectExtensionsMethods
    {
        public static T AddClass<T>(this T control, params string[] classes) where T : HtmlObject
        {
            if (classes != null)
            {
                foreach (var cssClass in classes)
                {
                    if (!control.CssClasses.Contains(cssClass))
                        control.CssClasses.Add(cssClass);
                }
            }
            return control;
        }

        public static T RemoveClass<T>(this T control, params string[] classes) where T : HtmlObject
        {
            if (classes != null)
            {
                foreach (var cssClass in classes)
                    control.CssClasses.Remove(cssClass);
            }
            return control;
        }

        public static T SetId<T>(this T control, String id) where T : HtmlObject
        {
            control.Id = id;
            return control;
        }

        public static T SetTag<T>(this T control, HtmlTag tag) where T : HtmlObject
        {
            control.Tag = tag;
            return control;
        }

        public static T SetAttribute<T>(this T control, HtmlAttr attr, String value) where T : HtmlObject
        {
            return control.SetAttribute(attr.ToString().ToLower(), value);
        }

        public static T SetAttribute<T>(this T control, String attr, String value) where T : HtmlObject
        {
            if (value == null)
                control.Attributes.Remove(attr);
            else
                control.Attributes[attr] = value;
            return control;
        }

        public static T ClearAttribute<T>(this T control, HtmlAttr attr) where T : HtmlObject
        {
            return control.SetAttribute(attr, null);
        }

        public static T ClearAttribute<T>(this T control, String attr) where T : HtmlObject
        {
            return control.SetAttribute(attr, null);
        }

        public static T SetDataContext<T>(this T control, Object data) where T : HtmlObject
        {
            control.DataContext = data;
            return control;
        }

        public static T SetRenderEnabled<T>(this T control, bool isEnabled) where T : HtmlObject
        {
            control.RenderEnabled = isEnabled;
            return control;
        }

        public static T OnDataBind<T>(this T control, HtmlObjectDelegate handler) where T : HtmlObject
        {
            control.DataBindEvent += handler;
            return control;
        }

        public static T OnDataBind<T>(this T control, TypedControlDelegate<T> handler) where T : HtmlObject
        {
            control.DataBindEvent += (ctrl) => handler.Invoke((T)ctrl);
            return control;
        }

        public static T OnPreRender<T>(this T control, HtmlObjectDelegate handler) where T : HtmlObject
        {
            control.PreRenderEvent += handler;
            return control;
        }

        public static T OnPreRender<T>(this T control, TypedControlDelegate<T> handler) where T : HtmlObject
        {
            control.PreRenderEvent += (ctrl) => handler.Invoke((T)ctrl);
            return control;
        }

        public static T OnRenderContent<T>(this T control, HtmlObjectDelegate handler) where T : HtmlObject
        {
            control.RenderContentEvent += handler;
            return control;
        }

        public static T OnRenderContent<T>(this T control, TypedControlDelegate<T> handler) where T : HtmlObject
        {
            control.RenderContentEvent += (ctrl) => handler.Invoke((T)ctrl);
            return control;
        }

        public static T OnPostRender<T>(this T control, HtmlObjectDelegate handler) where T : HtmlObject
        {
            control.PostRenderEvent += handler;
            return control;
        }

        public static T OnPostRender<T>(this T control, TypedControlDelegate<T> handler) where T : HtmlObject
        {
            control.PostRenderEvent += (ctrl) => handler.Invoke((T)ctrl);
            return control;
        }

        public static T Set<T>(this T control, TypedControlDelegate<T> handler) where T : HtmlObject
        {
            handler.Invoke(control);
            return control;
        }

        public static T SetBinding<T>(this T control, ControlProperty property, String propertyPath, IValueFormatter formatter = null, String formatString = null) where T : HtmlObject
        {
            control.PropertyBinding(property, new Binding(propertyPath, formatter, formatString));
            return control;
        }

        public static T SetBinding<T>(this T control, ControlProperty property, String propertyPath, String formatString) where T : HtmlObject
        {
            control.PropertyBinding(property, new Binding(propertyPath, null, formatString));
            return control;
        }


        public static T SetBinding<T>(this T control, ControlProperty property, IValueProvider valueProvider, IValueFormatter formatter = null, String formatString = null) where T : HtmlObject
        {
            control.PropertyBinding(property, new Binding(valueProvider, formatter, formatString));
            return control;
        }

        public static T SetBinding<T>(this T control, ControlProperty property, IValueProvider valueProvider, String formatString) where T : HtmlObject
        {
            control.PropertyBinding(property, new Binding(valueProvider, null, formatString));
            return control;
        }

        public static T SetBinding<T>(this T control, ControlProperty property, Func<Object, Object> valueProvider, IValueFormatter formatter, String formatString = null) where T : HtmlObject
        {
            control.PropertyBinding(property, new Binding(new CustomValueProvider((data) =>
            {
                return valueProvider.Invoke(data);
            }), formatter, formatString));
            return control;
        }

        public static T SetBinding<T>(this T control, ControlProperty property, Func<Object, Object> valueProvider, String formatString = null) where T : HtmlObject
        {
            control.PropertyBinding(property, new Binding(new CustomValueProvider((data) =>
            {
                return valueProvider.Invoke(data);
            }), null, formatString));
            return control;
        }

        public static T SetTemplateBinding<T>(this T control, ControlProperty targetProperty, ControlProperty sourceProperty) where T : HtmlObject
        {
            control.TemplateBinding(targetProperty, sourceProperty);
            return control;
        }

        public static T SetText<T>(this T control, String text) where T : HtmlObject
        {
            control.Text = text;
            return control;
        }

        public static T SetProperty<T>(this T control, ControlProperty property, Object value) where T : HtmlObject
        {
            control.SetValue(property, value);
            return control;
        }

        public static T Css<T>(this T control, CssProperty css, String value) where T : HtmlObject
        {
            control.Styles[css] = value;
            return control;
        }

        public static T ClearCss<T>(this T control, CssProperty css) where T : HtmlObject
        {
            var item = control.Styles.FirstOrDefault(s => s.Style == css);
            if (item != null)
                control.Styles.Remove(item);
            return control;
        }

        public static T SetTextBinding<T>(this T control, String propertyPath, IValueFormatter formatter, String formatString = null) where T : HtmlObject
        {
            control.SetBinding(HtmlObject.TextProperty, propertyPath, formatter, formatString);
            return control;
        }

        public static T SetTextBinding<T>(this T control, String propertyPath, String formatString = null) where T : HtmlObject
        {
            control.SetBinding(HtmlObject.TextProperty, propertyPath, formatString);
            return control;
        }

        public static T SetTextBinding<T>(this T control, IValueProvider valueProvider, IValueFormatter formatter = null, String formatString = null) where T : HtmlObject
        {
            control.SetBinding(HtmlObject.TextProperty, valueProvider, formatter, formatString);
            return control;
        }

        public static T SetTextBinding<T>(this T control, IValueProvider valueProvider, String formatString = null) where T : HtmlObject
        {
            control.SetBinding(HtmlObject.TextProperty, valueProvider, null, formatString);
            return control;
        }

        public static T SetTextBinding<T>(this T control, Func<Object, Object> valueProvider, IValueFormatter formatter, String formatString = null) where T : HtmlObject
        {
            control.SetBinding(HtmlObject.TextProperty, valueProvider, formatter, formatString);
            return control;
        }

        public static T SetTextBinding<T>(this T control, Func<Object, Object> valueProvider, String formatString = null) where T : HtmlObject
        {
            control.SetBinding(HtmlObject.TextProperty, valueProvider, null, formatString);
            return control;
        }
    }


    [System.Diagnostics.DebuggerStepThrough]
    public static class HtmlControlExtensionsMethods
    {
        #region HtmlControl

        public static T SetDisplay<T>(this T control, CssDisplay cssDisplay) where T : HtmlControl
        {
            control.Display = cssDisplay;
            return control;
        }

        public static T SetPadding<T>(this T control, Thickness padding) where T : HtmlControl
        {
            control.Padding = padding;
            return control;
        }

        public static T SetMargin<T>(this T control, Thickness margin) where T : HtmlControl
        {
            control.Margin = margin;
            return control;
        }

        public static T SetFontFamily<T>(this T control, String fontFamily) where T : HtmlControl
        {
            control.FontFamily = fontFamily;
            return control;
        }

        public static T SetFontSize<T>(this T control, String fontSize) where T : HtmlControl
        {
            control.FontSize = fontSize;
            return control;
        }

        public static T SetFontWeight<T>(this T control, CssFontWeight fontWeight) where T : HtmlControl
        {
            control.FontWeight = fontWeight;
            return control;
        }

        public static T SetFontStyle<T>(this T control, CssFontStyle fontStyle) where T : HtmlControl
        {
            control.FontStyle = fontStyle;
            return control;
        }

        public static T SetPosition<T>(this T control, CssPosition position) where T : HtmlControl
        {
            control.Position = position;
            return control;
        }

        public static T SetTop<T>(this T control, String value) where T : HtmlControl
        {
            control.Top = value;
            return control;
        }

        public static T SetLeft<T>(this T control, String value) where T : HtmlControl
        {
            control.Left = value;
            return control;
        }

        public static T SetRight<T>(this T control, String value) where T : HtmlControl
        {
            control.Right = value;
            return control;
        }

        public static T SetBottom<T>(this T control, String value) where T : HtmlControl
        {
            control.Bottom = value;
            return control;
        }

        public static T SetWidth<T>(this T control, String value) where T : HtmlControl
        {
            control.Width = value;
            return control;
        }

        public static T SetHeight<T>(this T control, String value) where T : HtmlControl
        {
            control.Height = value;
            return control;
        }

        public static T SetZIndex<T>(this T control, int zIndex) where T : HtmlControl
        {
            control.ZIndex = zIndex;
            return control;
        }

        public static T SetFloat<T>(this T control, CssFloat cssFloat) where T : HtmlControl
        {
            control.Float = cssFloat;
            return control;
        }

        public static T SetFloatClear<T>(this T control, CssClear cssFloatClear) where T : HtmlControl
        {
            control.FloatClear = cssFloatClear;
            return control;
        }

        public static T SetHint<T>(this T control, String hint) where T : HtmlControl
        {
            control.Hint = hint;
            return control;
        }

        public static T SetBackgroundColor<T>(this T control, String color) where T : HtmlControl
        {
            control.BackgroundColor = color;
            return control;
        }

        public static T SetBackgroundImage<T>(this T control, String url) where T : HtmlControl
        {
            control.BackgroundImage = url;
            return control;
        }

        public static T SetForegroundColor<T>(this T control, String color) where T : HtmlControl
        {
            control.ForegroundColor = color;
            return control;
        }

        public static T SetBorderStyle<T>(this T control, CssBorderStyle borderStyle) where T : HtmlControl
        {
            control.BorderStyle = borderStyle;
            return control;
        }

        public static T SetBorderColor<T>(this T control, String color) where T : HtmlControl
        {
            control.BorderColor = color;
            return control;
        }

        public static T SetBorderWidth<T>(this T control, string width) where T : HtmlControl
        {
            control.BorderWidth = width;
            return control;
        }

        public static T SetPaddingLeft<T>(this T control, int? value) where T : HtmlControl
        {
            control.PaddingLeft = value;
            return control;
        }

        public static T SetPaddingRight<T>(this T control, int? value) where T : HtmlControl
        {
            control.PaddingRight = value;
            return control;
        }

        public static T SetPaddingTop<T>(this T control, int? value) where T : HtmlControl
        {
            control.PaddingTop = value;
            return control;
        }

        public static T SetPaddingBottom<T>(this T control, int? value) where T : HtmlControl
        {
            control.PaddingLeft = value;
            return control;
        }

        public static T SetMarginLeft<T>(this T control, int? value) where T : HtmlControl
        {
            control.MarginLeft = value;
            return control;
        }

        public static T SetMarginRight<T>(this T control, int? value) where T : HtmlControl
        {
            control.MarginRight = value;
            return control;
        }

        public static T SetMarginTop<T>(this T control, int? value) where T : HtmlControl
        {
            control.MarginTop = value;
            return control;
        }

        public static T SetMarginBottom<T>(this T control, int? value) where T : HtmlControl
        {
            control.MarginBottom = value;
            return control;
        }

        public static T SetMarginUnit<T>(this T control, CssUnit unit) where T : HtmlControl
        {
            control.MarginUnit = unit;
            return control;
        }

        public static T SetPaddingUnit<T>(this T control, CssUnit unit) where T : HtmlControl
        {
            control.PaddingUnit = unit;
            return control;
        }

        public static T SetTextAlign<T>(this T control, TextAlignment align) where T : HtmlControl
        {
            control.TextAlign = align;
            return control;
        }

        #endregion
    }

    public static class FormControlExtensionMethods
    {
        #region FormControl

        public static T SetAction<T>(this T control, string action) where T : FormControl
        {
            control.Action = action;
            return control;
        }

        public static T SetMethod<T>(this T control, HtmlFormMethod method) where T : FormControl
        {
            control.Method = method;
            return control;
        }
        #endregion
    }

    public static class ContainerControlExtensionMethods
    {
        #region ContainerControl

        public static HtmlControl Find(this IChildrenCollectionProvider container, Predicate<HtmlObject> matchCriteria)
        {
            return container.FindAll(matchCriteria).FirstOrDefault();
        }

        public static IEnumerable<HtmlControl> FindAll(this IChildrenCollectionProvider container, Predicate<HtmlObject> matchCriteria)
        {
            if (!container.HasChildren)
                yield return null;
            var queue = new Queue<IChildrenCollectionProvider>();
            queue.Enqueue(container);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                var itemControl = item as HtmlControl;
                if (itemControl != null && matchCriteria.Invoke(itemControl))
                    yield return itemControl;
                foreach (var childrenCollection in item.GetChildrenCollections())
                {
                    foreach (var child in childrenCollection.OfType<HtmlControl>())
                    {
                        if (matchCriteria.Invoke(child))
                            yield return child;
                        if (child is IChildrenCollectionProvider)
                            queue.Enqueue((IChildrenCollectionProvider)child);
                    }
                }
            }
        }

        public static IEnumerable<TControl> FindAll<TControl>(this IChildrenCollectionProvider container) where TControl : HtmlObject
        {
            return container.FindAll((control) => control is TControl).Cast<TControl>();
        }

        public static TControl Find<TControl>(this IChildrenCollectionProvider container, String id) where TControl : HtmlObject
        {
            return (TControl)container.Find(typeof(TControl), id);
        }

        public static TControl Find<TControl>(this IChildrenCollectionProvider container) where TControl : HtmlObject
        {
            return (TControl)container.Find(typeof(TControl));
        }

        public static HtmlObject Find(this IChildrenCollectionProvider container, String id)
        {
            if (String.IsNullOrEmpty(id))
                return null;
            return container.Find((c) => id.Equals(c.Id, StringComparison.OrdinalIgnoreCase));
        }

        public static HtmlObject Find(this IChildrenCollectionProvider container, Type type)
        {
            Assert.NullArgument(type, "type");
            return container.Find((c) => type.IsAssignableFrom(c.GetType()));
        }

        public static HtmlObject Find(this IChildrenCollectionProvider container, Type type, String id)
        {
            Assert.NullArgument(type, "type");
            if (String.IsNullOrEmpty(id))
                return null;
            return container.Find((c) => type.IsAssignableFrom(c.GetType())
                && id.Equals(c.Id, StringComparison.OrdinalIgnoreCase));
        }

        public static T SetContentVerticalAligment<T>(this T control, VerticalAlignment alignment) where T : ContainerControlBase
        {
            control.ContentVerticalAlign = alignment;
            return control;
        }

        /// <summary>
        /// Adiciona um controle em um container sem que esse controle faça parte da coleção de filhos do container.
        /// A esse aspecto dizemos que o controle é um convidado do container, mas não faz parte dele.
        /// </summary>
        /// <typeparam name="T">Container concreto</typeparam>
        /// <param name="control">Container</param>
        /// <param name="guestControl">Controle convidado</param>
        /// <param name="beforeChildren">Adicionar antes ou depois dos filhos. Se 'true', indica que o controle deve ser renderizado antes dos 
        /// filhos do container, se 'false' então o controle será renderizado após a renderização dos filhos</param>
        /// <returns>O container</returns>
        /// <example>
        /// O exemplo abaixo cria um novo controle chamado HeaderedBox, onde é possível incluir outros controles.
        /// Como o nome diz, o box contém uma area onde o usuário pode incluir um cabeçalho. Para que isso seja possivel
        /// vamos adicionar a area do cabeçalho como um guest do container. Dessa forma o cabeçalho fica separado dos filhos
        /// que serão incluídos pelo usuário.
        /// <code>
        /// public class HeaderedBox : PanelControl
        /// {
        ///     private PanelControl _header;
        /// 
        ///     public HeaderedBox()
        ///     {
        ///         //criar o cabeçalho do box
        ///         _header = new PanelControl().AddClass("box-header");
        ///         // adicionar o  header como guest
        ///         this.AddGuestControl(guestControl: header, beforeChildren: true);
        ///     }
        ///     
        ///     //expor o header
        ///     public PanelControl Header
        ///     {
        ///         get { return _header; }
        ///     }
        /// }
        /// </code>
        /// <para>
        /// No exemplo abaixo, criamos uma classe HeaderedBox com o cabeçalho customizavel pelo usuário.
        /// Para isso criamos um propriedade do controle e nos inscrevemos no seu evento PropertyChanged.
        /// Caso o valor da propriedade seja alterado, nós removemos o antigo controle da lista de convidados
        /// e adicionamos o novo controle.
        /// </para>
        /// <code>
        /// public class HeaderedBox : PanelControl
        /// {
        ///     // registrar a propriedade no controle.
        ///     public static readonly ControlProperty HeaderTemplateProperty = ControlProperty.Register("HeaderTemplate",
        ///          typeof(HtmlObject),
        ///          typeof(HeaderedBox),
        ///          null,
        ///          HeaderTemplatePropertyChanged);
        /// 
        ///     // remover o controle antigo como convidado e adicionar o novo.
        ///     private static void HeaderTemplatePropertyChanged(ControlPropertyChangedEventArgs args)
        ///     {
        ///        var targetContainer = args.Target as HeaderedBox;
        ///        if (targetContainer != null)
        ///         {
        ///             if (args.OldValue != null)
        ///                 targetContainer.RemoveGuestControl((HtmlObject)args.OldValue);
        ///             if (args.NewValue != null)
        ///                 targetContainer.AddGuestControl((HtmlObject)args.NewValue, true);
        ///         }
        ///     }
        ///     
        ///     //atalho para a propriedade HeaderTemplateProperty
        ///     public HtmlObject HeaderTemplate 
        ///     {
        ///          get { return (HtmlObject)GetValue(HeaderTemplateProperty); }
        ///          set { SetValue(HeaderTemplateProperty, value); }
        ///     }
        ///  }
        /// </code>
        /// </example>
        public static T AddGuestControl<T>(this T control, HtmlObject guestControl, bool beforeChildren) where T : ContainerControlBase
        {
            control._AddGuestControl(guestControl, beforeChildren);
            return control;
        }

        /// <summary>
        /// Remove um controle convidado de um container.
        /// </summary>
        /// <typeparam name="T">Container concreto</typeparam>
        /// <param name="control">Container</param>
        /// <param name="guestControl">Controle convidado</param>
        /// <returns>O container</returns>
        public static T RemoveGuestControl<T>(this T control, HtmlObject guestControl) where T : ContainerControlBase
        {
            control._RemoveGuestControl(guestControl);
            return control;
        }
        #endregion
    }

    public static class DateTableControlExtensionsMethods
    {

        #region DataTableControl

        public static T SetEmptySourceTemplate<T>(this T table, TableCell template) where T : DataTableControl
        {
            table.EmptySourceTemplate = template;
            return table;
        }

        public static T SetEmptySourceTemplate<T>(this T table, HtmlControl innerControl) where T : DataTableControl
        {
            table.SetEmptySourceTemplate(new TableCell().Add(innerControl));
            return table;
        }

        public static DataTableColumn NewColumn<T>(this T table, TableCell itemTemplate = null, TableHeaderCell headerTemplate = null, TableHeaderCell footerTemplate = null)
            where T : DataTableControl
        {
            var column = new DataTableColumn(table)
            {
                ItemTemplate = itemTemplate,
                HeaderTemplate = headerTemplate,
                FooterTemplate = footerTemplate
            };
            table.Columns.Add(column);
            return column;
        }

        public static DataTableColumn NewColumn<T>(this T table, String caption, TableCell itemTemplate) where T : DataTableControl
        {
            var column = table.NewColumn(itemTemplate);
            column.Caption = caption;
            return column;
        }

        public static DataTableColumn NewColumn<T>(this T table, String caption, HtmlControl innerControlItemTemplate) where T : DataTableControl
        {
            var column = table.NewColumn(caption, new TableCell().Add(innerControlItemTemplate));
            return column;
        }

        public static T AddColumn<T>(this T table, Action<DataTableColumn> columnSetDelegate) where T : DataTableControl
        {
            var column = table.NewColumn();
            columnSetDelegate.Invoke(column);
            return table;
        }

        public static T AddColumn<T>(this T table, String caption, TableCell itemTemplate, TableHeaderCell headerTemplate = null, TableHeaderCell footerTemplate = null) where T : DataTableControl
        {
            var column = table.NewColumn(itemTemplate, headerTemplate, footerTemplate);
            column.Caption = caption;
            return table;
        }

        public static T AddColumn<T>(this T table, String caption, HtmlControl innerControlItemTemplate) where T : DataTableControl
        {
            var column = table.NewColumn(caption, new TableCell().Add(innerControlItemTemplate));
            return table;
        }

        #endregion
    }

    public static class InputControlBaseExtensionMethods
    {

        #region InputControlBase

        public static T SetName<T>(this T control, String name) where T : IFormField
        {
            control.Name = name;
            return control;
        }

        public static T SetIsDisabled<T>(this T control, bool isDisabled) where T : IFormField
        {
            control.IsDisabled = isDisabled;
            return control;
        }

        #endregion
    }

    public static class TextBoxControlExtensionMethods
    {
        #region TextBoxControl

        public static T SetIsHidden<T>(this T control, bool isHidden) where T : TextBoxControl
        {
            control.IsHidden = isHidden;
            return control;
        }

        public static T SetIsPassword<T>(this T control, bool isPassword) where T : TextBoxControl
        {
            control.IsPassword = isPassword;
            return control;
        }

        public static T SetIsReadOnly<T>(this T control, bool isReadonly) where T : TextBoxControl
        {
            control.IsReadOnly = isReadonly;
            return control;
        }

        /// <summary>
        /// Atribui o valor tamanho do campo na tela do browser.
        /// </summary>
        /// <typeparam name="T">Um controle TextBoxControl</typeparam>
        /// <param name="control">Controle</param>
        /// <param name="size">Novo tamanho</param>
        /// <returns></returns>
        public static T SetSize<T>(this T control, int size) where T : TextBoxControl
        {
            control.Size = size;
            return control;
        }

        /// <summary>
        /// Atribui o maximo de caracteres aceitos no campo.
        /// </summary>
        /// <typeparam name="T">Um controle TextBoxControl</typeparam>
        /// <param name="control">Controle</param>
        /// <param name="maxLength">Novo tamanho</param>
        /// <returns></returns>
        public static T SetMaxLength<T>(this T control, int maxLength) where T : TextBoxControl
        {
            control.MaxLength = maxLength;
            return control;
        }

        public static T SetMask<T>(this T control, UserMask mask) where T : TextBoxControl
        {
            control.UserMask = mask;
            return control;
        }

        public static T SetMask<T>(this T control, String customMask) where T : TextBoxControl
        {
            control.CustomMask = customMask;
            return control;
        }


        #endregion
    }

    public static class SelectListControlExtensionMethods
    {
        #region SelectListControl

        public static T SetListSize<T>(this T control, int size) where T : SelectListControl
        {
            control.Size = size;
            return control;
        }

        public static T SetGroupItemTemplate<T>(this T control, SelectOptionGroup template) where T : SelectListControl
        {
            control.GroupItemTemplate = template;
            return control;
        }
        #endregion
    }

    public static class LinkExtensionMethods
    {
        #region Link
        public static T SetGetParams<T>(this T control, Dictionary<String, Object> getParams) where T : Link
        {
            control.GetParams = getParams;
            return control;
        }

        public static T SetGetParams<T>(this T control, Object getParams) where T : Link
        {
            control.GetParams = ReflectionHelper.AnonymousToDictionary(getParams);
            return control;
        }

        public static T AddGetParam<T>(this T control, String name, Object value) where T : Link
        {
            control.GetParams[name] = value;
            return control;
        }
        #endregion
    }

    public static class RenderizationExtensionMethods
    {
        #region Render
        public static void Render(this HtmlObject control, ControllerBase controller, ViewDataDictionary viewData, bool writeLog = true)
        {
            RenderOutputWriter renderer = new RenderOutputWriter(controller, viewData, false);
            renderer.Render(control);
            renderer.WriteBodyContent();
            renderer.WriteScripts();
        }

        public static void Render(this HtmlHelper htmlHelper, HtmlObject control)
        {
            control.Render(htmlHelper.ViewContext.Controller, htmlHelper.ViewData, false);
        }

        public static void Render<TModel>(this HtmlHelper<TModel> htmlHelper, HtmlObject control) where TModel : class
        {
            control.Render(htmlHelper.ViewContext.Controller, htmlHelper.ViewData, false);
        }
        #endregion
    }

    public static class TextAreaControlExtensionMethods
    {
        public static T SetRows<T>(this T control, int rows) where T : TextAreaControl
        {
            control.Rows = rows;
            return control;
        }

        public static T SetColumns<T>(this T control, int columns) where T : TextAreaControl
        {
            control.Columns = columns;
            return control;
        }

    }

    public static class ListItemBaseExtensionMethods
    {
        public static T SetLabel<T>(this T control, string label) where T : ListItemBase
        {
            control.Label = label;
            return control;
        }

        public static T SetValue<T>(this T control, string value) where T : ListItemBase
        {
            control.Value = value;
            return control;
        }

        public static T SetIsSelected<T>(this T control, bool isSelected) where T : ListItemBase
        {
            control.IsSelected = isSelected;
            return control;
        }

    }

    public static class SelectOptionExtensionMethods
    {
        public static T SetGroup<T>(this T control, string group) where T : SelectOption
        {
            control.Group = group;
            return control;
        }
    }

    public static class DataListExtensionMethods
    {
        public static T SetColumns<T>(this T control, int columns) where T : DataListControl
        {
            control.Columns = columns;
            return control;
        }

        public static T SetItemTemplate<T>(this T control, HtmlControl itemTemplate) where T : DataListControl
        {
            control.ItemTemplate = itemTemplate;
            return control;
        }

        public static T SetItemsSource<T>(this T control, Object source) where T : DataListControl
        {
            control.ItemsSource = source;
            return control;
        }
    }

    public static class LabelControlExtensionMethods
    {
        public static T SetFor<T>(this T control, string forName) where T : LabelControl
        {
            control.For = forName;
            return control;
        }
    }

    public static class ImageControlExtensionMethods
    {
        public static T SetSrc<T>(this T control, string src) where T : ImageControl
        {
            control.Src = src;
            return control;
        }

        public static T SetAlt<T>(this T control, string alt) where T : ImageControl
        {
            control.Alt = alt;
            return control;
        }

    }

    public static class LinkExtensions
    {
        public static T SetHref<T>(this T control, String href) where T : Link
        {
            control.HRef = href;
            return control;
        }
    }

}
