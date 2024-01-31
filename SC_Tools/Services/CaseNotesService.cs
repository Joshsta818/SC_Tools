using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC_Tools.Services
{
    public static class CaseNotesService
    {
        public static List<string> filePaths = new List<string>();

        public static void AddFilepath(string filepath)
        {
            if (File.Exists(filepath))
            {
                filePaths.Add(filepath);
            }
        }
        public static void CreateCaseNotesFile()
        {

        }
    }
}





/*Things needed to be gatherd for CASE NOTES
 * sys info
 * 
 * 
 */