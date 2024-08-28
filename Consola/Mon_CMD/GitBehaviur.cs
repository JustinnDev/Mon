using Octokit; 
using LibGit2Sharp;
using Mon.DataTypes;
using LibGit2Sharp.Handlers;
using Mon.Tools;
using System.Reflection;

namespace Mon.Behaviur
{
    #region Behaviur

    /// <summary>
    /// Provides methods to interact with GitHub API for repository management and file operations.
    /// </summary>
    /// <remarks>
    /// This class encapsulates the functionality to interact with GitHub repositories using the Octokit library.
    /// It supports operations such as fetching user information, searching for repositories, updating and creating files,
    /// handling commits, and downloading commit files.
    /// </remarks>
    public static class Github
    {
        // Client Configuration
        public static GitHubClient client = new(new ProductHeaderValue(Definitions.ProgramName));
        public static User user = new ();
   
        // Repository Data
        public static Octokit.Repository currentRepository;

        static Github()
        {
            // Initializes GitHub client configuration with credentials and fetches initial data.
            client.Credentials = new Octokit.Credentials(Login.Credentials);
            currentRepository ??= new();
            

            UpdateRepositories();
        }

        /// <summary>
        /// Starts the GitHub interaction by fetching user data, repository search results, and API information.
        /// </summary>
        public static async Task Start()
        {
            user = await client.User.Get(Login.UserName);

            UpdateRepositories();

            Debug.CheckingReferences(typeof(Github));
            Debug.CheckingReferences(typeof(Git));
            Debug.CheckingReferences(typeof(Octokit.Repository), true, currentRepository);
            Debug.CheckingReferences(typeof(GitHubClient), true, client);
        }

        /// <summary>
        /// Updates the list of repositories and sets the current repository based on the configured repository name.
        /// </summary>
        public static void UpdateRepositories()
        {
            // Refreshes the list of repositories and identifies the current repository.
            var repositories = client.Repository.GetAllForCurrent().Result;

            foreach (var rep in repositories)
            {
                if (rep.Name == Repositories.RepositoryName)
                {
                    currentRepository = rep;
                    break;
                }
            }
        }

        /// <summary>
        /// Retrieves file content from the current repository based on the specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>A list of file contents if the file is found; otherwise, null.</returns>
        public static async Task<IReadOnlyList<RepositoryContent>?> GetFileOnRepository(string fileName)
        {
            try
            {
                return await client.Repository.Content.GetAllContentsByRef(
                    owner: user.Login,
                    name: currentRepository.Name,
                    path: fileName,
                    reference: currentRepository.DefaultBranch
                );
            }

            catch (ArgumentNullException e)
            {
                Debug.Log(e.Message , MessageType.Error);   
                return null;
            }

            catch (Octokit.NotFoundException e)
            {
                Debug.Log(e.Message, MessageType.Warning);
                return null;
            }
        }

        /// <summary>
        /// Updates a file in the current repository with the specified content.
        /// </summary>
        /// <param name="fileName">The name of the file to update.</param>
        /// <param name="content">The new content for the file.</param>
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
                    request: new UpdateFileRequest($"Update {fileName}", content, sha)
                );
            }
            catch (Octokit.NotFoundException e)
            {
                Debug.Log(e.Message, MessageType.Warning);
            }
        }

        /// <summary>
        /// Creates a new file in the current repository with the specified content.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="content">The content of the new file.</param>
        public static async Task CreateFileOnRepository(string fileName, string content)
        {
            try
            {
                var newFile = await client.Repository.Content.CreateFile(
                    owner: user.Login,
                    name: currentRepository.Name,
                    path: fileName,
                    request: new CreateFileRequest($"Create {fileName}", content)
                );

                Console.WriteLine($"Created {fileName}");
            }
            catch (Exception)
            {
                Debug.Log($"Failed to create {fileName}", MessageType.Warning);
            }
        }

        /// <summary>
        /// Retrieves all commits from the current repository.
        /// </summary>
        /// <returns>A list of commits if successful; otherwise, null.</returns>
        public static async Task<IReadOnlyList<GitHubCommit>?> GetAllCommits()
        {
            try
            {
                return await client.Repository.Commit.GetAll(currentRepository.Id);
            }
            catch (Exception)
            {
                Debug.Log("Failed to retrieve all commits", MessageType.Warning);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a specific commit by its SHA from the current repository.
        /// </summary>
        /// <param name="sha">The SHA of the commit to retrieve.</param>
        /// <returns>The commit if found; otherwise, null.</returns>
        public static async Task<GitHubCommit?> GetCommit(string sha)
        {
            try
            {
                return await client.Repository.Commit.Get(
                    repositoryId: currentRepository.Id,
                    reference: sha
                );
            }
            catch (Exception)
            {
                Debug.Log("Failed to retrieve the specified commit", MessageType.Warning);
                return null;
            }
        }

        /// <summary>
        /// Deletes commits from the current repository. (Functionality not yet implemented)
        /// </summary>
        public static async Task DeleteCommits()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Downloads files from a commit based on its SHA and saves them to the local file system.
        /// </summary>
        /// <param name="sha">The SHA of the commit from which to download files.</param>
        public static async Task DownloadCommitFiles(string sha)
        {
            var commit = await GetCommit(sha);

            if (commit == null)
                return;

            foreach (var file in commit.Files)
            {
                try
                {
                    // Download the file content as bytes
                    byte[] bytes = await client.Repository.Content.GetRawContentByRef(
                        owner: user.Login,
                        name: currentRepository.Name,
                        path: file.Filename,
                        reference: commit.Sha
                    );

                    // Create a valid folder path for Windows
                    string commitFolders = Path.Combine(
                        DefaultPaths.DownloadCommitsPaths,
                        Path.Combine(
                            Strings.StringToCorrectPathName(commit.Commit.Message),
                            Strings.FilePathToFolders(file.Filename)
                        )
                    );

                    // Add the file name to the path
                    string filePath = Path.Combine(
                        commitFolders,
                        Strings.FilePathToFileName(file.Filename)
                    );

                    Directory.CreateDirectory(commitFolders);

                    File.WriteAllBytes(filePath, bytes);

                    Debug.Log($"File downloaded: {filePath}", MessageType.Ok);
                }
                catch (Octokit.NotFoundException e)
                {
                    Debug.Log($"{e.Message} {file.Filename} {commit.Sha}", MessageType.Warning);
                }
                catch (NullReferenceException e)
                {
                    Debug.Log($"{e.Message} from commit {commit.Sha}", MessageType.Warning);
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.Log($"DirectoryNotFoundException {e.Message}", MessageType.Warning);
                }
                catch (IOException e)
                {
                    Debug.Log($"IOException {e.Message}", MessageType.Warning);
                }
            }
        }
    }


    /// <summary>
    /// Provides methods and options for interacting with Git repositories using the LibGit2Sharp library.
    /// </summary>
    /// <remarks>
    /// This class handles Git operations such as pushing local changes to a remote repository and pulling updates from a remote repository.
    /// It configures necessary options for both push and pull operations, including credentials and merge options.
    /// 
    /// The static constructor sets up the configuration for push and pull options, ensuring that credentials are provided and merge conflicts are handled properly.
    /// 
    /// Methods:
    /// - <see cref="PushLocalChanges"/>: Stages all changes, commits them with a timestamped message, and pushes them to the remote repository.
    /// - <see cref="PullRemoteChanges"/>: Pulls updates from the remote repository and merges them into the local repository.
    /// </remarks>
    public static class Git
    {
        // Options for Git operations
        public static PushOptions pushOptions;
        public static PullOptions pullOptions = new();

        // Repository and author information
        public static LibGit2Sharp.Repository currentRepository = new(Repositories.LocalRepository);
        public static LibGit2Sharp.Signature author = new(Github.user.Login, Login.Mail, DateTimeOffset.Now);

        static Git()
        {
            // Configure push options with credentials
            pushOptions = new PushOptions
            {
                CredentialsProvider = (url, usernameFromUrl, types) => new UsernamePasswordCredentials
                {
                    Username = Github.user.Login,
                    Password = Login.Credentials
                }
            };

            // Configure pull options with merge and fetch options
            pullOptions.MergeOptions = new MergeOptions();
            pullOptions.MergeOptions.FailOnConflict = true;
            pullOptions.FetchOptions = new FetchOptions();
            pullOptions.FetchOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new UsernamePasswordCredentials()
            {
                Username = Github.user.Login,
                Password = Login.Credentials
            });
        }

        /// <summary>
        /// Stages all changes, commits them with a timestamped message, and pushes them to the remote repository.
        /// </summary>
        public static void PushLocalChanges()
        {
            try
            {
                // Stage all changes in the repository
                Commands.Stage(currentRepository, "*");

                // Commit the staged changes with a timestamped message
                currentRepository.Commit(
                    message: DateTimeOffset.Now.ToString(),
                    author: author,
                    committer: author);

                // Push the commit to the remote repository
                currentRepository.Network.Push(
                    branch: currentRepository.Branches[Github.currentRepository.DefaultBranch],
                    pushOptions: pushOptions);

                Debug.Log("Push changes", MessageType.Ok);
            }
            catch (EmptyCommitException e)
            {
                Debug.Log(e.Message, MessageType.Warning);
            }
            catch (Exception)
            {
                Debug.Log("Fail to push local changes.", MessageType.Error);
            }
        }

        /// <summary>
        /// Pulls updates from the remote repository and merges them into the local repository.
        /// </summary>
        public static void PullRemoteChanges()
        {
            try
            {
                // Pull changes from the remote repository
                Commands.Pull(
                    repository: currentRepository,
                    merger: author,
                    options: pullOptions);

                Debug.Log("Pull Changes.", MessageType.Ok);
            }
            catch (InvalidSpecificationException e)
            {
                Debug.Log(e.Message, MessageType.Error);
            }
            catch (LibGit2SharpException e)
            {
                Debug.Log(e.Message, MessageType.Error);
            }
            catch (Exception)
            {
                Debug.Log("Fail to pull remote changes in local repository", MessageType.Error);
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
                case MessageType.Ok:
                    msgType = "OK:";
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case MessageType.Warning:
                    msgType = "WARNING:";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case MessageType.Error:
                    msgType = "ERROR:";
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine($"{msgType} {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void CheckingReferences(Type type, bool getAllData = false , object? instance = null)
        {
            Console.WriteLine(type.Name);

            FieldInfo[] fields = getAllData?  type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) : type.GetFields();
            
            foreach (var info in fields)
            {
                if (info.GetValue(instance) == null)
                    Log($"Null Reference {type.Name}.{info.Name}", MessageType.Error);

                else
                    Log($"{type.Name}.{info.Name}", MessageType.Ok);
            }

            Console.WriteLine();
        }
    }
    #endregion
}
