using System;
using System.Collections;

namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Representa uma lista não ordenada. Mesmo que a tag HTML 'ul'.
    /// </summary>
    public class UListControl : ContainerControl<UListControl, UListItem>
    {
        /// <summary>
        /// Template para os items da lista. Deve ser usado em conjunto com a propriedade <see cref="UListControl.ItemsSourceProperty"/>.
        /// </summary>
        public static readonly ControlProperty ItemTemplateProperty = ControlProperty.Register("ItemTemplate",
            typeof(UListItem),
            typeof(UListControl));

        /// <summary>
        /// Fonte de dados usada para renderizar os items da lista. A fonte de dados deve implementar a interface <see cref="System.Collections.IEnumerable"/>.
        /// Este propriedade deve ser usada em conjunto com a propriedade <see cref="UListControl.ItemTemplateProperty"/>.
        /// </summary>
        public static readonly ControlProperty ItemsSourceProperty = ControlProperty.Register("ItemsSource",
            typeof(Object),
            typeof(UListControl));

        /// <summary>
        /// Cria uma nova instancia de <see cref="UListControl"/>.
        /// </summary>
        public UListControl()
            : base(HtmlTag.Ul)
        {

        }

        /// <summary>
        /// Template para os items da lista.
        /// Atalho para a propriedade <see cref="UListControl.ItemTemplateProperty"/>
        /// </summary>
        public UListItem ItemTemplate
        {
            get { return GetValue<UListItem>(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Fonte de dados usada para renderizar os items da lista.
        /// Atalho para o propriedade <see cref="UListControl.ItemsSourceProperty"/>
        /// </summary>
        public Object ItemsSource
        {
            get { return GetValue<Object>(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent("", rawInnerText);
            if (ItemTemplate != null)
            {
                var template = ItemTemplate;
                var dataSource = ItemsSource as IEnumerable;
                if (dataSource != null)
                {
                    foreach (var item in dataSource)
                    {
                        template.DataContext = item;
                        template.Render(CurrentContext);
                    }
                }
                else
                {
                    ItemTemplate.DataContext = dataSource;
                    ItemTemplate.Render(CurrentContext);
                }
            }
        }

        public UListItem NewItem(params HtmlObject[] innerControls)
        {
            var item = new UListItem();
            Children.Add(item);
            if (innerControls != null)
                item.Add(innerControls);
            return item;
        }
    }
}
