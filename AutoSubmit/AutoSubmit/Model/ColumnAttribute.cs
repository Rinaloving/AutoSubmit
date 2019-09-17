using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// 实体表特性，用于描述实体字段和数据库表字段之间的映射关系
    /// </summary>
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 表字段名称，该属性的值最好与数据表的字段的名称相同。
        /// 该字段的值有两种格式:
        /// (1) [表名].[字段名] 
        /// (2) [字段名]
        /// 如果该字段的值为(1)情况，则应分割字符串，将字段名
        /// 赋值给name属性，表名则赋值给tableName
        /// </summary>
        private string fieldname;

        /// <summary>
        /// 表字段对应表名称
        /// 该值是可以为空的，如果name的值的情况满足(1)情况，
        /// 可以分割的值赋值给该属性
        /// </summary>
        private string tableName;

        /// <summary>
        /// 表字段的数据类型
        /// 该属性的类型为自定义类型，该字段是一个枚举类型。
        /// 该字段描述了25中数据类型
        /// </summary>
        private String fieldType;

        /// <summary>
        /// 表字段的长度
        /// 控制该字段对应的数据库表字段值的最大长度
        /// 可以不指定该值
        /// </summary>
        private int length;

        /// <summary>
        /// 表字段是否可以为空
        /// true 可以为空
        /// false 不能为空
        /// </summary>
        private bool nullable;

        /// <summary>
        /// 表字段的默认值
        /// 默认情况为null
        /// </summary>
        private object defaultValue;

        /// <summary>
        /// 表字段是否为主键
        /// true  为主键
        /// false 不是外键
        /// </summary>
        private bool isPrimaryKey = false;

        /// <summary>
        /// 表字段是否为自动增长列
        /// true  是自动增长列
        /// false 不是自动增长列
        /// </summary>
        private bool autoIncrement = false;

        /// <summary>
        /// 确定某个字段是否唯一
        /// true  是唯一的
        /// false 不是唯一
        /// </summary>
        private bool isUnique;

        /// <summary>
        /// 表字段的匹配规则
        /// 字段匹配规则正则表达式
        /// </summary>
        private string regularExpress;

        /// <summary>
        /// 表字段是否为外键
        /// true  为外键
        /// false 不是外键
        /// </summary>
        private bool isForeignKey = false;

        /// <summary>
        /// 表字段外键对应的表名称
        /// 如果isForeignKey 为true,则需要指定其值
        /// </summary>
        private string foreignTabName;

        /// <summary>
        /// 表字段的描述信息
        /// </summary>
        private string description;


        /// <summary>
        /// 无参数构造方法
        /// </summary>
        public ColumnAttribute()
        {
        }

        /// <summary>
        /// 部分参数构造方法，构造该特性实例时
        /// 初始化部分实体属性的值
        /// </summary>
        /// <param name="name">表字段名称</param>
        /// <param name="dataType">表字段的数据类型</param>
        /// <param name="isPrimaryKey">表字段是否为主键</param>
        /// <param name="autoIncrement">表字段是否为自动增长列</param>
        /// <param name="isUnique">确定某个字段是否唯一</param>
        /// <param name="isForeignKey">表字段是否为外键</param>
        /// <param name="foreignTabName">表字段外键对应的表名称</param>
        public ColumnAttribute(string name,
            String dataType,
            bool isPrimaryKey,
            bool autoIncrement,
            bool isUnique,
            bool isForeignKey,
            string foreignTabName)
        {
            this.fieldname = name;
            this.fieldType = dataType;
            this.isPrimaryKey = isPrimaryKey;
            this.autoIncrement = autoIncrement;
            this.isUnique = isUnique;
            this.isForeignKey = isForeignKey;
            this.foreignTabName = foreignTabName;
        }

        /// <summary>
        /// 全部参数构造方法，构造该特性实例时
        /// 初始化全部实体属性的值
        /// </summary>
        /// <param name="name">表字段名称</param>
        /// <param name="tableName">表字段对应表名称</param>
        /// <param name="dataType">表字段的数据类型</param>
        /// <param name="length">表字段的长度</param>
        /// <param name="canNull">表字段是否可以为空</param>
        /// <param name="defaultValue">表字段的默认值</param>
        /// <param name="isPrimaryKey">表字段是否为主键</param>
        /// <param name="autoIncrement">表字段是否为自动增长列</param>
        /// <param name="isUnique">确定某个字段是否唯一</param>
        /// <param name="regularExpress">表字段的匹配规则</param>
        /// <param name="isForeignKey">表字段是否为外键</param>
        /// <param name="foreignTabName">表字段外键对应的表名称</param>
        /// <param name="information">表字段的描述信息</param>
        public ColumnAttribute(string name,
            string tableName,
            String dataType,
            int length,
            bool canNull,
            object defaultValue,
            bool isPrimaryKey,
            bool autoIncrement,
            bool isUnique,
            string regularExpress,
            bool isForeignKey,
            string foreignTabName,
            string information)
        {
            this.fieldname = name;
            this.tableName = tableName;
            this.fieldType = dataType;
            this.length = length;
            this.nullable = canNull;
            this.defaultValue = defaultValue;
            this.isPrimaryKey = isPrimaryKey;
            this.autoIncrement = autoIncrement;
            this.regularExpress = regularExpress;
            this.isForeignKey = isForeignKey;
            this.ForeignTableName = foreignTabName;
            this.description = information;
            this.isUnique = isUnique;
        }

        /// <summary>
        /// 表字段名称
        /// </summary>
        public string FieldName
        {
            get { return fieldname; }
            set { fieldname = value; }
        }

        /// <summary>
        /// 表字段对应表名称
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// 表字段的数据类型
        /// </summary>
        public String FieldType
        {
            get { return fieldType; }
            set { fieldType = value; }
        }

        /// <summary>
        /// 表字段的长度
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// 表字段是否可以为空
        /// </summary>
        public bool Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        /// <summary>
        /// 表字段的默认值
        /// </summary>
        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// 表字段是否为主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; }
        }

        /// <summary>
        /// 表字段是否为自动增长列
        /// </summary>
        public bool AutoIncrement
        {
            get { return autoIncrement; }
            set { autoIncrement = value; }
        }

        /// <summary>
        /// 表字段的匹配规则
        /// </summary>
        public string RegularExpress
        {
            get { return regularExpress; }
            set { regularExpress = value; }
        }

        /// <summary>
        /// 表字段是否为外键
        /// </summary>
        public bool IsForeignKey
        {
            get { return isForeignKey; }
            set { isForeignKey = value; }
        }

        /// <summary>
        /// 表字段外键对应的表名称
        /// </summary>
        public string ForeignTableName
        {
            get { return foreignTabName; }
            set { foreignTabName = value; }
        }

        /// <summary>
        /// 表字段的描述信息
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// 确定某个字段是否唯一
        /// </summary>
        public bool IsUnique
        {
            get { return isUnique; }
            set { isUnique = value; }
        }
    }
}
