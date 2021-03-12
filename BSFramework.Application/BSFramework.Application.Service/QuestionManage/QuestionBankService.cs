using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Application.IService.ExperienceManage;
using BSFramework.Application.IService.QuestionManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.QuestionManage
{
    /// <summary>
    /// 答题service
    /// </summary>
    public class QuestionBankService : RepositoryFactory<QuestionBankEntity>, IQuestionBankService
    {
        #region 查询
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        //public DataTable GetPageList(Pagination pagination, string queryJson)
        //{
        //    var db = new RepositoryFactory().BaseRepository();
        //    var queryParam = queryJson.ToJObject();
        //    DataTable dt = db.FindTableByProcPager(pagination, DbHelper.DbType);
        //    return dt;
        //}

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <returns></returns>
        public List<QuestionBankEntity> GetPageList(string keyvalue, int pagesize, int page, out int total)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<QuestionBankEntity>(x => x.outkeyvalue == keyvalue);
            total = query.Count();
            query = query.OrderByDescending(x => x.CreateDate);
            return query.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <returns></returns>
        public List<HistoryQuestionEntity> GetHostoryPageList(string keyvalue, int pagesize, int page, out int total)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<HistoryQuestionEntity>(x => x.outkeyvalue == keyvalue);
            total = query.Count();
            query = query.OrderByDescending(x => x.CreateDate);
            return query.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public QuestionBankEntity GetDetailbyId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<QuestionBankEntity>(keyvalue);
            if (entity != null)
            {
                entity.TheAnswer = db.FindList<TheAnswerEntity>(x => x.questionid == entity.Id).OrderBy(x => x.answer).ToList();

            }
            var fileids = entity.fileids;
            if (!string.IsNullOrEmpty(fileids))
            {
                var files = new List<FileInfoEntity>();
                foreach (var item in fileids.Split(','))
                {
                    var one = db.FindEntity<FileInfoEntity>(x => x.FileId == item);
                    files.Add(one);
                }

                entity.Files = files;
            }
            return entity;
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public List<QuestionBankEntity> GetDetailbyOutId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindList<QuestionBankEntity>(x => x.outkeyvalue == keyvalue).ToList();
            foreach (var item in entity)
            {

                item.TheAnswer = db.FindList<TheAnswerEntity>(x => x.questionid == item.Id).OrderBy(x => x.answer).ToList();


                var fileids = item.fileids;
                if (!string.IsNullOrEmpty(fileids))
                {
                    var files = new List<FileInfoEntity>();
                    foreach (var filesId in fileids.Split(','))
                    {
                        var one = db.FindEntity<FileInfoEntity>(x => x.FileId == filesId);
                        files.Add(one);
                    }

                    item.Files = files;
                }
            }


            return entity;
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public HistoryQuestionEntity GetHistoryDetailbyId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<HistoryQuestionEntity>(keyvalue);
            if (entity != null)
            {
                entity.TheAnswer = db.FindList<HistoryAnswerEntity>(x => x.questionid == entity.Id).OrderBy(x => x.answer).ToList();

            }
            var fileids = entity.fileids;
            if (!string.IsNullOrEmpty(fileids))
            {
                var files = new List<FileInfoEntity>();
                foreach (var item in fileids.Split(','))
                {
                    var one = db.FindEntity<FileInfoEntity>(x => x.FileId == item);
                    files.Add(one);
                }

                entity.Files = files;
            }
            return entity;
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public List<HistoryQuestionEntity> GetHistoryDetailbyOutId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindList<HistoryQuestionEntity>(x => x.outkeyvalue == keyvalue).ToList();
            foreach (var item in entity)
            {
                item.TheAnswer = db.FindList<HistoryAnswerEntity>(x => x.questionid == item.Id).OrderBy(x => x.answer).ToList();
                var fileids = item.fileids;
                if (!string.IsNullOrEmpty(fileids))
                {
                    var files = new List<FileInfoEntity>();
                    foreach (var filesId in fileids.Split(','))
                    {
                        var one = db.FindEntity<FileInfoEntity>(x => x.FileId == filesId);
                        files.Add(one);
                    }

                    item.Files = files;
                }
            }


            return entity;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public List<HistoryQuestionEntity> GetHistoryDetailbyActivityId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindList<HistoryQuestionEntity>(x => x.outkeyvalue == keyvalue).OrderBy(x => x.sort).ToList();
            foreach (var item in entity)
            {
                item.TheAnswer = db.FindList<HistoryAnswerEntity>(x => x.questionid == item.Id).OrderBy(x => x.answer).ToList();
                var fileids = item.fileids;
                if (!string.IsNullOrEmpty(fileids))
                {
                    var files = new List<FileInfoEntity>();
                    foreach (var filesId in fileids.Split(','))
                    {
                        var one = db.FindEntity<FileInfoEntity>(x => x.FileId == filesId);
                        files.Add(one);
                    }

                    item.Files = files;
                }
            }


            return entity;
        }
        /// <summary>
        /// 获取答题目录
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public List<TheTitleEntity> GetHistoryAnswerTitle(string userid, string type, string starttime, string endtime, string iscomplete, int index, int size)
        {
            var db = new RepositoryFactory().BaseRepository();
            var user = db.FindEntity<UserEntity>(userid);

            var Expression = LinqExtensions.True<TheTitleEntity>();
            //if (user.RoleName.Contains("部门"))

            //{
            //    Expression = Expression.And(x => x.deptcode.Contains(user.DepartmentCode));
            //}
            //else
            //{
            Expression = Expression.And(x => x.userid == userid);
            //}

            if (!string.IsNullOrEmpty(type))
            {
                Expression = Expression.And(x => x.category == type);
            }
            if (!string.IsNullOrEmpty(starttime))
            {
                var time = Convert.ToDateTime(starttime);
                Expression = Expression.And(x => x.startTime >= time);
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                var time = Convert.ToDateTime(endtime).AddDays(1).AddMinutes(-1);
                Expression = Expression.And(x => x.startTime <= time);
            }
            if (!string.IsNullOrEmpty(iscomplete))
            {
                if (iscomplete != "false")
                {
                    Expression = Expression.And(x => x.iscomplete == true);
                }
                else
                {
                    Expression = Expression.And(x => x.iscomplete == false);
                }
            }
            var entity = db.FindList(Expression).OrderByDescending(x => x.startTime).Skip(size * (index - 1)).Take(size).ToList();
            foreach (var item in entity)
            {
                item.useranswer = db.FindList<HistoryUserAnswerEntity>(x => x.titleid == item.Id).ToList();
            }

            return entity;
        }

        /// <summary>
        /// 获取活动答题目录
        /// </summary>
        /// <param name="activityid">活动id</param>
        /// <returns></returns>
        public List<TheTitleEntity> GetHistoryAnswerTitlebyActivityId(string activityid, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = new List<TheTitleEntity>();

            if (!string.IsNullOrEmpty(userid))
            {
                entity = db.FindList<TheTitleEntity>(x => x.activityid == activityid && x.userid == userid).OrderByDescending(x => x.score).ToList();
            }
            else
            {
                entity = db.FindList<TheTitleEntity>(x => x.activityid == activityid).OrderByDescending(x => x.score).ToList();
            }
            foreach (var item in entity)
            {
                var useranswer = db.FindList<HistoryUserAnswerEntity>(x => x.titleid == item.Id).OrderBy(x => x.sort).ToList();
                if (useranswer.Count > 0)
                {
                    item.useranswer = useranswer;
                }
                else
                {
                    item.useranswer = new List<HistoryUserAnswerEntity>();
                }

            }
            return entity;

        }
        #endregion

        #region 提交

        /// <summary>
        /// 清除fileid
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public void DetailbyFileId(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.IQueryable<QuestionBankEntity>(x => x.fileids.Contains(keyvalue)).ToList();
                foreach (var item in entity)
                {
                    var fileid = item.fileids.Split(',').ToList(); ;
                    fileid.Remove(keyvalue);
                    item.fileids = string.Join(",", fileid);
                }
                db.Update(entity);
                db.Commit();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        /// <summary>
        ///add  modify
        /// </summary>
        public void SaveFormQuestion(QuestionBankEntity bankEntity, List<TheAnswerEntity> answerEntity, string delAnswer, UserEntity operationUser)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                bankEntity.TheAnswer = null;
                bankEntity.Files = null;
                //add or modify
                if (string.IsNullOrEmpty(bankEntity.Id))
                {
                    //新增
                    var keyvalue = Guid.NewGuid().ToString();
                    var count = db.IQueryable<QuestionBankEntity>(x => x.outkeyvalue == keyvalue).Count();
                    bankEntity.Id = keyvalue;
                    bankEntity.CreateUserId = operationUser.UserId;
                    bankEntity.CreateDate = DateTime.Now;
                    bankEntity.CreateUserName = operationUser.RealName;
                    bankEntity.ModifyUserId = operationUser.UserId;
                    bankEntity.ModifyDate = DateTime.Now;
                    bankEntity.sort = count;
                    bankEntity.ModifyUserName = operationUser.RealName;
                    if (answerEntity.Count > 0)
                    {
                        foreach (var item in answerEntity)
                        {
                            item.questionid = keyvalue;
                            item.Id = Guid.NewGuid().ToString();

                        }
                    }
                    db.Insert(bankEntity);
                    db.Insert(answerEntity);
                    //db.Insert(answerFiles);
                }
                else
                {
                    var old = db.FindEntity<QuestionBankEntity>(bankEntity.Id);
                    if (old != null)
                    {

                        old.ModifyUserId = operationUser.UserId;
                        old.ModifyDate = DateTime.Now;
                        old.ModifyUserName = operationUser.RealName;
                        old.topictitle = bankEntity.topictitle;
                        old.description = bankEntity.description;
                        old.istrue = bankEntity.istrue;
                        old.fileids = bankEntity.fileids;

                        //是否删除答案
                        //if (!string.IsNullOrEmpty(delAnswer))
                        //{
                        //    var dels = delAnswer.Split(',');
                        //    var delAnswerList = new List<TheAnswerEntity>();
                        //var delFileList = new List<FileInfoEntity>();
                        //foreach (var item in dels)
                        //{
                        //    var oldAnswer = db.FindEntity<TheAnswerEntity>(item);
                        //    if (oldAnswer != null)
                        //    {
                        //        var delFile = db.FindList<FileInfoEntity>(x => x.RecId == item);
                        //        delFileList.AddRange(delFile);
                        //        delAnswerList.Add(oldAnswer);
                        //    }
                        //}
                        //if (delAnswerList.Count > 0)
                        //{
                        //    db.Delete(delAnswerList);
                        //}
                        //if (delFileList.Count > 0)
                        //{
                        //    db.Delete(delFileList);
                        //}
                        //}
                        var oldAnswer = db.FindList<TheAnswerEntity>(x => x.questionid == bankEntity.Id);
                        foreach (var item in oldAnswer)
                        {
                            db.Delete(item);
                        }
                        var updateAnswer = new List<TheAnswerEntity>();
                        foreach (var item in answerEntity)
                        {
                            if (string.IsNullOrEmpty(item.Id))
                            {
                                item.questionid = bankEntity.Id;
                                item.Id = Guid.NewGuid().ToString();
                                updateAnswer.Add(item);
                            }
                            else
                            {

                                db.Update(item);
                            }
                        }
                        db.Insert(updateAnswer);
                        db.Update(old);

                    }
                }
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        ///history add  modify
        /// </summary>
        public void SaveFormHistoryQuestion(HistoryQuestionEntity bankEntity, List<HistoryAnswerEntity> answerEntity, string delAnswer, UserEntity operationUser)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                //bankEntity.TheAnswer = null;
                bankEntity.Files = null;
                //add or modify
                if (string.IsNullOrEmpty(bankEntity.Id))
                {
                    //新增
                    var keyvalue = Guid.NewGuid().ToString();
                    bankEntity.Id = keyvalue;
                    bankEntity.CreateUserId = operationUser.UserId;
                    bankEntity.CreateDate = DateTime.Now;
                    bankEntity.CreateUserName = operationUser.RealName;
                    bankEntity.ModifyUserId = operationUser.UserId;
                    bankEntity.ModifyDate = DateTime.Now;
                    bankEntity.ModifyUserName = operationUser.RealName;
                    List<FileInfoEntity> answerFiles = new List<FileInfoEntity>();
                    if (answerEntity.Count > 0)
                    {
                        foreach (var item in answerEntity)
                        {
                            item.questionid = keyvalue;
                            item.Id = Guid.NewGuid().ToString();

                        }
                    }
                    db.Insert(bankEntity);
                    db.Insert(answerEntity);
                    //db.Insert(answerFiles);


                }
                else
                {
                    var old = db.FindEntity<QuestionBankEntity>(bankEntity.Id);
                    if (old != null)
                    {
                        old.ModifyUserId = operationUser.UserId;
                        old.ModifyDate = DateTime.Now;
                        old.ModifyUserName = operationUser.RealName;
                        old.topictitle = bankEntity.topictitle;
                        old.description = bankEntity.description;
                        old.istrue = bankEntity.istrue;
                        old.fileids = bankEntity.fileids;
                        //是否删除答案
                        //if (!string.IsNullOrEmpty(delAnswer))
                        //{
                        //    var dels = delAnswer.Split(',');
                        //    var delAnswerList = new List<HistoryAnswerEntity>();
                        //    var delFileList = new List<FileInfoEntity>();
                        //    foreach (var item in dels)
                        //    {
                        //        var oldAnswer = db.FindEntity<HistoryAnswerEntity>(item);
                        //        if (oldAnswer != null)
                        //        {
                        //            var delFile = db.FindList<FileInfoEntity>(x => x.RecId == item);
                        //            delFileList.AddRange(delFile);
                        //            delAnswerList.Add(oldAnswer);
                        //        }
                        //    }
                        //    if (delAnswerList.Count > 0)
                        //    {
                        //        db.Delete(delAnswerList);
                        //    }
                        //    if (delFileList.Count > 0)
                        //    {
                        //        db.Delete(delFileList);
                        //    }
                        //}
                        var oldAnswer = db.FindList<HistoryAnswerEntity>(x => x.questionid == bankEntity.Id);

                        var updateAnswer = new List<HistoryAnswerEntity>();
                        if (bankEntity.TheAnswer != null)

                        {
                            var delStr = string.Join(",", bankEntity.TheAnswer.Select(x => x.Id));
                            var delEntity = oldAnswer.Where(x => !delStr.Contains(x.Id)).ToList();
                            foreach (var item in answerEntity)
                            {
                                if (string.IsNullOrEmpty(item.Id))
                                {
                                    item.questionid = bankEntity.Id;
                                    item.Id = Guid.NewGuid().ToString();
                                    updateAnswer.Add(item);
                                }
                                else
                                {

                                    var Answer = db.FindEntity<HistoryAnswerEntity>(x => x.Id == item.Id);
                                    if (Answer != null)
                                    {
                                        Answer.istrue = item.istrue;
                                        Answer.answer = item.answer;
                                        Answer.description = item.description;
                                        db.Update(Answer);
                                    }
                                }
                            }
                            db.Delete(delEntity);
                        }
                        db.Insert(updateAnswer);
                        db.Update(old);

                    }
                }
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 保存目录
        /// </summary>
        /// <param name="entity"></param>
        public void SaveFormTitle(TheTitleEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 开始答题
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="bankEntity"></param>
        /// <param name="operationUser"></param>
        public void SaveFormHistoryQuestion(List<TheTitleEntity> entity, List<HistoryQuestionEntity> bankEntity, UserEntity operationUser, string del)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {

                foreach (var Bank in bankEntity)
                {
                    //Bank.TheAnswer = null;
                    Bank.Files = null;
                    if (string.IsNullOrEmpty(Bank.Id))
                    {
                        //新增
                        var keyvalue = Guid.NewGuid().ToString();
                        Bank.Id = keyvalue;
                        //Bank.titleid = titles.Id;
                        Bank.CreateUserId = operationUser.UserId;
                        Bank.CreateDate = DateTime.Now;
                        Bank.CreateUserName = operationUser.RealName;
                        Bank.ModifyUserId = operationUser.UserId;
                        Bank.ModifyDate = DateTime.Now;
                        Bank.ModifyUserName = operationUser.RealName;
                        List<FileInfoEntity> answerFiles = new List<FileInfoEntity>();

                        if (Bank.TheAnswer != null)
                        {
                            foreach (var item in Bank.TheAnswer)
                            {
                                item.questionid = keyvalue;
                                item.Id = Guid.NewGuid().ToString();

                            }
                        }
                        db.Insert(Bank);
                        if (Bank.TheAnswer != null)
                        {
                            db.Insert(Bank.TheAnswer);
                        }
                    }
                    else
                    {

                        var old = db.FindEntity<HistoryQuestionEntity>(Bank.Id);
                        if (old != null)
                        {
                            old.ModifyUserId = operationUser.UserId;

                            old.ModifyDate = DateTime.Now;
                            old.ModifyUserName = operationUser.RealName;
                            old.topictitle = Bank.topictitle;
                            old.description = Bank.description;
                            old.istrue = Bank.istrue;
                            old.fileids = Bank.fileids;
                            var oldAnswer = db.FindList<HistoryAnswerEntity>(x => x.questionid == Bank.Id);

                            var updateAnswer = new List<HistoryAnswerEntity>();
                            if (Bank.TheAnswer != null)
                            {
                                var delStr = string.Join(",", Bank.TheAnswer.Select(x => x.Id));
                                var delEntity = oldAnswer.Where(x => !delStr.Contains(x.Id)).ToList();
                                foreach (var item in Bank.TheAnswer)
                                {
                                    if (string.IsNullOrEmpty(item.Id))
                                    {
                                        item.questionid = Bank.Id;
                                        item.Id = Guid.NewGuid().ToString();
                                        updateAnswer.Add(item);
                                    }
                                    else
                                    {
                                        var Answer = db.FindEntity<HistoryAnswerEntity>(x => x.Id == item.Id);
                                        if (Answer != null)
                                        {
                                            Answer.istrue = item.istrue;
                                            Answer.answer = item.answer;
                                            Answer.description = item.description;
                                            db.Update(Answer);
                                        }

                                    }
                                }
                                db.Delete(delEntity);

                            }

                            db.Insert(updateAnswer);
                            db.Update(old);

                        }
                    }
                }
                if (!string.IsNullOrEmpty(del))
                {
                    var dels = del.Split(',');
                    foreach (var item in dels)
                    {
                        if (string.IsNullOrEmpty(item)) continue;

                        var delEntity = db.FindEntity<HistoryQuestionEntity>(item);
                        db.Delete(delEntity);
                    }
                }

                db.Insert(entity);
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 答题
        /// </summary>
        /// <param name="type">0 判断题</param>
        /// <param name="mode"></param>
        /// <param name="entity"></param>
        public void answerWork(string type, HistoryAnswerEntity mode, HistoryQuestionEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (type == "0")
                {
                    db.Update(mode);
                }
                else
                {
                    entity.TheAnswer = null;
                    entity.Files = null;
                    db.Update(entity);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void RemoveForm(string keyvalue)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var question = db.FindEntity<QuestionBankEntity>(keyvalue);
                if (question != null)
                {
                    var answer = db.FindList<TheAnswerEntity>(x => x.questionid == keyvalue);
                    foreach (var item in answer)
                    {
                        db.Delete(item);
                    }
                    db.Delete(question);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void RemoveFormByOutId(string keyvalue)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var question = db.IQueryable<QuestionBankEntity>(x => x.outkeyvalue == keyvalue);
                if (question != null)
                {
                    var answer = db.IQueryable<TheAnswerEntity>(x => x.questionid == keyvalue);
                    foreach (var item in answer)
                    {
                        db.Delete(item);
                    }
                    db.Delete(question);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        ///history 删除
        /// </summary>
        public void RemoveFormHistory(string keyvalue)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var question = db.FindEntity<HistoryQuestionEntity>(keyvalue);
                if (question != null)
                {
                    var answer = db.FindList<HistoryAnswerEntity>(x => x.questionid == keyvalue);
                    foreach (var item in answer)
                    {
                        db.Delete(item);
                    }
                    db.Delete(question);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 答题
        /// </summary>
        /// <param name="titleid">表</param>
        /// <param name="sort">顺序</param>
        /// <param name="answer">答案</param>
        public void answerWork(string titleid, int sort, string answer)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<HistoryUserAnswerEntity>(x => x.sort == sort && x.titleid == titleid);
                if (entity != null)
                {
                    entity.answer = answer;
                    db.Update(entity);
                }
                else
                {
                    entity = new HistoryUserAnswerEntity();
                    entity.id = Guid.NewGuid().ToString();
                    entity.titleid = titleid;
                    entity.sort = sort;
                    entity.answer = answer;
                    if (answer == "正确")
                    {
                        entity.istrue = true;
                    }
                    if (answer == "错误")
                    {
                        entity.istrue = false;
                    }
                    db.Insert(entity);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 分数 
        /// </summary>
        /// <param name="keyvalue"></param>
        public string TitleScore(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var score = "";
            try
            {
                var titel = db.FindEntity<TheTitleEntity>(keyvalue);
                var Test = db.FindList<HistoryQuestionEntity>(x => x.outkeyvalue == titel.activityid);//题目
                var answer = db.FindList<HistoryUserAnswerEntity>(x => x.titleid == keyvalue);//答案
                var count = Test.Count();//题目数量
                var istrue = 0;
                foreach (var item in Test)
                {
                    var sortanswer = answer.FirstOrDefault(x => x.sort == item.sort);
                    if (sortanswer != null)
                    {
                        if (item.topictype == "判断题")
                        {
                            if (item.istrue && sortanswer.answer.Contains("正确"))
                            {
                                istrue++;
                            }
                            if (item.istrue == false && sortanswer.answer.Contains("错误"))
                            {
                                istrue++;
                            }
                        }
                        else
                        {
                            var Theanswer = db.FindList<HistoryAnswerEntity>(x => x.questionid == item.Id && x.istrue);
                            var theCode = string.Join(",", Theanswer.Select(x => x.answer));
                            var TheanswerCount = Theanswer.Count();
                            var useranswer = sortanswer.answer.Split(',').Where(x => !string.IsNullOrEmpty(x));
                            var useranswerCount = useranswer.Count();
                            var ckresult = true;
                            if (useranswerCount == TheanswerCount)
                            {
                                foreach (var ckanswer in useranswer)
                                {
                                    if (!theCode.Contains(ckanswer))
                                    {
                                        ckresult = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ckresult = false;
                            }
                            if (ckresult)
                            {
                                istrue++;
                            }
                        }
                    }
                }
                if (istrue == count)
                {
                    score = "100";
                }
                else
                {
                    double avg = 100.00 / count;
                    score = (Math.Round(avg * istrue)).ToString();

                }
                titel.iscomplete = true;
                titel.score = score;
                titel.endTime = DateTime.Now;
                db.Update(titel);
                db.Commit();
                var result = score + "," + istrue + "," + (count - istrue);
                return result;
            }
            catch (Exception)
            {
                db.Rollback();
                throw;

            }
            //return score;
        }

        #endregion
    }
}
