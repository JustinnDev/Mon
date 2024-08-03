using Mon.Behaviur;
using Mon.DataTypes;
using Mon.DataSaveBehaviur;

namespace Mon.Main
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Github.Start();
            //await Behaviur();

            var commits = await Github.GetAllComits();

            foreach (var commit in commits)
                await Github.DownloadCommitFiles(commit.Sha);
        }

        static async Task Behaviur()
        {
            while (true)
            {
                Github.UpdateRepositories();

                if (await Github.GetFileOnRepository(Definitions.gitignore) == null)
                {
                    await Github.CreateFileOnRepository(Definitions.gitignore, Contents.GitIgnore);
                }

                Git.PullRemoteChanges();
                Git.PushLocalChanges();
                await Console.Out.WriteLineAsync();

                await Task.Delay(TimeSpan.FromSeconds(Config.DefaultTimeTask));
            }
        }
    }
}