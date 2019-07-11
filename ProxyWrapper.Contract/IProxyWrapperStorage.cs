using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace ProxyWrapper
{
    public interface IProxyWrapperStorage
    {
        bool Invoke(InvokeCommand invokeCommad, out object result);
        void LastResult(InvokeCommand invokeCommad, object result);
        Task<IEnumerable<Interface>> GetInterfaces();
    }

    public struct InvokeCommand
    {
        public Type WrappedType { get; set; }
        public InvokeMemberBinder Binder { get; set; }
        public object[] Args { get; set; }
    }
}