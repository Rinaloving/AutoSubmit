using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// 实体集合属性特性，用于描述某个实体作为另外一个实体的一个集合属性的时候与数据库表之间的关系
    /// </summary>
    public sealed class RelationsAttribute : RelationAttribute
    {
        /// <summary>
        /// 无参数构造方法
        /// </summary>
        public RelationsAttribute()
        {
        }

        /// <summary>
        /// 部分参数构造方法，构造该特性实例的时候，
        /// 初始化部分属性，并且调用的是父类的构造
        /// 方法
        /// </summary>
        /// <param name="name">该实体属性对应的数据库表名</param>
        /// <param name="dataType">该实体属性对应的数据类型</param>
        /// <param name="keyName">该实体对应的主键类型</param>
        /// <param name="className">对应的实体的全路径类型</param>
        public RelationsAttribute(string name, Type dataType, string keyName, string className)
            : base(name, dataType, keyName, className)
        { }


        /// <summary>
        /// 全参数构造方法，构造该特性实例的时候
        /// 初始化全部属性 ，并且调用的是父类的构造
        /// 方法
        /// </summary>
        /// <param name="name">该实体属性对应的数据库表名</param>
        /// <param name="sqlPrefix">该实体属性对应的数据库表名前缀</param>
        /// <param name="dataType">该实体属性对应的数据类型</param>
        /// <param name="keyName">该实体对应的主键类型</param>
        /// <param name="className">对应的实体的全路径类型</param>
        public RelationsAttribute(string name, string sqlPrefix, Type dataType, string keyName, string className)
            : base(name, sqlPrefix, dataType, keyName, className)
        {

        }
    }
}
