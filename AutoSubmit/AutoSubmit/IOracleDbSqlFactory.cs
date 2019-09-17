using AutoSubmit.Entity;
using AutoSubmit.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit
{
    /// <summary>
    /// IDbFactory 接口实现了IDisposable 接口，实现该接口的类可以通过IDisposable 接口来释放对象占用的内存。
    /// 该接口的主要作用适用于根据实体对象来创建SQL Server 数据库脚本。
    /// ORM 的主要作用是为了实现对象化操作数据库，而此操作就是为了实现对象化操作和数据库语句操作的过渡
    /// </summary>
    public interface IOracleDbSqlFactory : IDisposable
    {

        /// <summary>
        /// 根据实体对象公共接口创建插入的sql语句
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateInsertSql(IEntity entity, out IDataParameter[] param);

        /// <summary>
        /// 根据实体类型创建插入sql语句
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateInsertSql(Type type, object value, out IDataParameter[] param);

        /// <summary>
        /// 根据实体类型创建插入sql语句
        /// </summary>
        /// <param name="whereclause">条件语句</param>
        /// <returns></returns>
        string CreateDeleteSql(Type type, string whereclause);

        /// <summary>
        /// 根据泛型类型创建插入sql语句
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="t">泛型实体类</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateInsertSql<T>(T t, out IDataParameter[] param) where T : IEntity;

        /// <summary>
        /// 根据泛型类型创建删除sql语句
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns></returns>
        string CreateDeleteAllSql<T>() where T : class;

        /// <summary>
        /// 根据实体对象公共接口创建修改的的sql语句
        /// 该sql语句是根据主键列修改的
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateUpdateSql(IEntity entity, out IDataParameter[] param);

        /// <summary>
        /// 根据实体对象类型创建修改的的sql语句
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateUpdateSql(Type type, object value, out IDataParameter[] param);

        /// <summary>
        /// 根据实体对象公共接口创建修改的的sql语句
        /// 该sql语句是根据一个特定的属性作为修改条件的
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        string CreateUpdateSql(IEntity entity, out IDataParameter[] param, string propertyName);

        /// <summary>
        /// 根据实体对象类型创建修改的的sql语句
        /// 该sql语句是根据一个特定的属性作为修改条件的
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        string CreateUpdateSql(Type type, object value, out IDataParameter[] param, string propertyName);

        /// <summary>
        /// 根据实体对象公共接口创建修改的的sql语句
        /// 该sql语句是根据多个特定的属性作为修改条件的
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyNames">属性名称</param>
        /// <returns></returns>
        string CreateUpdateSql(IEntity entity, out IDataParameter[] param, string[] propertyNames);

        /// <summary>
        /// 根据实体对象类型创建修改的的sql语句
        /// 该sql语句是根据多个特定的属性作为修改条件的
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyNames">属性名称</param>
        /// <returns></returns>
        string CreateUpdateSql(Type type, object value, out IDataParameter[] param, string[] propertyNames);

        /// <summary>
        /// 根据实体对象公共接口创建修改的的sql语句
        /// 该sql语句是根据查询组建创建的
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="component">查询条件组件</param>
        /// <returns></returns>
        string CreateUpdateSql(IEntity entity, out IDataParameter[] param, ConditionComponent component);

        /// <summary>
        /// 根据实体对象公共接口创建删除sql语句
        /// 该sql语句是根据实体主键删除
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateDeleteSql(IEntity entity, out IDataParameter[] param);

        /// <summary>
        /// 根据实体对象类型创建删除sql语句
        /// 该sql语句是根据实体主键删除
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateDeleteSql(Type type, object value, out IDataParameter[] param);

        /// <summary>
        /// 根据实体对象公共接口的某个属性创建删除sql语句
        /// 该sql语句是根据实体属性删除
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyName">实体属性名称</param>
        /// <returns></returns>
        string CreateDeleteSql(IEntity entity, out IDataParameter[] param, string propertyName);

        /// <summary>
        /// 根据实体对象类型的某个属性创建删除sql语句
        /// 该sql语句是根据实体属性删除
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyName">实体属性名称</param>
        /// <returns></returns>
        string CreateDeleteSql(Type type, object value, out IDataParameter[] param, string propertyName);

        /// <summary>
        /// 根据实体对象公共接口的多个属性创建删除sql语句
        /// 该sql语句是根据实体多个属性删除
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyName">实体属性名称数组</param>
        /// <returns></returns>
        string CreateDeleteSql(IEntity entity, out IDataParameter[] param, string[] propertyNames);

        /// <summary>
        /// 根据实体对象类型的多个属性创建删除sql语句
        /// 该sql语句是根据实体多个属性删除
        /// </summary>
        /// <param name="type">实体了姓</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyNames">实体属性名称数组</param>
        /// <returns></returns>
        string CreateDeleteSql(Type type, object value, out IDataParameter[] param, string[] propertyNames);

        /// <summary>
        /// 根据实体对象公共接口的多个属性创建删除sql语句
        /// 该sql语句使根据查询组建来创建的
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="component">实体属性名称数组</param>
        /// <returns></returns>
        string CreateDeleteSql(IEntity entity, out IDataParameter[] param, ConditionComponent component);

        string CreateDeleteByPropertySql(Type type, IDictionary<string, object> dic, out IDataParameter[] param, ConditionComponent component);

        /// <summary>
        /// 根据实体的公共接口创建查询单行数据的sql语句
        /// 该sql语句是根据数据库表的主键来查询的
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateSingleSql(IEntity entity, out IDataParameter[] param);

        /// <summary>
        /// 根据实体的公共接口创建查询单行数据的sql语句
        /// 该sql语句是根据实体的相应属性来查询
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateSingleSql(IEntity entity, out IDataParameter[] param, string[] propertyNames);

        /// <summary>
        /// 根据实体类型创建查询单行数据的sql语句
        /// 该sql语句是根据实体的相应属性来查询
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="value">实体对象</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <param name="propertyNames">属性名称数组</param>
        /// <returns></returns>
        string CreateSingleSql(Type type, object value, out IDataParameter[] param, string[] propertyNames);

        /// <summary>
        /// 根据实体的类型创建查询sql语句
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns></returns>
        string CreateSingleSql(Type entityType);

        /// <summary>
        /// 根据实体的类型创建查询sql语句,
        /// 该方法指定主键值
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="pkPropertyValue">主键值</param>
        /// <param name="param">创建sql语句对应占位符参数</param>
        /// <returns></returns>
        string CreateSingleSql(Type type, object pkPropertyValue, out IDataParameter[] param);


        string CreateSingleSqlExcludeField(Type type, object pkPropertyValue, string fieldName, out IDataParameter[] param);

        /// <summary>
        /// 根据实体的类型创建查询该实体对象对应数据库表的所有数据的sql语句
        /// 该sql语句用于查询所有数据，并转换为相应List T 集合
        /// </summary>
        /// <param name="type">实体的类型</param>
        /// <returns></returns>
        string CreateQuerySql(Type type);

        /// <summary>
        /// 根据实体的类型创建查询该实体对象对应数据库表的所有数据的sql语句
        /// 该sql语句用于查询所有数据，并转换为相应List T 集合
        /// </summary>
        /// <param name="type">实体的类型</param>
        /// <returns></returns>
        string CreateQuerySql(Type type, string whereclause);

        /// <summary>
        /// 根据实体的某个属性创建根据该属性字段查询数据的sql语句
        /// 该sql语句是使用参数中属性对应字段作为条件查询的
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">属性值</param>
        /// <param name="param">sql语句占位符参数</param>
        /// <returns></returns>
        string CreateQueryByPropertySql(Type type, string propertyName, object value, out IDataParameter[] param);

        /// <summary>
        /// 根据实体的某些属性创建根据该些属性字段查询数据的sql语句
        /// 该sql语句是使用参数中属性对应字段作为条件查询的，并且该
        /// 属性集合都是根据and条件组装的
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="dic">属性-值集合</param>
        /// <param name="param">sql语句占位符参数</param>
        /// <returns></returns>
        string CreateQueryByPropertySql(Type type, IDictionary<string, object> dic, out IDataParameter[] param);

        /// <summary>
        /// 根据实体的某些属性创建根据该些属性字段查询数据的sql语句
        /// 该sql语句是使用参数中属性对应字段作为条件查询的，并且查
        /// 询是根据查询组建来创建
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="dic">属性-值集合</param>
        /// <param name="param">sql语句占位符参数</param>
        /// <param name="component">查询组建</param>
        /// <returns></returns>
        string CreateQueryByPropertySql(Type type, IDictionary<string, object> dic, out IDataParameter[] param, ConditionComponent component);

        /// <summary>
        /// 根据实体类型来创建该实体对应数据库表的聚合函数查询sql语句
        /// 该方法创建的sql语句主要是用于查询数据行数
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="converage">聚合函数枚举类型</param>
        /// <returns></returns>
        string CreateConverageSql(Type type, Converage converage);

        /// <summary>
        /// 根据实体类型来创建该实体对应数据库表的聚合函数查询sql语句
        /// 该方法创建的sql语句主要是用于查询数据行数
        /// </summary>
        /// <param name="whereclause">条件</param>
        /// <returns></returns>
        string CreateConverageSql(Type type, string whereclause);

        /// <summary>
        /// 根据实体类型来创建该实体对应数据库表的聚合函数查询sql语句
        /// 该方法创建的sql语句主要是用于统计查询(最大值,最小值,求和,平均值,数据行数)
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="converage">聚合函数枚举类型</param>
        /// <param name="propertyName">聚合函数作用的属性名称</param>
        /// <returns></returns>
        string CreateConverageSql(Type type, Converage converage, string propertyName);

        /// <summary>
        /// 根据实体类型来创建该实体对应数据库表的聚合函数查询sql语句
        /// 该方法创建的sql语句主要是用于统计查询(最大值,最小值,求和,平均值,数据行数)，
        /// 同时该sql是有条件查询的
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="converage">聚合函数枚举类型</param>
        /// <param name="propertyName">聚合函数作用的属性名称</param>
        /// <param name="dic">查询条件属性键值</param>
        /// <param name="component">查询条件组建对象</param>
        /// <returns></returns>
        string CreateConverageSql(Type type, Converage converage, string propertyName, IDictionary<string, object> dic, out IDataParameter[] param, ConditionComponent component);

        /// <summary>
        /// 根据占位符名称创建参数
        /// </summary>
        /// <param name="name">占位符名称</param>
        /// <returns></returns>
        IDataParameter CreateParameter(string name);

        /// <summary>
        /// 根据占位符和值创建参数
        /// </summary>
        /// <param name="name">占位符名称</param>
        /// <param name="value">占位符的值</param>
        /// <returns></returns>
        IDataParameter CreateParameter(string name, object value);

        /// <summary>
        /// 根据占位符名称，类型和值创建参数
        /// </summary>
        /// <param name="name">占位符名称</param>
        /// <param name="type">参数的类型</param>
        /// <param name="value">参数的值</param>
        /// <returns></returns>
        IDataParameter CreateParameter(string name, OracleDataType type, object value);

        /// <summary>
        /// 根据占位符的名称，类型和大小创建参数
        /// </summary>
        /// <param name="name">占位符名称</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数值大小</param>
        /// <returns></returns>
        IDataParameter CreateParameter(string name, OracleDataType type, int size);

        /// <summary>
        /// 根据占位符的名称，类型，大小和值创建参数
        /// </summary>
        /// <param name="name">占位符名称</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数大小</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        IDataParameter CreateParameter(string name, OracleDataType type, int size, object value);

        /// <summary>
        /// 根据占位符名称和类型创建参数
        /// </summary>
        /// <param name="name">占位符名称</param>
        /// <param name="type">占位符类型</param>
        /// <returns></returns>
        IDataParameter CreateParameter(string name, OracleDataType type);
    }
}
