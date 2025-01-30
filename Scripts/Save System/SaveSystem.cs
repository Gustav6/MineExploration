using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.IO;

namespace MineExploration
{
    public static class SaveSystem
    {
        public const string PATH = "data.json";

        public static void Save(Data data)
        {
            string serializedData = JsonSerializer.Serialize(data);

            // Debug
            Trace.WriteLine(serializedData);

            File.WriteAllText(PATH, serializedData);
        }

        public static Data Load()
        {
            var deserializedData = File.ReadAllText(PATH);

            return JsonSerializer.Deserialize<Data>(deserializedData);
        }
    }
}
