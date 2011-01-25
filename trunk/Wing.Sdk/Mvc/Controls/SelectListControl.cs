using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class SelectListControl : ListControlBase<SelectListControl, SelectOption>, IFormField
    {
        public static readonly ControlProperty SizeProperty = ControlProperty.Register("Size",
            typeof(int),
            typeof(SelectListControl), 1);

        public static readonly ControlProperty GroupItemTemplateProperty = ControlProperty.Register("GroupItemTemplateProperty",
            typeof(SelectOptionGroup),
            typeof(SelectListControl));

        public static readonly ControlProperty NameProperty = ControlProperty.Register("Name",
                   typeof(String),
                   typeof(SelectListControl),
                   new HtmlAttributePropertyApplier(HtmlAttr.Name));

        public static readonly ControlProperty IsDisabledProperty = ControlProperty.Register("IsDisabled",
            typeof(bool),
            typeof(SelectListControl),
            new HtmlBooleanAttributePropertyApplier(HtmlAttr.Disabled, false, "disabled", ""), false);

        public SelectListControl() : base(HtmlTag.Select) { }

        public int Size
        {
            get { return GetValue<int>(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public SelectOptionGroup GroupItemTemplate
        {
            get { return GetValue<SelectOptionGroup>(GroupItemTemplateProperty); }
            set { SetValue(GroupItemTemplateProperty, value); }
        }

        public String Name
        {
            get { return GetValue<String>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }


        public bool IsDisabled
        {
            get { return GetValue<bool>(IsDisabledProperty); }
            set { SetValue(IsDisabledProperty, value); }
        }

        protected override void RenderItemTemplateItems(IEnumerable source)
        {
            if (GroupItemTemplate != null)
            {
                var cachedResult = new List<KeyValuePair<String, RenderContext>>();

                foreach (var sourceItem in source)
                {
                    var cachedOutput = CurrentContext.CreateChildContext();
                    ItemTemplate.DataContext = sourceItem;
                    ItemTemplate.Render(cachedOutput);
                    cachedResult.Add(new KeyValuePair<string, RenderContext>(ItemTemplate.Group ?? "", cachedOutput));
                }

                var groupsList = cachedResult
                    .Where(g => !String.IsNullOrEmpty(g.Key))
                    .Select(g => g.Key)
                    .Distinct()
                    .ToList();

                if (groupsList.Count > 0)
                {
                    var template = GroupItemTemplate;
                    template.Children.Clear();
                    List<RenderContext> currentItems = new List<RenderContext>();
                    HtmlObjectDelegate renderAction = (control) =>
                    {
                        foreach (var ctx in currentItems)
                            ctx.JoinToParent();
                    };

                    template.RenderContentEvent += renderAction;

                    foreach (var groupName in groupsList)
                    {
                        currentItems.Clear();
                        currentItems.AddRange(cachedResult
                            .Where(g => g.Key == groupName)
                            .Select(g => g.Value));
                        template.DataContext = groupName;
                        template.Render(CurrentContext);
                    }

                    template.RenderContentEvent -= renderAction;
                }
                else
                    cachedResult.ForEach(c => c.Value.JoinToParent());
            }
            else
            {
                foreach (var sourceItem in source)
                {
                    ItemTemplate.DataContext = sourceItem;
                    ItemTemplate.Render(CurrentContext);
                }
            }
        }

        protected override void PreRender()
        {
            base.PreRender();
            if (String.IsNullOrEmpty(this.Name) && String.IsNullOrEmpty(this.Id))
                this.Id = HtmlControl.CreateUniqueId();
            if (String.IsNullOrEmpty(this.Name))
                this.Name = this.Id;
            else if (String.IsNullOrEmpty(this.Id))
                this.Id = this.Name;
        }
    }
}
