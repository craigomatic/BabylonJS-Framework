using BabylonJs.Framework.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabylonJs.Framework.Converters
{
    public abstract class BabylonConverter : IBabylonConverter
    {
        protected async Task<string> ToJsonAsync(ConversionState state)
        {
            var babylonMesh = new BabylonMesh();
            
            babylonMesh.Name = state.Name == null ? string.Format("Converted_{0}", DateTime.UtcNow.Ticks) : state.Name;            

            babylonMesh.Indices = state.Indices.ToArray();
            babylonMesh.Normals = state.Normals.ToArray();
            babylonMesh.Positions = state.Vertices.SelectMany(v => v.ToArray()).ToArray();

            babylonMesh.IsVisible = true;
            babylonMesh.IsEnabled = true;
            babylonMesh.CheckCollisions = false;
            babylonMesh.BillboardMode = BillboardMode.None;
            babylonMesh.Position = new float[3] { 0, 0, 0 };
            babylonMesh.Rotation = new float[3] { 0, 0, 0 };
            babylonMesh.Scaling = new float[3] { 1, 1, 1 };

            var babylonFile = new BabylonFile();

            babylonFile.Meshes = new BabylonMesh[1] { babylonMesh };

            return await Task.Factory.StartNew(() => JsonConvert.SerializeObject(babylonFile, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

        public abstract Task<string> ToJsonAsync();
    }
}
