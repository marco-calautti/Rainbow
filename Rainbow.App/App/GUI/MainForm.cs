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

namespace Rainbow.App.GUI
{
    public partial class MainForm : Form
    {
        private TextureFormat texture;
        private TextureFormatSerializer serializer;
        private string filename;

        public MainForm(string filename) : this()
        {
            TextureFormatSerializer ser = null;
            TextureFormat tex = null;
            try
            {
                ser = TextureFormatSerializerProvider.FromFile(filename);
                
                using(Stream s=File.Open(filename,FileMode.Open))
                {
                    tex = ser.IsValidFormat(s) ? ser.Open(s) : ser.Import(s,Path.GetDirectoryName(filename),Path.GetFileNameWithoutExtension(filename));
                }
                
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            serializer = ser;
            SetTexture(tex);
            SetFilename(filename);
        }

        public MainForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Text = Application.ProductName;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Application.ProductName+", a console image format conversion tool.", "About Rainbow",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportTexture(true);
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
            OpenImportTexture(false);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExportTexture(true);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExportTexture(false);
        }

        private void OpenImportTexture(bool open)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = ConstructFilters(open);
            var result = dialog.ShowDialog();

            if (result != DialogResult.OK)
                return;

            string name = dialog.FileName;

            var curSerializer = open ?  TextureFormatSerializerProvider.FromFileFormatExtension(Path.GetExtension(name)) :
                                        TextureFormatSerializerProvider.FromFileMetadataExtension(Path.GetExtension(name));

            try
            {
                if (curSerializer == null)
                    curSerializer = TextureFormatSerializerProvider.FromFile(name);

                if (curSerializer == null)
                {
                    MessageBox.Show("Unsupported file format!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (Stream s = File.Open(name, FileMode.Open))
                    SetTexture(open ? curSerializer.Open(s) :
                                      curSerializer.Import(s, 
                                                           Path.GetDirectoryName(name), 
                                                           Path.GetFileNameWithoutExtension(name)));

                SetFilename(name);
                serializer = curSerializer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveExportTexture(bool save)
        {
            if (texture == null)
                return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = Path.GetFileNameWithoutExtension(filename);

            dialog.Filter = serializer.Name +
                            (save ? "|" : " metadata + editable data|") +
                            (save ? serializer.PreferredFormatExtension : serializer.PreferredMetadataExtension);

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            try
            {
                using (Stream s = File.Open(dialog.FileName, FileMode.Create))
                {
                    if (save)
                        serializer.Save(texture, s);
                    else
                        serializer.Export(texture, s, Path.GetDirectoryName(dialog.FileName), Path.GetFileNameWithoutExtension(dialog.FileName));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string ConstructFilters(bool format)
        {
            StringBuilder builder = new StringBuilder();

            IEnumerable<TextureFormatSerializer> ordered = TextureFormatSerializerProvider.RegisteredSerializers.OrderBy(s => s.Name);

            StringBuilder allFormatsBuilder = new StringBuilder();
            allFormatsBuilder.Append("All supported " + (format ? "formats|" : "metadata formats|"));

            foreach (var serializer in ordered)
            {
                string ext= format ?  serializer.PreferredFormatExtension :
                                      serializer.PreferredMetadataExtension;

                allFormatsBuilder.AppendFormat("*{0};", ext);
                builder.AppendFormat("{0}|*{1}|", format?   serializer.Name : 
                                                            serializer.Name+" metadata",
                                                            ext);
            }

            string f=allFormatsBuilder.Remove(allFormatsBuilder.Length - 1, 1).
                                     Append('|').Append(builder).
                                     Append("All files|*.*").ToString();
            return f;

        }

        private void SetTexture(TextureFormat tex)
        {
            texture = tex;
            propertyGrid.SelectedObject = texture;
            transparentPictureBox1.SetTexture(texture);
        }

        private void SetFilename(string name)
        {
            this.filename = Path.GetFileName(name);
            this.Text = filename + " - " + Application.ProductName;
        }

        private void transparentColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.CustomColors = new int[] { TransparentPictureBox.PreferredTransparencyColor.ToArgb() };
            dialog.FullOpen = true;

            var result=dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            transparentPictureBox1.SetTransparencyColor(dialog.Color);
        }

        private void chessboardBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item=sender as ToolStripMenuItem;
            if(item==null)
                return;
            item.Checked=!item.Checked;
            transparentPictureBox1.Chessboard = item.Checked;
        }
    }
}
