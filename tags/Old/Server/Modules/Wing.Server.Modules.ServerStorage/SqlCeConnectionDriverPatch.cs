using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using System.Data;

namespace Wing.Server.Modules.ServerStorage
{
    public class SqlCeConnectionDriverPatch : SqlServerCeDriver
    {
        protected override void InitializeParameter(System.Data.IDbDataParameter dbParam, string name, SqlType sqlType)
        {
            base.InitializeParameter(dbParam, name, sqlType);
            if (sqlType is StringClobSqlType)
            {
                var param = (SqlCeParameter)dbParam;
                param.SqlDbType = SqlDbType.NText;
            }
        }
    }
}
