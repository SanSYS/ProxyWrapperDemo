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
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                bool success = _wrapperStorage.Invoke(binder, args, out object resFromStorage);

                if (success)
                {
                    result = resFromStorage;
                    return true;
                }

                result = _wrappedObject
                    .GetType()
                    .GetMethod(binder.Name)
                    ?.Invoke(_wrappedObject, args);

                _wrapperStorage.LastResult(binder, args, result);
                
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
