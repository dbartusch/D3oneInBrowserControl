using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        ChromiumRequestHandler requestHandler = null;
        public Form2()
        {
            InitializeComponent();
            requestHandler = new ChromiumRequestHandler();
            chromiumWebBrowser1.RequestHandler = requestHandler;

        }
        public void Navigate(string url, string sessionId)
        {
            requestHandler.SetSessionId(sessionId);
            chromiumWebBrowser1.Load(url);
        }
    }
}
