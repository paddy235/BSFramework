using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkMeeting.ViewModel
{
    public class KeyValue
    {
        public KeyValue()
        {

        }
        public KeyValue(string key = null, decimal value = 0)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public decimal Value { get; set; }

        /// <summary>
        /// 填写预警指标卡的数量
        /// </summary>
        public decimal Num1 { get; set; }
        /// <summary>
        /// 人均填写预警指标卡的数量
        /// </summary>
        public decimal Num2 { get; set; }
        /// <summary>
        /// 安全观察项总数
        /// </summary>
        public decimal Num3 { get; set; }
        /// <summary>
        /// 有风险的观察项
        /// </summary>
        public decimal Num4 { get; set; }
        /// <summary>
        /// 安全比 
        /// </summary>
        public decimal Num5 { get; set; }

        public string DeptId { get; set; }

        public string Str1 { get; set; }

        /// <summary>
        ///  生成实体                    
        /// </summary>
        /// <param name="dataType">
        /// 1：一到十二个月的Key
        /// 2：只生成1到当前年的上一个月(如果当前月为一月，则生成空List实体)
        /// </param>
        /// <returns></returns>
        public List<KeyValue> InitData(int dataType)
        {
            List<KeyValue> data = new List<KeyValue>();
            switch (dataType)
            {
                case 1:
                    data.Add(new KeyValue("1月", 0));
                    data.Add(new KeyValue("2月", 0));
                    data.Add(new KeyValue("3月", 0));
                    data.Add(new KeyValue("4月", 0));
                    data.Add(new KeyValue("5月", 0));
                    data.Add(new KeyValue("6月", 0));
                    data.Add(new KeyValue("7月", 0));
                    data.Add(new KeyValue("8月", 0));
                    data.Add(new KeyValue("9月", 0));
                    data.Add(new KeyValue("10月", 0));
                    data.Add(new KeyValue("11月", 0));
                    data.Add(new KeyValue("12月", 0));
                    break;
                case 2:
                    int thisMonth = 1;
                    int nowMnth = DateTime.Now.Month;
                    do
                    {
                        data.Add(new KeyValue(Enum.GetName(typeof(MothName), thisMonth)));
                        thisMonth++;
                    }
                    while (thisMonth < nowMnth);
                    break;
            }
            return data;
        }

        /// <summary>
        ///  生成实体                    
        /// </summary>
        /// 一到十二个月的Key
        /// </param>
        /// <returns></returns>
        public List<KeyValue> InitData()
        {
            List<KeyValue> data = new List<KeyValue>();
            string[] names = Enum.GetNames(typeof(MothName));
            foreach (var item in names)
            {
                data.Add(new KeyValue(item, 0));
            }
            return data;
        }
    }

    /// <summary>
    /// 月份枚举
    /// </summary>
    public enum MothName
    {
        一月份 = 1,
        二月份 = 2,
        三月份 = 3,
        四月份 = 4,
        五月份 = 5,
        六月份 = 6,
        七月份 = 7,
        八月份 = 8,
        九月份 = 9,
        十月份 = 10,
        十一月份 = 11,
        十二月份 = 12
    }
}
