using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.AgOlap.Controls.ToolBar;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Metadata;
using System.Windows.Controls.Primitives;
using Ranet.AgOlap.Controls.General.Tree;

namespace Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers
{
    public partial class CalculationsEditor : UserControl
    {
        RanetToolBarButton m_AddCalcMemberButton;
        RanetToolBarButton m_AddNamedSetButton;
        RanetToolBarButton m_DeleteCalcMemberButton;
        RanetToolBarButton m_ClearButton;

        IList<String> m_FormatStrings;
        IList<String> FormatStrings
        {
            get
            {
                if (m_FormatStrings == null)
                {
                    m_FormatStrings = new List<String>();
                }
                return m_FormatStrings;
            }
        }

        public CalculationsEditor()
        {
            InitializeComponent();

            m_AddCalcMemberButton = new RanetToolBarButton();
            m_AddCalcMemberButton.Content = UiHelper.CreateIcon(UriResources.Images.AddMeasure16);
            m_AddCalcMemberButton.Click += new RoutedEventHandler(m_AddCalcMemberButton_Click);
            ToolTipService.SetToolTip(m_AddCalcMemberButton, Localization.CalcMemberEditor_AddMember);
            ToolBar.AddItem(m_AddCalcMemberButton);

            m_AddNamedSetButton = new RanetToolBarButton();
            m_AddNamedSetButton.Content = UiHelper.CreateIcon(UriResources.Images.AddNamedSet16);
            m_AddNamedSetButton.Click += new RoutedEventHandler(m_AddNamedSetButton_Click);
            ToolTipService.SetToolTip(m_AddNamedSetButton, Localization.CalcMemberEditor_AddNamedSet);
            ToolBar.AddItem(m_AddNamedSetButton);

            m_DeleteCalcMemberButton = new RanetToolBarButton();
            m_DeleteCalcMemberButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveCross16);
            m_DeleteCalcMemberButton.Click += new RoutedEventHandler(m_DeleteCalcMemberButton_Click);
            m_DeleteCalcMemberButton.IsEnabled = false;
            ToolTipService.SetToolTip(m_DeleteCalcMemberButton, Localization.CalcMemberEditor_DeleteMember);
            ToolBar.AddItem(m_DeleteCalcMemberButton);

            m_ClearButton = new RanetToolBarButton();
            m_ClearButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveAllCross16);
            m_ClearButton.Click += new RoutedEventHandler(m_ClearButton_Click);
            m_ClearButton.IsEnabled = (Members.Count + Sets.Count) > 0;
            ToolTipService.SetToolTip(m_ClearButton, Localization.CalcMemberEditor_Clear);
            ToolBar.AddItem(m_ClearButton);

            MembersList.SelectionChanged += new EventHandler<SelectionChangedEventArgs<CalculationInfoBase>>(MembersList_SelectionChanged);

            MemberCtrl.InnerEditStart += new EventHandler(MemberCtrl_InnerEditStart);
            MemberCtrl.InnerEditEnd += new EventHandler(MemberCtrl_InnerEditEnd);
            MemberCtrl.EditEnd += new EventHandler(MemberCtrl_EditEnd);

            SetCtrl.EditEnd += new EventHandler(MemberCtrl_EditEnd);

            FormatStrings.Add("#");
            FormatStrings.Add("#,#");
            FormatStrings.Add("#,#.00");
            FormatStrings.Add("$#,#;-$#,#;");
            FormatStrings.Add("$#,#.00;-$#,#.00;");
            FormatStrings.Add("Standard");
            FormatStrings.Add("Short Date");
            FormatStrings.Add("Short Time");
            FormatStrings.Add("Percent");

            CubeBrowser.DragNodes = true;
            CubeBrowser.DragStarted += new EventHandler<DragNodeArgs<DragStartedEventArgs>>(CubeBrowser_DragStarted);
            CubeBrowser.DragDelta += new EventHandler<DragNodeArgs<DragDeltaEventArgs>>(CubeBrowser_DragDelta);
            CubeBrowser.DragCompleted += new EventHandler<DragNodeArgs<DragCompletedEventArgs>>(CubeBrowser_DragCompleted);
        }

        void m_ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Localization.DeleteAll_Question, Localization.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Members.Clear();
                Sets.Clear();
                m_ClearButton.IsEnabled = (Members.Count + Sets.Count) > 0;
                MembersList.Initialize(Members, Sets);
                RefreshMetadataTree();
            }
        }

        #region Drag and Drop из метаданных куба
        /// <summary>
        /// Позиция старта таскания
        /// </summary>
        Point m_DragStart = new Point(0, 0);
        /// <summary>
        /// Предыдущая позиция при перетаскивании
        /// </summary>
        Point m_PrevDrag = new Point(0, 0);

        void CubeBrowser_DragCompleted(object sender, DragNodeArgs<DragCompletedEventArgs> e)
        {
            if (e.Args.Canceled == false)
            {
                TreeViewItem node = e.Node;

                Point point = new Point(m_DragStart.X + e.Args.HorizontalChange, m_DragStart.Y + e.Args.VerticalChange);
                if (MemberCtrl.Visibility == Visibility.Visible && MemberCtrl.CanDrop(point))
                {
                    MemberCtrl.Drop(point, CubeBrowser.GetNodeString(node as CustomTreeNode));
                }
                if (SetCtrl.Visibility == Visibility.Visible && SetCtrl.CanDrop(point))
                {
                    SetCtrl.Drop(point, CubeBrowser.GetNodeString(node as CustomTreeNode));
                }
            }
        }

        void CubeBrowser_DragDelta(object sender, DragNodeArgs<DragDeltaEventArgs> e)
        {
            Point m_DragDelta = new Point(m_PrevDrag.X + e.Args.HorizontalChange, m_PrevDrag.Y + e.Args.VerticalChange);

            if (MemberCtrl.Visibility == Visibility.Visible)
                MemberCtrl.HighlightDrop(m_DragDelta);
            if (SetCtrl.Visibility == Visibility.Visible)
                SetCtrl.HighlightDrop(m_DragDelta);

            m_PrevDrag = m_DragDelta;
        }

        void CubeBrowser_DragStarted(object sender, DragNodeArgs<DragStartedEventArgs> e)
        {
            m_DragStart = new Point(e.Args.HorizontalOffset, e.Args.VerticalOffset);
            m_PrevDrag = m_DragStart;
        }
        #endregion Drag and Drop из метаданных куба

        public void EndEdit()
        {
            if (MemberCtrl.IsEnabled && MemberCtrl.Visibility == Visibility.Visible)
            {
                if (MemberCtrl.Member != null && MemberCtrl.Member.Name != MemberCtrl.NameText)
                {
                    Members.Remove(MemberCtrl.Member.Name);
                    String newName = BuildNewCalculatedMemberName(MemberCtrl.NameText);
                    MemberCtrl.Member.Name = newName;
                    Members.Add(MemberCtrl.Member.Name, MemberCtrl.Member);
                    MemberCtrl.Initialize(MemberCtrl.Member);

                    MembersList.Refresh();
                }            
            }

            if (SetCtrl.IsEnabled && SetCtrl.Visibility == Visibility.Visible)
            {
                if (SetCtrl.Set != null && SetCtrl.Set.Name != SetCtrl.NameText)
                {
                    Sets.Remove(SetCtrl.Set.Name);
                    String newName = BuildNewCalculatedNamedSetName(SetCtrl.NameText);
                    SetCtrl.Set.Name = newName;
                    Sets.Add(SetCtrl.Set.Name, SetCtrl.Set);
                    SetCtrl.Initialize(SetCtrl.Set);

                    MembersList.Refresh();
                }
            }

            RefreshMetadataTree();
        }

        void MemberCtrl_EditEnd(object sender, EventArgs e)
        {
            EndEdit();

            MembersList.Refresh();
        }

        String BuildNewCalculatedMemberName(String defaultName)
        {
            String name = defaultName;
            if (String.IsNullOrEmpty(name))
                name = "[Calculated Member]";

            int i = 1;
            String start_name = name;
            while (Members.ContainsKey(name))
            {
                // Начинаем формировать имя заново
                name = start_name;
                bool add_bracket = false;
                if (name.EndsWith("]"))
                {
                    name = name.Substring(0, name.Length - 1);
                    add_bracket = true;
                }

                name += " " + i.ToString();
                i++;
                if (add_bracket)
                    name += "]";
            }
            return name;
        }

        String BuildNewCalculatedNamedSetName(String defaultName)
        {
            String name = defaultName;
            if (String.IsNullOrEmpty(name))
                name = "[Set]";

            int i = 1;
            String start_name = name;
            while (Sets.ContainsKey(name))
            {
                // Начинаем формировать имя заново
                name = start_name;
                bool add_bracket = false;
                if (name.EndsWith("]"))
                {
                    name = name.Substring(0, name.Length - 1);
                    add_bracket = true;
                }

                name += " " + i.ToString();
                i++;
                if (add_bracket)
                    name += "]";
            }
            return name;
        }

        void m_AddCalcMemberButton_Click(object sender, RoutedEventArgs e)
        {
            CalcMemberInfo info = new CalcMemberInfo();

            // Если по выбранному узлу можем определить иерархию, то уникальное имя иерархии добавляем в имя вычисляемого элемента
            // В этом случае элемент попадет в данную иерархию
            // В противном случае - в иерархию [Measures] даже если она явно не прописана в имени
            String name_prefix = String.Empty;
            var hierarchy = CubeBrowser.SelectedNode as HierarchyTreeNode;
            if (hierarchy != null)
            {
                if (hierarchy.Info != null && hierarchy.Info is HierarchyInfo)
                    name_prefix = ((HierarchyInfo)hierarchy.Info).UniqueName;
            }
            var level = CubeBrowser.SelectedNode as LevelTreeNode;
            if (level != null)
            {
                if (level.Info != null && level.Info is LevelInfo)
                    name_prefix = ((LevelInfo)level.Info).ParentHirerachyId;
            }
            var member = CubeBrowser.SelectedNode as MemberLiteTreeNode;
            if (member != null)
            {
                if (member.Info != null)
                    name_prefix = member.Info.HierarchyUniqueName;
            }

            String defaultName = String.IsNullOrEmpty(name_prefix) ? "[Calculated Member]" : name_prefix + "." + "[Calculated Member]";
            info.Name = BuildNewCalculatedMemberName(defaultName);
            Members.Add(info.Name, info);
            
            MembersList.Initialize(Members, Sets, info);
            RefreshMetadataTree();
            m_ClearButton.IsEnabled = (Members.Count + Sets.Count) > 0;
        }

        void m_AddNamedSetButton_Click(object sender, RoutedEventArgs e)
        {
            CalculatedNamedSetInfo info = new CalculatedNamedSetInfo();
            info.Name = BuildNewCalculatedNamedSetName(String.Empty);
            Sets.Add(info.Name, info);

            MembersList.Initialize(Members, Sets, info);
            RefreshMetadataTree();
            m_ClearButton.IsEnabled = (Members.Count + Sets.Count) > 0;
        }

        void MemberCtrl_InnerEditEnd(object sender, EventArgs e)
        {
            Raise_EditEnd();
        }

        void MemberCtrl_InnerEditStart(object sender, EventArgs e)
        {
            Raise_EditStart();
        }

        void MembersList_SelectionChanged(object sender, SelectionChangedEventArgs<CalculationInfoBase> e)
        {
            EndEdit();

            MemberCtrl.IsEnabled = SetCtrl.IsEnabled = e.NewValue != null;
            if (e.NewValue == null)
            {
                if(MemberCtrl.Visibility == Visibility.Visible)
                    MemberCtrl.Initialize(null);
                if(SetCtrl.Visibility == Visibility.Visible)
                    SetCtrl.Initialize(null);
            }

            CalcMemberInfo memberInfo = e.NewValue as CalcMemberInfo;
            if (memberInfo != null)
            {
                MemberCtrl.Visibility = Visibility.Visible;
                SetCtrl.Visibility = Visibility.Collapsed;
                MemberCtrl.Initialize(memberInfo);
            }

            CalculatedNamedSetInfo setInfo = e.NewValue as CalculatedNamedSetInfo;
            if (setInfo != null)
            {
                MemberCtrl.Visibility = Visibility.Collapsed;
                SetCtrl.Visibility = Visibility.Visible;
                SetCtrl.Initialize(setInfo);
            }

            m_DeleteCalcMemberButton.IsEnabled = e.NewValue != null;
        }

        public event EventHandler EditStart;
        void Raise_EditStart()
        {
            EventHandler handler = EditStart;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        public event EventHandler EditEnd;
        void Raise_EditEnd()
        {
            EventHandler handler = EditEnd;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void RefreshMetadataTree()
        {
            List<CalcMemberInfo> members = new List<CalcMemberInfo>();
            foreach (CalculationInfoBase info in Members.Values)
            {
                CalcMemberInfo memberInfo = info as CalcMemberInfo;
                if (memberInfo != null)
                {
                    members.Add(memberInfo);
                }
            }

            List<CalculatedNamedSetInfo> sets = new List<CalculatedNamedSetInfo>();
            foreach (CalculationInfoBase info in Sets.Values)
            {
                CalculatedNamedSetInfo setInfo = info as CalculatedNamedSetInfo;
                if (setInfo != null)
                {
                    sets.Add(setInfo);
                }
            }

            CubeBrowser.CalculatedMembers = members;
            CubeBrowser.CalculatedNamedSets = sets;
        }

        void m_DeleteCalcMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Localization.DeleteCurrent_Question, Localization.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CalcMemberInfo memberInfo = MembersList.CurrentObject as CalcMemberInfo;
                if (memberInfo != null)
                {
                    Members.Remove(memberInfo.Name);
                    MembersList.RemoveItem(memberInfo);
                }
                else
                {
                    CalculatedNamedSetInfo setInfo = MembersList.CurrentObject as CalculatedNamedSetInfo;
                    if (setInfo != null)
                    {
                        Sets.Remove(setInfo.Name);
                        MembersList.RemoveItem(setInfo);
                    }
                }

                m_ClearButton.IsEnabled = (Members.Count + Sets.Count) > 0;
                RefreshMetadataTree();
            }
            return;
        }

        Dictionary<String, CalculationInfoBase> m_Members;
        public Dictionary<String, CalculationInfoBase> Members
        {
            get
            {
                if(m_Members == null)
                {
                    m_Members = new Dictionary<string,CalculationInfoBase>();
                }
                return m_Members;
            }
            set
            {
                m_Members = value;
            }
        }

        Dictionary<String, CalculationInfoBase> m_Sets;
        public Dictionary<String, CalculationInfoBase> Sets
        {
            get
            {
                if (m_Sets == null)
                {
                    m_Sets = new Dictionary<string, CalculationInfoBase>();
                }
                return m_Sets;
            }
            set
            {
                m_Sets = value;
            }
        }

        public void Initialize(Dictionary<String, CalculationInfoBase> members, Dictionary<String, CalculationInfoBase> sets, CubeDefInfo cubeInfo, String measureGroupName)
        {
            m_Members = members;
            m_Sets = sets;
            m_ClearButton.IsEnabled = (Members.Count + Sets.Count) > 0;

            MemberCtrl.InitializeMetadata(cubeInfo);

            if(CubeBrowser.CubeInfo != cubeInfo)
                CubeBrowser.Initialize(cubeInfo);
            CubeBrowser.MeasureGroupName = measureGroupName;

            MemberCtrl.IsEnabled = SetCtrl.IsEnabled = false;

            foreach (CalculationInfoBase info in Members.Values)
            {
                CalcMemberInfo member = info as CalcMemberInfo;
                if (member != null)
                {
                    // Добавляем в список стандартных пользовательские строки форматирования
                    if (!String.IsNullOrEmpty(member.FormatString))
                    {
                        if (!FormatStrings.Contains(member.FormatString))
                            FormatStrings.Add(member.FormatString);
                    }
                }
            }

            MemberCtrl.FormatStrings = FormatStrings;
            MembersList.Initialize(members, sets);

            RefreshMetadataTree();

            //CalcMemberInfo memberInfo = MembersList.CurrentObject as CalcMemberInfo;
            //if (memberInfo != null)
            //{
            //    MemberCtrl.Initialize(memberInfo);
            //}
        }

    }

    
}
