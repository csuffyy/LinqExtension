using System;

namespace LinqExtension
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; } = "lzy";

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; } = 18;

        /// <summary>
        /// 是否被禁用
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTimeOffset RegisteredTime { get; set; } = DateTimeOffset.Now;

        public override string ToString()
        {
            return $"{Name}-{Age}-{RegisteredTime}";
        }
    }
}