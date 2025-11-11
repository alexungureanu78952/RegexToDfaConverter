using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        var fileManager = new FileManager();
        string regex = fileManager.ReadRegexFromFile("regex.txt");
        
        if (string.IsNullOrEmpty(regex))
        {
            Console.WriteLine("Eroare: Fișierul regex.txt nu există sau este gol.");
            return;
        }

        Console.WriteLine($"Expresie regulată citită: {regex}");
        Console.WriteLine();

        var converter = new RegexToDfaConverter();
        DeterministicFiniteAutomaton dfa;
        string postfixForm;
        
        try
        {
            dfa = converter.RegexToDFA(regex);
            postfixForm = converter.GetPostfixForm(regex);
            
            Console.WriteLine("Automatul finit determinist a fost construit cu succes!");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la construirea automatului: {ex.Message}");
            return;
        }

        var menuHandler = new MenuHandler(dfa, postfixForm);
        menuHandler.Run();
    }
}
