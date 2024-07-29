using Mon.Behaviur;
using Mon.DataTypes;
using System.Threading;

namespace Mon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Github.Start();
            //await Behaviur();

            var commits = await Github.GetAllComits();

            await Console.Out.WriteLineAsync(commits.Count.ToString());

            foreach (var commit in commits)
                await Console.Out.WriteLineAsync(commit.Sha);
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

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}