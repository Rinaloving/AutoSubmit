using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    ///  实体表特性，用于描述实体和数据库表之间的映射关系
    /// </summary>
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// 数据表名
        /// 该字段用于描述数据库对应表的名称,而且该值最好与
        /// 数据表名大小写相同。该值有两种类型。
        /// (1)直接自定表的名称
        /// (2)[数据库名].[表名]
        /// 如果是(2)情况，则需要分割字符串,将数据库名分割
        /// 出来赋值给dBName
        /// </summary>
        private string tablename;

        /// <summary>
        /// 数据库名
        /// 该字段用于描述数据的名称，而且该值最好与
        /// 数据库名称大小写相同
        /// </summary>
        private string databaseName;

        /// <summary>
        /// 主键字段名
        /// 该实体必须指定对应数据库表的主键
        /// </summary>
        private string primaryKey = "";

        /// <summary>
        /// 表实体描述信息
        /// 默认为""
        /// </summary>
        private string description = "";


        /// <summary>
        /// 无参数构造方法
        /// </summary>
        public TableAttribute()
        {
        }

        /// <summary>
        /// 部分参数构造方法,构造改表特性的实体类
        /// 并实现部分字段的初始化
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <param name="dBName">数据库名</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="isInternal">表实体是否国际化</param>
        public TableAttribute(string name,
            string dBName,
            string primaryKeyName,
            bool isInternal)
        {
            this.tablename = name;
            this.databaseName = dBName;
            this.primaryKey = primaryKeyName;
        }

        /// <summary>
        /// 全参数构造方法，构造改表特性的实体类
        /// 并实现所有字段的初始化
        /// </summary>
        /// <param name="name">数据表名</param>
        /// <param name="dBName">数据库名</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="information">表实体描述信息</param>
        /// <param name="isInternal">表实体是否国际化</param>
        /// <param name="version">表实体版本号</param>
        public TableAttribute(string name,
            string dBName,
            string primaryKeyName,
            string information,
            bool isInternal,
            string version)
        {
            this.tablename = name;
            this.databaseName = dBName;
            this.primaryKey = primaryKeyName;
            this.description = information;
        }

        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName
        {
            get { return tablename; }
            set { tablename = value; }
        }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// 主键字段名
        /// </summary>
        public string PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        /// <summary>
        /// 表实体描述信息
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
