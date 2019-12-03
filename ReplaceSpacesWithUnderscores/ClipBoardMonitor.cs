using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplaceSpacesWithUnderscores
{

    // https://stackoverflow.com/questions/31258905/how-to-monitor-clipboard-changes-in-c-sharp-to-capture-only-hyperlinks

    public class ClipBoardMonitor : NativeWindow
    {

        private const int WM_DESTROY = 0x2;
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30d;

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("user32.dll")]
        private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public event NewUrlHandler NewUrl;
        public delegate void NewUrlHandler(string txt);

        private IntPtr NextClipBoardViewerHandle;

        public ClipBoardMonitor()
        {
            this.CreateHandle(new CreateParams());
            this.NextClipBoardViewerHandle = SetClipboardViewer(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (Clipboard.ContainsText())
                    {
                        string txt = Clipboard.GetText();
                        if (this.NewUrl != null && this.IsValidUrl(txt))
                        {
                            this.NewUrl(txt);
                        }
                    }
                    SendMessage(this.NextClipBoardViewerHandle, m.Msg, m.WParam, m.LParam);

                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam.Equals(this.NextClipBoardViewerHandle))
                    {
                        this.NextClipBoardViewerHandle = m.LParam;
                    }
                    else if (!this.NextClipBoardViewerHandle.Equals(IntPtr.Zero))
                    {
                        SendMessage(this.NextClipBoardViewerHandle, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                case WM_DESTROY:
                    ChangeClipboardChain(this.Handle, this.NextClipBoardViewerHandle);
                    break;

            }

            base.WndProc(ref m);
        }

        private bool IsValidUrl(string txt)
        {
            Uri uriResult;
            return Uri.TryCreate(txt, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

    }
}
