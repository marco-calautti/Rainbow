using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Filters
{
    /// <summary>
    /// A filter is any kind of object that arranges a given array of objects into another array of the same length.
    /// An example filter is a filter that gets a tiled array of pixel and returns a linear representation of such an array.
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
}
