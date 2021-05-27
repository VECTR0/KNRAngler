using System;
using System.Windows.Forms;

namespace KNRAnglerN
{
    public partial class ConsoleForm : Form
    {
        MainForm instance;

        public string Log { get { return "nothing"; } set
            {
                txtConsole.AppendText(value);
                txtConsole.AppendText(Environment.NewLine);
            }
        }

        public ConsoleForm(MainForm mainForm)
        {
            InitializeComponent();
            instance = mainForm;
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            foreach (string s in Enum.GetNames(typeof(MainForm.Packet))) cmbPacket.Items.Add(s);
        }


        private void cmbPacket_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Enum.Parse(typeof(MainForm.Packet), cmbPacket.Text))
            {
                case MainForm.Packet.SET_MTR:
                    txtJson.Text = "{\"FL\":0.0,\"FR\":0.0,\"ML\":0.0,\"MR\":0.0,\"B\":0.0}";
                    break;
                case MainForm.Packet.SET_ORIEN:
                    txtJson.Text = "{ \"rot\":{ \"x\":0,\"y\":0,\"z\":0},\"pos\":{ \"x\":0,\"y\":0,\"z\":0} }";
                    break;
                case MainForm.Packet.CHK_AP:
                    txtJson.Text = "{\"id\":\"\"}";
                    break;
                default:
                    txtJson.Text = "{}";
                    break;
            }
        }

        private void btnSendJson_Click(object sender, EventArgs e)
        {
            int maxLength = 0;
            foreach (var s in Enum.GetNames(typeof(MainForm.Packet))) maxLength = Math.Max(maxLength, s.Length);
            Log = "SENT [" + cmbPacket.Text.PadRight(maxLength) + "] " + txtJson.Text;
            try
            {
                instance.okonClient.EnqueuePacket((byte)Enum.Parse(typeof(MainForm.Packet), cmbPacket.Text), txtJson.Text);
            }
            catch
            {
                Log = "Error, packet not sent";
            }
        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void txtJson_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) btnSendJson_Click(sender, e);
        }
    }
}
