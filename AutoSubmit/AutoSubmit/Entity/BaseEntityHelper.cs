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
    /// 该类是抽象类BaseEntityAbstract 的子类，它实现了抽象类BaseEntityAbstract 的所有抽象方法。
    /// </summary>
    public class BaseEntityHelper : BaseEntityAbstract
    {
        private const int LENGTH = 1;

        /// <summary>
        /// 根据实体的类型获得实体表信息
        /// </summary>
        /// <param name="type">实体类类型</param>
        /// <returns></returns>
        public override TableInfo GetTableInfo(Type type)
        {
            TableInfo tableInfo = EntityTypeCache.GetTableInfo(type);
            if (tableInfo == null)
            {
                tableInfo = new TableInfo();
                TableAttribute[] tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), false) as TableAttribute[];

                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    tableInfo.DicProperties.Add(property.Name, property);
                    if (property.GetCustomAttributes(typeof(ColumnAttribute), false).Length == LENGTH)
                    {
                        tableInfo.DicColumns.Add(property.Name, property.GetCustomAttributes(typeof(ColumnAttribute), false)[0] as ColumnAttribute);
                    }
                    if (property.GetCustomAttributes(typeof(RelationAttribute), false).Length == LENGTH)
                    {
                        tableInfo.DicLinkTable.Add(property.Name, property.GetCustomAttributes(typeof(RelationAttribute), false)[0] as RelationAttribute);
                    }
                    if (property.GetCustomAttributes(typeof(RelationsAttribute), false).Length == LENGTH)
                    {
                        tableInfo.DicLinkTables.Add(property.Name, property.GetCustomAttributes(typeof(RelationsAttribute), false)[0] as RelationsAttribute);
                    }
                }
                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo field in fields)
                {
                    tableInfo.DicFields.Add(field.Name, field);
                }

                if (tableAttribute.Length == LENGTH)
                {
                    tableInfo.Table = tableAttribute[0];
                }
                else
                {
                    throw new Exception("一个实体类上不能有相同的特性");
                }
                tableInfo.Columns = tableInfo.DicColumns.Values.ToArray();
                tableInfo.Fields = tableInfo.DicFields.Values.ToArray();
                tableInfo.LinkTable = tableInfo.DicLinkTable.Values.ToArray();
                tableInfo.LinkTables = tableInfo.DicLinkTables.Values.ToArray();
                tableInfo.Properties = tableInfo.DicProperties.Values.ToArray();
                EntityTypeCache.InsertTableInfo(type, tableInfo);
            }
            return tableInfo;
        }


        /// <summary>
        /// 根据实体类的公共接口获得实体表信息
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <returns></returns>
        public override TableInfo GetTableInfo(IEntity entity)
        {
            return GetTableInfo(entity.GetType());
        }

        /// <summary>
        /// 根据实体泛型类型获得实体表信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public override TableInfo GetTableInfo<T>()
        {
            Type type = typeof(T);
            return GetTableInfo(type);
        }

        /// <summary>
        /// 根据实体的类型获得实体表列信息
        /// </summary>
        /// <param name="type">实体类类型</param>
        /// <returns></returns>
        public override ColumnAttribute[] GetColumnAttribute(Type type)
        {
            return GetTableInfo(type).Columns;
        }


        /// <summary>
        /// 根据实体类的公共接口获得实体表列信息
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <returns></returns>
        public override ColumnAttribute[] GetColumnAttribute(IEntity entity)
        {
            return GetTableInfo(entity).Columns;
        }

        /// <summary>
        /// 根据实体泛型类型获得实体表列信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public override ColumnAttribute[] GetColumnAttribute<T>()
        {
            return GetTableInfo<T>().Columns;
        }

        /// <summary>
        /// 根据实体的类型获得实体字段信息
        /// </summary>
        /// <param name="type">实体类类型</param>
        /// <returns></returns>
        public override FieldInfo[] GetFieldInfo(Type type)
        {
            return GetTableInfo(type).Fields;
        }


        /// <summary>
        /// 根据实体类的公共接口获得实体字段信息
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <returns></returns>
        public override FieldInfo[] GetFieldInfo(IEntity entity)
        {
            return GetTableInfo(entity).Fields;
        }

        /// <summary>
        /// 根据实体泛型类型获得实体字段信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public override FieldInfo[] GetFieldInfo<T>()
        {
            return GetTableInfo<T>().Fields;
        }

        /// <summary>
        /// 根据实体的类型获得实体属性信息
        /// </summary>
        /// <param name="type">实体类类型</param>
        /// <returns></returns>
        public override PropertyInfo[] GetPropertyInfo(Type type)
        {
            return GetTableInfo(type).Properties;
        }

        /// <summary>
        /// 根据实体类的公共接口获得实体属性信息
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <returns></returns>
        public override PropertyInfo[] GetPropertyInfo(IEntity entity)
        {
            return GetTableInfo(entity).Properties;
        }

        /// <summary>
        /// 根据实体泛型类型获得实体属性信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public override PropertyInfo[] GetPropertyInfo<T>()
        {
            return GetTableInfo<T>().Properties;
        }

        /// <summary>
        /// 根据实体的类型获得实体字段属性信息
        /// </summary>
        /// <param name="type">实体类类型</param>
        /// <returns></returns>
        public override RelationAttribute[] GetLinkTableAttribute(Type type)
        {
            return GetTableInfo(type).LinkTable;
        }

        /// <summary>
        /// 根据实体类的公共接口获得实体字段属性信息
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <returns></returns>
        public override RelationAttribute[] GetLinkTableAttribute(IEntity entity)
        {
            return GetTableInfo(entity).LinkTable;
        }

        /// <summary>
        /// 根据实体泛型类型获得实体字段属性信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public override RelationAttribute[] GetLinkTableAttribute<T>()
        {
            return GetTableInfo<T>().LinkTable;
        }

        /// <summary>
        /// 根据实体的类型获得实体字段集合属性信息
        /// </summary>
        /// <param name="type">实体类类型</param>
        /// <returns></returns>
        public override RelationsAttribute[] GetLinkTablesAttribute(Type type)
        {
            return GetTableInfo(type).LinkTables;
        }

        /// <summary>
        /// 根据实体类的公共接口获得实体字段集合属性信息
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <returns></returns>
        public override RelationsAttribute[] GetLinkTablesAttribute(IEntity entity)
        {
            return GetTableInfo(entity).LinkTables;
        }

        /// <summary>
        /// 根据实体的类型获得实体字段集合属性信息
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        public override RelationsAttribute[] GetLinkTablesAttribute<T>()
        {
            return GetTableInfo<T>().LinkTables;
        }
    }
}
