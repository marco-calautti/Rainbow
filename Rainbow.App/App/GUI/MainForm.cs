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

using Rainbow.App.GUI.Controls;
using Rainbow.App.GUI.Model;
using Rainbow.ImgLib.Formats;
using Rainbow.ImgLib.Formats.Serialization;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Rainbow.App.GUI
{
    public partial class MainForm : Form
    {
        private TextureFormat texture;
        private TextureFormatSerializer serializer;
        private string filename;
        private ListViewItemComparer listviewSorter = new ListViewItemComparer();

        public MainForm(string filename)
            : this()
        {
            try
            {
                using (Stream s = File.Open(filename, FileMode.Open))
                    OpenImportStream(s, filename, TextureFormatMode.Format);

                FillListView(new string[] { filename });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            splitContainer1.Panel2.MouseWheel += new MouseEventHandler(OnMouseWheel);
            splitContainer1.Panel2.MouseEnter += (s, ev) => splitContainer1.Panel2.Select();

            listView.ListViewItemSorter = listviewSorter;
        }

        private void OnListViewColumnClicked(object sender, ColumnClickEventArgs e)
        {
            listviewSorter.ColumnIndex = e.Column;
            if (listviewSorter.SortOrder == SortOrder.Ascending)
            {
                listviewSorter.SortOrder = SortOrder.Descending;
            }
            else
            {
                listviewSorter.SortOrder = SortOrder.Ascending;
            }

            listView.Sort();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
            {
                return;
            }

            TextureFormatProxy proxy = (TextureFormatProxy)listView.SelectedItems[0].Tag;
            try
            {
                FileTextureFormatProxy fproxy = proxy as FileTextureFormatProxy;
                string fullPath = null;
                if (fproxy != null)
                {
                    fullPath = fproxy.FullPath;
                }

                using (Stream s = proxy.GetTextureStream())
                    OpenImportStream(s, fullPath, TextureFormatMode.Unspecified);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnMouseWheel(Object sender, MouseEventArgs e)
        {
            Zoom(e.Delta > 0 ? 1.2f : 0.8f);
        }
    }
}
