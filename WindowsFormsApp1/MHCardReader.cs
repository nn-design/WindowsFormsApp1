using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class MHCardReader
    {
        /// <summary>
        /// 通讯设备标识符
        /// </summary>
        public int icdev = 0;

        public int MH_Open(Int16 port, int baud)
        {
            return rf_init(port, baud);
        }

        public int MH_Close(int icdev)
        {
            int res = rf_exit(icdev);
            this.icdev = 0;
            return res;
        }

        public int MH_SeekCard(out string ErrMsg)
        {
            if (icdev == 0)
            {
                ErrMsg = "设备通讯接口未打开";
                return -1;
            }

            UInt16 tagtype = 0;
            byte size = 0;
            uint snr = 0;
            Int16 st = 0;
            ErrMsg = "";

            st = rf_reset(icdev, 3);

            st = rf_request(icdev, 1, out tagtype);
            if (st != 0)
            {
                ErrMsg = "request error!";
                return st;
            }

            st = rf_anticoll(icdev, 0, out snr);
            if (st != 0)
            {
                ErrMsg = "anticoll error!";
                return st;
            }

            string snrstr = "";
            snrstr = snr.ToString("X");

            st = rf_select(icdev, snr, out size);
            if (st != 0)
            {
                ErrMsg = "select error!";
                return st;
            }

            return st;
        }

        public int MH_Auth(int sec, string skey, out string ErrMsg)
        {
            byte[] key1 = new byte[17];
            byte[] key2 = new byte[7];

            Int16 st = 0;
            ErrMsg = "";

            key1 = Encoding.ASCII.GetBytes(skey);

            a_hex(key1, key2, 12);
            st = rf_load_key(icdev, 0, sec, key2);
            if (st != 0)
            {
                ErrMsg = "装载密码失败！";
                return st;
            }
            st = rf_authentication(icdev, 0, sec);
            if (st != 0)
            {
                ErrMsg = "认证失败！";
                return st;
            }

            return st;
        }

        public string MH_Read(int sec, int block)
        {
            Int16 st = 0;
            byte[] buff = new byte[32];
            string resData = string.Empty;

            st = rf_read_hex(icdev, sec * 4 + block, buff);
            if (st == 0)
            {
                resData = System.Text.Encoding.ASCII.GetString(buff);
            }

            return resData;
        }

        public int MH_Write(int sec, int block, string data)
        {
            Int16 st = 0;
            byte[] databuff = new byte[32];

            databuff = Encoding.ASCII.GetBytes(data);
            st = rf_write_hex(icdev, sec * 4 + block, databuff);

            return st;
        }


        public void Beep()
        {
            rf_beep(icdev, 10);
        }

        #region MyRegion
        /// <summary>
        /// 说明：初始化串口通讯接口
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <returns>成功则返回串口标识符>0，失败返回负值</returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_init", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        static extern int rf_init(Int16 port, int baud);

        /// <summary>
        /// 说明：关闭通讯接口
        /// </summary>
        /// <param name="icdev"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_exit", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        static extern Int16 rf_exit(int icdev);

        /// <summary>
        /// 说明：蜂鸣
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="msec"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_beep", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        static extern Int16 rf_beep(int icdev, int msec);

        /// <summary>
        /// 说明：射频读写模块复位
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="msec"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_reset", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        static extern Int16 rf_reset(int icdev, int msec);


        /// <summary>
        /// 说明：寻卡请求
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode"></param>
        /// <param name="tagtype"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_request", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 rf_request(int icdev, int mode, out UInt16 tagtype);

        /// <summary>
        /// 说明：卡防冲突，返回卡的序列号
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="bcnt"></param>
        /// <param name="snr"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_anticoll", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        static extern Int16 rf_anticoll(int icdev, int bcnt, out uint snr);


        /// <summary>
        /// 说明：
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="snr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_select", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        static extern Int16 rf_select(int icdev, uint snr, out byte size);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="icdev"></param>
        /// <param name="mode"></param>
        /// <param name="secnr"></param>
        /// <param name="keybuff"></param>
        /// <returns></returns>
        [DllImport("mwrf32.dll", EntryPoint = "rf_load_key", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 rf_load_key(int icdev, int mode, int secnr, [MarshalAs(UnmanagedType.LPArray)] byte[] keybuff);



        [DllImport("mwrf32.dll", EntryPoint = "rf_authentication", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 rf_authentication(int icdev, int mode, int secnr);

        [DllImport("mwrf32.dll", EntryPoint = "rf_authentication_2", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 rf_authentication_2(int icdev, int mode, int keynr, int blocknr);

        [DllImport("mwrf32.dll", EntryPoint = "a_hex", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 a_hex([MarshalAs(UnmanagedType.LPArray)] byte[] asc, [MarshalAs(UnmanagedType.LPArray)] byte[] hex, int len);

        [DllImport("mwrf32.dll", EntryPoint = "hex_a", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 hex_a([MarshalAs(UnmanagedType.LPArray)] byte[] hex, [MarshalAs(UnmanagedType.LPArray)] byte[] asc, int len);


        [DllImport("mwrf32.dll", EntryPoint = "rf_read", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 rf_read(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)] byte[] databuff);

        [DllImport("mwrf32.dll", EntryPoint = "rf_read_hex", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        static extern Int16 rf_read_hex(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)] byte[] databuff);

        [DllImport("mwrf32.dll", EntryPoint = "rf_write", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        public static extern Int16 rf_write(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)] byte[] databuff);

        [DllImport("mwrf32.dll", EntryPoint = "rf_write_hex", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        //说明：     向卡中写入数据
        public static extern Int16 rf_write_hex(int icdev, int blocknr, [MarshalAs(UnmanagedType.LPArray)] byte[] databuff);
        #endregion
    }
}
