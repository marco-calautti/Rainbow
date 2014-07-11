using Rainbow.ImgLib.Formats;
using Rainbow.ImgLib.Formats.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Rainbow.App.GUI.Controls;
using Rainbow.App.GUI.Model;

namespace Rainbow.App.GUI
{
    public partial class MainForm : Form
    {
        private TextureFormat texture;
        private TextureFormatSerializer serializer;
        private string filename;

        public MainForm(string filename)
            : this()
        {
            try
            {
                using(Stream s=File.Open(filename,FileMode.Open))
                    OpenImportStream(s,filename,TextureOpenMode.Open);
            }catch(Exception ex)
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Application.ProductName + ", a console image format conversion tool.", "About Rainbow", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportTexture(TextureOpenMode.Open);
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SetTexture(texture);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportTexture(TextureOpenMode.Import);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExportTexture(TextureOpenMode.Open);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExportTexture(TextureOpenMode.Import);
        }


        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportFolder(TextureOpenMode.Open);

        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            TextureFormatProxy proxy = (TextureFormatProxy)listView.SelectedItems[0].Tag;
            try
            {
                FileTextureFormatProxy fproxy=proxy as FileTextureFormatProxy;
                string fullPath=null;
                if(fproxy!=null)
                    fullPath=fproxy.FullPath;
 
                using (Stream s = proxy.GetTextureStream())
                    OpenImportStream(s, fullPath, TextureOpenMode.Unspecified);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void transparentColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.CustomColors = new int[] { TransparentPictureBox.PreferredTransparencyColor.ToArgb() };
            dialog.FullOpen = true;

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            transparentPictureBox1.SetTransparencyColor(dialog.Color);
        }

        private void chessboardBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
                return;
            item.Checked = !item.Checked;
            transparentPictureBox1.Chessboard = item.Checked;
        }
    }
}
