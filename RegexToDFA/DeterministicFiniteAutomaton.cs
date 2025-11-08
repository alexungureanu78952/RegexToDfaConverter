using System.Text;

public class DeterministicFiniteAutomaton
{
    // Membrii clasei: Q, Σ, δ, q0, F
    public HashSet<string> States { get; set; }
    public HashSet<char> Alphabet { get; set; }
    public Dictionary<Tuple<string, char>, string> Transitions { get; set; }
    public string StartState { get; set; }
    public HashSet<string> FinalStates { get; set; }

    public DeterministicFiniteAutomaton()
    {
        States = new HashSet<string>();
        Alphabet = new HashSet<char>();
        Transitions = new Dictionary<Tuple<string, char>, string>();
        FinalStates = new HashSet<string>();
        StartState = string.Empty;
    }

    // Metoda pentru verificarea validității automatului
    public bool VerifyAutomaton()
    {
        // 1. Starea inițială q0 aparține mulțimii Q
        if (string.IsNullOrEmpty(StartState) || !States.Contains(StartState))
        {
            return false;
        }

        // 2. Toate stările finale F se află în Q
        foreach (var finalState in FinalStates)
        {
            if (!States.Contains(finalState))
            {
                return false;
            }
        }

        // 3. Funcția de tranziție conține doar simboluri din Σ și stări din Q
        foreach (var transition in Transitions)
        {
            var (state, symbol) = transition.Key;
            var nextState = transition.Value;

            if (!States.Contains(state) || !Alphabet.Contains(symbol) || !States.Contains(nextState))
            {
                return false;
            }
        }

        return true;
    }

    // Metoda pentru afișarea tabelului de tranziții
    public string PrintAutomaton()
    {
        StringBuilder sb = new StringBuilder();
        
        // Header (Alfabetul)
        sb.Append("Stare\t| ");
        foreach (char symbol in Alphabet)
        {
            sb.Append($"{symbol}\t| ");
        }
        sb.AppendLine();
        sb.AppendLine(new string('-', (Alphabet.Count + 1) * 8));

        // Rânduri (Stările)
        foreach (string state in States)
        {
            // Marchează starea inițială și finală
            string stateLabel = state;
            if (state == StartState) stateLabel = "->" + stateLabel;
            if (FinalStates.Contains(state)) stateLabel = stateLabel + "*";
            
            sb.Append($"{stateLabel}\t| ");

            foreach (char symbol in Alphabet)
            {
                var transitionKey = Tuple.Create(state, symbol);
                if (Transitions.TryGetValue(transitionKey, out string? nextState))
                {
                    sb.Append($"{nextState}\t| ");
                }
                else
                {
                    sb.Append("-\t| "); // Fără tranziție
                }
            }
            sb.AppendLine();
        }

        sb.AppendLine($"\nStare inițială: {StartState}");
        sb.AppendLine($"Stări finale: {{ {string.Join(", ", FinalStates)} }}");
        
        return sb.ToString();
    }

    // Metoda pentru verificarea unui cuvânt
    public bool CheckWord(string word)
    {
        string currentState = StartState;

        foreach (char symbol in word)
        {
            // Verifică dacă simbolul este în alfabet
            if (!Alphabet.Contains(symbol))
            {
                return false; // Respins (simbol necunoscut)
            }

            var transitionKey = Tuple.Create(currentState, symbol);
            if (Transitions.TryGetValue(transitionKey, out string? nextState))
            {
                currentState = nextState;
            }
            else
            {
                return false; // Respins (tranziție blocată)
            }
        }

        // Acceptat doar dacă starea curentă este o stare finală
        return FinalStates.Contains(currentState);
    }
}