using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace MillSuppoter
{
    public partial class MainForm : Form
    {
        private SerialPort Com;
        // private Toilet FrmToilet;
        private TltRoom[] TRM;
        private DateTime[] dt;
        private Remote[] RM;

        public MainForm()
        {
         //   FrmToilet = new Toilet();
            InitializeComponent();
            TRM = new TltRoom[5];
            dt = new DateTime[5];
            RM = new Remote[5];

            for (int i = 0; i < 5; i++)
            {
                RM[i] = new Remote();
                TRM[i] = new TltRoom();
                dt[i] = new DateTime();

                RM[i].LbTmp = (Label)GetControlFromName(this.groupBox2,"LBTmp" + (i + 1).ToString());
                RM[i].LbHum = (Label)GetControlFromName(this.groupBox2, "LBHum" + (i + 1).ToString());
                RM[i].LbUnpleased = (Label)GetControlFromName(this.groupBox2, "LBUnpl" + (i + 1).ToString());
                RM[i].PicBox = (PictureBox)GetControlFromName(this.groupBox2, "PcbxAir" + (i + 1).ToString());
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Com = new SerialPort();

            this.Com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Com_DataReceived);

            CheckForIllegalCrossThreadCalls = false;

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {   
            if(Com.IsOpen)
                Com.Close();
        }

        private void 아두이노연결AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnetCom FrmCntCom = new ConnetCom();

            FrmCntCom.ShowDialog();
            if(FrmCntCom.CdnToCnt == 1)
            {
                //Serial 셋팅

                if (ConnectCom(FrmCntCom.ComName,FrmCntCom.Bauad) == -1)
                    MessageBox.Show("아두이노 연결에 실패하였습니다.");

                if (Com.IsOpen == false)
                {
                    MessageBox.Show("연결 안됨");
                }
                else
                {
                    this.Text += " Connected : " + Com.PortName;
                    MessageBox.Show("연결!!");
                    timer1.Start();
                    
                }
            }
            else if(FrmCntCom.CdnToCnt == -1){
                MessageBox.Show("해제");
                this.Text = "스마트 병영";
                DisConnectCom();
            }
        }

        

        public int ConnectCom(string ComNum,int BaudRate)
        {
            
            try
            {
                Com.PortName = ComNum;
                Com.BaudRate = BaudRate;
                Com.DataBits = 8;
                Com.Parity = Parity.None;
                Com.StopBits = StopBits.One;

            }
            catch{
                return -1;
            }

            Com.Open();
            Com.BaseStream.WriteTimeout = 1;
            return 0;
        }

        public void DisConnectCom()
        {
            Com.Close();
        }


        private void DecodeCommand(string C)
        {
            string[] result = C.Split(new char[] { ' ' });

            if (result[0] == "CMD") //명령어일 경우
            {
                if (result[1] == "TLT") //화장실 관련
                {
                    ExcuteTltCommand(Cmd.Substring(Cmd.IndexOf("TLT") + 4));
                }
                else if(result[1] == "REM")
                {
                    ExcuteRemCommand(Cmd.Substring(Cmd.IndexOf("REM") + 4));
                }
            }
        }

        public void ExcuteRemCommand(string cmd)
        {
            string[] Params = cmd.Split(new char[] { ' ' });

            int roomNum = 0;
            int.TryParse(Params[0], out roomNum);

            int roomTmp = 0;
            int.TryParse(Params[1], out roomTmp);

            int roomHum = 0;
            int.TryParse(Params[2], out roomHum);

            int roomUnpl = 0;
            int.TryParse(Params[3], out roomUnpl);

            int AirCondition = 0;
            int.TryParse(Params[4], out AirCondition);

            RM[roomNum].LbTmp.Text = roomTmp.ToString();
            RM[roomNum].LbHum.Text = roomHum.ToString();
            RM[roomNum].LbUnpleased.Text = roomUnpl.ToString();
            if (AirCondition == 1)
            {
                RM[roomNum].AirConStatus = true;
            }
            else
            {
                RM[roomNum].AirConStatus = false;
            }
         
        }

        public void ExcuteTltCommand(string cmd)
        {
            string[] Params = cmd.Split(new char[] { ' ' });

            int roomNum = 0;
            int.TryParse(Params[0], out roomNum);

            int roomStatus = 0;
            int.TryParse(Params[1], out roomStatus);



            TRM[roomNum].Status = roomStatus;

            Invalidate();
        }

        private string Cmd;

        private void Com_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            string msg = sp.ReadExisting();
           
            
            foreach(char c in msg)
            {
                if(c == '\n')
                {
                    DecodeCommand(Cmd);
                    Cmd = "";
                }
                else
                {
                    Cmd += c;
                }
            }

        }

        private Control GetControlFromName(Control frm,string name)
        {
            for (int i = 0; i < frm.Controls.Count; i++)
            {
                if (frm.Controls[i].Name == name)
                    return frm.Controls[i];
            }

            return null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int Tltcnt = 0;
            int Aircnt = 0;

              
            try
            {
                Com.Write("10\n");
            }
            catch { this.Text = "Err"; }

            for (int i = 0;i < 5;i++)
            {
                if (RM[i].AirConStatus)
                    Aircnt++;

                if (TRM[i].IsDoorOpen)
                {
                    Tltcnt++;
                    dt[i] = DateTime.Parse("00:00:00");
                    GetControlFromName(groupBox1, "LBTlt" + (i + 1).ToString()).Text = "비어 있음";
                }
                else
                {
                    dt[i] = dt[i].AddSeconds(1);
                    if(TRM[i].IsPeopleIn == true)
                        GetControlFromName(groupBox1, "LBTlt" + (i + 1).ToString()).Text = "사람 있음";
                    else
                        GetControlFromName(groupBox1, "LBTlt" + (i + 1).ToString()).Text = "비어 있음";
                   
                    if(dt[i].Second == 10)
                    {
                        MessageBox.Show("위험!!");
                    }
                    else if(dt[i].Second >= 10)
                    {
                        TRM[i].Danger = true;
                    }

                }


                PictureBox ConPicture = (PictureBox)GetControlFromName(groupBox1, "pictureBox" + (i + 1).ToString());

                if (TRM[i].Danger)
                    ConPicture.Load(@"alert.png");
                else if (TRM[i].IsWaterOverflow)
                    ConPicture.Load(@"repair.png");
                else
                    ConPicture.Load(@"nomal.png");
                
                GetControlFromName(this.groupBox1, "maskedTextBox" + (i + 1).ToString()).Text = dt[i].ToString("mm:ss");
            }



            this.LBToilet.Text = Tltcnt.ToString() + "/5";
            this.LBAir1.Text = Aircnt.ToString() + "/5";

            //System.Threading.Thread.Sleep(1);
            try
            {
                Com.Write("20 " + this.LBToilet.Text);
            }
            catch { }
            //System.Threading.Thread.Sleep(1);
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
           
            for (int i = 0; i < 5; i++)
            {
         
                TRM[i].DrawRoom(e.Graphics, 100, 20 + (i * (TRM[i].Height + 30)));
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

            groupBox1.Invalidate();
        }


        private void LBHum_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }

    public class Remote
    {
        public Label LbTmp;
        public Label LbHum;
        public Label LbUnpleased;
        public PictureBox PicBox;

        private bool IsAirconOn;

        public bool AirConStatus{
            get{
                return IsAirconOn; 
            }
            set
            {
                if (value == true)
                {
                    IsAirconOn = true;
                    try
                    {
                        PicBox.Load(@"on.png");
                    }
                    catch { }
                }
                else
                {
                    IsAirconOn = false;
                    try
                    {
                        PicBox.Load(@"off.png");

                    }
                    catch { }
                }
            }
        }

        public Remote()
        {
            LbHum = null;
            LbTmp = null;
            LbUnpleased = null;
        }
    }

    public class TltRoom
    {
        private bool DoorStatus;
        private bool WaterStatus;
        public bool Danger;
        public bool Clashed;
        public bool IsPeopleIn;
        private Pen pBlue, pRed, pGreen;
        public int Height { get { return 100; } }
        public int Width { get { return 100; } }

        public bool IsDoorOpen { get { return DoorStatus; } set { DoorStatus = value; } }
        public bool IsWaterOverflow { get { return WaterStatus; } set { WaterStatus = value; } }

        public int Status
        {
            get
            {
                int st  = 0;
                if (IsDoorOpen)
                    st |= 0x01;
                else if (IsPeopleIn)
                    st |= 0x02;
                else if (IsWaterOverflow)
                    st |= 0x04;
                else if (Danger)
                    st |= 0x10;

                return st;
            }
            set
            {
                if((value & 0x01) == 0x01)
                    IsDoorOpen = false;
                else
                    IsDoorOpen = true;

                if ((value & 0x02) == 0x02)
                    IsPeopleIn = true;
                else
                    IsPeopleIn = false;

                if ((value & 0x04) == 0x04)
                    IsWaterOverflow = true;
                else
                    IsWaterOverflow = false;

                if ((value & 0x10) == 0x10)
                    Danger = true;
                else
                    Danger = false;
            }
        }

        public TltRoom()
        {
            IsDoorOpen = true;
            IsWaterOverflow = false;
            Danger = false;
            pBlue = new Pen(Color.Blue);
            pRed = new Pen(Color.Red);
            pGreen = new Pen(Color.Green);
        }

        public void DrawRoom(Graphics g, int x, int y)
        {
            Pen tPen;

            if (this.IsDoorOpen == false && this.IsPeopleIn == true)
                tPen = pGreen;
            else if (this.IsDoorOpen == false)
                tPen = pRed;
            else
                tPen = pBlue;

            g.DrawRectangle(tPen, new Rectangle(x - 1, y - 1, Width + 1, Height + 1));
        }
    }
}

