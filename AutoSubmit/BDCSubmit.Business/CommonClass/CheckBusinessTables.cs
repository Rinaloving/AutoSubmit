using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BDCSubmit.Business.CommonClass
{
    [XmlRoot("CheckBusinessTables")]
    public class CheckBusinessTables
    {
        /// <summary>
        /// 检查业务
        /// </summary>
        [XmlElement("BusinessType")]
        public List<BusinessTypeClass> Business { get; set; }
    }
    [XmlRoot("CheckTableColumns")]
    public class CheckTableColumns
    {
        /// <summary>
        /// 检查表字段
        /// </summary>
        [XmlElement("Table")]
        public List<TableClass> Tables { get; set; }
    }
    /// <summary>
    /// 检查业务
    /// </summary>
    [Serializable]
    public class BusinessTypeClass
    {
        /// <summary>
        /// 接入业务编码
        /// </summary>
        [XmlAttribute("BusinessCode")]
        public string BusinessCode { get; set; }
        /// <summary>
        /// 业务名称
        /// </summary>
        [XmlAttribute("BusinessName")]
        public string BusinessName { get; set; }
        /// <summary>
        /// 上报表
        /// </summary>
        [XmlElement("Table")]
        public List<TableClass> 检查表 { get; set; }
    }
    /// <summary>
    /// 表
    /// </summary>
    [Serializable]
    public class TableClass
    {
        /// <summary>
        /// 表名
        /// </summary>
        [XmlAttribute("TableName")]

        public string 表名 { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [XmlAttribute("TableAliasName")]
        public string 别名 { get; set; }
        /// <summary>
        /// 是否必选
        /// </summary>
        [XmlAttribute("IsMust")]
        public bool 是否必选 { get; set; }
        /// <summary>
        /// 表中所包含的列
        /// </summary>
        [XmlElement("Column")]
        public List<ColumnClass> 检查列 { get; set; }
    }
    /// <summary>
    /// 列
    /// </summary>
    [Serializable]
    public class ColumnClass
    {
        /// <summary>
        /// 列名
        /// </summary>
        [XmlAttribute("ColumnName")]
        public string 列名 { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        [XmlAttribute("ColumnAliasName")]
        public string 别名 { get; set; }
        /// <summary>
        /// 列数据类型
        /// </summary>
        [XmlAttribute("ColumnType")]
        public ColumnType 列数据类型 { get; set; }
        /// <summary>
        /// 值在表内是否惟一
        /// </summary>
        [XmlAttribute("IsOnly")]
        public bool 值在表内是否惟一 { get; set; }
        /// <summary>
        /// 是否可为空
        /// </summary>
        [XmlAttribute("IsNotNull")]
        public bool 是否可为空 { get; set; }
        /// <summary>
        /// 是否检查字典值
        /// </summary>
        [XmlAttribute("IsCheckDictionary")]
        public bool 是否检查字典值 { get; set; }
        /// <summary>
        /// 字典代码
        /// </summary>
        [XmlAttribute("DictionaryCodeNo")]
        public string 字典代码 { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        [XmlAttribute("DictionaryType")]
        public string 字典类型 { get; set; }
    }

}
