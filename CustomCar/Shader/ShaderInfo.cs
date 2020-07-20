using Spectrum.API.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCar
{
    [Serializable]
    class UniformInfo
    {
        public UniformType type;
        public string name;
    }

    [Serializable]
    class ShaderInfo
    {
        public string name;
        public List<UniformInfo> uniforms = new List<UniformInfo>();
    }
    
    static class SerializeShaderListInfos
    {
        public static List<ShaderInfo> Read(string file)
        {
            var list = new List<ShaderInfo>();

            var settings = new Settings(file);

            foreach(var s in settings)
            {
                var sections = settings.GetItem<Section[]>(s.Key) as Section[];
                if (sections == null)
                    continue;
                
                var shader = new ShaderInfo();
                shader.name = s.Key;

                foreach(var uniformItem in sections)
                {
                    var uniform = new UniformInfo();
                    uniform.name = uniformItem.GetItem<string>("name");
                    var typeName = uniformItem.GetItem<string>("type");
                    try
                    {
                        uniform.type = (UniformType)Enum.Parse(typeof(UniformType), typeName, true);
                        shader.uniforms.Add(uniform);
                    }
                    catch(Exception e)
                    {
                        Console.Out.WriteLine("Shader " + shader.name + ": Can't read uniform type " + typeName);
                    }
                }

                list.Add(shader);
            }

            return list;
        }

        public static void Write(List<ShaderInfo> shaders, string file)
        {
            throw new NotImplementedException("Todo later, not needed for now");
        }
    }

    
}
