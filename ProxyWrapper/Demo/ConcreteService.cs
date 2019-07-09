using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyWrapper.Demo
{
    public class ConcreteService:ISomeService
    {
        public string GetString(int id)
        {
            return "concrete" + id;
        }

        public void VoidWork()
        {
            Console.WriteLine("concrete void work");
        }

        public Result GetResultObject(Filter filter)
        {
            return new Result
            {
                Name = "concrete " + filter.Name
            };
        }

        public List<Result> GetResultObjects(List<Filter> filter)
        {
            return filter
                .Select(p => new Result()
                {
                    Name = $"concrete {p.Limit} {p.Name}"
                })
                .ToList();
        }
    }
}