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
        static public bool Save<T>(T obj, string filename)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Stream stream = File.Create(filename);
                bf.Serialize(stream, obj);
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static public bool Load<T>(out T obj, string filename) where T : class //  <-- allow obj to be null
        {
            if (File.Exists(filename))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Stream stream = File.OpenRead(filename);
                obj = (T)bf.Deserialize(stream);
                stream.Close();
                return true;
            }
            obj = null;
            return false;
        }


        static public Genome LoadGenome(string filename)
        {
            if (File.Exists(filename))
            {
                Genome obj;
                BinaryFormatter bf = new BinaryFormatter();
                Stream stream = File.OpenRead(filename);
                obj = (Genome)bf.Deserialize(stream);
                stream.Close();
                return obj;
            }
            return null;
        }
    }
}
