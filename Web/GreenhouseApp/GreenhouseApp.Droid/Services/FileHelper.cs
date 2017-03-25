using System.Collections.Generic;
using GreenhouseApp.Interfaces;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(GreenhouseApp.Droid.Services.FileHelper))]

namespace GreenhouseApp.Droid.Services
{

    class FileHelper : IFileHelper
    {
        public bool Exists(string filename) { string filepath = GetFilePath(filename); return File.Exists(filepath); }
        public void WriteText(string filename, string text) { string filepath = GetFilePath(filename); File.WriteAllText(filepath, text); }
        public string ReadText(string filename) { string filepath = GetFilePath(filename); return File.ReadAllText(filepath); }
        public IEnumerable<string> GetFiles() { return Directory.GetFiles(GetDocsPath()); }
        public void Delete(string filename) { File.Delete(GetFilePath(filename)); }

        // Private methods. 
        string GetFilePath(string filename) { return Path.Combine(GetDocsPath(), filename); }
        string GetDocsPath() { return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); }
    }
}
