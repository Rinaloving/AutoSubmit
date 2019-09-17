using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Entity
{
    /// <summary>
    /// 这个实体类是所有实体模型类的父类，该实体类实现了IEntity 接口，实现了手动释放内存方法。
    /// </summary>
    [Serializable]
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// 隐式实现接口方法，用于释放内存
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
