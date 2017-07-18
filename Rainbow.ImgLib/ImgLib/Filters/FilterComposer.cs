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

namespace Rainbow.ImgLib.Filters
{
    public class ImageFilterComposer : ImageFilter, IEnumerable<ImageFilter>
    {
        private readonly IList<ImageFilter> filters;

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
            foreach(var filter in filters.Reverse())
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

        public override int GetWidthForEncoding(int realWidth)
        {
            int encodedWidth = realWidth;
            foreach (var filter in filters)
            {
                encodedWidth = filter.GetWidthForEncoding(encodedWidth);
            }

            return encodedWidth;
        }

        public override int GetHeightForEncoding(int realHeight)
        {
            int encodedHeight = realHeight;
            foreach (var filter in filters)
            {
                encodedHeight = filter.GetWidthForEncoding(encodedHeight);
            }

            return encodedHeight;
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
