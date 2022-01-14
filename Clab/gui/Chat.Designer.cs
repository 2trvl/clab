
namespace Clab
{
    partial class Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.input_message_box = new System.Windows.Forms.TextBox();
            this.send_message_button = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.attachFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messages_rich_box = new ExRichTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // input_message_box
            // 
            this.input_message_box.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.input_message_box.Location = new System.Drawing.Point(12, 391);
            this.input_message_box.Name = "input_message_box";
            this.input_message_box.Size = new System.Drawing.Size(429, 29);
            this.input_message_box.TabIndex = 0;
            // 
            // send_message_button
            // 
            this.send_message_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.send_message_button.Location = new System.Drawing.Point(456, 391);
            this.send_message_button.Name = "send_message_button";
            this.send_message_button.Size = new System.Drawing.Size(83, 29);
            this.send_message_button.TabIndex = 1;
            this.send_message_button.Text = "Send";
            this.send_message_button.UseVisualStyleBackColor = true;
            this.send_message_button.Click += new System.EventHandler(this.send_message_GUI);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attachFileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(552, 27);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // attachFileToolStripMenuItem
            // 
            this.attachFileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.attachFileToolStripMenuItem.Name = "attachFileToolStripMenuItem";
            this.attachFileToolStripMenuItem.Size = new System.Drawing.Size(85, 23);
            this.attachFileToolStripMenuItem.Text = "Attach File";
            this.attachFileToolStripMenuItem.Click += new System.EventHandler(this.Attach_File_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(70, 23);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.Settings_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(59, 23);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.About_Click);
            // 
            // messages_rich_box
            // 
            this.messages_rich_box.AddInnerMargins = true;
            this.messages_rich_box.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.messages_rich_box.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.messages_rich_box.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.messages_rich_box.Location = new System.Drawing.Point(12, 39);
            this.messages_rich_box.Name = "messages_rich_box";
            this.messages_rich_box.OffKeyPressAlerts = true;
            this.messages_rich_box.ReadOnly = true;
            this.messages_rich_box.Selectable = false;
            this.messages_rich_box.ShortcutsEnabled = false;
            this.messages_rich_box.Size = new System.Drawing.Size(527, 341);
            this.messages_rich_box.TabIndex = 2;
            this.messages_rich_box.Text = "";
            //  ExRichTextBox settings
            this.messages_rich_box.Selectable = false;
            this.messages_rich_box.OffKeyPressAlerts = true;
            this.messages_rich_box.AddInnerMargins = true;
            this.messages_rich_box.padding = new int[4] { 8, 5, 8, 6 };
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 435);
            this.Controls.Add(this.messages_rich_box);
            this.Controls.Add(this.send_message_button);
            this.Controls.Add(this.input_message_box);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Chat";
            this.Text = "Clab";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button send_message_button;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attachFileToolStripMenuItem;
        private System.Windows.Forms.TextBox input_message_box;
        private ExRichTextBox messages_rich_box;
    }
}