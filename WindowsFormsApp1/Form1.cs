using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            bgWorker.RunWorkerAsync();
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string ErrMsg = string.Empty;
            int x = 60006407;

            while (true)
            {
                string CardNo = MHCardHelper.ReadCard(out ErrMsg);

                //if (CardNo == "00000000")
                {
                    MHCardHelper.WriteCard(x.ToString(), out ErrMsg);
                }

                CardNo = MHCardHelper.ReadCard(out ErrMsg);

                if (!string.IsNullOrEmpty(CardNo))
                {
                    bgWorker.ReportProgress(0, CardNo);
                    x--;
                }
                else
                {
                    bgWorker.ReportProgress(0, "");
                }

                Thread.Sleep(1000);

            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.label1.Text = e.UserState.ToString();
        }
    }
}