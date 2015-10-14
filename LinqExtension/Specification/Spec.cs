using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtension
{
    /// <summary>
    /// 规约委托
    /// </summary>
    /// <typeparam name="T">实体类型约束</typeparam>
    /// <param name="candidate">约束对象</param>
    /// <returns>约束对象是否符合规约</returns>
    public delegate bool Spec<in T>(T candidate);
}