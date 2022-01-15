using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clab
{
    public partial class Chat : Form
    {
        public Chat()
        {
            InitializeComponent();
            this.Load += new EventHandler(messages_rich_box.OnLoad);
            this.AcceptButton = send_message_button;
            add_message_to_box("Welcome to the Clab, buddy!", AppMessages.info);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                Network.destructor();
                Logging.destructor();
                Clab.history.destructor();
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.StartPosition = FormStartPosition.CenterParent;
            settings.ShowDialog(this);
        }

        private void About_Click(object sender, EventArgs e)
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show($"Your PC: {Network.IP}\nRelease: Clab v0.0.1", "About");
            }
        }

        /// <summary>adds colored string to chatbox</summary>
        public void add_message_to_box(string message, Color? color = null)
        {
            if (InvokeRequired)  //  add message from another thread
            {
                this.Invoke(new Action<string, Color?>(add_message_to_box), new object[] { message, color });
                return;
            }

            color ??= Color.Black;
            messages_rich_box.AppendText(message, color.Value);
        }

        private void send_message_GUI(object sender, EventArgs e)
        {
            input_message_box.Text = Common.remove_spaces(input_message_box.Text);

            if (input_message_box.Text != "")
            {
                Network.send_message(input_message_box.Text);
                input_message_box.Text = "";
            }

            this.ActiveControl = input_message_box;
        }

        private void Attach_File_Click(object sender, EventArgs e)
        {
            OpenFileDialog openAttachFileDialog = new OpenFileDialog
            {
                Title = "Attach File",

                CheckFileExists = true,
                CheckPathExists = true,

                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            using (new CenterWinDialog(this))
            {
                if (openAttachFileDialog.ShowDialog() == DialogResult.OK)
                {
                    input_message_box.Text = Common.remove_spaces(input_message_box.Text);
                    Network.send_message(input_message_box.Text, openAttachFileDialog.FileName);
                    input_message_box.Text = "";
                }
            }
        }
    }
}
