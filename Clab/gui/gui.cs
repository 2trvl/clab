using System;
using System.Text;
using System.Drawing;
using System.Resources;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Clab
{
    /// <summary>allows to center dialog window relative to the parent form</summary>
    class CenterWinDialog : IDisposable
    {
        private int mTries = 0;
        private Form mOwner;

        public CenterWinDialog(Form owner)
        {
            mOwner = owner;
            owner.BeginInvoke(new MethodInvoker(findDialog));
        }

        private void findDialog()
        {
            // Enumerate windows to find the message box
            if (mTries < 0) return;
            EnumThreadWndProc callback = new EnumThreadWndProc(checkWindow);
            if (EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero))
            {
                if (++mTries < 10) mOwner.BeginInvoke(new MethodInvoker(findDialog));
            }
        }

        private bool checkWindow(IntPtr hWnd, IntPtr lp)
        {
            // Checks if <hWnd> is a dialog
            StringBuilder sb = new StringBuilder(260);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString() != "#32770") return true;
            // Got it
            Rectangle frmRect = new Rectangle(mOwner.Location, mOwner.Size);
            RECT dlgRect;
            GetWindowRect(hWnd, out dlgRect);
            MoveWindow(hWnd,
                       frmRect.Left + (frmRect.Width - dlgRect.Right + dlgRect.Left) / 2,
                       frmRect.Top + (frmRect.Height - dlgRect.Bottom + dlgRect.Top) / 2,
                       dlgRect.Right - dlgRect.Left,
                       dlgRect.Bottom - dlgRect.Top, true);
            return false;
        }

        public void Dispose()
        {
            mTries = -1;
        }

        // P/Invoke declarations
        private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lp);
        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int tid, EnumThreadWndProc callback, IntPtr lp);
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder buffer, int buflen);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
    }

    /// <summary>custom text box with alerts muting, selection discarding and inner margins & buttons</summary>
    public class ExRichTextBox : RichTextBox
    {
        [DefaultValue(true)]
        public bool Selectable { get; set; }

        [DefaultValue(false)]
        public bool OffKeyPressAlerts { get; set; }

        [DefaultValue(false)]
        public bool AddInnerMargins { get; set; }

        [DefaultValue(false)]
        public bool AddInnerButton { get; set; }

        /// <summary>int: left, top, right, bottom</summary>
        public int[] padding = new int[4] { 0, 0, 0, 0 };
        /// <summary>int: width, font</summary>
        public int[] buttonSize = new int[2] { 25, 8 };
        /// <summary>string: text</summary>
        public string buttonText = null;
        /// <summary>string: Namespace.Form, .resx element name, "image" or "icon"</summary>
        public string[] buttonImage = new string[3] { null, null, null };
        /// <summary>delegate for buttonActions</summary>
        public delegate void buttonAction(object sender, EventArgs e);
        /// <summary>void func(object sender, EventArgs e)</summary>
        public buttonAction buttonActions = (object sender, EventArgs e) => { MessageBox.Show("Sample func, just overwrite it"); };

        /// <summary>Add this to your Form Load Event</summary>
        public void OnLoad(object sender, EventArgs e)
        {
            if (OffKeyPressAlerts)
            {
                this.KeyPress += new KeyPressEventHandler(sound_off);
            }

            if (AddInnerMargins)
            {
                SetInnerMargins(padding[0], padding[1], padding[2], padding[3]);
            }

            // doesn't work for multiline properly
            if (AddInnerButton)
            {
                Button innerBtn = new Button();
                innerBtn.Dock = DockStyle.Right;
                innerBtn.Cursor = Cursors.Default;
                innerBtn.FlatAppearance.BorderSize = 0;
                innerBtn.Size = new Size(buttonSize[0], this.ClientSize.Height + 2);
                innerBtn.Click += new EventHandler(buttonActions);

                if (buttonText != null)
                {
                    innerBtn.Text = buttonText;
                    innerBtn.Font = new Font("Calibri", buttonSize[1]);
                }

                if (buttonImage[0] != null)
                {
                    innerBtn.ForeColor = Color.White;
                    innerBtn.FlatStyle = FlatStyle.Flat;

                    ResourceManager resources = new ResourceManager(buttonImage[0], this.GetType().Assembly);

                    if (buttonImage[2] == "image")
                        innerBtn.BackgroundImage = (Image)resources.GetObject(buttonImage[1]);
                    else
                        innerBtn.BackgroundImage = ((Icon)resources.GetObject(buttonImage[1])).ToBitmap();

                    innerBtn.BackgroundImageLayout = ImageLayout.Zoom;
                }

                this.Controls.Add(innerBtn);

                //  prevent text from disappearing underneath the button
                SendMessage(this.Handle, EM_SETMARGINS, (IntPtr)2, (IntPtr)(innerBtn.Width << 16));
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETFOCUS && !Selectable)
                m.Msg = WM_KILLFOCUS;

            base.WndProc(ref m);
        }

        private void sound_off(Object o, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>Append colored text</summary>
        public void AppendText(string text, Color color)
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;

            this.SelectionColor = color;
            this.AppendText(text);
            this.SelectionColor = this.ForeColor;
        }

        public void SetInnerMargins(int left, int top, int right, int bottom)
        {
            var rect = this.GetFormattingRect();

            var newRect = new Rectangle(left, top, rect.Width - left - right, rect.Height - top - bottom);
            this.SetFormattingRect(newRect);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }
        }

        private void SetFormattingRect(Rectangle rect)
        {
            var rc = new RECT(rect);
            SendMessageRectSet(this.Handle, EmSetrect, 0, ref rc);
        }

        private Rectangle GetFormattingRect()
        {
            var rect = new Rectangle();
            SendMessageRectGet(this.Handle, EmGetrect, (IntPtr)0, ref rect);
            return rect;
        }

        const int EmGetrect = 0xB2;
        const int EmSetrect = 0xB3;

        const int WM_SETFOCUS = 0x0007;
        const int WM_KILLFOCUS = 0x0008;

        const int EM_SETMARGINS = 0xd3;

        [DllImport(@"user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport(@"User32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessageRectSet(IntPtr hWnd, uint msg, int wParam, ref RECT rect);

        [DllImport(@"user32.dll", EntryPoint = @"SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessageRectGet(IntPtr hwnd, int wMsg, IntPtr wParam, ref Rectangle lParam);
    }

    /// <summary>aliases for chatbox colored messages</summary>
    public static class AppMessages
    {
        public static Color error = Color.Red;
        public static Color info = Color.Gray;
        public static Color warning = Color.DarkOrange;
    }

    public static partial class Common
    {
        static readonly Regex messagePattern = new Regex(@"\s\s+", RegexOptions.Compiled);

        public static string remove_spaces(string text)
        {
            text = messagePattern.Replace(text, " ");
            text = text.Trim(' ');
            return text;
        }
    }
}
