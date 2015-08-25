using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    public class ImageFilterComposer : ImageFilter, IEnumerable<ImageFilter>
    {
        private IList<ImageFilter> filters;

        public ImageFilterComposer()
        {
            filters = new List<ImageFilter>();
        }

        public void Add(ImageFilter filter)
        {
            filters.Add(filter);
        }

        public override byte[] ApplyFilter(byte[] originalData, int index, int length)
        {
            byte[] data = originalData;
            foreach(var filter in filters)
            {
                data = filter.ApplyFilter(data,index,length);
            }

            return data;
        }

        public override byte[] Defilter(byte[] originalData, int index, int length)
        {
            byte[] data = originalData;
            foreach (var filter in filters)
            {
                data = filter.Defilter(data,index,length);
            }

            return data;
        }

        public IEnumerator<ImageFilter> GetEnumerator()
        {
            return filters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return filters.GetEnumerator();
        }
    }
}
