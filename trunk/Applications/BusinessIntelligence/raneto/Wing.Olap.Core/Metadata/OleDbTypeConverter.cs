/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Ranet.Olap.Core.Metadata
{
    public static class OleDbTypeConverter
    {
        // Methods
        public static Type Convert(OleDbType oleDbType)
        {
            OleDbType type = oleDbType;
            if (type <= OleDbType.Filetime)
            {
                switch (type)
                {
                    case OleDbType.SmallInt:
                        return typeof(short);

                    case OleDbType.Integer:
                        return typeof(int);

                    case OleDbType.Single:
                        return typeof(float);

                    case OleDbType.Double:
                        return typeof(double);

                    case OleDbType.Currency:
                        return typeof(decimal);

                    case OleDbType.Date:
                        return typeof(DateTime);

                    case OleDbType.BSTR:
                        return typeof(string);

                    case OleDbType.IDispatch:
                        return typeof(object);

                    case OleDbType.Error:
                        return typeof(Exception);

                    case OleDbType.Boolean:
                        return typeof(bool);

                    case OleDbType.Variant:
                        return typeof(object);

                    case OleDbType.IUnknown:
                        return typeof(object);

                    case OleDbType.Decimal:
                        return typeof(decimal);

                    case ((OleDbType)15):
                        goto Label_0265;

                    case OleDbType.TinyInt:
                        return typeof(sbyte);

                    case OleDbType.UnsignedTinyInt:
                        return typeof(byte);

                    case OleDbType.UnsignedSmallInt:
                        return typeof(ushort);

                    case OleDbType.UnsignedInt:
                        return typeof(uint);

                    case OleDbType.BigInt:
                        return typeof(long);

                    case OleDbType.UnsignedBigInt:
                        return typeof(ulong);

                    case OleDbType.Filetime:
                        return typeof(DateTime);
                }
            }
            else
            {
                switch (type)
                {
                    case OleDbType.Binary:
                        return typeof(byte[]);

                    case OleDbType.Char:
                        return typeof(string);

                    case OleDbType.WChar:
                        return typeof(string);

                    case OleDbType.Numeric:
                        return typeof(decimal);

                    case (OleDbType.Binary | OleDbType.Single):
                    case (OleDbType.Binary | OleDbType.BSTR):
                    case (OleDbType.Char | OleDbType.BSTR):
                        goto Label_0265;

                    case OleDbType.DBDate:
                        return typeof(DateTime);

                    case OleDbType.DBTime:
                        return typeof(TimeSpan);

                    case OleDbType.DBTimeStamp:
                        return typeof(DateTime);

                    case OleDbType.PropVariant:
                        return typeof(object);

                    case OleDbType.VarNumeric:
                        return typeof(decimal);

                    case OleDbType.Guid:
                        return typeof(Guid);

                    case OleDbType.VarChar:
                        return typeof(string);

                    case OleDbType.LongVarChar:
                        return typeof(string);

                    case OleDbType.VarWChar:
                        return typeof(string);

                    case OleDbType.LongVarWChar:
                        return typeof(string);

                    case OleDbType.VarBinary:
                        return typeof(byte[]);

                    case OleDbType.LongVarBinary:
                        return typeof(byte[]);
                }
            }
        Label_0265:
            return null;
        }

        public static OleDbType GetRestrictedOleDbType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type == typeof(byte[]))
            {
                return OleDbType.Binary;
            }
            if (type == typeof(bool))
            {
                return OleDbType.Boolean;
            }
            if (type == typeof(string))
            {
                return OleDbType.WChar;
            }
            if (type == typeof(DateTime))
            {
                return OleDbType.Date;
            }
            if (type == typeof(decimal))
            {
                return OleDbType.Double;
            }
            if (type == typeof(double))
            {
                return OleDbType.Double;
            }
            if (type == typeof(Guid))
            {
                return OleDbType.WChar;
            }
            if (type == typeof(object))
            {
                return OleDbType.WChar;
            }
            if (type == typeof(sbyte))
            {
                return OleDbType.TinyInt;
            }
            if (type == typeof(short))
            {
                return OleDbType.SmallInt;
            }
            if (type == typeof(int))
            {
                return OleDbType.Integer;
            }
            if (type == typeof(long))
            {
                return OleDbType.BigInt;
            }
            if (type == typeof(byte))
            {
                return OleDbType.UnsignedTinyInt;
            }
            if (type == typeof(ushort))
            {
                return OleDbType.UnsignedSmallInt;
            }
            if (type == typeof(uint))
            {
                return OleDbType.UnsignedInt;
            }
            if (type == typeof(ulong))
            {
                return OleDbType.UnsignedBigInt;
            }
            if (type != typeof(float))
            {
                throw new ArgumentOutOfRangeException("type", type, String.Format("System Type {0} has no restricted OleDbType equivalent", type.Name));
            }
            return OleDbType.Single;
        }
    }
}
