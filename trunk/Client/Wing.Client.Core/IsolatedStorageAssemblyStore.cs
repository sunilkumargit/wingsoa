using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.IO.IsolatedStorage;

namespace Wing.Client.Core
{
    public class IsolatedStorageAssemblyStore : IAssemblyStore, IDisposable
    {
        private string storeDir;
        private IsolatedStorageFile _storage;

        #region IServerAssemblyStore Members

        public void SetBasePath(string dirName)
        {
            Unload();
            _storage = IsolatedStorageFile.GetUserStoreForApplication();
            storeDir = dirName;
            if (!_storage.DirectoryExists(dirName))
                _storage.CreateDirectory(dirName);
        }

        private String GetAssemblyPath(String name)
        {
            return String.Format(@"{0}\{1}", storeDir, name);
        }

        public void Unload()
        {
            if (_storage != null)
            {
                _storage.Dispose();
                _storage = null;
            }
        }

        public void AddAssembly(string name, byte[] data)
        {
            var asmPath = GetAssemblyPath(name);
            if (_storage.FileExists(asmPath))
                _storage.DeleteFile(asmPath);
            var stream = _storage.CreateFile(asmPath);
            try
            {
                stream.Write(data, 0, data.Length);
            }
            finally
            {
                stream.Close();
            }
        }

        public void RemoveAssembly(string name)
        {
            var asmPath = GetAssemblyPath(name);
            if (_storage.FileExists(asmPath))
                _storage.DeleteFile(asmPath);
        }

        public void ConsolidateStore()
        {
            //nothing to do
        }

        public String[] GetAssemblyNames()
        {
            return _storage.GetFileNames(Path.Combine(storeDir, "*.dll"));
        }

        public byte[] GetAssemblyData(String name)
        {
            var asmPath = GetAssemblyPath(name);
            if (_storage.FileExists(asmPath))
            {
                var stream = _storage.OpenFile(asmPath, FileMode.Open, FileAccess.Read);
                try
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    return data;
                }
                finally
                {
                    stream.Close();
                }
            }
            else
                return null;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}