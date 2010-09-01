﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wing.Olap.Controls.General.Tree
{
    public class MeasureGroupTreeNode : FolderTreeNode
    {
        public MeasureGroupTreeNode()
            : base(UriResources.Images.MeasuresFolderOpen16, UriResources.Images.MeasuresFolder16)
        { 
        }
    }
}