using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// 该类封装修改数据，条件查询数据的相关条件。
    /// </summary>
    public class ConditionComponent
    {
        private static ConditionComponent mInstance = null;
        private static readonly object lockAssistant = new object();
        /// <summary>
        /// 单例对象，封装条件查询数据的相关条件
        /// </summary>
        public static ConditionComponent Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (lockAssistant)
                    {
                        if (mInstance == null)
                            mInstance = new ConditionComponent();
                    }
                }
                return mInstance;
            }
        }
        /// <summary>
        /// 不可以实例化
        /// </summary>
        private ConditionComponent()
        { }


        private IDictionary<string, SearchComponent> _dicComponent = new Dictionary<string, SearchComponent>();

        /// <summary>
        /// 用于存储属性查询类型
        /// </summary>
        public IDictionary<string, SearchComponent> DicComponent
        {
            get { return _dicComponent; }
            set { _dicComponent = value; }
        }

        public ConditionComponent Add(params SearchComponent[] param)
        {
            return ConditionComponent.Instance;
        }

        /// <summary>
        /// 添加属性查询类型
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="component">查询类型</param>
        /// <returns></returns>
        public ConditionComponent AddComponent(string propertyName, SearchComponent component)
        {
            if (!_dicComponent.Keys.Contains(propertyName))
                _dicComponent.Add(propertyName, component);
            return ConditionComponent.Instance;
        }

        /// <summary>
        /// 清除属性查询列表
        /// </summary>
        public void ClearComponent()
        {
            _dicComponent.Clear();
        }

    }
}
