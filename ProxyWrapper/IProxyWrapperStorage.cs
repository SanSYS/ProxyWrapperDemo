using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace ProxyWrapper
{
    public interface IProxyWrapperStorage
    {
        bool Invoke(InvokeCommand invokeCommad, out object result);
        void LastResult(InvokeCommand invokeCommad, object result);
    }

    public struct InvokeCommand
    {
        public Type WrappedType { get; set; }
        public InvokeMemberBinder Binder { get; set; }
        public object[] Args { get; set; }
    }

    public class MethodCall
    {
        public string WrappedService { get; set; }
        
        public string Method { get; set; }

        public string Args { get; set; }

        public object Response { get; set; }

        public bool ActiveMock { get; set; }
    }
}