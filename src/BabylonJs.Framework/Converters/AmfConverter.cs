using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BabylonJs.Framework.Converters
{
    public class AmfConverter : BabylonConverter
    {
        public Stream Stream { get; private set; }

        public AmfConverter(Stream stream)
        {
            this.Stream = stream;
        }

        public async override Task<string> ToJsonAsync()
        {
            var state = new ConversionState();

            //unzip xml

            var zipArchive = new ZipArchive(this.Stream, ZipArchiveMode.Read);
            var amfXml = zipArchive.Entries.FirstOrDefault();

            if (amfXml == null)
            {
                throw new Exception("Invalid AMF stream");
            }

            var filename = Guid.NewGuid().ToString() + ".tmp";
            var file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            
            using (var amfXmlStream = amfXml.Open())
            {
                using (var outputStream = await file.OpenStreamForWriteAsync())
                {
                    await amfXmlStream.CopyToAsync(outputStream);

                    await outputStream.FlushAsync();
                }
            }

            //parse XML
            var xDoc = await XmlDocument.LoadFromFileAsync(file);

            var units = xDoc.DocumentElement.GetAttribute("unit");
            var version = xDoc.DocumentElement.GetAttribute("version");

            foreach (var node in xDoc.DocumentElement.ChildNodes)
            {
                if (node.NodeType != NodeType.ElementNode)
                {
                    continue;
                }
                var xmlElement = node as XmlElement;

                switch(xmlElement.TagName)
                {
                    case AmfConstants.Metadata:
                        {
                            var type = xmlElement.GetAttribute("type");

                            if (type == "name")
                            {
                                state.Name = xmlElement.NodeValue.ToString();
                            }

                            break;
                        }
                    case AmfConstants.Object:
                        {
                            //TODO: support multiple objects
                            break;
                        }
                    case AmfConstants.Material:
                        {
                            //TODO: support multiple mesh
                            break;
                        }
                    case AmfConstants.Vertex:
                        {

                            break;
                        }
                }
            }


            return await base.ToJsonAsync(state);
        }
    }
}
