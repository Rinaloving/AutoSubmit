using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// Oracle数据类型
    /// </summary>
    public enum OracleDataType
    {
        /// <summary>
        /// 对应.NET中的数据类型 Int32 
        /// </summary>
        Int,

        /// <summary>
        /// 对应.NET中的数据类型 Int16 
        /// </summary>
        Int16,

        /// <summary>
        /// 对应.NET中的数据类型 System.Byte 
        /// </summary>
        Tinyint,

        /// <summary>
        /// 对应.NET中的数据类型 System.Decimal 
        /// </summary>
        Decimal,

        /// <summary>
        /// 对应.NET中的数据类型 System.Decimal 
        /// </summary>
        Numeric,

        /// <summary>
        /// 对应.NET中的数据类型 System.Double 
        /// </summary>
        Float,

        /// <summary>
        /// 对应.NET中的数据类型 System.Single 
        /// </summary>
        Real,

        /// <summary>
        /// 对应.NET中的数据类型 System.DateTime 
        /// </summary>
        Datetime,

        /// <summary>
        /// 对应.NET中的数据类型 String 
        /// </summary>
        Char,

        /// <summary>
        /// 对应.NET中的数据类型 String 
        /// </summary>
        Varchar,

        /// <summary>
        /// 对应.NET中的数据类型 String 
        /// </summary>
        Text,

        /// <summary>
        /// 对应.NET中的数据类型 String 
        /// </summary>
        Nchar,

        /// <summary>
        /// 对应.NET中的数据类型 String 
        /// </summary>
        Nvarchar,

        /// <summary>
        /// 对应.NET中的数据类型 String
        /// </summary>
        Ntext,

        /// <summary> 
        /// 对应.NET中的数据类型 System.Byte[] 
        /// </summary>
        Varbinary,

        /// <summary>
        /// 对应.NET中的数据类型 System.Byte[] 
        /// </summary>
        Image,

        /// <summary>
        /// 对应.NET中的数据类型 System.DateTime 
        /// </summary>
        Timestamp,

        /// <summary>
        /// 对应.NET中的数据类型 Object 
        /// </summary>
        Variant

    }
}
