using Bst.Fx.IMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bst.Fx.MessageData;
using Bst.Bzzd.DataSource;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using Bst.Bzzd.DataSource.Entities;

namespace Bst.Fx.Message
{
    public class MessageService : IMessageService
    {
        public void FinishTodo(string messagekey, string businessId)
        {
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.Messages
                            where q.BusinessId == businessId && q.MessageKey == messagekey
                            select q;

                var data = query.ToList();
                foreach (var item in data)
                {
                    item.IsFinished = true;
                }

                ctx.SaveChanges();
            }
        }

        public List<MessageEntity> GetMessage(string userid, out int total)
        {
            var result = new List<MessageEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.Messages
                            where q.UserId == userid && (q.Category == MessageCategory.Message || q.Category == MessageCategory.Warning) && q.HasReaded == false
                            orderby q.CreateTime descending
                            select q;

                total = query.Count();

                var data = query.ToList();
                foreach (var item in data)
                {
                    result.Add(new MessageEntity()
                    {
                        MessageId = item.MessageId,
                        BusinessId = item.BusinessId,
                        Title = item.Title,
                        Content = item.Content,
                        UserId = item.UserId,
                        CreateTime = item.CreateTime,
                        MessageKey = item.MessageKey
                    });

                    item.HasReaded = true;
                }

                ctx.SaveChanges();
            }

            return result;
        }

        public MessageEntity GetMessageDetail(Guid messageid)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.Messages.Find(messageid);
                if (entity == null) return null;

                return new MessageEntity() { BusinessId = entity.BusinessId, Content = entity.Content, CreateTime = entity.CreateTime, MessageId = entity.MessageId, Title = entity.Title, UserId = entity.UserId };
            }
        }

        public int GetMessageTotal(string userid)
        {
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.Messages
                            where q.UserId == userid && (((q.Category == MessageCategory.Message || q.Category == MessageCategory.Warning) && q.HasReaded == false) || (q.Category == MessageCategory.Todo && q.IsFinished == false))
                            orderby q.CreateTime descending
                            select q;

                return query.Count();
            }
        }

        public List<MessageEntity> GetTodo(string userid, out int total)
        {
            var result = new List<MessageEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.Messages
                            where q.UserId == userid && q.Category == Bzzd.DataSource.Entities.MessageCategory.Todo && q.IsFinished == false
                            orderby q.CreateTime descending
                            select q;

                total = query.Count();

                var data = query.ToList();
                foreach (var item in data)
                {
                    result.Add(new MessageEntity()
                    {
                        MessageId = item.MessageId,
                        BusinessId = item.BusinessId,
                        Title = item.Title,
                        Content = item.Content,
                        UserId = item.UserId,
                        CreateTime = item.CreateTime,
                        MessageKey = item.MessageKey
                    });
                }
            }

            return result;
        }

        public void SendMessage(string messagekey, string businessId)
        {
            var config = default(MessageConfig);
            using (var ctx = new DataContext())
            {
                config = ctx.MessageConfigs.FirstOrDefault(x => x.ConfigKey == messagekey);
            }
            if (config == null) return;

            var type = Type.GetType(config.Assembly);
            var service = Activator.CreateInstance(type, messagekey, businessId) as BaseService;
            service.SendMessage();
        }
    }
}