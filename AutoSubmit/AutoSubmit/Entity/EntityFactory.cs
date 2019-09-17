using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Entity
{
    /// <summary>
    /// 该实体类主要是通过反射机制来获取泛型实体类的实例的
    /// </summary>
    public static class EntityFactory
    {
        /// <summary>
        /// 根据泛型类获取该泛型类的实例，
        /// T 泛型必须是class 类
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>() where T : class
        {
            Type type = typeof(T);
            Object result;
            result = Activator.CreateInstance<T>();
            return (T)result;
        }

        /// <summary>
        /// 根据实体类型来创建实体实例
        /// </summary>
        /// <param name="type">要创建的对象的类型</param>
        /// <param name="nonPublic">如果有显示构造方法为true,如果没有显示构造方法为false</param>
        /// <returns></returns>
        public static object CreateInstance(Type type, bool nonPublic)
        {
            return Activator.CreateInstance(type, nonPublic);
        }

        /// <summary>
        /// 根据实体类型和相关参数构造实体实例
        /// </summary>
        /// <param name="type">要创建的对象的类型</param>
        /// <param name="param">与要调用构造函数的参数数量、顺序和类型匹配的参数数组。如果 param 为空数组，则调用不带任何参数的构造函数</param>
        /// <returns></returns>
        public static object CreateInstance(Type type, params object[] param)
        {
            return Activator.CreateInstance(type, param);
        }

        /// <summary>
        /// 根据实体公共接口获得特定属性名称的值
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="name">实体属性名称</param>
        /// <returns></returns>
        public static object GetPropertyValue(IEntity entity, string name)
        {
            PropertyInfo property = entity.GetType().GetProperty(name);
            object result = null;
            if (property != null)
            {
                result = property.GetValue(entity, null);
            }
            return result;
        }

        /// <summary>
        /// 根据实体公共接口获得特定属性名称的值,这个属性是一个类
        /// </summary>
        /// <typeparam name="T">返回属性的泛型类型</typeparam>
        /// <param name="entity">实体公共接口</param>
        /// <param name="name">实体属性名称</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(IEntity entity, string name) where T : class
        {
            object result = GetPropertyValue(entity, name);
            if (result == null)
            {
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }

        /// <summary>
        /// 获得实体属性值
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="name">属性名称</param>
        /// <returns></returns>
        public static object GetPropertyValue(object entity, string name)
        {
            PropertyInfo property = entity.GetType().GetProperty(name);
            object result = null;
            if (property != null)
            {
                result = property.GetValue(entity, null);
            }
            return result;
        }

        /// <summary>
        /// 根据实体公共接口设置某属性的值
        /// </summary>
        /// <param name="entity">实体公共接口</param>
        /// <param name="name">实体属性名称</param>
        /// <param name="value">实体属性值</param>
        public static void SetPropertyValue(IEntity entity, string name, object value)
        {
            PropertyInfo property = entity.GetType().GetProperty(name);
            if (property != null)
            {
                property.SetValue(name, value, null);
            }
        }

        /// <summary>
        /// 将某个值转化为特定的类型
        /// </summary>
        /// <param name="type">转化为的目标类型</param>
        /// <param name="value">被转化的值</param>
        /// <returns></returns>
        public static object ConvertValue(Type type, object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 将某个值转化为特定的泛型类型
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="type">转化为的目标类型</param>
        /// <param name="value">被转化的值</param>
        /// <returns></returns>
        public static T ConvertValue<T>(Type type, object value)
        {
            if (value == DBNull.Value)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(value, type);
        }
    }
}
