using System.Text;

public class RegexToDfaConverter
{
    private int stateCounter = 0; // Folosit pentru a genera nume unice de stări (q0, q1, ...)

    // Funcția principală cerută 
    public DeterministicFiniteAutomaton RegexToDFA(string regex)
    {
        // Pasul 0: Adaugă operatorii de concatenare expliciți (ex. 'ab' -> 'a.b')
        string explicitRegex = AddExplicitConcatenation(regex);
        
        // Pasul I.a: Transformă expresia în formă postfixată 
        string postfixRegex = ConvertToPostfix(explicitRegex);
        // Aceasta va fi necesară și pentru Cerința 3 (afișare)

        // Pasul I.b: Construiește AFN-λ din forma postfixată
        NondeterministicFiniteAutomaton nfa = BuildNfaFromPostfix(postfixRegex);

        // Pasul II: Transformă AFN-λ în AFD
        DeterministicFiniteAutomaton dfa = ConvertNfaToDfa(nfa);

        return dfa;
    }

    // --- Pasul I.a: Conversia Postfixată ---
    private string AddExplicitConcatenation(string regex)
    {
        var result = new StringBuilder();
        
        for (int i = 0; i < regex.Length; i++)
        {
            char current = regex[i];
            result.Append(current);
            
            if (i < regex.Length - 1)
            {
                char next = regex[i + 1];
                
                // Adaugă '.' după un caracter/literă, ')' sau '*' dacă urmează un caracter/literă sau '('
                bool currentIsOperand = char.IsLetterOrDigit(current);
                bool currentIsCloseParen = current == ')';
                bool currentIsStar = current == '*';
                bool nextIsOperand = char.IsLetterOrDigit(next);
                bool nextIsOpenParen = next == '(';
                
                if ((currentIsOperand || currentIsCloseParen || currentIsStar) && 
                    (nextIsOperand || nextIsOpenParen))
                {
                    result.Append('.');
                }
            }
        }
        
        return result.ToString();
    }
    
    private string ConvertToPostfix(string regex)
    {
        // Implementează algoritmul Shunting-Yard 
        // Precedență: '*' (unar) > '.' (concatenare) > '|' (alternare)
        var output = new StringBuilder();
        var operatorStack = new Stack<char>();
        
        var precedence = new Dictionary<char, int>
        {
            { '|', 1 },
            { '.', 2 },
            { '*', 3 }
        };
        
        foreach (char c in regex)
        {
            if (char.IsLetterOrDigit(c))
            {
                // Operand -> adaugă direct la output
                output.Append(c);
            }
            else if (c == '(')
            {
                operatorStack.Push(c);
            }
            else if (c == ')')
            {
                // Pop până la '('
                while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                {
                    output.Append(operatorStack.Pop());
                }
                if (operatorStack.Count > 0)
                {
                    operatorStack.Pop(); // Elimină '('
                }
            }
            else if (precedence.ContainsKey(c))
            {
                // Operator
                while (operatorStack.Count > 0 && 
                       operatorStack.Peek() != '(' && 
                       precedence.ContainsKey(operatorStack.Peek()) &&
                       precedence[operatorStack.Peek()] >= precedence[c])
                {
                    output.Append(operatorStack.Pop());
                }
                operatorStack.Push(c);
            }
        }
        
        // Pop toți operatorii rămași
        while (operatorStack.Count > 0)
        {
            output.Append(operatorStack.Pop());
        }
        
        return output.ToString();
    }

    // --- Pasul I.b: Construcție AFN-λ (Thompson) ---
    private NondeterministicFiniteAutomaton BuildNfaFromPostfix(string postfix)
    {
        Stack<NondeterministicFiniteAutomaton> stack = new Stack<NondeterministicFiniteAutomaton>();
        stateCounter = 0;

        foreach (char c in postfix)
        {
            if (char.IsLetterOrDigit(c)) // Sau orice consideri tu a fi un simbol al alfabetului
            {
                // Construiește automat simplu pentru un caracter
                stack.Push(CreateNfaForCharacter(c));
            }
            else if (c == '|')
            {
                // Operator de alternare
                var nfa2 = stack.Pop();
                var nfa1 = stack.Pop();
                stack.Push(CreateNfaForAlternation(nfa1, nfa2));
            }
            else if (c == '.')
            {
                // Operator de concatenare
                var nfa2 = stack.Pop();
                var nfa1 = stack.Pop();
                stack.Push(CreateNfaForConcatenation(nfa1, nfa2));
            }
            else if (c == '*')
            {
                // Operator Kleene star
                var nfa = stack.Pop();
                stack.Push(CreateNfaForKleene(nfa));
            }
        }

        return stack.Pop(); // Automatul final
    }
    
    // Construiește AFN pentru un singur caracter
    private NondeterministicFiniteAutomaton CreateNfaForCharacter(char c)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        string startState = "q" + stateCounter++;
        string finalState = "q" + stateCounter++;
        
        nfa.States.Add(startState);
        nfa.States.Add(finalState);
        nfa.StartState = startState;
        nfa.FinalState = finalState;
        nfa.Alphabet.Add(c);
        
        // Tranziție de la start la final pe caracterul c
        nfa.Transitions[Tuple.Create(startState, (char?)c)] = new HashSet<string> { finalState };
        
        return nfa;
    }
    
    // Construiește AFN pentru alternare (nfa1 | nfa2)
    private NondeterministicFiniteAutomaton CreateNfaForAlternation(NondeterministicFiniteAutomaton nfa1, NondeterministicFiniteAutomaton nfa2)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        string newStart = "q" + stateCounter++;
        string newFinal = "q" + stateCounter++;
        
        // Copiază toate stările
        foreach (var state in nfa1.States) nfa.States.Add(state);
        foreach (var state in nfa2.States) nfa.States.Add(state);
        nfa.States.Add(newStart);
        nfa.States.Add(newFinal);
        
        // Copiază alfabetul
        foreach (var symbol in nfa1.Alphabet) nfa.Alphabet.Add(symbol);
        foreach (var symbol in nfa2.Alphabet) nfa.Alphabet.Add(symbol);
        
        // Copiază tranzițiile
        foreach (var transition in nfa1.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        foreach (var transition in nfa2.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        
        // Tranziții λ de la noul start către starturile lui nfa1 și nfa2
        nfa.Transitions[Tuple.Create(newStart, (char?)null)] = new HashSet<string> { nfa1.StartState, nfa2.StartState };
        
        // Tranziții λ de la finalurile lui nfa1 și nfa2 către noul final
        nfa.Transitions[Tuple.Create(nfa1.FinalState, (char?)null)] = new HashSet<string> { newFinal };
        nfa.Transitions[Tuple.Create(nfa2.FinalState, (char?)null)] = new HashSet<string> { newFinal };
        
        nfa.StartState = newStart;
        nfa.FinalState = newFinal;
        
        return nfa;
    }
    
    // Construiește AFN pentru concatenare (nfa1 . nfa2)
    private NondeterministicFiniteAutomaton CreateNfaForConcatenation(NondeterministicFiniteAutomaton nfa1, NondeterministicFiniteAutomaton nfa2)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        
        // Copiază toate stările
        foreach (var state in nfa1.States) nfa.States.Add(state);
        foreach (var state in nfa2.States) nfa.States.Add(state);
        
        // Copiază alfabetul
        foreach (var symbol in nfa1.Alphabet) nfa.Alphabet.Add(symbol);
        foreach (var symbol in nfa2.Alphabet) nfa.Alphabet.Add(symbol);
        
        // Copiază tranzițiile
        foreach (var transition in nfa1.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        foreach (var transition in nfa2.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        
        // Tranziție λ de la finalul lui nfa1 către startul lui nfa2
        nfa.Transitions[Tuple.Create(nfa1.FinalState, (char?)null)] = new HashSet<string> { nfa2.StartState };
        
        nfa.StartState = nfa1.StartState;
        nfa.FinalState = nfa2.FinalState;
        
        return nfa;
    }
    
    // Construiește AFN pentru Kleene star (nfa*)
    private NondeterministicFiniteAutomaton CreateNfaForKleene(NondeterministicFiniteAutomaton nfa1)
    {
        var nfa = new NondeterministicFiniteAutomaton();
        string newStart = "q" + stateCounter++;
        string newFinal = "q" + stateCounter++;
        
        // Copiază toate stările
        foreach (var state in nfa1.States) nfa.States.Add(state);
        nfa.States.Add(newStart);
        nfa.States.Add(newFinal);
        
        // Copiază alfabetul
        foreach (var symbol in nfa1.Alphabet) nfa.Alphabet.Add(symbol);
        
        // Copiază tranzițiile
        foreach (var transition in nfa1.Transitions)
        {
            nfa.Transitions[transition.Key] = new HashSet<string>(transition.Value);
        }
        
        // Tranziții λ pentru Kleene star
        // De la noul start către startul lui nfa1 și către noul final (pentru ε)
        nfa.Transitions[Tuple.Create(newStart, (char?)null)] = new HashSet<string> { nfa1.StartState, newFinal };
        
        // De la finalul lui nfa1 către startul lui nfa1 (pentru repetare) și către noul final
        nfa.Transitions[Tuple.Create(nfa1.FinalState, (char?)null)] = new HashSet<string> { nfa1.StartState, newFinal };
        
        nfa.StartState = newStart;
        nfa.FinalState = newFinal;
        
        return nfa;
    }
    
    // --- Pasul II: Conversie AFN-λ -> AFD (Subset Construction) ---
    private DeterministicFiniteAutomaton ConvertNfaToDfa(NondeterministicFiniteAutomaton nfa)
    {
        var dfa = new DeterministicFiniteAutomaton();
        dfa.Alphabet = nfa.Alphabet;

        // Folosește un dicționar pentru a mapa seturi de stări NFA la noile stări DFA
        var dfaStatesMap = new Dictionary<HashSet<string>, string>(HashSet<string>.CreateSetComparer());
        var worklist = new Queue<HashSet<string>>(); // Coada stărilor DFA de procesat

        // 1. Calculează starea de start a AFD-ului
        // q0' = λ-închid(q0_nfa)
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

            // Verifică dacă această nouă stare DFA este finală
            // (dacă conține starea finală a AFN-ului)
            if (currentNfaStates.Contains(nfa.FinalState))
            {
                dfa.FinalStates.Add(currentDfaState);
            }

            // 2. Pentru fiecare simbol din alfabet...
            foreach (char symbol in dfa.Alphabet)
            {
                // Calculează 'Move(stare, simbol)'
                HashSet<string> moveSet = Move(currentNfaStates, symbol, nfa);
                
                // Calculează 'λ-închid(Move(...))'
                HashSet<string> nextNfaStatesSet = EpsilonClosure(moveSet, nfa);

                if (nextNfaStatesSet.Count == 0)
                {
                    continue; // Nu există tranziție, merge către "starea groapă" (implicit)
                }

                // 3. Verifică dacă acest set de stări AFN este o nouă stare AFD
                if (!dfaStatesMap.ContainsKey(nextNfaStatesSet))
                {
                    string newDfaState = "Q" + dfaStateCounter++;
                    dfa.States.Add(newDfaState);
                    dfaStatesMap.Add(nextNfaStatesSet, newDfaState);
                    worklist.Enqueue(nextNfaStatesSet);
                }

                // 4. Adaugă tranziția în AFD
                string nextDfaState = dfaStatesMap[nextNfaStatesSet];
                dfa.Transitions[Tuple.Create(currentDfaState, symbol)] = nextDfaState;
            }
        }

        return dfa;
    }

    // Calculează λ-închiderea (λ-closure) pentru un set de stări
    private HashSet<string> EpsilonClosure(HashSet<string> states, NondeterministicFiniteAutomaton nfa)
    {
        var closure = new HashSet<string>(states);
        var stack = new Stack<string>(states);
        
        while (stack.Count > 0)
        {
            string state = stack.Pop();
            
            // Caută tranziții λ (null) din această stare
            var epsilonKey = Tuple.Create(state, (char?)null);
            if (nfa.Transitions.ContainsKey(epsilonKey))
            {
                foreach (string nextState in nfa.Transitions[epsilonKey])
                {
                    if (!closure.Contains(nextState))
                    {
                        closure.Add(nextState);
                        stack.Push(nextState);
                    }
                }
            }
        }
        
        return closure;
    }

    // Calculează 'Move' pentru un set de stări și un simbol
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

