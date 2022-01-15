namespace Clab
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.auto_delete_history = new System.Windows.Forms.CheckBox();
            this.auto_delete_log = new System.Windows.Forms.CheckBox();
            this.Auto_Delete_Group = new System.Windows.Forms.GroupBox();
            this.username_textbox = new System.Windows.Forms.TextBox();
            this.username_group = new System.Windows.Forms.GroupBox();
            this.save_settings_button = new System.Windows.Forms.Button();
            this.downloads_group = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.downloads_folder = new ExRichTextBox();
            this.Auto_Delete_Group.SuspendLayout();
            this.username_group.SuspendLayout();
            this.downloads_group.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // auto_delete_history
            // 
            this.auto_delete_history.AutoSize = true;
            this.auto_delete_history.Location = new System.Drawing.Point(6, 47);
            this.auto_delete_history.Name = "auto_delete_history";
            this.auto_delete_history.Size = new System.Drawing.Size(64, 19);
            this.auto_delete_history.TabIndex = 0;
            this.auto_delete_history.Text = "History";
            this.auto_delete_history.UseVisualStyleBackColor = true;
            // 
            // auto_delete_log
            // 
            this.auto_delete_log.AutoSize = true;
            this.auto_delete_log.Location = new System.Drawing.Point(6, 22);
            this.auto_delete_log.Name = "auto_delete_log";
            this.auto_delete_log.Size = new System.Drawing.Size(46, 19);
            this.auto_delete_log.TabIndex = 1;
            this.auto_delete_log.Text = "Log";
            this.auto_delete_log.UseVisualStyleBackColor = true;
            // 
            // Auto_Delete_Group
            // 
            this.Auto_Delete_Group.Controls.Add(this.auto_delete_history);
            this.Auto_Delete_Group.Controls.Add(this.auto_delete_log);
            this.Auto_Delete_Group.Location = new System.Drawing.Point(12, 74);
            this.Auto_Delete_Group.Name = "Auto_Delete_Group";
            this.Auto_Delete_Group.Size = new System.Drawing.Size(195, 74);
            this.Auto_Delete_Group.TabIndex = 2;
            this.Auto_Delete_Group.TabStop = false;
            this.Auto_Delete_Group.Text = "Auto Delete";
            // 
            // username_textbox
            // 
            this.username_textbox.Location = new System.Drawing.Point(7, 22);
            this.username_textbox.Name = "username_textbox";
            this.username_textbox.Size = new System.Drawing.Size(182, 23);
            this.username_textbox.TabIndex = 3;
            // 
            // username_group
            // 
            this.username_group.Controls.Add(this.username_textbox);
            this.username_group.Location = new System.Drawing.Point(12, 12);
            this.username_group.Name = "username_group";
            this.username_group.Size = new System.Drawing.Size(195, 56);
            this.username_group.TabIndex = 4;
            this.username_group.TabStop = false;
            this.username_group.Text = "Username";
            // 
            // save_settings_button
            // 
            this.save_settings_button.Location = new System.Drawing.Point(12, 216);
            this.save_settings_button.Name = "save_settings_button";
            this.save_settings_button.Size = new System.Drawing.Size(195, 23);
            this.save_settings_button.TabIndex = 5;
            this.save_settings_button.Text = "Apply Settings";
            this.save_settings_button.UseVisualStyleBackColor = true;
            this.save_settings_button.Click += new System.EventHandler(this.settings_save_Click);
            // 
            // downloads_group
            // 
            this.downloads_group.Controls.Add(this.panel1);
            this.downloads_group.Location = new System.Drawing.Point(12, 154);
            this.downloads_group.Name = "downloads_group";
            this.downloads_group.Size = new System.Drawing.Size(195, 56);
            this.downloads_group.TabIndex = 6;
            this.downloads_group.TabStop = false;
            this.downloads_group.Text = "Save Files To";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.downloads_folder);
            this.panel1.Location = new System.Drawing.Point(7, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(183, 23);
            this.panel1.TabIndex = 7;
            // 
            // downloads_folder
            // 
            this.downloads_folder.BackColor = System.Drawing.SystemColors.Window;
            this.downloads_folder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.downloads_folder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloads_folder.ForeColor = System.Drawing.SystemColors.WindowText;
            this.downloads_folder.Location = new System.Drawing.Point(0, 0);
            this.downloads_folder.Multiline = false;
            this.downloads_folder.Name = "downloads_folder";
            this.downloads_folder.ReadOnly = true;
            this.downloads_folder.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.downloads_folder.ShortcutsEnabled = false;
            this.downloads_folder.Size = new System.Drawing.Size(181, 23);
            this.downloads_folder.TabIndex = 0;
            this.downloads_folder.Text = "";
            //  ExRichTextBox settings
            this.downloads_folder.Selectable = false;
            this.downloads_folder.OffKeyPressAlerts = true;
            this.downloads_folder.AddInnerMargins = true;
            this.downloads_folder.padding = new int[4] { 2, 2, 0, 0 };
            this.downloads_folder.AddInnerButton = true;
            this.downloads_folder.buttonSize = new int[2] { 20, 8 };
            this.downloads_folder.buttonImage = new string[3] { "Clab.Settings", "open-folder", "icon" };
            this.downloads_folder.buttonActions = choose_folder_Click;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 251);
            this.Controls.Add(this.downloads_group);
            this.Controls.Add(this.save_settings_button);
            this.Controls.Add(this.username_group);
            this.Controls.Add(this.Auto_Delete_Group);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.Auto_Delete_Group.ResumeLayout(false);
            this.Auto_Delete_Group.PerformLayout();
            this.username_group.ResumeLayout(false);
            this.username_group.PerformLayout();
            this.downloads_group.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox auto_delete_history;
        private System.Windows.Forms.CheckBox auto_delete_log;
        private System.Windows.Forms.GroupBox Auto_Delete_Group;
        private System.Windows.Forms.TextBox username_textbox;
        private System.Windows.Forms.GroupBox username_group;
        private System.Windows.Forms.Button save_settings_button;
        private System.Windows.Forms.GroupBox downloads_group;
        private ExRichTextBox downloads_folder;
        private System.Windows.Forms.Panel panel1;
    }
}
