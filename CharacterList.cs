using System.Collections.Generic;
using System.Linq;

namespace Word_Puzzle_Prototype
{
    class CharacterList
    {
        private List<char> alphabetList;
        private List<bool> usedTracker;

        public CharacterList()
        {
            alphabetList = new List<char>();

            for (int i = 65; i < 91; i++)
            {
                alphabetList.Add((char)i);
            }

            usedTracker = new List<bool>();

            foreach (char c in alphabetList)
                usedTracker.Add(false);
        }

        public char GetChar(int index)
        {
            return alphabetList[index];
        }

        public int GetCharCount()
        {
            return alphabetList.Count();
        }

        public int FindChar(char c)
        {
            return alphabetList.FindIndex(x => x == c);
        }

        public void UseChar(int index)
        {
            usedTracker[index] = true;
        }

        public bool GetUsed(int index)
        {
            return usedTracker[index];
        }

        public void ResetUsed()
        {
            for (int i = 0; i < usedTracker.Count; i++)
                usedTracker[i] = false;
        }
    }
}
