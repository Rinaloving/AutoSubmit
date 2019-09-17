using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit
{
    /// <summary>
    /// 数据库提供加载驱动接口，该接口继承IDisposable，用于释放对象占用的内存。
    /// 该接口定义了数据库链接语句，链接对象，执行命令，适配器模式接口。
    /// 同时还提供了打开数据库连接和关闭的方法，还有三个方法是用于控制数据库操作事务
    /// </summary>
    public interface IDbProvider : IDisposable
    {
        /// <summary>
        /// 数据库链接语句
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        IDbConnection Connection { get; set; }

        /// <summary>
        /// 数据库操作命令
        /// </summary>
        IDbCommand Command { get; set; }

        /// <summary>
        /// 数据库操作适配器
        /// </summary>
        IDbDataAdapter Adapter { get; set; }

        /// <summary>
        /// 打开数据库连接方法
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭数据库连接方法
        /// </summary>
        void Close();

        /// <summary>
        /// 开始事务控制方法
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 事务回滚方法
        /// </summary>
        void RollBack();

        /// <summary>
        /// 事务提交方法
        /// </summary>
        void Commit();
    }
}
