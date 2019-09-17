using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// 实体属性特性，用于描述某个实体作为另外一个实体的一个属性字段的时候与数据库表之间的关系
    /// </summary>
    public class RelationAttribute : Attribute
    {
        /// <summary>
        /// 该实体属性对应的数据库表名
        /// </summary>
        private string name;

        /// <summary>
        /// 该实体属性对应的数据库表名前缀
        /// </summary>
        private string sqlPrefix;

        /// <summary>
        /// 该实体属性对应的数据类型
        /// 可以通过反射获得类型
        /// </summary>
        private Type dataType;

        /// <summary>
        /// 该实体对应的主键类型
        /// (用数据的列名来表示,不要使用实体的属性来定义)
        /// </summary>
        private string keyName;

        /// <summary>
        /// 对应的实体的全路径类型
        /// </summary>
        private string className;

        /// <summary>
        /// 对应的实体是否延长加载
        /// </summary>
        private bool isLazy;

        /// <summary>
        /// 无参数构造方法
        /// </summary>
        public RelationAttribute()
        {
        }

        /// <summary>
        /// 部分参数构造方法，构造该特性实例的时候
        /// 初始化部分属性
        /// </summary>
        /// <param name="name">该实体属性对应的数据库表名</param>
        /// <param name="dataType">该实体属性对应的数据类型</param>
        /// <param name="keyName">该实体对应的主键类型</param>
        /// <param name="className">对应的实体的全路径类型</param>
        public RelationAttribute(string name, Type dataType, string keyName, string className)
        {
            this.name = name;
            this.dataType = dataType;
            this.keyName = keyName;
            this.className = className;
        }

        /// <summary>
        /// 全参数构造方法，构造该特性实例的时候
        /// 初始化全部属性
        /// </summary>
        /// <param name="name">该实体属性对应的数据库表名</param>
        /// <param name="sqlPrefix">该实体属性对应的数据库表名前缀</param>
        /// <param name="dataType">该实体属性对应的数据类型</param>
        /// <param name="keyName">该实体对应的主键类型</param>
        /// <param name="className">对应的实体的全路径类型</param>
        public RelationAttribute(string name, string sqlPrefix, Type dataType, string keyName, string className)
        {
            this.name = name;
            this.sqlPrefix = sqlPrefix;
            this.dataType = dataType;
            this.keyName = keyName;
            this.className = className;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public string SqlPrefix
        {
            get { return sqlPrefix; }
            set { sqlPrefix = value; }
        }


        public Type DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }


        public string KeyName
        {
            get { return keyName; }
            set { keyName = value; }
        }


        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public bool IsLazy
        {
            get { return isLazy; }
            set { isLazy = value; }
        }
    }
}
