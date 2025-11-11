using System.Text;

public class RegexToDfaConverter
{
    private readonly RegexParser _parser;
    private readonly NfaBuilder _nfaBuilder;
    private readonly NfaToDfaConverter _nfaToDfaConverter;

    public RegexToDfaConverter()
    {
        _parser = new RegexParser();
        _nfaBuilder = new NfaBuilder();
        _nfaToDfaConverter = new NfaToDfaConverter();
    }

    public DeterministicFiniteAutomaton RegexToDFA(string regex)
    {
        string explicitRegex = _parser.AddExplicitConcatenation(regex);
        string postfixRegex = _parser.ConvertToPostfix(explicitRegex);
        NondeterministicFiniteAutomaton nfa = _nfaBuilder.BuildFromPostfix(postfixRegex);
        DeterministicFiniteAutomaton dfa = _nfaToDfaConverter.Convert(nfa);
        return dfa;
    }

    public string GetPostfixForm(string regex)
    {
        string explicitRegex = _parser.AddExplicitConcatenation(regex);
        return _parser.ConvertToPostfix(explicitRegex);
    }

    public string AddExplicitConcatenation(string regex)
    {
        return _parser.AddExplicitConcatenation(regex);
    }

    public string ConvertToPostfix(string regex)
    {
        return _parser.ConvertToPostfix(regex);
    }
}
