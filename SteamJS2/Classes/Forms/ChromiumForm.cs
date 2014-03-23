using System;
using System.Diagnostics;
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
        private CefWebBrowser devBrowser;
        private string devBrowserUrl = null;

        public ChromiumForm(string url)
        {
            InitializeComponent();

            webBrowser = new CefWebBrowser();
            webBrowser.StartUrl = url;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.BrowserSettings = new CefBrowserSettings
                                         {
                                             JavaScriptAccessClipboard = CefState.Enabled,
                                             //UniversalAccessFromFileUrls = CefState.Enabled,
                                             //FileAccessFromFileUrls = CefState.Enabled
                                         };
            webBrowser.BrowserCreated += WebBrowserOnBrowserCreated;
            splitContainer1.Panel1.Controls.Add(webBrowser);

            devBrowser = new CefWebBrowser();
            devBrowser.StartUrl = "about:blank";
            devBrowser.Dock = DockStyle.Fill;
            devBrowser.BrowserCreated += DevBrowserOnBrowserCreated;
            splitContainer1.Panel2.Controls.Add(devBrowser);
            splitContainer1.Panel2Collapsed = true;

            webBrowser.PreviewKeyDown += OnKeyDown;
            devBrowser.PreviewKeyDown += OnKeyDown;
        }

        private void WebBrowserOnBrowserCreated(object sender, EventArgs e)
        {
        }

        private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                if (splitContainer1.Panel2Collapsed)
                    OpenDevTools();
                else
                    CloseDevTools();
            }
        }

        private void DevBrowserOnBrowserCreated(object sender, EventArgs eventArgs)
        {
            if (devBrowserUrl != null)
            {
                devBrowser.Browser.GetMainFrame().LoadUrl(devBrowserUrl);
            }
        }

        private void OpenDevTools()
        {
            var devToolsUrl = webBrowser.Browser.GetHost().GetDevToolsUrl(true);

            if (devBrowser.Address != devToolsUrl)
            {
                if (devBrowser.Browser != null)
                    devBrowser.Browser.GetMainFrame().LoadUrl(devToolsUrl);
                else
                    devBrowserUrl = devToolsUrl;
            }

            splitContainer1.Panel2Collapsed = false;
        }

        private void CloseDevTools()
        {
            splitContainer1.Panel2Collapsed = true;
        }
    }
}