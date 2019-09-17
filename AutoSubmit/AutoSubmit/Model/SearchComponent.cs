using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSubmit.Model
{
    /// <summary>
    /// 查询组合条件方式
    /// </summary>
    public enum SM
    {
        /// <summary>
        /// 对应数据库中的 "="
        /// </summary>
        Equals,

        /// <summary>
        /// 对应数据库中的 "!="
        /// </summary>
        UnEquals,

        /// <summary>
        /// 对应数据库中的 ">"
        /// </summary>
        Greater,

        /// <summary>
        /// 对应数据库中的 ">="
        /// </summary>
        GreaterOrEquals,

        /// <summary>
        /// 对应数据库中的 "<"
        /// </summary>
        Less,

        /// <summary>
        /// 对应数据库中的 "<="
        /// </summary>
        LessOrEquals,

        /// <summary>
        /// 对应数据库中的 "like"
        /// </summary>
        Like,

        /// <summary>
        /// 对应数据库中的 "in"
        /// </summary>
        In,

        /// <summary>
        /// 对应数据库中的 "between and"
        /// </summary>
        Between,

        /// <summary>
        /// 对应数据库中的 "order by asc"
        /// </summary>
        OrderAsc,

        /// <summary>
        /// 对应数据库中的 "order by desc"
        /// </summary>
        OrderDesc,

        /// <summary>
        /// 对应数据库中的 "group by"
        /// </summary>
        GroupBy,

        /// <summary>
        /// 对应数据库中的 "or"
        /// </summary>
        Or
    }

    public class SearchComponent
    {
        private string _fieldName;
        public string FieldName
        {
            get { return this._fieldName; }
            set { this._fieldName = value; }
        }

        private SM _searchmode = SM.Equals;
        public SM SearchMode
        {
            get { return this._searchmode; }
            set { this._searchmode = value; }
        }


        private object _value;
        public object Value
        {
            get { return this._value; }
            set { this._value = value; }
        }


        private CM _conditionMode = CM.And;
        public CM ConditionMode
        {
            get { return this._conditionMode; }
            set { this._conditionMode = value; }
        }


        /// <summary>
        /// 条件组合
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="searchMode"></param>
        public SearchComponent(SM searchMode, object value, CM conditionMode)
        {
            this._searchmode = searchMode;
            this._value = value;
            this._conditionMode = conditionMode;
        }

        public SearchComponent(string fieldName, SM searchMode, object value, CM conditionMode)
        {
            this._fieldName = fieldName;
            this._searchmode = searchMode;
            this._value = value;
            this._conditionMode = conditionMode;
        }

    }
}
