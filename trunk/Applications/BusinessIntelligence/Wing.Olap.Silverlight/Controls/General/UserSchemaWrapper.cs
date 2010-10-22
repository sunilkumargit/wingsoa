/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


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
