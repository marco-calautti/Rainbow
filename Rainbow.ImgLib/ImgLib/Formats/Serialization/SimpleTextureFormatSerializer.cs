using Rainbow.ImgLib.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Formats.Serialization
{
    public abstract class SimpleTextureFormatSerializer<T> : TextureFormatSerializer where T : TextureFormat
    {

        public abstract string Name { get; }

        public abstract string PreferredFormatExtension { get; }

        public abstract TextureFormat Open(System.IO.Stream formatData);

        public abstract void Save(TextureFormat texture, System.IO.Stream outFormatData);


        public abstract byte[] MagicID { get; }

        public abstract string MetadataID { get; }

        protected void OnExportGeneralTextureMetadata(T texture, Metadata.MetadataWriter metadata)
        {
            InteropUtils.WriteTo(texture.FormatSpecificData, metadata);
        }

        protected void OnExportFrameMetadata(T texture, int frame, Metadata.MetadataWriter metadata)
        {
            TextureFormat tFrame = GetTextureFrame(texture, frame);
            InteropUtils.WriteTo(tFrame.FormatSpecificData, metadata);
        }

        protected abstract TextureFormat GetTextureFrame(T texture, int frame);

        protected abstract T OnImportGeneralTextureMetadata(Metadata.MetadataReader metadata);

        protected abstract void OnImportFrameMetadata(T texture, int frame, Metadata.MetadataReader metadata, IList<Image> images, Image referenceImage);

        public bool IsValidFormat(System.IO.Stream inputFormat)
        {
            long oldPos = inputFormat.Position;
            try
            {
                byte[] magic = new BinaryReader(inputFormat).ReadBytes(MagicID.Length);
                return magic.SequenceEqual(MagicID);
            }
            finally
            {
                inputFormat.Position = oldPos;
            }
        }

        public bool IsValidMetadataFormat(Metadata.MetadataReader metadata)
        {
            try
            {
                metadata.EnterSection(MetadataID);
            }
            catch (Exception)
            {
                metadata.Rewind();
                return false;
            }
            metadata.ExitSection();
            metadata.Rewind();
            return true;
        }

        public void Export(TextureFormat texture, Metadata.MetadataWriter metadata, string directory, string basename)
        {
            if(texture.GetType() != typeof(T))
            {
                throw new TextureFormatException("Wrong TextureFormat type, expected: " + typeof(T).FullName);
            }

            metadata.BeginSection(MetadataID);
            metadata.PutAttribute("Textures", texture.FramesCount);
            metadata.PutAttribute("Basename", basename);

                metadata.BeginSection("GeneralMetadata");
                OnExportGeneralTextureMetadata((T)texture,metadata);
                metadata.EndSection();

                int oldSelected=texture.SelectedFrame;
                for(int frame=0;frame<texture.FramesCount;frame++)
                {
                    texture.SelectedFrame = frame;

                    metadata.BeginSection("FrameMetadata");
                    metadata.PutAttribute("PalettesCount", texture.PalettesCount);

                    OnExportFrameMetadata((T)texture, frame, metadata);
                    metadata.EndSection();

                    int i = 0;
                    Image referenceImage;
                    ICollection<Image> list=ConstructImages(texture, out referenceImage);
                    foreach (Image img in list)
                    {
                        img.Save(Path.Combine(directory, basename + "_layer" + frame + "_" + i++ + ".png"));
                    }

                    if(referenceImage!=null)
                    {
                        referenceImage.Save(Path.Combine(directory, basename + "_layer" + frame + "_reference.png"));
                    }
                }
                texture.SelectedFrame = oldSelected;

            metadata.EndSection();
        }

        public TextureFormat Import(Metadata.MetadataReader metadata, string directory)
        {
            metadata.EnterSection(MetadataID);
                int count = metadata.GetAttributeInt("Textures");
                string basename = metadata.GetAttributeString("Basename");

                metadata.EnterSection("GeneralMetadata");
                    T texture=OnImportGeneralTextureMetadata(metadata);
                metadata.ExitSection();

                for (int frame = 0; frame < count;frame++ )
                {
                    metadata.EnterSection("FrameMetadata");
                        int palCount=metadata.GetAttributeInt("PalettesCount");

                        IList<Image> images = new List<Image>();
                        Image referenceImage = null;
                        for (int i = 0; i < (palCount == 0 ? 1 : palCount); i++)
                        {
                            Image img = Image.FromFile(Path.Combine(directory, basename + "_layer" + frame + "_" + i + ".png"));
                            images.Add(img);
                        }
                    
                        if(palCount>1)
                        {
                            referenceImage = Image.FromFile(Path.Combine(directory, basename + "_layer" + frame + "_reference.png"));
                        }

                        OnImportFrameMetadata(texture, frame, metadata, images, referenceImage);

                    metadata.ExitSection();
                }

            metadata.ExitSection();

            return texture;
        }

        private ICollection<Image> ConstructImages(TextureFormat texture, out Image referenceImage)
        {

            var list = new List<Image>();
            int oldSelected = texture.SelectedPalette;
            for (int i = 0; i < (texture.PalettesCount == 0 ? 1 : texture.PalettesCount); i++)
            {
                texture.SelectedPalette = i;
                list.Add(texture.GetImage());
            }
            texture.SelectedPalette = oldSelected;
            referenceImage = texture.GetReferenceImage();
            return list;
        }
    }
}
