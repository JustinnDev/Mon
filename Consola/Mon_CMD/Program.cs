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
          
            while (true)
            {
                await Behaviur();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        static async Task Behaviur()
        {
            Github.UpdateRepositories();

            if (await Github.GetFileOnRepository(Definitions.gitignore) == null)
            {
                await Github.CreateFileOnRepository(Definitions.gitignore, Contents.GitIgnore);
            }

            Git.PullRemoteChanges();
            Git.PushLocalChanges();
            await Console.Out.WriteLineAsync();
        }
    }
}