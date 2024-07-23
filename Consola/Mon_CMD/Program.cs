using Mon.Behaviur;
using Mon.DataTypes;
using Octokit;

namespace Mon
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await GitHub.Start();
        }


    }
}