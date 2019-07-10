using System;
using System.Dynamic;

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
}