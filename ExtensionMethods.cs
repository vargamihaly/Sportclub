using System.Text;

namespace Rendelesek;

public static class ExtensionMethods
{
    public static void WriteToCsv(this IEnumerable<string> content, string destinationFolderPath)
    {
        var csvBuilder = new StringBuilder();
        
        foreach (var line in content)
        {
            csvBuilder.AppendLine(line);
        }
        
        File.WriteAllText(destinationFolderPath, csvBuilder.ToString());
    }
}