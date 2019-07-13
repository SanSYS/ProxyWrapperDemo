using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProxyWrapper.Contract;

namespace ProxyWrapper
{
    public class ProxyWrapperFileStorage: IProxyWrapperStorage
    {
        private readonly string _filePath;
        private readonly List<MethodCall> _calls;
        private readonly JsonSerializerSettings _jsonSettigs;

        public ProxyWrapperFileStorage(string filePath)
        {
            _filePath = filePath;

            _calls = new List<MethodCall>();

            _jsonSettigs = new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto};

            if (File.Exists(_filePath))
            {
                string cache = File.ReadAllText(_filePath);

                if (!string.IsNullOrWhiteSpace(cache))
                {
                    _calls = JsonConvert.DeserializeObject<List<MethodCall>>(cache, _jsonSettigs);
                }
            }

        }

        public bool Invoke(InvokeCommand invokeCommad, out object result)
        {
            string jsonArgs = JsonConvert.SerializeObject(invokeCommad.Args);

            var callRes = _calls.FirstOrDefault(p => p.WrappedService == invokeCommad.WrappedType.FullName
                                                         && p.Method == invokeCommad.Binder.Name
                                                         && p.Args == jsonArgs);

            if (callRes != null && callRes.ActiveMock)
            {
                result = callRes.Response;
                return true;
            }

            result = null;
            return false;
        }

        public void LastResult(InvokeCommand invokeCommad, object result)
        {
            string jsonArgs = JsonConvert.SerializeObject(invokeCommad.Args);

            var callRes = _calls.FirstOrDefault(p => p.WrappedService == invokeCommad.WrappedType.FullName
                                                         && p.Method == invokeCommad.Binder.Name
                                                         && p.Args == jsonArgs);

            if (callRes != null)
                return;

            _calls.Add(new MethodCall()
            {
                Id = Guid.NewGuid(),
                WrappedService = invokeCommad.WrappedType.FullName,
                Method = invokeCommad.Binder.Name,
                Response = result,
                Args = jsonArgs
            });

            Save();
        }

        private void Save()
        {
            string res = JsonConvert.SerializeObject(_calls, Formatting.Indented, _jsonSettigs);

            File.WriteAllText(_filePath, res);
        }

        public Task<IEnumerable<Interface>> GetInterfaces()
        {
            return Task.FromResult(_calls.GroupBy(p => p.WrappedService).Select(p => new Interface()
            {
                WrappedService = p.Key,
                ActiveMocks = p.Count()
            }));
        }

        public Task<IEnumerable<ServiceMethodInfo>> GetServiceMethods(string service)
        {
            return Task.FromResult(_calls.Where(p => p.WrappedService == service).Select(p =>
                    new ServiceMethodInfo(p.Id, p.Method, p.Args, JsonConvert.SerializeObject(p.Response, _jsonSettigs), p.ActiveMock)
                )
                .OrderBy(p => p.Method)
                .ThenBy(p => p.Id)
                .AsEnumerable()
            );
        }

        public Task Save(ServiceMethodInfo info)
        {
            var call = _calls.Single(p => p.Id == info.Id);

            call.Args = info.Args;
            call.Method = info.Method;
            call.Response = info.Response;
            call.ActiveMock = info.ActiveMock;

            Save();

            return Task.CompletedTask;
        }
    }

    internal class MethodCall
    {
        public string WrappedService { get; set; }

        public string Method { get; set; }

        public string Args { get; set; }

        public object Response { get; set; }

        public bool ActiveMock { get; set; }
        public Guid Id { get; set; }
    }

}