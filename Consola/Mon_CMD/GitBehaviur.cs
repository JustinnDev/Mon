using Octokit; 
using LibGit2Sharp;
using Mon.DataTypes;
using LibGit2Sharp.Handlers;

namespace Mon.Behaviur
{
    #region Behaviur  
    public static class Github
    {
        //Client
        public static ProductHeaderValue header = new(Definitions.ProgramName);
        public static GitHubClient client = new(header);
        public static User user = new User();

        //Searching
        public static SearchRepositoriesRequest repositotyRequest = new(Repositories.RepositoryName);
        public static SearchRepositoryResult repositoryResult = new();

        //Repositories
        public static IReadOnlyList<Octokit.Repository> repositories;
        public static Octokit.Repository currentRepository;

        //API Information
        public static ApiInfo apiInfo;

        static Github()
        {
            //Ingresa la configuracion de la API 

            client.Credentials = new Octokit.Credentials(Login.Credentials);
            apiInfo = client.GetLastApiInfo();
            repositories = client.Repository.GetAllForCurrent().Result;
            currentRepository ??= new();

            UpdateRepositories();
        }

        public static async Task Start()
        {
            user = await client.User.Get(Login.UserName);
            repositoryResult = await client.Search.SearchRepo(repositotyRequest);
            apiInfo = client.GetLastApiInfo();

            Debug.CheckingReference();
        }

        public static void UpdateRepositories()
        {
            //Actualiza los datos de los  repositorios remotos
            repositories = client.Repository.GetAllForCurrent().Result;

            foreach (var rep in repositories)
            {
                if (rep.Name == Repositories.RepositoryName)
                {
                    currentRepository = rep;
                    break;
                }
            }
        }

        public static async Task<IReadOnlyList<RepositoryContent>?> GetFileOnRepository(string filenName)
        {
            try
            {
                return await client.Repository.Content.GetAllContentsByRef(
                    owner: user.Login,
                    name: currentRepository.Name,
                    path: filenName,
                    reference: currentRepository.DefaultBranch
                    );
            }
            catch (Octokit.NotFoundException e)
            {
                Debug.Log(e.Message, MessageType.warning);
                return null;
            }
        }

        public static async Task UpdateFileOnRepository(string fileName, string content)
        {
            try
            {
                var sha = await client.Repository.Commit.GetSha1(
                    owner: user.Login,
                    name: currentRepository.Name,
                    reference: fileName
                    );

                await client.Repository.Content.UpdateFile(
                    owner: user.Login,
                    name: currentRepository.Name,
                    path: fileName,
                    request: new UpdateFileRequest($"Update {fileName}" , content , sha)
                    );
            }
            catch (Octokit.NotFoundException e)
            {
                Debug.Log(e.Message , MessageType.warning);
            }
        }

        public static async Task CreateFileOnRepository(string filenName, string content)
        {
            try
            {
                var newFile = await client.Repository.Content.CreateFile(
                owner: user.Login,
                name: currentRepository.Name,
                path: filenName,
                request: new CreateFileRequest($"Create {filenName}", content)
                );

                Console.WriteLine($"Creado {filenName}");
            }
            catch (Exception)
            {
                Debug.Log($"No se creo {filenName}", MessageType.warning);
            }
        }
    }

    public static class Git
    {
        //Options
        public static PushOptions pushOptions;
        public static PullOptions pullOptions = new();

        //Credentials/Data
        public static LibGit2Sharp.Repository currentRepository = new(Repositories.LocalRepository);
        public static LibGit2Sharp.Signature author = new(Github.user.Login, Login.Mail, DateTimeOffset.Now);

        static Git()
        {
            //Configuracion de las opciones de Pull/Push

            pushOptions = new PushOptions
            {
                CredentialsProvider = (url, usernameFromUrl, types) => new UsernamePasswordCredentials
                {
                    Username = Github.user.Login,
                    Password = Login.Credentials
                }
            };

            pullOptions.MergeOptions = new MergeOptions();
            pullOptions.MergeOptions.FailOnConflict = true;
            pullOptions.FetchOptions = new FetchOptions();
            pullOptions.FetchOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new UsernamePasswordCredentials()
            {
                Username = Github.user.Login,
                Password = Login.Credentials
            });
        }

        public static void PushLocalChanges()
        {
            try
            {
                Commands.Stage(currentRepository, "*");

                currentRepository.Commit(
                      message: $"Cambio {DateTimeOffset.Now}",
                      author: author,
                      committer: author);

                currentRepository.Network.Push(
                branch: currentRepository.Branches[Github.currentRepository.DefaultBranch],
                pushOptions: pushOptions);

                Debug.Log("Push de cambios.", MessageType.ok);
            }

            catch(EmptyCommitException e)
            {
                Debug.Log(e.Message , MessageType.warning);
            }

            catch (Exception)
            {
                Debug.Log("Error al hacer Push de los cambios locales", MessageType.error);
            }
        }

        public static void PullRemoteChanges()
        {
            try
            {
                Commands.Pull(
                    repository: currentRepository,
                    merger: author,
                    options: pullOptions
                    );

                Debug.Log("Pull de cambios." , MessageType.ok);
            }
          
            catch (InvalidSpecificationException e)
            {
                Debug.Log(e.Message , MessageType.error);
            }

            catch (LibGit2SharpException e)
            {
                Debug.Log(e.Message, MessageType.error);
            }

            catch (Exception)
            {
                Debug.Log("Error al hacer Pull de la rama remota", MessageType.error);
            }
        }
    }
    #endregion

    #region Debug Behaviur
    public static class Debug
    {
        public static void Log(string msg, MessageType type)
        {
            string msgType = "";

            switch (type)
            {
                case MessageType.ok:
                    msgType = "OK:";
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case MessageType.warning:
                    msgType = "WARNING:";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case MessageType.error:
                    msgType = "ERROR:";
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine($"{msgType} {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void CheckingReference()
        {
            List<DebugObject> listReferences =
            [
                new DebugObject("Mon.Behaviur.Github.header", Github.header),
                new DebugObject("Mon.Behaviur.Github.client", Github.client),
                new DebugObject("Mon.Behaviur.Github.user", Github.user),
                new DebugObject("Mon.Behaviur.Github.repositotyRequest", Github.repositotyRequest),
                new DebugObject("Mon.Behaviur.Github.repositoryResult", Github.repositoryResult),
                new DebugObject("Mon.Behaviur.Github.apiInfo" , Github.apiInfo),
                new DebugObject("Mon.Behaviur.Github.repositories" , Github.repositories),
                new DebugObject("Mon.Behaviur.Github.currentRepository" , Github.currentRepository),
                new DebugObject("Mon.Behaviur.Git.pushOptions" , Git.pushOptions),
                new DebugObject("Mon.Behaviur.Git.currentRepository" , Git.currentRepository),
                new DebugObject("Mon.Behaviur.Git.pullOptions" , Git.pullOptions),
                new DebugObject("Mon.Behaviur.Git.author" , Git.author),
                new DebugObject("Mon.Behaviur.Git.currentRepository.Branches[Github.currentRepository.DefaultBranch]" , Git.currentRepository.Branches[Github.currentRepository.DefaultBranch])
            ];

            Console.WriteLine("-----------------------------------------------------");

            foreach (var obj in listReferences)
            {
                if (obj.Value == null)
                    Log($"No se cargo {obj.Name}", MessageType.error);

                else
                    Log($"{obj.Name}", MessageType.ok);
            }

            Console.WriteLine("-----------------------------------------------------");
        }

        struct DebugObject
        {
            public string Name;
            public object Value;

            public DebugObject(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }
    }
    #endregion
}
