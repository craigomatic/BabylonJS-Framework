using BabylonJs.Framework.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework.Converters
{
    public class StlConverter : BabylonConverter
    {
        public Stream Stream { get; private set; }

        public StlConverter(Stream stream)
        {
            this.Stream = stream;
        }

        public async override Task<string> ToJsonAsync()
        {
            //STL spec ref: http://en.wikipedia.org/wiki/STL_(file_format)

            var header = new byte[80];

            await this.Stream.ReadAsync(header, 0, 80);

            //test if the stream represents ASCII STL or binary
            if (UTF8Encoding.UTF8.GetString(header, 0, 80).StartsWith("solid"))
            {
                return await _ConvertAsciiStlAsync();
            }
            else
            {
                return await _ConvertBinaryStlAsync();
            }
        }

        private async Task<string> _ConvertBinaryStlAsync()
        {
            //format:

            //UINT8[80] – Header
            //UINT32 – Number of triangles

            //foreach triangle
            //REAL32[3] – Normal vector
            //REAL32[3] – Vertex 1
            //REAL32[3] – Vertex 2
            //REAL32[3] – Vertex 3
            //UINT16 – Attribute byte count
            //end

            var fourByteArray = new byte[4];
            
            var state = new ConversionState();

            await this.Stream.ReadAsync(fourByteArray, 0, fourByteArray.Length);

            var numberOfTris = (uint)BitConverter.ToInt32(fourByteArray, 0);

            for (int i = 0; i < numberOfTris; i++)
            {
                var triangleData = new byte[50];

                try
                {
                    await this.Stream.ReadAsync(triangleData, 0, 50);

                    var n1 = BitConverter.ToSingle(triangleData, 0);
                    var n2 = BitConverter.ToSingle(triangleData, 4);
                    var n3 = BitConverter.ToSingle(triangleData, 8);

                    var v1x = BitConverter.ToSingle(triangleData, 12);
                    var v1y = BitConverter.ToSingle(triangleData, 16);
                    var v1z = BitConverter.ToSingle(triangleData, 20);

                    var v2x = BitConverter.ToSingle(triangleData, 24);
                    var v2y = BitConverter.ToSingle(triangleData, 28);
                    var v2z = BitConverter.ToSingle(triangleData, 32);

                    var v3x = BitConverter.ToSingle(triangleData, 36);
                    var v3y = BitConverter.ToSingle(triangleData, 40);
                    var v3z = BitConverter.ToSingle(triangleData, 44);

                    var v1 = new Vector3(v1x, v1y, v1z);
                    var v2 = new Vector3(v2x, v2y, v2z);
                    var v3 = new Vector3(v3x, v3y, v3z);

                    var oldState = state.CurrentIndex;

                    _StoreOrIndexVertex(state, v1);
                    _StoreOrIndexVertex(state, v2);
                    _StoreOrIndexVertex(state, v3);

                    if (oldState != state.CurrentIndex)
                    {
                        var vNormal = new Vector3(n1, n2, n3);
                        vNormal.Normalise();

                        state.Normals.AddRange(vNormal.ToArray());
                    }
                }
                catch
                {
                    //TODO: malformed STL file, raise error
                }
            }            

            return await base.ToJsonAsync(state);
        }

        private async Task<string> _ConvertAsciiStlAsync()
        {
            //format:

            //solid <name>
            //  facet normal x y z
            //    outer loop
            //      vertex x y z
            //      vertex x y z
            //      vertex x y z
            //    endloop
            //  endfacet
            //endsolid <name>


            var state = new ConversionState();

            this.Stream.Position = 0;

            using (var streamReader = new StreamReader(this.Stream))
            {
                //read the name
                var firstLine = await streamReader.ReadLineAsync();
                state.Name = firstLine.Split(' ')[1];

                Vector3 lastNormal = null;

                //start processing facets
                while (!streamReader.EndOfStream)
                {
                    var lineToProcess = await streamReader.ReadLineAsync();

                    var token = _GetToken(lineToProcess);

                    switch (token)
                    {
                        case StlConstants.FacetNormal:
                            {
                                var trimmedStart = lineToProcess.TrimStart(' ');

                                var normalString = trimmedStart.Substring(StlConstants.FacetNormal.Length, trimmedStart.Length - StlConstants.FacetNormal.Length);
                                var vNormal = Vector3.FromString(normalString);
                                vNormal.Normalise();

                                lastNormal = vNormal;

                                break;
                            }
                        case StlConstants.OuterLoop:
                            {
                                //read next 3 lines

                                var v1 = await _ReadVertexAsync(streamReader);
                                var v2 = await _ReadVertexAsync(streamReader);
                                var v3 = await _ReadVertexAsync(streamReader);

                                var oldState = state.CurrentIndex;

                                _StoreOrIndexVertex(state, v1);
                                _StoreOrIndexVertex(state, v2);
                                _StoreOrIndexVertex(state, v3);

                                if (oldState != state.CurrentIndex)
                                {
                                    if (lastNormal != null)
                                    {
                                        state.Normals.AddRange(lastNormal.ToArray());

                                        lastNormal = null;
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            return await base.ToJsonAsync(state);
        }

        private string _GetToken(string lineToProcess)
        {
            var trimmedLine = lineToProcess.Trim();

            var endIndex = trimmedLine.IndexOf(" ");

            if (endIndex == -1)
            {
                endIndex = trimmedLine.Length;
            }

            var token = trimmedLine.Substring(0, endIndex);

            if (token.StartsWith("facet"))
            {
                return StlConstants.FacetNormal;
            }

            if (token.StartsWith("outer"))
            {
                return StlConstants.OuterLoop;
            }

            return token;
        }
        
        private void _StoreOrIndexVertex(ConversionState state, Vector3 vertex)
        {
            //swap the y and z values (invert the y) to acount for right-hand coordinate system vs left hand coordinate system
            var tmp = vertex.Y * -1;
            vertex.Y = vertex.Z;
            vertex.Z = tmp;

            if (!state.Vertices.Contains(vertex))
            {
                state.Vertices.Add(vertex);
                state.Indices.Add(state.CurrentIndex);

                //store index for later
                state.Index.Add(vertex, state.CurrentIndex);

                state.CurrentIndex++;
            }
            else
            {
                //it must be indexed, where?
                state.Indices.Add(state.Index[vertex]);
            }
        }
        
        private async Task<Vector3> _ReadVertexAsync(StreamReader streamReader)
        {
            var lineToProcess = await streamReader.ReadLineAsync();
            var trimmedStart = lineToProcess.TrimStart(' ');

            var vertexString = trimmedStart.Substring(StlConstants.Vertex.Length, trimmedStart.Length - StlConstants.Vertex.Length);
            return Vector3.FromString(vertexString);
        }
    }    
}
