using System;
using System.Dynamic;
using System.Reflection;
using RazorEngine.Compilation.ImpromptuInterface;

namespace ProxyWrapper
{
    public class ProxyWrapper<T> : DynamicObject where T : class
    {
        private readonly T _wrappedObject;
        private IProxyWrapperStorage _wrapperStorage;
        private readonly Type _wrappedService;

        public static T Wrap(T obj, IProxyWrapperStorage wrapperStorage)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T1 must be an Interface");
            
            return new ProxyWrapper<T>(obj)
            {
                _wrapperStorage = wrapperStorage
            }.ActLike<T>();
        }

        private ProxyWrapper(T obj)
        {
            _wrappedObject = obj;
            _wrappedService = typeof(T);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                var cmd = new InvokeCommand()
                {
                    WrappedType = _wrappedService,
                    Binder = binder,
                    Args = args
                };
                
                bool success = _wrapperStorage.Invoke(cmd, out object resFromStorage);

                if (success)
                {
                    result = resFromStorage;
                    return true;
                }

                result = _wrappedObject.GetType().GetMethod(binder.Name)?.Invoke(_wrappedObject, args);

                _wrapperStorage.LastResult(cmd, result);
                
                return true;
            }
            catch (TargetInvocationException ex)
            {
                result = null;
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }
    }
}
