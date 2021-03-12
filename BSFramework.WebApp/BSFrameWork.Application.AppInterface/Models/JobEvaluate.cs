using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSFramework.Application.Entity.SystemManage;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class JobEvaluate
    {
        public JobEvaluate()
        {
            Option = new Option();
        }

        public JobEvaluate(DataItemDetailEntity p) : this()
        {
            if (p == null) throw new Exception("构造函数JobEvaluate()的参数DataDetailEntity不能为空");
            try
            {
                Title = p.ItemName;
                SortCode = p.SortCode ?? 0;
                var arry = p.Description.Split('|');
                Type = arry[0];
                Option = new Option(arry[1]);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 评分项类型   普通/特殊
        /// </summary>
        public string Type { get; set; }

        public Option Option { get; set; }
        /// <summary>
        /// 排序
        /// </summary>

        public int SortCode { get; set; }
    }

    public class Option
    {
        public Option()
        {
            Name = new List<string>();
            ScoreList = new List<int>();
        }
        public Option(string Str) : this()
        {
            var arry = Str.Split(',');
            foreach (var item in arry)
            {
                var itemArry = item.Split(':');
                Name.Add(itemArry[0]);
                ScoreList.Add(int.Parse(itemArry[1]));
            }
        }
        public List<string> Name { get; set; }
        public string Value { get; set; }
        public List<int> ScoreList { get; set; }
        public int Score { get; set; }
    }

    public class JobEvaluateItem
    {
        public JobEvaluateItem()
        {
            Items = new List<JobEvaluateItemDetail>();
        }

        public JobEvaluateItem(DataItemDetailEntity p) : this()
        {
            if (p == null) throw new Exception("构造函数JobEvaluate()的参数DataDetailEntity不能为空");
            Title = p.ItemName;
            var arrys = p.Description.Split('|');
            var items = arrys[1].Split(',');
            foreach (var item in items)
            {
                var itemArry = item.Split(':');
                Items.Add(new JobEvaluateItemDetail()
                {
                    Name = itemArry[0],
                    Value = int.Parse(itemArry[1])
                });
            }

        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        public List<JobEvaluateItemDetail> Items { get; set; }
    }
    public class JobEvaluateItemDetail
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}