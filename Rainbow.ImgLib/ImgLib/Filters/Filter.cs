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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    /// <summary>
    /// A filter is any kind of object that arranges a given array of objects into another array of the same type.
    /// An example filter is a filter that gets a tiled array of pixels and returns a linear representation of such an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Filter<T>
    {
        /// <summary>
        /// Applies the filter to the given array.
        /// </summary>
        /// <param name="originalData"></param>
        /// <returns></returns>
        public T[] ApplyFilter(T[] originalData)
        {
            return ApplyFilter(originalData, 0, originalData.Length);
        }

        /// <summary>
        /// Applies the reverse of the this filter to the given array.
        /// </summary>
        /// <param name="originalData"></param>
        /// <returns></returns>
        public T[] Defilter(T[] originalData)
        {
            return Defilter(originalData, 0, originalData.Length);

        }
        /// <summary>
        /// The same as ApplyFilter(T[]) but starting at the given position and filtering a number of entries equal to "length".
        /// </summary>
        /// <param name="originalData"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract T[] ApplyFilter(T[] originalData,int index,int length);

        /// <summary>
        /// The same as Defilter(T[]) but starting at the given position and defiltering a number of entries equal to "length".
        /// </summary>
        /// <param name="originalData"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract T[] Defilter(T[] originalData, int index, int length);


    }

    public abstract class ImageFilter : Filter<byte>
    {
        public virtual int GetWidthForEncoding(int realWidth)
        {
            return realWidth;
        }

        public virtual int GetHeightForEncoding(int realHeight)
        {
            return realHeight;
        }
    }

    public abstract class PaletteFilter : Filter<Color>
    {

    }
}
