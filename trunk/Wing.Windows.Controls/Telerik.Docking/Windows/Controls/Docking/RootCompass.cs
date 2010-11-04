namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Windows;

    [TemplateVisualState(Name="HighlightBottomIndicator", GroupName="CommonStates"), TemplateVisualState(Name="LeftIndicatorVisibile", GroupName="LeftIndicatorVisibilityStates"), TemplateVisualState(Name="LeftIndicatorHidden", GroupName="LeftIndicatorVisibilityStates"), TemplateVisualState(Name="BottomIndicatorHidden", GroupName="BottomIndicatorVisibilityStates"), TemplatePart(Name="PART_LeftIndicator", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_TopIndicator", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_RightIndicator", Type=typeof(FrameworkElement)), TemplatePart(Name="PART_BottomIndicator", Type=typeof(FrameworkElement)), TemplateVisualState(Name="HighlightLeftIndicator", GroupName="CommonStates"), TemplateVisualState(Name="HighlightTopIndicator", GroupName="CommonStates"), TemplateVisualState(Name="HighlightRightIndicator", GroupName="CommonStates"), TemplateVisualState(Name="Normal", GroupName="CommonStates"), TemplateVisualState(Name="TopIndicatorVisibile", GroupName="TopIndicatorVisibilityStates"), TemplateVisualState(Name="TopIndicatorHidden", GroupName="TopIndicatorVisibilityStates"), TemplateVisualState(Name="RightIndicatorVisibile", GroupName="RightIndicatorVisibilityStates"), TemplateVisualState(Name="RightIndicatorHidden", GroupName="RightIndicatorVisibilityStates"), TemplateVisualState(Name="BottomIndicatorVisibile", GroupName="BottomIndicatorVisibilityStates")]
    public class RootCompass : Compass
    {
        public RootCompass()
        {
            base.DefaultStyleKey = typeof(RootCompass);
        }
    }
}

