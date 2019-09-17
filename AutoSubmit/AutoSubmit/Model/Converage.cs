using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// 该类是一个枚举类型，定义了数据库聚合函数操作的各种情况。该枚举值可以在使用时候来区分sql执行那个聚合函数
    /// </summary>
    public enum Converage
    {
        /// <summary>
        /// 聚合函数取最小值
        /// </summary>
        Min,

        /// <summary>
        /// 聚合函数取最大值
        /// </summary>
        Max,

        /// <summary>
        /// 聚合函数取和
        /// </summary>
        Sum,

        /// <summary>
        /// 聚合函数取所有数据行
        /// </summary>
        Count,

        /// <summary>
        /// 聚合函数取所有非空数据行
        /// </summary>
        CountNotNll,

        /// <summary>
        /// 聚合函数取平均值
        /// </summary>
        Avg,
    }
}
