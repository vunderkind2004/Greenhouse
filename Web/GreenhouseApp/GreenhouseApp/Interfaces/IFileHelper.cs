using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseApp.Interfaces
{
    public interface IFileHelper
    {
        bool Exists(string filename);
        void WriteText(string filename, string text);
        string ReadText(string filename);
        IEnumerable<string> GetFiles();
        void Delete(string filename);
    }
}
