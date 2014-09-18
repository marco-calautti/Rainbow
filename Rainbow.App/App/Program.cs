//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Rainbow.ImgLib;
using Rainbow.ImgLib.Formats;
using Rainbow.ImgLib.Formats.Serialization;
using Rainbow.ImgLib.Formats.Serialization.Metadata;
using Rainbow.App.GUI;

namespace Rainbow.App
{
    class Program
    {
        private static void PrintUsage()
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("Rainbow: a texture format converter (" + Application.ProductVersion + ")");
            writer.WriteLine("(C) 2014+ Marco (Phoenix) Calautti.");
            writer.WriteLine();
            writer.WriteLine("Usage: Rainbow.exe [--help] [--export] [--import] [<args>]");
            writer.WriteLine();
            writer.WriteLine("--help\tDisplays this help message\n");
            writer.WriteLine("--export <texture> [output]\n\tConverts the given texture into pngs and saves a metadata xml file into \"output\" or in a file with the same name as <texture> if [output] is not specified\n");
            writer.WriteLine("--import <xml> [output]\n\tConverts the given xml file + pngs into the origianl texture format and saves it to \"output\" or in a file with the same name as <texture> if [output] is not specified\n");

            writer.WriteLine("Examples:");
            writer.WriteLine("Rainbow.exe texture.tm2\n\tOpens the TIM2 in the GUI program\n");
            writer.WriteLine("Rainbow.exe --export texture.tm2 \n\tExports the TIM2 into pngs and saves the metadata file texture.xml\n");
            MessageBox.Show(writer.ToString(), "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            Form form = args.Length > 0 ? new MainForm(args[0]) : new MainForm();

            form.StartPosition = FormStartPosition.CenterScreen;
            Application.Run(form);
        }
    }
}
