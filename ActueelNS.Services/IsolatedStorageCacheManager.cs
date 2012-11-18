using System;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace ActueelNS.Services
{
    public static class IsolatedStorageCacheManager<T>
    {
        public static void Store(string filename, T obj)
        {
            try
            {
                IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication();
                using (IsolatedStorageFileStream fileStream = appStore.OpenFile(filename, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(fileStream, obj);
                }
            }
            catch(Exception e) 
            {
            }
        }

        public static T Retrieve(string filename)
        {
            T obj = default(T);
            try
            {
                IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (appStore.FileExists(filename))
                {
                    using (IsolatedStorageFileStream fileStream = appStore.OpenFile(filename, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        obj = (T)serializer.Deserialize(fileStream);
                    }
                }
            }
            catch(Exception e) 
            {
            }
            return obj;
        }

        public static void Delete(string filename)
        {
            try
            {
                IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication();
                appStore.DeleteFile(filename);
            }
            catch { }
        }
    }
}
