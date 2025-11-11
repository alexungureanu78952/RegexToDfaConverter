 public class NfaBuilder
{
    private int _stateCounter;

    public NondeterministicFiniteAutomaton BuildFromPostfix(string postfix)
    {
        Stack<NondeterministicFiniteAutomaton> stack = new Stack<NondeterministicFiniteAutomaton>();
        _stateCounter = 0;

        foreach (char c in postfix)
        {
            if (char.IsLetterOrDigit(c))
            {
                stack.Push(CreateNfaForCharacter(c));
            }
            else if (c == '|')
            {
                var nfa2 = stack.Pop();
                var nfa1 = stack.Pop();
                stack.Push(CreateNfaForAlternation(nfa1, nfa2));
            }
            else if (c == '.')
            {
                var nfa2 = stack.Pop();
                var nfa1 = stack.Pop();
                stack.Push(CreateNfaForConcatenation(nfa1, nfa2));
            }
            else if (c == '*')
            {
                var nfa = stack.Pop();
                stack.Push(CreateNfaForKleene(nfa));
            }
        }

        return stack.Pop();
    }

    private NondeterministicFiniteAutomaton CreateNfaForCharacter(char c)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        string startState = "q" + _stateCounter++;
        string finalState = "q" + _stateCounter++;

        nfa.States.Add(startState);
        nfa.States.Add(finalState);
        nfa.StartState = startState;
        nfa.FinalState = finalState;
        nfa.Alphabet.Add(c);

        nfa.Transitions[Tuple.Create(startState, (char?)c)] = new HashSet<string> { finalState };

        return nfa;
    }

    private NondeterministicFiniteAutomaton CreateNfaForAlternation(NondeterministicFiniteAutomaton nfa1, NondeterministicFiniteAutomaton nfa2)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        string newStart = "q" + _stateCounter++;
        string newFinal = "q" + _stateCounter++;

        foreach (var state in nfa1.States) nfa.States.Add(state);
        foreach (var state in nfa2.States) nfa.States.Add(state);
        nfa.States.Add(newStart);
        nfa.States.Add(newFinal);

        foreach (var symbol in nfa1.Alphabet) nfa.Alphabet.Add(symbol);
        foreach (var symbol in nfa2.Alphabet) nfa.Alphabet.Add(symbol);

        foreach (var transition in nfa1.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        foreach (var transition in nfa2.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }

        nfa.Transitions[Tuple.Create(newStart, (char?)null)] = new HashSet<string> { nfa1.StartState, nfa2.StartState };

        nfa.Transitions[Tuple.Create(nfa1.FinalState, (char?)null)] = new HashSet<string> { newFinal };
        nfa.Transitions[Tuple.Create(nfa2.FinalState, (char?)null)] = new HashSet<string> { newFinal };

        nfa.StartState = newStart;
        nfa.FinalState = newFinal;

        return nfa;
    }

    private NondeterministicFiniteAutomaton CreateNfaForConcatenation(NondeterministicFiniteAutomaton nfa1, NondeterministicFiniteAutomaton nfa2)
    {
        var nfa = new NondeterministicFiniteAutomaton();

        foreach (var state in nfa1.States) nfa.States.Add(state);
        foreach (var state in nfa2.States) nfa.States.Add(state);

        foreach (var symbol in nfa1.Alphabet) nfa.Alphabet.Add(symbol);
        foreach (var symbol in nfa2.Alphabet) nfa.Alphabet.Add(symbol);

        foreach (var transition in nfa1.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        foreach (var transition in nfa2.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }

        nfa.Transitions[Tuple.Create(nfa1.FinalState, (char?)null)] = new HashSet<string> { nfa2.StartState };

        nfa.StartState = nfa1.StartState;
        nfa.FinalState = nfa2.FinalState;

        return nfa;
    }

    private NondeterministicFiniteAutomaton CreateNfaForKleene(NondeterministicFiniteAutomaton nfa1)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        string newStart = "q" + _stateCounter++;
        string newFinal = "q" + _stateCounter++;

        foreach (var state in nfa1.States) nfa.States.Add(state);
        nfa.States.Add(newStart);
        nfa.States.Add(newFinal);

        foreach (var symbol in nfa1.Alphabet) nfa.Alphabet.Add(symbol);

        foreach (var transition in nfa1.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }

        nfa.Transitions[Tuple.Create(newStart, (char?)null)] = new HashSet<string> { nfa1.StartState, newFinal };

        nfa.Transitions[Tuple.Create(nfa1.FinalState, (char?)null)] = new HashSet<string> { nfa1.StartState, newFinal };

        nfa.StartState = newStart;
        nfa.FinalState = newFinal;

        return nfa;
    }
}

