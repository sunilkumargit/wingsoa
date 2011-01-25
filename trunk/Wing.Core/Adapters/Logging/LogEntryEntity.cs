using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;
using Wing.Logging;

namespace Wing.Adapters.Logging
{
    public class LogEntryEntity : StoreEntity
    {
        [PersistentMember]
        public String LogName { get; set; }

        [PersistentMember]
        public int CategoryIndex
        {
            get { return (int)Category; }
            set { Category = (Category)value; }
        }

        public Category Category { get; set; }

        [PersistentMember]
        public int PriorityIndex
        {
            get { return (int)Priority; }
            set { Priority = (Priority)value; }
        }

        public Priority Priority { get; set; }

        [PersistentMember(int.MaxValue)]
        public String Message { get; set; }
    }
}
