/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Galaktika.BusinessMonitor
 
    Galaktika.BusinessMonitor is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Galaktika.BusinessMonitor.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Galaktika.BusinessMonitor under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Ranet.Olap.Core.Data;

namespace Ranet.AgOlap.Providers
{
    public class DataTable 
    {
        private List<DataRow> m_Rows;
        private List<DataColumn> m_Columns;

        public DataTable()
        {
            m_Rows = new List<DataRow>();
            m_Columns = new List<DataColumn>();
        }


        public List<DataColumn> Columns
        {
            get { return m_Columns; }
        }

        public List<DataRow> Rows
        {
            get { return m_Rows; }
        }

        public static DataTable Create(DataTableWrapper wrapper)
        {
            DataTable view = new DataTable();
            if (wrapper != null)
            {
                foreach (var columnDefinition in wrapper.Columns)
                {
                    view.Columns.Add(new DataColumn(columnDefinition.Name,columnDefinition.Caption,columnDefinition.Type));
                }
                foreach (var row in wrapper.Rows)
                {
                    DataRow dataRow = new DataRow();                    
                    foreach (var cell in row.Cells)
                    {
                        dataRow.Add(new DataCell(cell.ColumnName,cell.Value,cell.Type));
                    }
                    view.Rows.Add(dataRow);
                }
                return view;
            }
            return null;
        }


    }

    public class DataColumn
    {
        public DataColumn()
        {
        }

        public DataColumn(string name, string caption)
            : this(name, caption, TypeCode.Object)
        {
        }

        public DataColumn(string name, string caption, Type type)
            : this(name, caption, System.Type.GetTypeCode(type))
        {
        }

        public DataColumn(string name, string caption, TypeCode type)
        {
            this.Name = name;
            this.Caption = caption;
            this.Type = type;
        }
      
        public string Name { get; set; }
      
        public string Caption { get; set; }
   
        public TypeCode Type { get; set; }
    }

    public class DataRow : List<DataCell>
    {
        //private List<DataCell> m_Cells = new List<DataCell>();
        
        //public List<DataCell> Cells
        //{
        //    get { return m_Cells; }
        //}
    }

    public class DataCell
    {
        public DataCell()
        {
        }

        public DataCell(string name, object value)
            : this(name, value, TypeCode.Object)
        {
        }

        public DataCell(string name, object value, Type type)
            : this(name, value, System.Type.GetTypeCode(type))
        {
        }

        public DataCell(string name, object value, TypeCode type)
        {
            this.ColumnName = name;
            this.Value = value;
            this.Type = type;
        }

        public string ColumnName { get; set; }
        
        public object Value { get; set; }
        
        public TypeCode Type { get; set; }
    }

//    public static class DataSourceCreator
//    {
//        private static readonly Regex PropertNameRegex =
//                new Regex(@"^[A-Za-z]+[A-Za-z0-9_]*$", RegexOptions.Singleline);

//        private static readonly Dictionary<string, Type> _typeBySigniture =
//                new Dictionary<string, Type>();



//    private static void CreateProperty(
//                        TypeBuilder tb, string propertyName, Type propertyType)
//        {
//            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName,
//                                                        propertyType,
//                                                        FieldAttributes.Private);


//            PropertyBuilder propertyBuilder =
//                tb.DefineProperty(
//                    propertyName, PropertyAttributes.HasDefault, propertyType, null);
//            MethodBuilder getPropMthdBldr =
//                tb.DefineMethod("get_" + propertyName,
//                    MethodAttributes.Public |
//                    MethodAttributes.SpecialName |
//                    MethodAttributes.HideBySig,
//                    propertyType, Type.EmptyTypes);

//            ILGenerator getIL = getPropMthdBldr.GetILGenerator();

//            getIL.Emit(OpCodes.Ldarg_0);
//            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
//            getIL.Emit(OpCodes.Ret);

//            MethodBuilder setPropMthdBldr =
//                tb.DefineMethod("set_" + propertyName,
//                  MethodAttributes.Public |
//                  MethodAttributes.SpecialName |
//                  MethodAttributes.HideBySig,
//                  null, new Type[] { propertyType });

//            ILGenerator setIL = setPropMthdBldr.GetILGenerator();

//            setIL.Emit(OpCodes.Ldarg_0);
//            setIL.Emit(OpCodes.Ldarg_1);
//            setIL.Emit(OpCodes.Stfld, fieldBuilder);
//            setIL.Emit(OpCodes.Ret);

//            propertyBuilder.SetGetMethod(getPropMthdBldr);
//            propertyBuilder.SetSetMethod(setPropMthdBldr);
//        }

//        private static TypeBuilder GetTypeBuilder(string typeSigniture)
//        {
//            AssemblyName an = new AssemblyName("TempAssembly" + typeSigniture);
//            AssemblyBuilder assemblyBuilder =
//                AppDomain.CurrentDomain.DefineDynamicAssembly(
//                    an, AssemblyBuilderAccess.Run);
//            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

//            TypeBuilder tb = moduleBuilder.DefineType("TempType" + typeSigniture
//                                , TypeAttributes.Public |
//                                TypeAttributes.Class |
//                                TypeAttributes.AutoClass |
//                                TypeAttributes.AnsiClass |
//                                TypeAttributes.BeforeFieldInit |
//                                TypeAttributes.AutoLayout
//                                , typeof(object));
//            return tb;
//        }
////        public static IEnumerable ToDataSource(this IEnumerable<IDictionary> list)
//        {
//            IDictionary firstDict = null;
//            bool hasData = false;
//            foreach (IDictionary currentDict in list)
//            {
//                hasData = true;
//                firstDict = currentDict;
//                break;
//            }
//            if (!hasData)
//            {
//                return new object[] { };
//            }
//            if (firstDict == null)
//            {
//                throw new ArgumentException("IDictionary entry cannot be null");
//            }

//            string typeSigniture = GetTypeSigniture(firstDict);

//            Type objectType = GetTypeByTypeSigniture(typeSigniture);

//            if (objectType == null)
//            {
//                TypeBuilder tb = GetTypeBuilder(typeSigniture);

//                ConstructorBuilder constructor =
//                            tb.DefineDefaultConstructor(
//                                        MethodAttributes.Public |
//                                        MethodAttributes.SpecialName |
//                                        MethodAttributes.RTSpecialName);


//                foreach (DictionaryEntry pair in firstDict)
//                {
//                    if (PropertNameRegex.IsMatch(Convert.ToString(pair.Key), 0))
//                    {
//                        CreateProperty(tb,
//                                        Convert.ToString(pair.Key),
//                                        GetValueType(pair.Value));
//                    }
//                    else
//                    {
//                        throw new ArgumentException(
//                                    @"Each key of IDictionary must be 
//                                alphanumeric and start with character.");
//                    }
//                }
//                objectType = tb.CreateType();

//                _typeBySigniture.Add(typeSigniture, objectType);
//            }

//            return GenerateEnumerable(objectType, list, firstDict);
//        }

//        private static Type GetTypeByTypeSigniture(string typeSigniture)
//        {
//            Type type;
//            return _typeBySigniture.TryGetValue(typeSigniture, out type) ? type : null;
//        }

//        private static Type GetValueType(object value)
//        {
//            return value == null ? typeof(object) : value.GetType();
//        }

//        private static string GetTypeSigniture(IDictionary firstDict)
//        {
//            StringBuilder sb = new StringBuilder();
//            foreach (DictionaryEntry pair in firstDict)
//            {
//                sb.AppendFormat("_{0}_{1}", pair.Key, GetValueType(pair.Value));
//            }
//            return sb.ToString().GetHashCode().ToString().Replace("-", "Minus");
//        }

//        private static IEnumerable GenerateEnumerable(
//                 Type objectType, IEnumerable<IDictionary> list, IDictionary firstDict)
//        {
//            var listType = typeof(List<>).MakeGenericType(new[] { objectType });
//            var listOfCustom = Activator.CreateInstance(listType);

//            foreach (var currentDict in list)
//            {
//                if (currentDict == null)
//                {
//                    throw new ArgumentException("IDictionary entry cannot be null");
//                }
//                var row = Activator.CreateInstance(objectType);
//                foreach (DictionaryEntry pair in firstDict)
//                {
//                    if (currentDict.Contains(pair.Key))
//                    {
//                        PropertyInfo property =
//                            objectType.GetProperty(Convert.ToString(pair.Key));
//                        property.SetValue(
//                            row,
//                            Convert.ChangeType(
//                                    currentDict[pair.Key],
//                                    property.PropertyType,
//                                    null),
//                            null);
//                    }
//                }
//                listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });
//            }
//            return listOfCustom as IEnumerable;
//        }

        

        
//    }


}
