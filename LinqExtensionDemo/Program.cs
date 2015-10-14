using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqExtension
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                //Console.WriteLine("Start Test1:");
                //Test1();
                //Console.WriteLine();

                //Console.WriteLine("Start Test2:");
                //Test2();
                //Console.WriteLine();

                //Console.WriteLine("Start Test3:");
                //Test3();
                //Console.WriteLine();

                Console.WriteLine("Start Test4:");
                Test4();
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        private static void Test1()
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

        private static void Test2()
        {
            var items = Enumerable.Range(-5, 10);

            //奇数
            ISpec<int> oddSpec = new Specification<int>(it => it % 2 != 0);
            //正数
            ISpec<int> positiveSpec = new Specification<int>(it => it > 0);

            var spec = oddSpec.Or(positiveSpec);
            //var spec = oddSpec.Add(positiveSpec);

            foreach (var i in items.Where(it => spec.IsSatisfiedBy(it)))
            {
                Console.WriteLine(i);
            }
        }

        private static void Test3()
        {
            var items = Enumerable.Range(-5, 10);

            //奇数
            Spec<int> oddSpec = it => it % 2 != 0;
            //正数
            Spec<int> positiveSpec = it => it > 0;

            var spec = oddSpec.Or(positiveSpec);

            foreach (var i in items.Where(it => spec(it)))
            {
                Console.WriteLine(i);
            }
        }

        private static void Test4()
        {
            var items = Enumerable.Range(-5, 10);

            //奇数
            Func<int, bool> oddSpec = it => it % 2 != 0;
            //正数
            Func<int, bool> positiveSpec = it => it > 0;

            var spec = oddSpec.Or(positiveSpec);

            foreach (var i in items.Where(it => spec(it)))
            {
                Console.WriteLine(i);
            }
        }
    }
}