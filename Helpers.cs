using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Brother_Robot
{
    class Helpers
    {
        private static Random m_Random = new Random();

        static public Random GetRandom() { return m_Random; }

        static public void SaveToFile(
            object oSave,
            string sFileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, oSave);
            stream.Close();
        }

        static private object LoadFromFile(
            string sFileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            object oLoaded = formatter.Deserialize(stream);
            stream.Close();
            return oLoaded;
        }

    }
}
