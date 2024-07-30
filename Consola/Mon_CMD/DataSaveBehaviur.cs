using Newtonsoft.Json.Linq;
using Mon.DataTypes;
using Mon.Behaviur;

namespace Mon.DataSaveBehaviur
{
    static class Json
    {
        public static List<Commit> commitList = new List<Commit>();

        public static void Save()
        {
            JObject jsonObject = JObject.Parse(
                File.ReadAllText(JsonPaths.CommitsPath)
                );

            JArray commitsArray = jsonObject["commits"] as JArray ?? new JArray();
       
            foreach (var commit in commitList)
            {
                JObject commitObject = JObject.FromObject(commit);
                commitsArray.Add(commitObject);
            }

            jsonObject["commits"] = commitsArray;

            File.WriteAllText(JsonPaths.CommitsPath, jsonObject.ToString());

            Debug.Log("Archivo JSON modificado exitosamente.", MessageType.ok);
        }

        public static void AddCommitSha(string Sha)
        {
            commitList.Add(
                new Commit()
                {
                    Sha = Sha,
                    Name = $"Commit {commitList.Count}"
                }
                );
        }
    }

    struct Commit
    {
        public string? Name;
        public string? Sha;
   
        public Commit() { }
    }
}
