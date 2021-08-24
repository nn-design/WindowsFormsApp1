using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class MHCardHelper
    {

        static MHCardReader mh = new MHCardReader();
        /// <summary>
        /// 读卡
        /// </summary>
        /// <returns></returns>
        public static string ReadCard(out string ErrMsg)
        {
            int res = -1;
            ErrMsg = string.Empty;

            if (mh.icdev == 0)
                mh.icdev = mh.MH_Open(0, 9600);

            if (mh.icdev < 0)
            {
                ErrMsg = "读卡器连接失败";
                return string.Empty;
            }

            res = mh.MH_SeekCard(out ErrMsg);//寻卡
            if (res != 0)
            {
                mh.MH_Close(mh.icdev);
                return string.Empty;
            }

            res = mh.MH_Auth(12, "387f1e04c26d", out ErrMsg);  //验证
            if (res != 0)
            {
                W12key();

                //R9to12();
                //mh.MH_Close(mh.icdev);
                //return string.Empty;
            }

            mh.MH_Auth(12, "387f1e04c26d", out ErrMsg);


            string data = mh.MH_Read(12, 2);   //.Substring(0, 8); //StringHelper.HexStringToString(mh.MH_Read(2, 2), Encoding.UTF8);//读卡
            if (data.Length > 8)
                data = data.Substring(0, 8);

            mh.Beep();

            mh.MH_Close(mh.icdev);

            return data;
        }

        /// <summary>
        /// 写卡
        /// </summary>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static int WriteCard(string CardNo, out string ErrMsg)
        {
            int res = -1;
            ErrMsg = string.Empty;

            if (mh.icdev == 0)
                mh.icdev = mh.MH_Open(0, 9600);

            if (mh.icdev < 0)
            {
                ErrMsg = "读卡器连接失败";
            }

            res = mh.MH_SeekCard(out ErrMsg);//寻卡
            if (res != 0)
            {
                mh.MH_Close(mh.icdev);
            }

            res = mh.MH_Auth(12, "387f1e04c26d", out ErrMsg);  //验证
            if (res != 0)
            {
                mh.MH_Close(mh.icdev);
                return res;
            }

            CardNo = CardNo.PadRight(32, '0');

            res = mh.MH_Write(12, 2, CardNo);

            mh.Beep();

            mh.MH_Close(mh.icdev);

            return res;
        }


        static void R9to12()
        {
            string ErrMsg = string.Empty;
            int res = 0;
            string data = string.Empty;

            //打开
            //mh.icdev = mh.MH_Open(0, 9600);
            //寻卡
            mh.MH_SeekCard(out ErrMsg);
            //认证
            mh.MH_Auth(9, "ffffffffffff", out ErrMsg);
            //读卡
            data = mh.MH_Read(9, 2);

            //认证
            res = mh.MH_Auth(12, "387f1e04c26d", out ErrMsg);
            //写卡
            res = mh.MH_Write(12, 2, data);

            //mh.MH_Close(mh.icdev);
        }

        static void W12key()
        {
            string ErrMsg = string.Empty;
            int res = 0;
            string key = "387f1e04c26dff078069ed2c759134ba";

            //打开
            //mh.icdev = mh.MH_Open(0, 9600);
            //寻卡
            mh.MH_SeekCard(out ErrMsg);
            //认证
            res = mh.MH_Auth(12, "ffffffffffff", out ErrMsg);

            //写卡
            res = mh.MH_Write(12, 3, key);

            //mh.MH_Close(mh.icdev);
        }



        //static string R12data()
        //{
        //    string ErrMsg = string.Empty;
        //    int res = 0;
        //    string data1 = string.Empty;

        //    //打开
        //    mh.icdev = mh.MH_Open(0, 9600);
        //    //寻卡
        //    mh.MH_SeekCard(out ErrMsg);
        //    //认证
        //    res = mh.MH_Auth(12, "777HBRJDZ444", out ErrMsg);
        //    if (res != 0)
        //    {
        //        W12key();
        //        R9to12();
        //    }

        //    //读卡
        //    data1 = mh.MH_Read(12, 2);

        //    mh.MH_Close(mh.icdev);

        //    return data1.Substring(0, 8);

        //}


    }
}
