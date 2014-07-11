using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rainbow.App.GUI.Model
{
    public interface TextureFormatProxy
    {
        string Name { get;  }
        long Size { get;  }
        Stream GetTextureStream();
    }

    public class FileTextureFormatProxy : TextureFormatProxy
    {
        private string filename;

        public FileTextureFormatProxy(string file)
        {
            filename = file;
        }

        public Stream GetTextureStream()
        {
            return File.Open(filename, FileMode.Open);
        }

        public string Name
        {
            get
            {
                return Path.GetFileName(filename);
            }
        }

        public long Size
        {
            get
            {
                return new FileInfo(filename).Length;
            }
        }

        public string FullPath
        {
            get
            {
                return filename;
            }
        }
    }
}
