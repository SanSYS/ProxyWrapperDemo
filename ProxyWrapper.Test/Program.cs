using System;
using System.Collections.Generic;
using System.Linq;
using ProxyWrapper.Test.Demo;

namespace ProxyWrapper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IProxyWrapperStorage storage =
                //new ProxyWrapperFileStorage("mock.json");
                new ProxyWrapperPostgres("server=localhost;port=6432;userid=postgres;database=surrogatesdb;Pooling=false");

            ISomeService<Filter, Filter> service = new ConcreteService();

            service = ProxyWrapper<ISomeService<Filter, Filter>>.Wrap(service, storage);

            service.VoidWork();

            Console.WriteLine(service.GetString(3));
            Console.WriteLine(service.GetString(1));

            Console.WriteLine(service.GetResultObject(new Filter(){ Limit = 4, Name = "name1"}).Name);

            service.GetResultObjects(new List<Filter>
                {
                    new Filter() {Limit = 1, Name = "name1"},
                    new Filter() {Limit = 1, Name = "name2"}
                })
                .Select(p => {
                    Console.WriteLine(p.Name);
                    return p;
                }).ToList();
        }
    }
}