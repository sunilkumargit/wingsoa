using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Wing.Soa.Interop
{
    [DataContract]
    public class OperationResult
    {
        [DataMember]
        private OperationStatus _status;

        [DataMember]
        private Exception _error;

        [DataMember]
        private string _message;

        public OperationStatus Status { get { return _status; } set { _status = value; } }
        public Exception Error
        {
            get { return _error; }
            set
            {
                _error = value;
                if (_error != null)
                    Status = OperationStatus.Error;
            }
        }

        public String Message { get { return _message; } set { _message = value; } }
    }
}