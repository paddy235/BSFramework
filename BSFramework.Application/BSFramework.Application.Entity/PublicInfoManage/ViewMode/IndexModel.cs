using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.PublicInfoManage.ViewMode
{
    #region 指标
    public class IndexManageModel
    {
        public string TitleId { get; set; }
        public int? Srot { get; set; }
        public string TitleName { get; set; }
        public bool HasChild { get; set; }
        public List<IndexModel> Childs { get; set; }
        public void AddChilds(List<IndexModel> indexModels)
        {
            if (indexModels != null && indexModels.Count > 0)
            {
                if (Childs == null) Childs = new List<IndexModel>();
                indexModels.ForEach(child =>
                {
                    Childs.Add(child);
                });
                HasChild = true;
            }
        }
    }

    public class IndexModel
    {
        /// <summary>
        /// 自定义编码
        /// </summary>
        public string CustomCode;
        public string Key { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public int? Sort { get; set; }
        /// <summary>
        /// 是否班组指标
        /// </summary>
        public string IsBZ { get; set; }

        public string Unit { get; set; }
        /// <summary>
        /// 指标图片
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string Address { get; set; }

    }
    #endregion
}
