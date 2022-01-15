using System;
using System.Windows.Forms;

namespace Clab
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            this.Load += new EventHandler(downloads_folder.OnLoad);
            username_textbox.Text = Network.username;
            auto_delete_log.Checked = Logging.delete;
            auto_delete_history.Checked = Clab.history.delete;
            downloads_folder.Text = Network.downloadPath;
        }

        private void choose_folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openChooseFolderDialog = new FolderBrowserDialog
            {
                SelectedPath = Network.downloadPath
            };

            using (new CenterWinDialog(this))
            {
                if (openChooseFolderDialog.ShowDialog() == DialogResult.OK && 
                    !string.IsNullOrWhiteSpace(openChooseFolderDialog.SelectedPath))
                {
                    downloads_folder.Text = openChooseFolderDialog.SelectedPath;
                }
            }
        }

        private void settings_save_Click(object sender, EventArgs e)
        {
            username_textbox.Text = Common.remove_spaces(username_textbox.Text);

            if (username_textbox.Text != "" && username_textbox.Text != Network.username)
            {
                //  notify other members of the name change 
                Network.send_message($"{Network.username} changed username to \"{username_textbox.Text}\"", system: true);

                Network.username = username_textbox.Text;
                Clab.settings.add(username_textbox.Text, "username");
                Clab.chat.add_message_to_box($"\nUsername changed to \"{Network.username}\"", AppMessages.info);
            }

            Logging.delete = auto_delete_log.Checked;
            Clab.settings.add(auto_delete_log.Checked, "autoDeleteLog");

            Clab.history.delete = auto_delete_history.Checked;
            Clab.settings.add(auto_delete_history.Checked, "autoDeleteHistory");

            Network.downloadPath = downloads_folder.Text;
            Clab.settings.add(downloads_folder.Text, "downloadPath");

            this.Close();
        }
    }
}
