using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Views.EduTrain
{
    public class Security
    {
        private string _Key = "PhoenixK";
        private string _IV = "PhoenixI";
        /**/
        /// <summary> 
        /// 加密密钥(8個英文字) 
        /// </summary> 
        public string Key
        {
            set
            {
                _Key = value;
            }
            get
            {
                return _Key;
            }
        }
        /**/
        /// <summary> 
        /// 初始化向量(8個英文字) 
        /// </summary> 
        public string IV
        {
            set
            {
                _IV = value;
            }
            get
            {
                return _IV;
            }
        }

        /**/
        /// <summary> 
        /// 默认初始化
        /// </summary> 
        public Security()
        {
            _Key = "BossienX";
            _IV = "BossienY";
        }

        /**/
        /// <summary> 
        /// 
        /// </summary> 
        /// <param name="newKey">加密密钥</param> 
        /// <param name="newIV">初始化向量</param> 
        public Security(string newKey, string newIV)
        {
            this.Key = newKey;
            this.IV = newIV;
        }

        /// <!--DEC 加密法 --> 
        /**/
        /// <summary> 
        /// DEC 加密法 
        /// </summary> 
        /// <param name="pToEncrypt">加密的字串</param> 
        /// <param name="sKey">加密密钥</param> 
        /// <param name="sIV">初始化向量</param> 
        /// <returns></returns> 
        public string Encrypt(string pToEncrypt, string sKey, string sIV)
        {
            StringBuilder ret = new StringBuilder();
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                //设置加密密钥
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //设置向量
                des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    //输出
                    foreach (byte b in ms.ToArray())
                        ret.AppendFormat("{0:X2}", b);
                }
            }
            return ret.ToString();
        }

        /// <!--DEC 解密法--> 
        /**/
        /// <summary> 
        /// DEC 解密法
        /// </summary> 
        /// <param name="pToDecrypt">解密的字串</param> 
        /// <param name="sKey">加密密钥</param> 
        /// <param name="sIV">初始化向量</param> 
        /// <returns></returns> 
        public string Decrypt(string pToDecrypt, string sKey, string sIV)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {

                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                //反转
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                //设定加密密钥
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //设定初始化向量
                des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        try
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                        }
                        catch (CryptographicException)
                        {
                            //错误返回N/A 
                            return "N/A";
                        }
                    }
                }
            }
        }


        /// <!--验证加密字符--> 
        /**/
        /// <summary> 
        /// 验证加密字符
        /// </summary> 
        /// <param name="EnString">加密后的字串</param> 
        /// <param name="FoString">加密前的字串</param> 
        /// <returns>是/否</returns> 
        public bool ValidateString(string EnString, string FoString)
        {
            return Decrypt(EnString, _Key, _IV) == FoString.ToString() ? true : false;
        }
    }
}