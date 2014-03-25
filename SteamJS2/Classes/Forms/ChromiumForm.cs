using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SteamJS2.JavascriptBindings;
using Xilium.CefGlue;
using Xilium.CefGlue.WindowsForms;

namespace SteamJS2.Forms
{
    public partial class ChromiumForm : Form
    {
        private CefWebBrowser webBrowser;
        private bool isDeveloperForm;

        private ChromiumForm devForm;

        public ChromiumForm(string url, bool isDeveloperForm = false)
        {
            InitializeComponent();

            this.isDeveloperForm = isDeveloperForm;

            CreateWebBrowser(url);

            // Hide form on close if developer form.
            if (isDeveloperForm)
            {
                Closing += (sender, args) =>
                {
                    args.Cancel = true;
                    Hide();
                };
            }
        }

        private void CreateWebBrowser(string url)
        {
            webBrowser = new CefWebBrowser();
            webBrowser.StartUrl = url;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.BrowserSettings = new CefBrowserSettings
                                         {
                                             JavaScriptAccessClipboard = CefState.Enabled,
                                             //UniversalAccessFromFileUrls = CefState.Enabled,
                                             //FileAccessFromFileUrls = CefState.Enabled
                                         };
            webBrowser.TitleChanged += OnTitleChanged;
            Controls.Add(webBrowser);

            webBrowser.PreviewKeyDown += OnKeyDown;
        }

        private void OnTitleChanged(object sender, TitleChangedEventArgs e)
        {
            Text = e.Title;
        }

        private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                if (isDeveloperForm)
                {
                    Hide();
                    return;
                }

                if (devForm == null || !devForm.Visible)
                    OpenDevTools();
                else
                    CloseDevTools();
            }
        }

        private void OpenDevTools()
        {
            var devToolsUrl = webBrowser.Browser.GetHost().GetDevToolsUrl(true);

            if (devForm == null)
            {
                devForm = new ChromiumForm(devToolsUrl, true);
                Rectangle resolution = Screen.FromControl(this).Bounds;
                devForm.Size = new Size((int)(resolution.Width * 0.6f), (int)(resolution.Height * 0.6f));
                devForm.CenterToScreen();
                devForm.Show();
            }
            else
            {
                devForm.Show();
            }
        }

        private void CloseDevTools()
        {
            if (devForm != null)
            {
                devForm.Hide();
            }
        }
    }
}