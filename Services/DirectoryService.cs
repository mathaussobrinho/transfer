using System.IO;

namespace TransferFilesApp.Services
{
    public static class DirectoryService
    {
        public static void CreateDirectories()
        {
            var baseDir = @"C:\Transfer";
            var enviarDir = Path.Combine(baseDir, "A enviar");
            var enviadosDir = Path.Combine(baseDir, "Enviados");

            Directory.CreateDirectory(enviarDir);
            Directory.CreateDirectory(enviadosDir);
        }
    }
}

