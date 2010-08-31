/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

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
using Wing.Olap.Controls.General.Tree;
using Wing.Olap.Controls.General.ClientServer;

namespace Wing.Olap.Controls.General
{
    public class UserSchemaWrapper<SchemaType, ValueType>
    {
        public readonly SchemaType Schema = default(SchemaType);
        public readonly ValueType UserData = default(ValueType);

        public UserSchemaWrapper(SchemaType schema)
        {
            Schema = schema;
        }

        public UserSchemaWrapper(SchemaType schema, ValueType userData)
            : this(schema)
        {
            UserData = userData;
        }
    }
}
