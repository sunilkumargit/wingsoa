using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;

namespace Wing.Server.Core
{
    public class FileSystemAssemblyStore : IAssemblyStore
    {
        private string storePath;
        private string storeAddedPath;
        private string storeRemovedPath;

        #region IServerAssemblyStore Members

        public void SetBasePath(string path)
        {
            storePath = path;
            storeAddedPath = Path.Combine(path, "add");
            storeRemovedPath = Path.Combine(path, "remove");
            Directory.CreateDirectory(storeAddedPath);
            Directory.CreateDirectory(storeRemovedPath);
            Unload();
        }

        private String GetAssemblyPath(String name, PathMode mode)
        {
            switch (mode)
            {
                case PathMode.Added: return Path.Combine(storeAddedPath, name);
                case PathMode.Removed: return Path.Combine(storeRemovedPath, name);
                case PathMode.Store: return Path.Combine(storePath, name);
            }
            return "";
        }

        public void Unload()
        {
        }

        public void AddAssembly(string name, byte[] data)
        {
            File.WriteAllBytes(GetAssemblyPath(name, PathMode.Added), data);
        }

        public void RemoveAssembly(string name)
        {
            //verificar se o assembly existe no diretorio added
            var addedPath = GetAssemblyPath(name, PathMode.Added);
            if (File.Exists(addedPath))
                File.Delete(addedPath);

            //verificar se ele já existe no store
            var storePath = GetAssemblyPath(name, PathMode.Store);
            if (File.Exists(storePath))
            {
                //gravar o assembly no directorio de remocao
                File.WriteAllBytes(GetAssemblyPath(name, PathMode.Removed), File.ReadAllBytes(storePath));
            }
        }

        public void ConsolidateStore()
        {
            //remover os assemblies que estão na pasta de remoção
            var removedFiles = Directory.GetFiles(storeRemovedPath, "*.dll");
            foreach (var file in removedFiles)
            {
                var fileName = Path.GetFileName(file);
                var fileStorePath = GetAssemblyPath(fileName, PathMode.Store);
                //existe na pasta store?
                if (File.Exists(fileStorePath))
                {
                    try
                    {
                        File.Delete(fileStorePath);
                        //excluir o arquivo do diretorio de remoção se tudo correu bem
                        File.Delete(file);
                    }
                    catch { }
                }
            }

            var addedFiles = Directory.GetFiles(storeAddedPath, "*.dll");
            foreach (var file in addedFiles)
            {
                var fileName = Path.GetFileName(file);
                var fileStorePath = GetAssemblyPath(fileName, PathMode.Store);
                try
                {
                    File.WriteAllBytes(fileStorePath, File.ReadAllBytes(file));
                    File.Delete(file);
                }
                catch { }
            }
        }

        public String[] GetAssemblyNames(PathMode mode)
        {
            var path = GetAssemblyPath("", mode);
            return Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                .Select(f => Path.GetFileName(f))
                .ToArray();
        }

        public byte[] GetAssemblyData(String name)
        {
            var filePath = GetAssemblyPath(name, PathMode.Store);
            if (!File.Exists(filePath))
                filePath = filePath += ".dll";
            return File.ReadAllBytes(filePath);
        }
        #endregion
    }
}
