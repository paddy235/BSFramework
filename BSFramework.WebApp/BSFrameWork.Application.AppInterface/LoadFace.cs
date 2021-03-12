//using Micropattern.InterFace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;


namespace BSFrameWork.Application.AppInterface
{
    public class LoadFace 
    {
        static byte[] ReadFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                byte[] byteData = new byte[fs.Length];
                fs.Read(byteData, 0, byteData.Length);
                fs.Close();
                fs.Dispose();
                return byteData;
            }

        }

        int featureslength = 0;

        public int InitAL()
        {
            int init = FaceImportDLL.FRInitAL();
            if (0 != init)
            {  

                //Console.WriteLine(String.Format("[LOADFACE]初始化算法失败，错误码[{0}]", init));
                //Console.ReadKey();

                return -1;
            }

            featureslength = FaceImportDLL.GetFeatureLength();

            return init;
        }

        public int UnInitAL()
        {
            return FaceImportDLL.FRUnInitAL();
        }

        public byte[] GetFeaturesFromFile(byte[] pImageData, int iDateLen)
        {
            if (null == pImageData)
            {
                Console.WriteLine(String.Format("[LOADFACE]传入图片流为空"));
                Console.ReadKey();
            }

            byte[] bytes = new byte[featureslength];
            FaceImportDLL.GetFeaturesFromFile(pImageData, iDateLen, bytes);
            return bytes;
        }

        public int GetFeatureSimilarity(byte[] jarg1, byte[] jarg2, ref float pfScore)
        {
            if (null == jarg1 || null == jarg2)
            {
                //Console.WriteLine(String.Format("[LOADFACE]传入特征值为空"));
                //Console.ReadKey();

                return -1;
            }

            return FaceImportDLL.GetFeatureSimilarity(jarg1, jarg2, ref pfScore);
        }

        public int GetSimilarity(string imgUrl1, string imgUrl2, ref float pfScore)
        {
            byte[] jarg1 = ReadFile(imgUrl1);
            byte[] jarg2 = ReadFile(imgUrl2);
            pfScore = 0.0f;

            if (null == jarg1 || null == jarg2)
            {
                Console.WriteLine(String.Format("[LOADFACE]读取图片失败"));
                Console.ReadKey();
            }

            return FaceImportDLL.GetSimilarity(jarg1, jarg1.Length, jarg2, jarg2.Length, ref pfScore);
        }

        public int FRDetection(string imgUrl, ref byte[] pResult)
        {
            byte[] jarg = ReadFile(imgUrl);

            return FaceImportDLL.FRDetection(jarg, jarg.Length, pResult, pResult.Length);
        }

        public int FRSetDetectionPara(int iMinFaceSize, float fMinFaceRatio, int iBestPolicy)
        {
            return FaceImportDLL.FRSetDetectionPara(iMinFaceSize, fMinFaceRatio, iBestPolicy);
        }

        public int FRSetDetectionType(int iType)
        {
            return FaceImportDLL.FRSetDetectionType(iType);
        }
		
        public int LoadSet(string ip, int port, string deviceID)
        {
            return 0;
        }
    }
}
