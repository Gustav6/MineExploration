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

        public static void Save(SaveData data)
        {
            string serializedData = JsonSerializer.Serialize(data);

            // Debug
            Trace.WriteLine(serializedData);

            File.WriteAllText(PATH, serializedData);
        }

        public static SaveData Load()
        {
            return JsonSerializer.Deserialize<SaveData>(File.ReadAllText(PATH));
        }
    }
}
