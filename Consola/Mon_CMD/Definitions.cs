namespace Mon.DataTypes
{
    public struct Config
    {
        public const int DefaultTimeTask = 60;
        public const int DefaultInputTimeTask = 4;
    }

    public struct JsonPaths
    {
        public const string CommitsPath = @"C:\Users\Mondongo\Documents\Repositorios\Mon\Consola\Mon_CMD\Commits.json";
        public const string RelativeCommitsPath = @"Commits.json";
    }

    public struct DefaultPaths
    {
        public const string DownloadCommitsPaths = "C:\\Users\\Mondongo\\Documents\\Repositorios\\Mon\\Consola\\Mon_CMD\\DownloadsCommits\\";
        public const string DownloadCommitsRelativePath = "DownloadsCommits\\";
    }

    public struct Definitions
    {
        public const string ProgramName = "Mon";
        public const string Semestre1xlsx = "Semestre1.xlsx";
        public const string gitignore = ".gitignore";
    }

    public struct Login
    {
        public const string UserName = "JustinnDev";
        public const string Credentials = "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";
        public const string Mail = "justinalmaodev@gmail.com";
    }

    public struct Repositories
    {
        public const string RepositoryName = "PruebaMon";
        public const string LocalRepository = @"C:\Users\Mondongo\Documents\Repositorios\PruebaMon";
        public const string RemoteRepository = @"https://github.com/JustinnDev/PruebaMon";
    }

    public struct Contents
    {
        public const string GitIgnore = "# Ignorar archivos temporales de Microsoft Office\r\n~$*.docx\r\n~$*.xlsx\r\n~$*.pptx\r\n\r\n# Ignorar archivos de respaldo de Microsoft Office\r\n*.bak\r\n*.tmp\r\n*.swp\r\n\r\n# Ignorar archivos de caché de Microsoft Office\r\n*.docx~\r\n*.xlsx~\r\n*.pptx~\r\n\r\n# Ignorar archivos de configuración de Microsoft Office\r\n*.xlsb\r\n\r\n# Ignorar archivos de salida generados por herramientas ofimáticas\r\n*.pdf\r\n\r\n# Ignorar archivos de caché de IDEs\r\n.idea/\r\n*.suo\r\n*.ntvs_analysis.dat\r\n*.njsproj\r\n*.sln.docstates\r\n*.vs/\r\n\r\n# Ignorar archivos de compilación y directorios de compilación\r\nbuild/\r\nbin/\r\nobj/\r\n\r\n# Ignorar archivos específicos del sistema operativo\r\n.DS_Store\r\nThumbs.db\r\nDesktop.ini\r\n\r\n# Ignorar archivos específicos de editores de texto\r\n*.sublime-workspace\r\n*.sublime-project\r\n*.vscode/\r\n\r\n# Ignorar archivos de logs\r\n*.log\r\n\r\n# Ignorar archivos generados por herramientas de pruebas\r\ncoverage/\r\n*.gcov\r\n*.clover\r\n*.tgz\r\n*.zip\r\n\r\n# Ignorar archivos de base de datos locales\r\n*.sqlite\r\n*.db\r\n*.db-journal\r\n\r\n# Ignorar archivos de caché de paquetes de gestión\r\nnode_modules/\r\n";
    }

    public enum MessageType
    {
        Ok,
        Warning,
        Error
    }
}
