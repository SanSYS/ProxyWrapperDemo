using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Newtonsoft.Json;

namespace ProxyWrapper
{
    public interface IProxyWrapperStorage
    {
        bool Invoke(InvokeMemberBinder binder, object[] args, out object result);
        void LastResult(InvokeMemberBinder binder, object[] args, object result);
    }

    public class ProxyWrapperStorage : IProxyWrapperStorage
    {
        private readonly List<MethodCall> _callsResults;

        public ProxyWrapperStorage()
        {
            _callsResults = new List<MethodCall>();

            if (File.Exists("mocks.json"))
            {
                string cache = File.ReadAllText("mocks.json");

                if (!string.IsNullOrWhiteSpace(cache))
                {
                    _callsResults = JsonConvert.DeserializeObject<List<MethodCall>>(cache, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                }
            }

        }
        
        public bool Invoke(InvokeMemberBinder binder, object[] args, out object result)
        {
            string jsonArgs = JsonConvert.SerializeObject(args);
            
            foreach (var callsResult in _callsResults)
            {
                if (callsResult.Name == binder.Name && callsResult.Args == jsonArgs)
                {
                    if (callsResult.ActiveMock)
                    {
                        result = callsResult.Response;
                        return true;
                    }
                }
            }
            
            result = null;
            return false;
        }

        public void LastResult(InvokeMemberBinder binder, object[] args, object result)
        {
            string jsonArgs = JsonConvert.SerializeObject(args);
            
            foreach (var callsResult in _callsResults)
            {
                if (callsResult.Name == binder.Name && callsResult.Args == jsonArgs)
                {
                    return;
                }
            }
            
            _callsResults.Add(new MethodCall()
            {
                Name = binder.Name,
                Response = result,
                Args = jsonArgs
            });
            
            File.WriteAllText("mocks.json", JsonConvert.SerializeObject(_callsResults, Formatting.Indented, new JsonSerializerSettings(){ TypeNameHandling = TypeNameHandling.Auto}));
        }
    }

    public class MethodCall
    {
        public string Name { get; set; }

        public string Args { get; set; }

        public object Response { get; set; }

        public bool ActiveMock { get; set; }
    }
}