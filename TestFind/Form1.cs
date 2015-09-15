using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.VideoStab;
using System.Threading;

namespace TestFind
{
    public partial class Form1 : Form
    {
        private Capture cap = new Capture(0);
        Image<Bgr, Byte> originalImg;
        Image<Gray, Byte> grayImg;
        System.Windows.Forms.Timer times;
        long s = 0;

        private int height;
        private int width;
        
        public Form1()
        {
            InitializeComponent();
        }

        void OnlineCam()
        {
            while (true)
            {
                if (pictureBox1.InvokeRequired)
                {
                    pictureBox1.Image = cap.QueryFrame().ToBitmap();
                    originalImg = new Image<Bgr, byte>(cap.QueryFrame().ToBitmap());
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            height = cap.Height;
            width = cap.Width;
            Thread t = new Thread(OnlineCam);
            t.Start();
            times = new System.Windows.Forms.Timer();
            times.Interval = 1000;// dat 1000 ms
            times.Tick += new EventHandler(TimerOnTick);
            times.Start();
        }

        public void NhiPhanHoa()
        {
            byte[, ,] data = originalImg.Data;
            byte[,,] result = new byte[height,width,1];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (data[i, j, 0] >= 105 && data[i, j, 0] <= 240)  //b
                    {
                        if (data[i, j, 1] >= 85 && data[i, j, 1] <= 198) //g
                        {
                            if (data[i, j, 2] >= 40 && data[i, j, 2] <= 130) //r
                            {
                                result[i, j, 0] = 255;
                            }
                            else
                            {
                                result[i, j, 0] = 0;
                            }
                        }
                        else
                        {
                            result[i, j, 0] = 0;
                        }
                    }
                    else
                    {
                        result[i, j, 0] = 0;
                    }
                }
            }

            grayImg = new Image<Gray, byte>(result);
            //pictureBox3.Image = grayImg.ToBitmap();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnCap_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save(@"C:\result.png");
            label1.Enabled = true;
        }

        private void cogian()
        {
            Image<Gray, Byte> eroded = new Image<Gray, byte>(grayImg.Size);
            Image<Gray, Byte> temp = new Image<Gray, byte>(grayImg.Size);
            Image<Gray, Byte> skel = new Image<Gray, byte>(grayImg.Size);
            //skel.SetValue(0);
            // CvInvoke.cvThreshold(grayImg, grayImg, 127, 256, 0);
            StructuringElementEx element = new StructuringElementEx(10, 10, 1, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_CROSS);
            CvInvoke.cvErode(grayImg, eroded, element, 1);
            CvInvoke.cvDilate(eroded, temp, element, 1);
            temp = grayImg.Sub(temp);
            skel = skel | temp;
            grayImg = eroded;
            pictureBox3.Image = grayImg.ToBitmap();
        }

        private void TimerOnTick(object sender, EventArgs args)
        {
            if (s % 5 == 0)
            {
                NhiPhanHoa();
                cogian();
            }
            lbTimes.Text = s.ToString();
            s++;
        }
    }
}
