using GreenhouseApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Dependency(typeof(GreenhouseApp.iOS.Services.FileHelper))]

namespace GreenhouseApp.Windows.Services
{
    public class FileHelper : IFileHelper
    {
        public void Delete(string filename)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string filename)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFiles()
        {
            throw new NotImplementedException();
        }

        public string ReadText(string filename)
        {
            throw new NotImplementedException();
        }

        public void WriteText(string filename, string text)
        {
            throw new NotImplementedException();
        }
    }
}
