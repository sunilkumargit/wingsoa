using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.Olap.Core.Providers;
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls.PivotGrid
{
    /// <summary>
    /// Описывает сортировку оси по конкретному таплу
    /// </summary>
    public class SortByValueDescriptor : SortDescriptor
    {
        /// <summary>
        /// Уникальное имя меры, по которой проиизводится сортировка (если мера не задана в тапле)
        /// </summary>
        public String MeasureUniqueName = String.Empty;
        
        Dictionary<String, String> m_Tuple;
        /// <summary>
        /// Тапл: Ключ - уник. имя иерархии. Значение - уник. имя элемента
        /// </summary>
        public Dictionary<String, String> Tuple
        {
            get {
                if (m_Tuple == null)
                {
                    m_Tuple = new Dictionary<string, string>();
                }
                return m_Tuple; 
            }
            set { m_Tuple = value; }
        }

        public override string SortBy
        {
            get
            {
                String res = String.Empty;
                foreach (var un in Tuple.Values)
                {
                    if (!String.IsNullOrEmpty(un))
                    {
                        if (!String.IsNullOrEmpty(res))
                            res += ", ";
                        res += un;
                    }
                }

                // Если меры в тапле нет, то пытаемся ее дописать
                if (!Tuple.ContainsKey("[Measures]") && !String.IsNullOrEmpty(MeasureUniqueName))
                {
                    if (!String.IsNullOrEmpty(res))
                        res += ", ";
                    res += MeasureUniqueName;
                }

                return res;
            }
            set
            {
                base.SortBy = value;
            }
        }

        public override SortDescriptor Clone()
        {
            var target = new SortByValueDescriptor();
            target.Type = Type;
            foreach(var item in Tuple)
            target.Tuple.Add(item.Key, item.Value);
            return target;
        }

        public bool CompareByTuple(Dictionary<String, String> tuple)
        {
            if(tuple == null)
                return false;
            if (Tuple.Count != tuple.Count)
                return false;

            foreach (var item in Tuple)
            {
                if (!tuple.ContainsKey(item.Key))
                    return false;
                if (tuple[item.Key] != item.Value)
                    return false;
            }
            return true;
        }
    }
}
