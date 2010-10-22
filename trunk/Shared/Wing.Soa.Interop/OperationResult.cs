﻿using System;
using System.Runtime.Serialization;

namespace Wing.Soa.Interop
{
    [DataContract]
    public class OperationResult
    {
        [DataMember(Name = "Status")]
        internal OperationStatus _status;

        [DataMember(Name = "Error")]
        internal Exception _error;

        [DataMember(Name = "Message")]
        internal string _message;

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