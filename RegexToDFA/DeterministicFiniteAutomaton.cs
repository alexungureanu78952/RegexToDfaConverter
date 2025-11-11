using System.Text;

public class DeterministicFiniteAutomaton
{
    private HashSet<string> _states;
    private HashSet<char> _alphabet;
    private Dictionary<Tuple<string, char>, string> _transitions;
    private string _startState;
    private HashSet<string> _finalStates;

    public HashSet<string> States 
    { 
        get => _states; 
        set => _states = value; 
    }
    
    public HashSet<char> Alphabet 
    { 
        get => _alphabet; 
        set => _alphabet = value; 
    }
    
    public Dictionary<Tuple<string, char>, string> Transitions 
    { 
        get => _transitions; 
        set => _transitions = value; 
    }
    
    public string StartState 
    { 
        get => _startState; 
        set => _startState = value; 
    }
    
    public HashSet<string> FinalStates 
    { 
        get => _finalStates; 
        set => _finalStates = value; 
    }

    public DeterministicFiniteAutomaton()
    {
        _states = new HashSet<string>();
        _alphabet = new HashSet<char>();
        _transitions = new Dictionary<Tuple<string, char>, string>();
        _finalStates = new HashSet<string>();
        _startState = string.Empty;
    }

    public bool VerifyAutomaton()
    {
        if (string.IsNullOrEmpty(StartState) || !States.Contains(StartState))
        {
            return false;
        }

        foreach (var finalState in FinalStates)
        {
            if (!States.Contains(finalState))
            {
                return false;
            }
        }

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

    public string PrintAutomaton()
    {
        StringBuilder sb = new StringBuilder();
        
        sb.Append("Stare\t| ");
        foreach (char symbol in Alphabet)
        {
            sb.Append($"{symbol}\t| ");
        }
        sb.AppendLine();
        sb.AppendLine(new string('-', (Alphabet.Count + 1) * 8));

        foreach (string state in States)
        {
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
                    sb.Append("-\t| ");
                }
            }
            sb.AppendLine();
        }

        sb.AppendLine($"\nStare inițială: {StartState}");
        sb.AppendLine($"Stări finale: {{ {string.Join(", ", FinalStates)} }}");
        
        return sb.ToString();
    }

    public bool CheckWord(string word)
    {
        string currentState = StartState;

        foreach (char symbol in word)
        {
            if (!Alphabet.Contains(symbol))
            {
                return false;
            }

            var transitionKey = Tuple.Create(currentState, symbol);
            if (Transitions.TryGetValue(transitionKey, out string? nextState))
            {
                currentState = nextState;
            }
            else
            {
                return false;
            }
        }

        return FinalStates.Contains(currentState);
    }
}