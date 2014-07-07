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

namespace Rainbow.App.GUI
{
    public partial class MainForm : Form
    {
        private TextureFormat texture;
        private TextureFormatSerializer serializer;

        public MainForm()
        {
            InitializeComponent();
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName;
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

        private void SaveExportTexture(bool save)
        {
            if (texture == null)
                return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = serializer.Name + 
                            (save ? "|" : " metadata + editable data|")+
                            (save? serializer.PreferredFormatExtension : serializer.PreferredMetadataExtension);

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            try
            {
                using(Stream s=File.Open(dialog.FileName,FileMode.Create))
                {
                    if (save)
                        serializer.Save(texture, s);
                    else
                        serializer.Export(texture, s, Path.GetDirectoryName(dialog.FileName), Path.GetFileNameWithoutExtension(dialog.FileName));
                }
                
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OpenImportTexture(bool open)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = ConstructFilters(open);
            var result = dialog.ShowDialog();

            if (result != DialogResult.OK)
                return;

            string name = dialog.FileName;

            var curSerializer = open ? TextureFormatSerializerProvider.FromFileFormatExtension(Path.GetExtension(name)) :
                                    TextureFormatSerializerProvider.FromFileMetadataExtension(Path.GetExtension(name));

            if (curSerializer == null)
                curSerializer = TextureFormatSerializerProvider.FromFile(name);

            if (curSerializer == null)
            {
                MessageBox.Show("Unsupported file format!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (Stream s = File.Open(name, FileMode.Open))
                    SetTexture(open ? curSerializer.Open(s) :
                                      curSerializer.Import(s, 
                                                           Path.GetDirectoryName(name), 
                                                           Path.GetFileNameWithoutExtension(name)));

                this.Text = Path.GetFileName(name) + " - " + Application.ProductName;
                serializer = curSerializer;
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
            foreach (var serializer in ordered)
            {
                builder.AppendFormat("{0}|*{1}|", format?   serializer.Name : 
                                                            serializer.Name+" metadata",

                                                  format ?  serializer.PreferredFormatExtension :
                                                            serializer.PreferredMetadataExtension);
            }
            builder.AppendFormat("All files|*.*");
            return builder.ToString();
        }

        private void SetTexture(TextureFormat tex)
        {
            texture = tex;
            propertyGrid.SelectedObject = texture;
            transparentPictureBox1.SetTexture(texture);
        }

        
    }
}
