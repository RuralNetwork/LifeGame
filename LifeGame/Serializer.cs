using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

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

        static public bool Load<T>(ref T obj, string filename) where T : class //  <-- allow obj to be null
        {
            try
            {

                if (File.Exists(filename))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Stream stream = File.OpenRead(filename);
                    obj = (T)bf.Deserialize(stream);
                    stream.Close();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
