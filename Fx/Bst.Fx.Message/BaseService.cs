using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Data.Repository;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using Bst.Fx.MessageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Message
{
    public abstract class BaseService
    {
        //业务数据主键
        private string _businessId;
        //通知设置
        private ConfigEntity _config;
        //业务数据
        public object BusinessData { get; set; }

        public ConfigEntity Config { get { return _config; } private set { } }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="messagekey">消息键</param>
        /// <param name="businessId">业务数据主键</param>
        public BaseService(string messagekey, string businessId)
        {
            _businessId = businessId;
            _config = this.GetConfig(messagekey);
            this.BusinessData = this.GetData(businessId);
        }

        /// <summary>
        /// 获取消息设置
        /// </summary>
        /// <param name="messagekey">消息键</param>
        public virtual ConfigEntity GetConfig(string messagekey)
        {
            var configservice = new ConfigService();
            return configservice.GetConfig(messagekey);
        }

        /// <summary>
        /// 获取数据数据
        /// </summary>
        /// <param name="businessId"></param>
        public abstract object GetData(string businessId);

        /// <summary>
        /// 获取消息标题，默认获取设置的标题
        /// </summary>
        /// <returns>标题</returns>
        public virtual string GetTitle()
        {
            return _config.Title;
        }

        /// <summary>
        /// 获取消息内容，需要实现
        /// </summary>
        /// <returns>消息内容</returns>
        public abstract string GetContent();

        /// <summary>
        /// 获取班组
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetDeptId();

        /// <summary>
        /// 获取消息接收人
        /// </summary>
        /// <returns>接收人ID</returns>
        public abstract string GetBusinessUserId();

        /// <summary>
        /// 获取按职务方式接收人，
        /// </summary>
        /// <returns>接收人</returns>
        public virtual string GetRoleUserId()
        {
            var deptids = this.GetDeptId();
            if (deptids == null) deptids = new string[0];

            if (_config.RecieveType != "接收人")
            {
                var ary = _config.RecieveType.Split(',');
                var db = new RepositoryFactory().BaseRepository();
                var query = from q in db.IQueryable<UserEntity>()
                            where q.UserId == null
                            select q.UserId;
                foreach (var item in ary)
                {
                    query = query.Concat(from q1 in db.IQueryable<UserEntity>()
                                         join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                                         where deptids.Contains(q1.DepartmentId) && ("," + q2.Quarters + ",").Contains("," + item + ",")
                                         select q1.UserId);
                }

                var data = query.Distinct().ToList();
                return string.Join(",", data);
            }

            return null;
        }

        /// <summary>
        /// 获取接收人
        /// </summary>
        /// <returns>接收人</returns>
        public virtual string[] GetUserId()
        {
            var userid = string.Empty;
            if (_config.RecieveType == "接收人") userid = this.GetBusinessUserId();
            else userid = this.GetRoleUserId();

            return string.IsNullOrEmpty(userid) ? new string[0] : userid.Split(',');
        }

        /// <summary>
        /// 获取接收人
        /// </summary>
        /// <returns>接收人账号</returns>
        public virtual string[] GetAlias()
        {
            var userid = this.GetUserId();

            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<UserEntity>()
                        where userid.Contains(q.UserId)
                        select q.Account;
            var data = query.ToArray();
            return data;
        }

        /// <summary>
        /// 获取消息体
        /// </summary>
        /// <returns></returns>
        public virtual string GetMessage()
        {
            var template = _config.Template;
            var content = this.GetContent();
            return template.Replace("{content}", content);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public virtual void SendMessage()
        {
            if (!_config.Enabled) return;

            var title = this.GetTitle();
            var content = this.GetContent();
            var alias = this.GetAlias();
            var users = this.GetUserId();

            using (var ctx = new DataContext())
            {
                //var msg = _config.Template.Replace("{content}", content);
                var msg = this.GetMessage();
                foreach (var item in users)
                {
                    var entity = new Bst.Bzzd.DataSource.Entities.Message()
                    {
                        MessageId = Guid.NewGuid(),
                        BusinessId = _businessId,
                        Content = msg,
                        Title = title,
                        UserId = item,
                        Category = (MessageCategory)Enum.Parse(typeof(MessageCategory), _config.Category),
                        MessageKey = _config.ConfigKey,
                        CreateTime = DateTime.Now
                    };

                    ctx.Messages.Add(entity);
                }

                ctx.SaveChanges();
            }
            if ((_config.Category == "Todo" || _config.Category == "Warning") && !string.IsNullOrEmpty(_businessId))
                JPushClient.SendRequest(alias, _businessId, title, title, content);
        }
    }
}