﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50401.0
// 
namespace Wing.Olap.OlapWebService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="OlapWebService.OlapWebServiceSoap")]
    public interface OlapWebServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/PerformOlapServiceAction", ReplyAction="*")]
        System.IAsyncResult BeginPerformOlapServiceAction(Wing.Olap.OlapWebService.PerformOlapServiceActionRequest request, System.AsyncCallback callback, object asyncState);
        
        Wing.Olap.OlapWebService.PerformOlapServiceActionResponse EndPerformOlapServiceAction(System.IAsyncResult result);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PerformOlapServiceActionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PerformOlapServiceAction", Namespace="http://tempuri.org/", Order=0)]
        public Wing.Olap.OlapWebService.PerformOlapServiceActionRequestBody Body;
        
        public PerformOlapServiceActionRequest() {
        }
        
        public PerformOlapServiceActionRequest(Wing.Olap.OlapWebService.PerformOlapServiceActionRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class PerformOlapServiceActionRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string schemaType;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string schema;
        
        public PerformOlapServiceActionRequestBody() {
        }
        
        public PerformOlapServiceActionRequestBody(string schemaType, string schema) {
            this.schemaType = schemaType;
            this.schema = schema;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class PerformOlapServiceActionResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="PerformOlapServiceActionResponse", Namespace="http://tempuri.org/", Order=0)]
        public Wing.Olap.OlapWebService.PerformOlapServiceActionResponseBody Body;
        
        public PerformOlapServiceActionResponse() {
        }
        
        public PerformOlapServiceActionResponse(Wing.Olap.OlapWebService.PerformOlapServiceActionResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class PerformOlapServiceActionResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string PerformOlapServiceActionResult;
        
        public PerformOlapServiceActionResponseBody() {
        }
        
        public PerformOlapServiceActionResponseBody(string PerformOlapServiceActionResult) {
            this.PerformOlapServiceActionResult = PerformOlapServiceActionResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface OlapWebServiceSoapChannel : Wing.Olap.OlapWebService.OlapWebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PerformOlapServiceActionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public PerformOlapServiceActionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OlapWebServiceSoapClient : System.ServiceModel.ClientBase<Wing.Olap.OlapWebService.OlapWebServiceSoap>, Wing.Olap.OlapWebService.OlapWebServiceSoap {
        
        private BeginOperationDelegate onBeginPerformOlapServiceActionDelegate;
        
        private EndOperationDelegate onEndPerformOlapServiceActionDelegate;
        
        private System.Threading.SendOrPostCallback onPerformOlapServiceActionCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public OlapWebServiceSoapClient() {
        }
        
        public OlapWebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OlapWebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OlapWebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OlapWebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<PerformOlapServiceActionCompletedEventArgs> PerformOlapServiceActionCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Wing.Olap.OlapWebService.OlapWebServiceSoap.BeginPerformOlapServiceAction(Wing.Olap.OlapWebService.PerformOlapServiceActionRequest request, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginPerformOlapServiceAction(request, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        private System.IAsyncResult BeginPerformOlapServiceAction(string schemaType, string schema, System.AsyncCallback callback, object asyncState) {
            Wing.Olap.OlapWebService.PerformOlapServiceActionRequest inValue = new Wing.Olap.OlapWebService.PerformOlapServiceActionRequest();
            inValue.Body = new Wing.Olap.OlapWebService.PerformOlapServiceActionRequestBody();
            inValue.Body.schemaType = schemaType;
            inValue.Body.schema = schema;
            return ((Wing.Olap.OlapWebService.OlapWebServiceSoap)(this)).BeginPerformOlapServiceAction(inValue, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Wing.Olap.OlapWebService.PerformOlapServiceActionResponse Wing.Olap.OlapWebService.OlapWebServiceSoap.EndPerformOlapServiceAction(System.IAsyncResult result) {
            return base.Channel.EndPerformOlapServiceAction(result);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        private string EndPerformOlapServiceAction(System.IAsyncResult result) {
            Wing.Olap.OlapWebService.PerformOlapServiceActionResponse retVal = ((Wing.Olap.OlapWebService.OlapWebServiceSoap)(this)).EndPerformOlapServiceAction(result);
            return retVal.Body.PerformOlapServiceActionResult;
        }
        
        private System.IAsyncResult OnBeginPerformOlapServiceAction(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string schemaType = ((string)(inValues[0]));
            string schema = ((string)(inValues[1]));
            return this.BeginPerformOlapServiceAction(schemaType, schema, callback, asyncState);
        }
        
        private object[] OnEndPerformOlapServiceAction(System.IAsyncResult result) {
            string retVal = this.EndPerformOlapServiceAction(result);
            return new object[] {
                    retVal};
        }
        
        private void OnPerformOlapServiceActionCompleted(object state) {
            if ((this.PerformOlapServiceActionCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.PerformOlapServiceActionCompleted(this, new PerformOlapServiceActionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void PerformOlapServiceActionAsync(string schemaType, string schema) {
            this.PerformOlapServiceActionAsync(schemaType, schema, null);
        }
        
        public void PerformOlapServiceActionAsync(string schemaType, string schema, object userState) {
            if ((this.onBeginPerformOlapServiceActionDelegate == null)) {
                this.onBeginPerformOlapServiceActionDelegate = new BeginOperationDelegate(this.OnBeginPerformOlapServiceAction);
            }
            if ((this.onEndPerformOlapServiceActionDelegate == null)) {
                this.onEndPerformOlapServiceActionDelegate = new EndOperationDelegate(this.OnEndPerformOlapServiceAction);
            }
            if ((this.onPerformOlapServiceActionCompletedDelegate == null)) {
                this.onPerformOlapServiceActionCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPerformOlapServiceActionCompleted);
            }
            base.InvokeAsync(this.onBeginPerformOlapServiceActionDelegate, new object[] {
                        schemaType,
                        schema}, this.onEndPerformOlapServiceActionDelegate, this.onPerformOlapServiceActionCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override Wing.Olap.OlapWebService.OlapWebServiceSoap CreateChannel() {
            return new OlapWebServiceSoapClientChannel(this);
        }
        
        private class OlapWebServiceSoapClientChannel : ChannelBase<Wing.Olap.OlapWebService.OlapWebServiceSoap>, Wing.Olap.OlapWebService.OlapWebServiceSoap {
            
            public OlapWebServiceSoapClientChannel(System.ServiceModel.ClientBase<Wing.Olap.OlapWebService.OlapWebServiceSoap> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginPerformOlapServiceAction(Wing.Olap.OlapWebService.PerformOlapServiceActionRequest request, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = request;
                System.IAsyncResult _result = base.BeginInvoke("PerformOlapServiceAction", _args, callback, asyncState);
                return _result;
            }
            
            public Wing.Olap.OlapWebService.PerformOlapServiceActionResponse EndPerformOlapServiceAction(System.IAsyncResult result) {
                object[] _args = new object[0];
                Wing.Olap.OlapWebService.PerformOlapServiceActionResponse _result = ((Wing.Olap.OlapWebService.PerformOlapServiceActionResponse)(base.EndInvoke("PerformOlapServiceAction", _args, result)));
                return _result;
            }
        }
    }
}
