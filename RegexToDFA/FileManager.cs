using System.IO;

public class FileManager
{
    public string ReadRegexFromFile(string filename)
    {
        try
        {
            string[] possiblePaths = new string[]
            {
                filename,
                Path.Combine("..", filename),
                Path.Combine("..", "..", filename),
                Path.Combine("..", "..", "..", filename),
                Path.Combine("RegexToDFA", filename),
                Path.Combine("bin", "Debug", "net9.0", filename),
                Path.Combine(@"D:\cod\LFC\RegexToDFA", filename),
                Path.Combine(@"D:\cod\LFC\RegexToDFA\RegexToDFA", filename)
            };

            Console.WriteLine($"Căutare fișier '{filename}'...");
            Console.WriteLine($"Directorul curent: {Directory.GetCurrentDirectory()}");
            Console.WriteLine();

            foreach (string path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                Console.WriteLine($"Verific: {fullPath}");

                if (File.Exists(fullPath))
                {
                    string content = File.ReadAllText(fullPath).Trim();
                    Console.WriteLine($"✓ Fișier găsit!");
                    Console.WriteLine();
                    return content;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"✗ Fișierul '{filename}' nu a fost găsit în nicio locație.");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea fișierului: {ex.Message}");
        }
        return null;
    }

    public void SaveToFile(string filename, string content)
    {
        try
        {
            File.WriteAllText(filename, content);
            Console.WriteLine($"✓ Conținutul a fost salvat în fișierul '{filename}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Eroare la salvarea fișierului: {ex.Message}");
        }
    }
}
