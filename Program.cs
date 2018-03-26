using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;


namespace ConsoleApplication11
{
    class Program
    {
        //此处写.dll引入声明
        [DllImport("DepthDll1205", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]//,SetLastError=true)
        public static extern void InitialMemory();

        [DllImport("DepthDll1205", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void DataExecute(Int16[] low, Int16[] high, ref int range, ref int length, int fs,
            ref int powL, ref int powH, ref int GainL, ref int GainH, ref int widL, ref int widH, ref int count,
            double thred, ref int depthPointL, ref int depthPointH, ref int maxl, ref int maxh, double lower, double upper, double[] arr);

        [DllImport("DepthDll1205", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void FreeMemory();


        //[DllImport("DepthDll0301", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]//?GetInstance@Variables@DualFreqDepthDll@@SAPAV12@XZ
        //public static extern void InitialMemory();

        //[DllImport("DepthDll0301", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]//?GetInstance@Variables@DualFreqDepthDll@@SAPAV12@XZ
        //public static extern void DataExecute(Int16[] low, Int16[] high, ref int range, ref int length, int fs,
        //    ref int powL, ref int powH, ref int GainL, ref int GainH, ref int widL, ref int widH, ref int count,
        //    double thred, ref int depthPointL, ref int depthPointH, ref int maxl, ref int maxh, double lower, double upper, double[] arr);

        //[DllImport("DepthDll0301", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public static extern void FreeMemory();

        //[DllImport("DepthDll0301", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //public static extern int Add();



        //[DllImport("SampleCppWrapper.dll")]
        //private static extern int Add(int n1, int n2);
        //[DllImport("SampleCppWrapper.dll")]
        //private static extern int Sub(int n1, int n2);
        //[DllImport("SampleCppWrapper.dll")]
        //private static extern void Free();



        static void Main(string[] args)
        {
            //InitialMemory();
            //Console.WriteLine(Add(2,3));    
            //Free();
            //Console.WriteLine(Add());
            //InitialMemory();
            //FreeMemory();
            int a = 3; int c = 0;
            c = a--;//先赋值，再自减
            int b = 3; int d = 0;
            d = --b;//先自减，再赋值
            Console.WriteLine(a);
            Console.WriteLine(c);
            Console.WriteLine(b);
            Console.WriteLine(d);

            FileStream file;
            file = new FileStream(@"F:\20180322SBES_yujiawan\20180322_025510.dat", FileMode.Open);//F:\20171227SBES_yujiawan\20171227_034154.dat
            //F:\20171227SBES_yujiawan\20171227_030319.dat
            //F:\20180207SBES_yangtze\20180207_015825.dat
            //C:\Users\saa\Desktop\20171220_nanxinda\20171220_104040.dat
            //C:\Users\saa\Desktop\20171220_nanxinda\20171220_104040.dat
            //F:\20180117SBES_yangtze\20180117_053150.dat
            //

            BinaryReader br = new BinaryReader(file);
            int count = 0;
            List<int> DEPTHL = new List<int>();
            List<int> DEPTHH = new List<int>();
            List<int> RANGE = new List<int>();
            List<int> LENST = new List<int>();
            List<int> MAXL = new List<int>();
            List<int> MAXH = new List<int>();

            InitialMemory();


            while(true)
            {
                byte[] DataHead = br.ReadBytes(4);
                if (DataHead[0] != 60)
                    break;
                int FrameCount = br.ReadInt32();
                Int16 DataType = br.ReadInt16();
                Int16 Range = br.ReadInt16();
                Int16 CWFMFlg = br.ReadInt16();
                Int16 VolBattery = br.ReadInt16();
                Int16 SampleRateLow = br.ReadInt16();
                Int16 SampleRateHigh = br.ReadInt16();
                Int16 VolLow = br.ReadInt16();
                Int16 VolHigh = br.ReadInt16();
                byte[] NoUse = br.ReadBytes(16);
                int DataLong = br.ReadInt32();
                Int16[] Data = new Int16[DataLong>>1];
                for(int co = 0;co < (DataLong>>1);co++)
                {
                    Data[co] = br.ReadInt16();
                }
                int SensorLength = br.ReadInt32();
                byte[] SensorData = br.ReadBytes(SensorLength);
                byte[] CheckBit = br.ReadBytes(4);
                byte[] DataEnd = br.ReadBytes(4);

                count++;

                int eachlen = DataLong >> 5;
                Int16[] DataLow = new Int16[DataLong>>4];
                Int16[] DataHigh = new Int16[DataLong>>4];
                for(int cp = 0;cp < (DataLong>>4);cp++)
                {
                    DataLow[cp] = Data[cp];
                }
                for(int cq = (DataLong>>4);cq < (DataLong>>3);cq++)
                {
                    DataHigh[cq-(DataLong>>4)] = Data[cq];
                }

                //初始参数设置
                int range = 100;
                int powL = 5;
                int powH = 5;
                int GainL = -40;
                int GainH = -10;
                int widL = 200;
                int widH = 100;
                int depthPointL = 50;
                int depthPointH = 50;
                //int countAll = 1;
                int maxl = 0;
                int maxh = 0;
                double lower = 0;
                double upper = 0;
                double[] arr = new double[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                //设置盲区深度
                arr[1] = 0.1;

                DateTime beforDT = System.DateTime.Now;

                //耗时巨大的代码
                int lenst = DataLow.Length;
                range = (int)Range;
                if (count >= 175)
                {
                    maxl = 0;
                    //continue;
                }

                DataExecute(DataLow, DataHigh, ref range, ref lenst, 160, ref powL, ref powH, ref GainL, ref GainH,
            ref widL, ref widH, ref count, 0.3, ref depthPointL, ref depthPointH, ref maxl, ref maxh, lower, upper, arr);

                DateTime afterDT = System.DateTime.Now;
                TimeSpan ts = afterDT.Subtract(beforDT);
//                Console.WriteLine("DataExecute总共花费{0}ms.", ts.TotalMilliseconds);
                //if (count >= 2000) break;

///////////
                bool flg1 = false;
                if (count == 1)
                    flg1 = false;
                else
                    flg1 = true;
                StreamWriter cr = new StreamWriter(@"g:\bo.dat", flg1);
                for (int jj = 0; jj < 2048; jj++)//DataLow.Length/2
                {
                    cr.WriteLine(DataLow[jj]);
                }
                cr.WriteLine();
                cr.Close();
                StreamWriter cs = new StreamWriter(@"g:\co.dat", flg1);
                for (int jj = 0; jj < 2048; jj++)
                {
                    cs.WriteLine(DataHigh[jj]);
                }
                cs.WriteLine();
                cs.Close();


                DEPTHL.Add(depthPointL);
                DEPTHH.Add(depthPointH);
                RANGE.Add(range);
                LENST.Add(lenst);//eachlen
                MAXL.Add(maxl);
                MAXH.Add(maxh);

            }

            ///读取完毕所有数据后进行处理
            ///
            FreeMemory();
            br.Close();
            file.Close();

            int[] LOWPNT = DEPTHL.ToArray();
            int[] HIGHPNT = DEPTHH.ToArray();
            int[] RANGENT = RANGE.ToArray();
            int[] LENSTNT = LENST.ToArray();
            int[] MAXLNT = MAXL.ToArray();
            int[] MAXHNT = MAXH.ToArray();
            StreamWriter mag = new StreamWriter(@"g:\low.dat", false);
            for (int jk = 0; jk < LOWPNT.Length; jk++)
            {
                mag.WriteLine(LOWPNT[jk]);
            }
            mag.Close();

            StreamWriter sil = new StreamWriter(@"g:\high.dat", false);
            for (int jl = 0; jl < LOWPNT.Length; jl++)
            {
                sil.WriteLine(HIGHPNT[jl]);
            }
            sil.Close();

            StreamWriter qop = new StreamWriter(@"g:\range.dat", false);
            for (int jm = 0; jm < RANGENT.Length; jm++)
            {
                qop.WriteLine(RANGENT[jm]);
            }
            qop.Close();

            StreamWriter pa = new StreamWriter(@"g:\lens.dat", false);
            for (int jn = 0; jn < LENSTNT.Length; jn++)
            {
                pa.WriteLine(LENSTNT[jn]);
            }
            pa.Close();

            StreamWriter jugg = new StreamWriter(@"g:\maxl.dat", false);
            for (int jo = 0; jo < MAXLNT.Length; jo++)
            {
                jugg.WriteLine(MAXLNT[jo]);
            }
            jugg.Close();

            StreamWriter wd = new StreamWriter(@"g:\maxh.dat", false);
            for (int jp = 0; jp < MAXHNT.Length; jp++)
            {
                wd.WriteLine(MAXHNT[jp]);
            }
            wd.Close();



        }
    }
}
