using AutoSubmit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Entity
{
    /// <summary>
    /// 该实体类主要封装了实体模型类的一些特新信息和字段信息该实体封的信息包括该实体类的表特性对象，
    /// 所有该实体对象的属性和字段。还包括实体属性特性和实体属性集合特性
    /// </summary>
    [Serializable]
    public class TableInfo : BaseEntity
    {
        /// <summary>
        /// 表实体特性信息
        /// </summary>
        private TableAttribute table;

        /// <summary>
        /// 表实体对应数据字段特性信息
        /// </summary>
        private ColumnAttribute[] columns;

        /// <summary>
        /// 表实体对应数据字段特性信息键值存储
        /// </summary>
        private IDictionary<string, ColumnAttribute> dicColumns = new Dictionary<string, ColumnAttribute>();

        /// <summary>
        /// 实体模型字段信息
        /// </summary>
        private FieldInfo[] fields;

        /// <summary>
        /// 实体模型字段信息键值存储
        /// </summary>
        private IDictionary<string, FieldInfo> dicFields = new Dictionary<string, FieldInfo>();

        /// <summary>
        /// 实体模型属性信息
        /// </summary>
        private PropertyInfo[] properties;

        /// <summary>
        /// 实体模型属性信息键值存储
        /// </summary>
        private IDictionary<string, PropertyInfo> dicProperties = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// 表实体 实体特性信息
        /// </summary>
        private RelationAttribute[] linkTable;

        /// <summary>
        /// 表实体 实体特性信息键值存储
        /// </summary>
        private IDictionary<string, RelationAttribute> dicLinkTable = new Dictionary<string, RelationAttribute>();

        /// <summary>
        /// 表实体 实体集合特性信息
        /// </summary>
        private RelationsAttribute[] linkTables;

        /// <summary>
        /// 表实体 实体集合特性信息键值存储
        /// </summary>
        private IDictionary<string, RelationsAttribute> dicLinkTables = new Dictionary<string, RelationsAttribute>();

        /// <summary>
        /// 无参数构造方法
        /// </summary>
        public TableInfo()
        {
        }

        /// <summary>
        /// 全字段构造方法，构造该对象实例的时候
        /// 初始化所有属性
        /// </summary>
        /// <param name="table">表实体特性信息</param>
        /// <param name="columns">表实体对应数据字段特性信息</param>
        /// <param name="fields">实体模型字段信息</param>
        /// <param name="properties">实体模型属性信息</param>
        /// <param name="linkTable">表实体 实体特性信息</param>
        /// <param name="linkTables">表实体 实体集合特性信息</param>
        public TableInfo(TableAttribute table,
            ColumnAttribute[] columns,
            FieldInfo[] fields,
            PropertyInfo[] properties,
            RelationAttribute[] linkTable,
            RelationsAttribute[] linkTables)
        {
            this.table = table;
            this.columns = columns;
            this.fields = fields;
            this.properties = properties;
            this.linkTable = linkTable;
            this.linkTables = linkTables;
        }

        /// <summary>
        /// 表
        /// </summary>
        public TableAttribute Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// 列
        /// </summary>
        public ColumnAttribute[] Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        /// <summary>
        /// 字段
        /// </summary>
        public FieldInfo[] Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo[] Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        /// 关系链接表
        /// </summary>
        public RelationAttribute[] LinkTable
        {
            get { return linkTable; }
            set { linkTable = value; }
        }

        /// <summary>
        /// 关系链接表
        /// </summary>
        public RelationsAttribute[] LinkTables
        {
            get { return linkTables; }
            set { linkTables = value; }
        }
        /// <summary>
        /// 关系链接字段
        /// </summary>
        public IDictionary<string, ColumnAttribute> DicColumns
        {
            get { return dicColumns; }
            set { dicColumns = value; }
        }

        /// <summary>
        /// 关系链接字段
        /// </summary>
        public IDictionary<string, FieldInfo> DicFields
        {
            get { return dicFields; }
            set { dicFields = value; }
        }

        /// <summary>
        /// 关系链接属性
        /// </summary>
        public IDictionary<string, PropertyInfo> DicProperties
        {
            get { return dicProperties; }
            set { dicProperties = value; }
        }

        /// <summary>
        /// 关系链接表
        /// </summary>
        public IDictionary<string, RelationAttribute> DicLinkTable
        {
            get { return dicLinkTable; }
            set { dicLinkTable = value; }
        }

        /// <summary>
        /// 关系链接表
        /// </summary>
        public IDictionary<string, RelationsAttribute> DicLinkTables
        {
            get { return dicLinkTables; }
            set { dicLinkTables = value; }
        }
    }
}
