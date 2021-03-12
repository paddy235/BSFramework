using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Entity.BaseManage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface
{
    public class FaceUtil : IDisposable
    {
        private float _validScore = 0;

        public FaceUtil()
        {
            this.Init();
        }

        public void Dispose()
        {
            FaceImportDLL.FRUnInitAL();
        }

        public UserEntity Valid(string featureString)
        {
            var userFeature = Convert.FromBase64String(featureString);

            float pfScore = 0;
            var maxScore = 0f;
            var validUser = default(UserFaceEntity);

            var users = new UserBLL().GetUserFaces();

            foreach (var item in users)
            {
                var storeFeature = Convert.FromBase64String(item.FaceStream);
                var result1 = FaceImportDLL.GetFeatureSimilarity(userFeature, storeFeature, ref pfScore);
                if (pfScore >= _validScore)
                {
                    if (maxScore == 0)
                    {
                        maxScore = pfScore;
                        validUser = item;
                    }
                    else
                    {
                        if (pfScore > maxScore)
                        {
                            maxScore = pfScore;
                            validUser = item;
                        }
                    }
                }
            }

            if (validUser != null)
            {
                var user = new UserBLL().GetEntity(validUser.UserId);
                return user;
            }

            return null;
        }

        public List<UserEntity> Valid(string[] featureStrings)
        {
            featureStrings = featureStrings.Distinct().ToArray();
            var list = new List<UserFaceEntity>();

            var userfaces = new UserBLL().GetUserFaces();
            foreach (var item in featureStrings)
            {
                var userFeature = Convert.FromBase64String(item);
                float pfScore = 0;
                var maxScore = 0f;
                var validUser = default(UserFaceEntity);

                for (int i = 0; i < userfaces.Count; i++)
                {
                    var storeFeature = Convert.FromBase64String(userfaces[i].FaceStream);
                    var result1 = FaceImportDLL.GetFeatureSimilarity(userFeature, storeFeature, ref pfScore);
                    if (pfScore >= _validScore)
                    {
                        if (maxScore == 0)
                        {
                            maxScore = pfScore;
                            validUser = userfaces[i];
                        }
                        else
                        {
                            if (pfScore > maxScore)
                            {
                                maxScore = pfScore;
                                validUser = userfaces[i];
                            }
                        }
                    }
                }
                if (validUser != null)
                {
                    list.Add(validUser);
                    userfaces.Remove(validUser);
                }
            }

            return new UserBLL().GetUsersByIds(list.Select(x => x.UserId));
        }

        private void Init()
        {
            int init = FaceImportDLL.FRInitAL();
            if (0 != init)
                throw new Exception($"人脸识别模块加载失败，错误码 [{init}]");

            //设置人脸检测的策略, 如下为默认策略
            FaceImportDLL.FRSetDetectionPara(40, 0.2f, 1);
            FaceImportDLL.FRSetDetectionType(4);

            _validScore = float.Parse((ConfigurationManager.AppSettings["CompareValue"] ?? "0.8").ToString());
        }
    }
}