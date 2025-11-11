public class NfaToDfaConverter
{
    public DeterministicFiniteAutomaton Convert(NondeterministicFiniteAutomaton nfa)
    {
        var dfa = new DeterministicFiniteAutomaton();
        dfa.Alphabet = nfa.Alphabet;

        var dfaStatesMap = new Dictionary<HashSet<string>, string>(HashSet<string>.CreateSetComparer());
        var worklist = new Queue<HashSet<string>>();

        HashSet<string> nfaStartStateSet = EpsilonClosure(new HashSet<string> { nfa.StartState }, nfa);
        string dfaStartState = "Q0";

        dfa.StartState = dfaStartState;
        dfa.States.Add(dfaStartState);
        dfaStatesMap.Add(nfaStartStateSet, dfaStartState);
        worklist.Enqueue(nfaStartStateSet);

        int dfaStateCounter = 1;

        while (worklist.Count > 0)
        {
            HashSet<string> currentNfaStates = worklist.Dequeue();
            string currentDfaState = dfaStatesMap[currentNfaStates];

            if (currentNfaStates.Contains(nfa.FinalState))
            {
                dfa.FinalStates.Add(currentDfaState);
            }

            foreach (char symbol in dfa.Alphabet)
            {
                HashSet<string> moveSet = Move(currentNfaStates, symbol, nfa);
                HashSet<string> nextNfaStatesSet = EpsilonClosure(moveSet, nfa);

                if (nextNfaStatesSet.Count == 0)
                {
                    continue;
                }

                if (!dfaStatesMap.ContainsKey(nextNfaStatesSet))
                {
                    string newDfaState = "Q" + dfaStateCounter++;
                    dfa.States.Add(newDfaState);
                    dfaStatesMap.Add(nextNfaStatesSet, newDfaState);
                    worklist.Enqueue(nextNfaStatesSet);
                }

                string nextDfaState = dfaStatesMap[nextNfaStatesSet];
                dfa.Transitions[Tuple.Create(currentDfaState, symbol)] = nextDfaState;
            }
        }

        return dfa;
    }

    private HashSet<string> EpsilonClosure(HashSet<string> states, NondeterministicFiniteAutomaton nfa)
    {
        var closure = new HashSet<string>(states);
        var queue = new Queue<string>(states);

        while (queue.Count > 0)
        {
            string state = queue.Dequeue();

            var epsilonKey = Tuple.Create(state, (char?)null);
            if (nfa.Transitions.ContainsKey(epsilonKey))
            {
                foreach (string nextState in nfa.Transitions[epsilonKey])
                {
                    if (!closure.Contains(nextState))
                    {
                        closure.Add(nextState);
                        queue.Enqueue(nextState);
                    }
                }
            }
        }

        return closure;
    }

    private HashSet<string> Move(HashSet<string> states, char symbol, NondeterministicFiniteAutomaton nfa)
    {
        var result = new HashSet<string>();

        foreach (string state in states)
        {
            var transitionKey = Tuple.Create(state, (char?)symbol);
            if (nfa.Transitions.ContainsKey(transitionKey))
            {
                foreach (string nextState in nfa.Transitions[transitionKey])
                {
                    result.Add(nextState);
                }
            }
        }

        return result;
    }
}
