﻿using System;
using System.Runtime.Serialization;
using Wing.Utils;

namespace Wing.Soa.Interop
{
    [DataContract]
    public class SoaServiceDescriptor
    {
        public SoaServiceDescriptor(String serviceName, String contractTypeName, String serviceTypeName, bool isSingletonInstance)
        {
            Assert.NullArgument(serviceName, "serviceName");
            Assert.NullArgument(contractTypeName, "contractType");
            Assert.NullArgument(serviceTypeName, "serviceType");
            ServiceName = serviceName;
            ContractTypeName = contractTypeName;
            ServiceTypeName = serviceTypeName;
            IsSingletonInstance = isSingletonInstance;

            var typeFullName = contractTypeName.Substring(0, contractTypeName.IndexOf(","));
            if (typeFullName.IndexOf(".") == -1)
                ContractTypeRefName = typeFullName;
            else
                ContractTypeRefName = typeFullName.Substring(typeFullName.LastIndexOf(".") + 1);
        }

        public SoaServiceDescriptor(String serviceName, Type contractType, Type serviceType, bool isSingletonInstance)
            : this(serviceName, contractType.AssemblyQualifiedName, serviceType.AssemblyQualifiedName, isSingletonInstance) { }

        [DataMember]
        private string _ServiceName;

        public string ServiceName
        {
            get { return _ServiceName; }
            private set { _ServiceName = value; }
        }

        [DataMember]
        private String _ContractTypeName;
        public String ContractTypeName
        {
            get { return _ContractTypeName; }
            private set { _ContractTypeName = value; }
        }

        [DataMember]
        private String _ServiceTypeName;
        public String ServiceTypeName
        {
            get { return _ServiceTypeName; }
            private set { _ServiceTypeName = value; }
        }

        [DataMember]
        private bool _IsSingletonInstance;
        public bool IsSingletonInstance
        {
            get { return _IsSingletonInstance; }
            private set { _IsSingletonInstance = value; }
        }

        [DataMember]
        private string _ContractTypeRefName;
        public String ContractTypeRefName
        {
            get { return _ContractTypeRefName; }
            set { _ContractTypeRefName = value; }
        }
    }
}
