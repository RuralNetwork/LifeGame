using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LifeGame
{
    static public class Serializer
    {
        static public void Save<T>(T obj, string filename)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream stream = File.Create(filename);
            bf.Serialize(stream, obj);
            stream.Close();
        }

        static public void Load<T>(out T obj, string filename) where T : class //  <-- allow obj to be null
        {
            if (File.Exists(filename))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Stream stream = File.OpenRead(filename);
                obj = (T)bf.Deserialize(stream);
                stream.Close();
            }
            obj = null;
        }
    }
}
