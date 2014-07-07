using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Rainbow.ImgLib;
using Rainbow.ImgLib.Formats;
using Rainbow.ImgLib.Formats.Serializers;
using Rainbow.App.GUI;

namespace Rainbow.App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Form form = new MainForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            Application.Run(form);
        }
    }
}
