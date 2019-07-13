using System.Collections.Generic;

namespace ProxyWrapper.Test.Demo
{
    public interface ISomeService
    {
        string GetString(int id);

        void VoidWork();

        Result GetResultObject(Filter filter);

        List<Result> GetResultObjects(List<Filter> filter);
    }

    public class Filter
    {
        public int Limit { get; set; }

        public string Name { get; set; }
    }

    public class Result
    {
        public string Name { get; set; }
    }
}