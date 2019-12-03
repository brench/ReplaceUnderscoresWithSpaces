using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplaceSpacesWithUnderscores
{
    public partial class Form1 : Form
    {
        // https://www.fluxbytes.com/csharp/how-to-monitor-for-clipboard-changes-using-addclipboardformatlistener/

        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        // See http://msdn.microsoft.com/en-us/library/ms633541%28v=vs.85%29.aspx
        // See http://msdn.microsoft.com/en-us/library/ms649033%28VS.85%29.aspx
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //private ClipBoardMonitor cbm = null;

        /// <summary>
        /// Occurs when the contents of the clipboard is updated.
        /// </summary>
        //public static event EventHandler ClipboardUpdate;

        private string _userStoryString = null;
        private string _taskString = null;

        public Form1()
        {
            InitializeComponent();

            //NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
            //NativeMethods.AddClipboardFormatListener(Handle);

            AddClipboardFormatListener(this.Handle);
            

            //cbm = new ClipBoardMonitor();
            //cbm.NewUrl += Cbm_NewUrl;

            //ClipboardNotification.ClipboardUpdate += ClipboardNotification_ClipboardUpdate;
        }

        //private void ClipboardNotification_ClipboardUpdate(object sender, EventArgs e)
        //{
        //    textBox1.Text = Clipboard.GetText();
        //}

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                //OnClipboardUpdate(null);
                IDataObject iData = Clipboard.GetDataObject();      // Clipboard's data.

                /* Depending on the clipboard's current data format we can process the data differently. 
                 * Feel free to add more checks if you want to process more formats. */
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)iData.GetData(DataFormats.Text);
                    // do something with it
                    if (text.Contains("User Story"))
                    {
                        _userStoryString = text.Replace("User Story", string.Empty).Trim();
                        _userStoryString = _userStoryString.Insert(0, "feature/");
                        _userStoryString = _userStoryString.Replace(":", string.Empty);
                        _userStoryString = _userStoryString.Replace(" ", "_");
                    }
                    else if (_userStoryString != null) // Task
                    {
                        _taskString = text.Replace("Task", string.Empty).Trim();
                        _taskString = _taskString.Replace(":", string.Empty);
                        _taskString = _taskString.Replace(" ", "_");
                        var stringForClipboard = $"{_userStoryString}.{_taskString}";
                        _userStoryString = null;
                        _taskString = null;
                        Clipboard.SetText(stringForClipboard);
                    }


                }
                else if (iData.GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap image = (Bitmap)iData.GetData(DataFormats.Bitmap);
                    // do something with it
                }
            }
            base.WndProc(ref m);
        }

        ///// <summary>
        ///// Raises the <see cref="ClipboardUpdate"/> event.
        ///// </summary>
        ///// <param name="e">Event arguments for the event.</param>
        //private static void OnClipboardUpdate(EventArgs e)
        //{
        //    var handler = ClipboardUpdate;
        //    if (handler != null)
        //    {
        //        handler(null, e);
        //    }
        //}

        private void Cbm_NewUrl(string txt)
        {
            this.textBox1.Text = txt;
        }

        private void ChangeClipboardText()
        {
            var clipboardText = Clipboard.GetText();
            
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var text = textBox1.Text;

            if (Clipboard.GetText().Contains("feature/"))
            {

            }

            if (text.Contains("User Story"))
            {
                text = textBox1.Text.Replace("User Story", string.Empty).Trim();
                text = text.Insert(0, "feature/");
            }
            else // Task
            {
                text = text.Replace("Task", string.Empty);
            }

            text = text.Replace(":", string.Empty);
            text = text.Replace(" ", "_");

            Clipboard.SetText(text.Trim());
            textBox1.SelectAll();
        }
    }

    //internal static class NativeMethods
    //{
    //    // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
    //    public const int WM_CLIPBOARDUPDATE = 0x031D;
    //    public static IntPtr HWND_MESSAGE = new IntPtr(-3);

    //    // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
    //    [DllImport("user32.dll", SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    public static extern bool AddClipboardFormatListener(IntPtr hwnd);

    //    // See http://msdn.microsoft.com/en-us/library/ms633541%28v=vs.85%29.aspx
    //    // See http://msdn.microsoft.com/en-us/library/ms649033%28VS.85%29.aspx
    //    [DllImport("user32.dll", SetLastError = true)]
    //    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    //}
}
