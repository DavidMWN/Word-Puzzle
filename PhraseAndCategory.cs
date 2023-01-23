using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Word_Puzzle_Prototype
{
    class PhraseAndCategory
    {
        private List<string> phrases;
        private List<string> categories;
        private List<bool> used;
        Random rnd;

        public PhraseAndCategory()
        {
            phrases = new List<string>();
            categories = new List<string>();
            used = new List<bool>();
            rnd = new Random();
        }

        public void LoadList(string phraseFile, string categoryFile)
        {
            phrases = File.ReadAllLines(phraseFile).ToList();
            categories = File.ReadAllLines(categoryFile).ToList();

            foreach (string s in phrases)
                used.Add(false);
        }

        public string GetNewPhrase()
        {
            int r = rnd.Next(0, phrases.Count);

            while (used[r])
                r = rnd.Next(0, phrases.Count);

            used[r] = true;

            return phrases[r] + ";" + categories[r];            
        }
    }
}
