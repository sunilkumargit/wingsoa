using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security.Model;

namespace Wing.Security.Impl
{
    class SchemaImpl : ISchema
    {
        private SchemaModel _model;

        public SchemaImpl(SchemaModel model)
        {
            _model = model;
            Id = model.SchemaId;
            Name = model.Name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
    }
}
