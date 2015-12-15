using BabylonJs.Framework.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;

namespace BabylonJs.WebView
{
    public static class ConversionFactory
    {
        public static async Task<IBabylonConverter> GetConverter(StorageFile file)
        {
            if (file.FileType.EndsWith("stl"))
            {
                return new StlConverter((await file.OpenReadAsync()).AsStream());
            }
            else if (file.FileType.EndsWith("amf"))
            {
                return new AmfConverter((await file.OpenReadAsync()).AsStream());
            }

            throw new NotImplementedException();
        }
    }
}
