using Octokit; 
using LibGit2Sharp;
using Mon.DataTypes;

namespace Mon.Behaviur
{
    public static class GitHub
    {
        //Client
        public static ProductHeaderValue header = new (Definitions.ProgramName);
        public static GitHubClient client = new (header); 
        public static User user = new User();
        
        //Searching
        public static SearchRepositoriesRequest repositotyRequest = new(Repositories.PruebaMon);
        public static SearchRepositoryResult repositoryResult = new();

        //Repositories
        public static IReadOnlyList<Octokit.Repository> repositories;
        public static Octokit.Repository currentRepository;

        //API Information
        public static ApiInfo apiInfo;

        static GitHub()
        {
            client.Credentials = new Octokit.Credentials(Login.credentials);
            repositories = client.Repository.GetAllForCurrent().Result;
            apiInfo = client.GetLastApiInfo();

            foreach (var rep in repositories)
            {
                if (rep.Name == Repositories.PruebaMon)
                {
                    currentRepository = rep;
                    break;
                }
            }

            currentRepository ??= new();
        }

        public static async Task Start()
        {
            user = await client.User.Get(Login.JustinnDev);
            repositoryResult = await client.Search.SearchRepo(repositotyRequest);
          
            Debug.CheckingReference();
        }

    }

    public static class Git
    {

    }

    public static class Debug
    {
        public static void CheckingCredentials()
        {
        }

        public static void CheckingReference()
        {
            List<ClientObject> listReferences =
            [
                new ClientObject("Mon.Behaviur.Github.header", GitHub.header),
                new ClientObject("Mon.Behaviur.Github.client", GitHub.client),
                new ClientObject("Mon.Behaviur.Github.user", GitHub.user),
                new ClientObject("Mon.Behaviur.Github.repositotyRequest", GitHub.repositotyRequest),
                new ClientObject("Mon.Behaviur.Github.repositoryResult", GitHub.repositoryResult),
                new ClientObject("Mon.Behaviur.Github.apiInfo" , GitHub.apiInfo),
                new ClientObject("Mon.Behaviur.Github.repositories" , GitHub.repositories),
                new ClientObject("Mon.Behaviur.Github.currentRepository" , GitHub.currentRepository),
            ];


            foreach (var obj in listReferences)
            {
                Console.ForegroundColor = obj.Value == null ? ConsoleColor.Red : ConsoleColor.Green;

                if (obj.Value == null)                   
                    Console.WriteLine($"No se cargo {obj.Name} (FAIL)");
                    
                else
                    Console.WriteLine($"{obj.Name} (OK)");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        struct ClientObject
        {
            public string Name;
            public object Value;

            public ClientObject(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
