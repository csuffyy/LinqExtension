using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtension
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start:");

            try
            {
                Test();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        private static void Test()
        {
            var users = Enumerable.Range(0, 100).Select((x, i) => new User
            {
                Name = "lzy" + i,
                Age = 18 + i,
                RegisteredTime = DateTimeOffset.Now.AddDays(i)
            });

            //C#6.0
            var contirions = new Dictionary<string, List<string>>()
            {
                ["Name"] = new List<string> {"lzy9"},
                ["Age"] = new List<string> {"<110", ">100"},
                ["RegisteredTime"] = new List<string> {">=2016/01/18"},
            };

            var query = users.AsQueryable().Query(contirions);

            foreach (var user in query)
            {
                Console.WriteLine(user);
            }
        }
    }
}
