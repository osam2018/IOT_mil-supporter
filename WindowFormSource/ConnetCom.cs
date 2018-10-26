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
    public partial class ConnetCom : Form
    {
        public int CdnToCnt;

        public ConnetCom()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CdnToCnt = -1;
            this.Close();
        }

        private void ConnetCom_Load(object sender, EventArgs e)
        {
            string[] PortName = SerialPort.GetPortNames();

            this.CBPort.Items.AddRange(PortName);
            this.CBBauad.Items.AddRange(new String[]{"300","1200","2400","4800","9600","19200","38400","57600","74880","115200"});
            this.CBBauad.SelectionStart = 5;
        }
        public string ComName { get { return this.CBPort.Text; } }

        public int Bauad
        {
            get {
                int rt = 0;
                int.TryParse(this.CBBauad.Text, out rt);
                return rt;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CdnToCnt = 1;
            this.Close();
        }

    }
}
