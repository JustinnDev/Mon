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
                try
                {
                    await Behaviur();
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch (Exception)
                {
                    Debug.Log("Error fatal en el Sistema", MessageType.error);  
                }
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

            await Console.Out.WriteLineAsync($"Pull/Push");
        }


        static async Task Test()
        {
            await Console.Out.WriteLineAsync("Test");
        }
    }
}