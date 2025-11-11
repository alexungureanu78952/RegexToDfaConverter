public class AutomatonService
{
    public void DisplayAutomaton(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║     AUTOMATUL FINIT DETERMINIST                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();

        if (dfa.VerifyAutomaton())
        {
            Console.WriteLine("✓ Automatul este valid");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("✗ ATENȚIE: Automatul nu este valid!");
            Console.WriteLine();
        }

        Console.WriteLine(dfa.PrintAutomaton());
    }

    public void CheckSingleWord(DeterministicFiniteAutomaton dfa)
    {
        Console.Write("Introduceți cuvântul de verificat: ");
        string word = Console.ReadLine();

        bool accepted = dfa.CheckWord(word);

        Console.WriteLine();
        if (accepted)
        {
            Console.WriteLine($"✓ Cuvântul '{word}' este ACCEPTAT de automat.");
        }
        else
        {
            Console.WriteLine($"✗ Cuvântul '{word}' este RESPINS de automat.");
        }
        Console.WriteLine();
    }

    public void CheckMultipleWords(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("Introduceți cuvintele de verificat (separate prin virgulă):");
        string input = Console.ReadLine();

        string[] words = input.Split(',');
        Console.WriteLine();
        Console.WriteLine("Rezultate:");
        Console.WriteLine(new string('-', 50));

        foreach (string word in words)
        {
            string trimmedWord = word.Trim();
            bool accepted = dfa.CheckWord(trimmedWord);
            string status = accepted ? "✓ ACCEPTAT" : "✗ RESPINS";
            Console.WriteLine($"{status,-15} | {trimmedWord}");
        }
        Console.WriteLine();
    }
}

