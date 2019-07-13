using System;

namespace ProxyWrapper.Contract
{
        public class ServiceMethodInfo
        {
            public Guid Id { get; }
            public string Method { get; }
            public string Args { get; }
            public string Response { get; }
            public bool ActiveMock { get; }

            public ServiceMethodInfo(Guid id, string method, string args, string response, bool activeMock)
            {
                Id = id;
                Method = method;
                Args = args;
                Response = response;
                ActiveMock = activeMock;
            }
        }
}