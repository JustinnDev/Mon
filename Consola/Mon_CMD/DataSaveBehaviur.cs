using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mon.DataTypes;

namespace Mon.DataSaveBehaviur
{
    static class Json
    {
        public static List<string> commitList = new List<string>();

        public static void Save()
        {
            JObject jsonObject = JObject.Parse(
                json: File.ReadAllText(JsonPaths.CommitsPath)
                );

            for (int i = 0; i < 0; i++)
                jsonObject[$"Commit {i}"] = commitList[i];

            string updatedJson = jsonObject.ToString();

            File.WriteAllText(JsonPaths.CommitsPath, updatedJson);

            Console.WriteLine("Archivo JSON modificado exitosamente.");
        }
    }

    struct ShaCommit
    {
        public string Sha;

        public ShaCommit(string Sha)
        {
            this.Sha = Sha;
        }
    }
}
