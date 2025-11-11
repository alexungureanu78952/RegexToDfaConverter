public class NondeterministicFiniteAutomaton
{
    private HashSet<string> _states;
    private HashSet<char> _alphabet;
    private Dictionary<Tuple<string, char?>, HashSet<string>> _transitions;
    private string _startState;
    private string _finalState;

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
    
    public Dictionary<Tuple<string, char?>, HashSet<string>> Transitions 
    { 
        get => _transitions; 
        set => _transitions = value; 
    }
    
    public string StartState 
    { 
        get => _startState; 
        set => _startState = value; 
    }
    
    public string FinalState 
    { 
        get => _finalState; 
        set => _finalState = value; 
    }
    
    public NondeterministicFiniteAutomaton()
    {
        _states = new HashSet<string>();
        _alphabet = new HashSet<char>();
        _transitions = new Dictionary<Tuple<string, char?>, HashSet<string>>();
        _startState = string.Empty;
        _finalState = string.Empty;
    }
}