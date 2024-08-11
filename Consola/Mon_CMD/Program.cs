using Mon.Behaviur;
using Mon.DataTypes;
using Mon.DataSaveBehaviur;
using Mon.Tools;

namespace Mon.Main
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Github.Start();

            if (await Github.GetFileOnRepository(Definitions.gitignore) == null)
                await Github.CreateFileOnRepository(Definitions.gitignore, Contents.GitIgnore);

            await Update();
        }

        static async Task Update()
        {
            while (true)
            {
                await PullPushBehaviur();
                await DownloadCommitBehaviur();
                await Task.Delay(TimeSpan.FromSeconds(Config.DefaultTimeTask));
            }
        } 

        static async Task PullPushBehaviur()
        {
            Github.UpdateRepositories();
            Git.PullRemoteChanges();
            Git.PushLocalChanges();
            await Console.Out.WriteLineAsync();
        }

        static async Task DownloadCommitBehaviur()
        {
            using (var cts = new CancellationTokenSource())
            {
                await Console.Out.WriteLineAsync("Quieres descargar alguna version de archivos? Si/No");

                Task timeoutRequest = Task.Delay(millisecondsDelay: 5000, cancellationToken: cts.Token);
                Task<string?> request = Task.Run(Console.ReadLine);
                Task completedTask = await Task.WhenAny(timeoutRequest, request);

                cts.Cancel();

                if(completedTask == timeoutRequest)
                {
                    var commits = await Github.GetAllCommits();

                    for (int i = 0; i < commits.Count; i++)
                    {
                        await Console.Out.WriteLineAsync($"Numero {i + 1} : {commits[i].Commit.Message}");
                    }

                    await Console.Out.WriteLineAsync("Cual Version quieres descargar?");
                    await Console.Out.WriteAsync("Numero: ");
                    await Github.DownloadCommitFiles(commits[int.Parse(Console.ReadLine()) - 1].Sha);
                }
            }
        }
    }
}