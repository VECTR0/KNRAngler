using System;
using System.Windows.Forms;

namespace KNRAnglerN
{
    public partial class SettingsForm : Form
    {
        private readonly MainForm mainFormInstance;
        public SettingsForm(MainForm mainForm)
        {
            InitializeComponent();
            mainFormInstance = mainForm;
        }

        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void btnConnectJson_Click(object sender, EventArgs e)
        {
            
            mainFormInstance.consoleForm.Show();
            btnConnectJson.Enabled = false;
            if (mainFormInstance.okonClient != null)
            {
                if (mainFormInstance.okonClient.IsConnected())
                {
                    mainFormInstance.okonClient.Dispose();
                    mainFormInstance.okonClient = null;
                    btnConnectJson.Enabled = true;
                    btnConnectJson.Text = "Connect";
                    mainFormInstance.consoleForm.Log = "Disconnected";
                    return;
                }

                mainFormInstance.okonClient.Dispose();
            }
            try
            {
                mainFormInstance.consoleForm.txtConsole.Clear();
                mainFormInstance.okonClient = new OkonClient(txtIp.Text, ushort.Parse(txtJsonPort.Text), new MainForm.Info(mainFormInstance));
            }
            catch
            {
                mainFormInstance.consoleForm.Log = "Error, wrong parameters";
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                return;
            }
            mainFormInstance.okonClient.PacketReceived += mainFormInstance.HandleReceivedPacket;
            try
            {
                mainFormInstance.okonClient.Connect();
            }
            catch
            {
                mainFormInstance.okonClient.Dispose();
                mainFormInstance.okonClient = null;
                btnConnectJson.Enabled = true;
                btnConnectJson.Text = "Connect";
                return;
            }
            btnConnectJson.Text = "Disconnect";
            btnConnectJson.Enabled = true;
            mainFormInstance.consoleForm.Log = "Connected";
            mainFormInstance.requestedVideoFeedFrames = 0;
            mainFormInstance.requestedDepthMapFrames = 0;
        }

        private void chkDepth_CheckedChanged(object sender, EventArgs e)
        {
            mainFormInstance.picDepthMap.Visible = chkDepth.Checked;
        }
    }
}
