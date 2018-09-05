using System.IO;
using System.Linq;

namespace TeamServicesTools.Web.Shared
{
    public class FileUtility
    {
        public static string RemoveInvalidFileCharacters(string input)
        {
            return input
                .ToCharArray()
                .Where(c => !Path.GetInvalidFileNameChars().Contains(c))
                .Aggregate(string.Empty, (current, c) => current + c);
        }
    }
}
