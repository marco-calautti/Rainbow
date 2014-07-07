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

        public MainForm()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Rainbow, a console image format handling tool.", "About Rainbow",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = ConstructFilters(true);
            var result=dialog.ShowDialog();

            if (result != DialogResult.OK)
                return;
            string name = dialog.FileName;
            var serializer=TextureFormatSerializerProvider.FromFileFormatExtension(Path.GetExtension(name));

            if (serializer == null)
                serializer = TextureFormatSerializerProvider.FromFile(name);

            if(serializer==null)
            {
                MessageBox.Show("Unsupported file format!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Stream s = File.Open(name, FileMode.Open))
                SetTexture(serializer.Open(s));
        }

        private string ConstructFilters(bool format)
        {
            StringBuilder builder=new StringBuilder();

            IEnumerable<TextureFormatSerializer> ordered=TextureFormatSerializerProvider.RegisteredSerializers.OrderBy(s => s.Name);
            foreach(var serializer in ordered)
            {
                builder.AppendFormat("{0}|*{1}|",serializer.Name, 
                                                                    format? serializer.PreferredFormatExtension : 
                                                                    serializer.PreferredMetadataExtension);
            }
            builder.AppendFormat("All files|*.*");
            return builder.ToString();
        }

        private void SetTexture(TextureFormat tex)
        {
            texture = tex;
            propertyGrid.SelectedObject = texture;
            pictureBox.Image = texture.GetImage();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SetTexture(texture);
        }
    }
}
