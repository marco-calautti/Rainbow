using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.ImgLib.Common
{
    public class GenericDictionary
    {
        private IDictionary<string, object> dict=new Dictionary<string,object>();

        /// <summary>
        /// String representation of value with key "key".
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            return Get<object>(key).ToString();
        }

        public T Get<T>(string key)
        {
            return (T)dict[key];
        }

        public GenericDictionary Put<T>(string key, T value)
        {
            dict[key] = value;
            return this;
        }

        public ICollection<string> Keys
        {
            get { return dict.Keys; }
        }
    }
}
