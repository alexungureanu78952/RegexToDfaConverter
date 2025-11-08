using System;
using System.Collections.Generic;

// Clasă internă pentru a reprezenta AFN-λ (Thompson's Construction)
public class NondeterministicFiniteAutomaton
{
    // Un AFN are un set de stări, un alfabet, o singură stare de start
    // și o singură stare finală (conform construcției Thompson)
    public HashSet<string> States { get; set; }
    public HashSet<char> Alphabet { get; set; }
    
    // Tranzițiile pot fi pe un 'char' sau 'null' (pentru λ)
    // O tranziție duce la un SET de stări
    public Dictionary<Tuple<string, char?>, HashSet<string>> Transitions { get; set; }
    public string StartState { get; set; }
    public string FinalState { get; set; } // Doar o stare finală în construcția Thompson
    
    public NondeterministicFiniteAutomaton()
    {
        States = new HashSet<string>();
        Alphabet = new HashSet<char>();
        Transitions = new Dictionary<Tuple<string, char?>, HashSet<string>>();
        StartState = string.Empty;
        FinalState = string.Empty;
    }
}