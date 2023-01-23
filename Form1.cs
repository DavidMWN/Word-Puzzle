using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Word_Puzzle_Prototype
{
    public partial class Form1 : Form
    {
        private CharacterList characterList;
        private PhraseAndCategory phraseCategoryList;
        private List<char> encryptionList;
        private List<char> inputList;
        private List<char> decryptionList;
        private List<TextBox> allTextboxes;
        private List<int> encryptionIndexesUsed;
        private string solution;
        private int puzzleScoreStart = 15;
        private int currentPuzzleScore;
        private int scoreTotal = 0;
        private bool solved = true;
        private int givePenalty = 3;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblPuzzleText.Text = "";
            lblPuzzleOutput.Text = "";
            lblCategoryText.Text = "";

            characterList = new CharacterList();
            phraseCategoryList = new PhraseAndCategory();
            encryptionList = new List<char>();
            inputList = new List<char>();
            decryptionList = new List<char>();
            allTextboxes = new List<TextBox>();
            encryptionIndexesUsed = new List<int>();

            foreach (GroupBox g in Controls.OfType<GroupBox>())
            {
                foreach (TextBox t in g.Controls.OfType<TextBox>())
                    allTextboxes.Add(t);
            }

            currentPuzzleScore = puzzleScoreStart;
            lblTriesLeft.Text = currentPuzzleScore.ToString();
            lblScore.Text = scoreTotal.ToString();

            // Phrase/Category list taken from: https://sites.google.com/site/wheeloffortunepuzzlecompendium/home/compendium/season-31-compendium
            phraseCategoryList.LoadList("../../PhraseList.txt", "../../CategoryList.txt");
        }

        private void btnNewPuzzle_Click(object sender, EventArgs e)
        {
            if (solved)            
                solved = false;

            string[] temp = phraseCategoryList.GetNewPhrase().Split(';');

            temp[0].Trim();
            temp[1].Trim();

            lblPuzzleText.Text = temp[0];
            lblCategoryText.Text = temp[1];
            solution = lblPuzzleText.Text;
            lblPuzzleOutput.Text = "";

            currentPuzzleScore = puzzleScoreStart;
            lblTriesLeft.Text = currentPuzzleScore.ToString();

            NewPuzzle();

            if (!btnGiveVowels.Enabled)
                btnGiveVowels.Enabled = true;

            if (!btnGiveRSTLN.Enabled)
                btnGiveRSTLN.Enabled = true;

            if (!btnInputLetters.Enabled)
                btnInputLetters.Enabled = true;

            foreach (TextBox t in allTextboxes)
            {
                if (!t.Enabled)
                    t.Enabled = true;

                t.Clear();
            }
        }

        private void btnInputLetters_Click(object sender, EventArgs e)
        {
            ParseInput();
            CheckInput();
            CheckResult();            
        }

        private void btnGiveVowels_Click(object sender, EventArgs e)
        {
            char[] vowels = new char[] { 'A', 'E', 'I', 'O', 'U' };

            GiveLetters(vowels);
                        
            currentPuzzleScore -= givePenalty;
            
            btnGiveVowels.Enabled = false;

            ParseInput();
            CheckInput();
            CheckResult();
        }

        private void btnGiveRSTLN_Click(object sender, EventArgs e)
        {
            char[] rstln = new char[] { 'R', 'S', 'T', 'L', 'N' };

            GiveLetters(rstln);

            currentPuzzleScore -= givePenalty;

            btnGiveRSTLN.Enabled = false;

            ParseInput();
            CheckInput();
            CheckResult();
        }

        private void GiveLetters(char[] letters)
        {
            List<int> letterIndices = new List<int>();
            List<char> letterEncryptions = new List<char>();
            var sortedTextBoxes = allTextboxes.OrderBy(i => i.Name).ToArray();

            for (int i = 0; i < letters.Length; i++)
                letterEncryptions.Add(encryptionList[characterList.FindChar(letters[i])]);

            for (int i = 0; i < letterEncryptions.Count; i++)
                letterIndices.Add(characterList.FindChar(letterEncryptions[i]));

            for (int i = 0; i < letterIndices.Count; i++)
            {
                if (sortedTextBoxes[letterIndices[i]].Enabled)
                    sortedTextBoxes[letterIndices[i]].Text = characterList.GetChar(encryptionList.FindIndex(x => x == letterEncryptions[i])).ToString();
            }
        }

        private void NewPuzzle()
        {
            if (encryptionList.Count > 0)
            {
                encryptionList.Clear();
                decryptionList.Clear();
                encryptionIndexesUsed.Clear();
            }

            Random rnd = new Random();

            for (int i = 0; i < characterList.GetCharCount(); i++)
            {
                bool cont = false;

                while (cont == false) 
                {
                    int r = rnd.Next(0, characterList.GetCharCount());

                    if (characterList.GetUsed(r))
                        continue;

                    if (r == encryptionList.Count)
                        continue;

                    encryptionList.Add(characterList.GetChar(r));
                    decryptionList.Add('_');
                    characterList.UseChar(r);

                    cont = true;
                }
            }

            string temp = solution;
            List<char> encrypted = new List<char>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (!Char.IsLetter(temp[i]))
                {
                    encrypted.Add(temp[i]);
                    encryptionIndexesUsed.Add(-1);
                    continue;
                }

                encrypted.Add(encryptionList[characterList.FindChar(Char.ToUpper(temp[i]))]);
                encryptionIndexesUsed.Add(characterList.FindChar(Char.ToUpper(temp[i])));
            }

            var builder = new StringBuilder();

            foreach (char c in encrypted)
                builder.Append(c);

            lblPuzzleText.Text = builder.ToString();

            characterList.ResetUsed();
        }

        private void ParseInput()
        {
            if (inputList.Count > 0)
                inputList.Clear();

            var sortedTextboxes = allTextboxes.OrderBy(i => i.Name).ToArray();

            for (int i = 0; i < sortedTextboxes.Length; i++)
            {
                if (sortedTextboxes[i].Text == "")
                {
                    inputList.Add('*');
                    continue;
                }

                if (!Char.IsLetter(sortedTextboxes[i].Text[0]))
                {
                    inputList.Add('*');
                    continue;
                }

                inputList.Add(Char.ToUpper(sortedTextboxes[i].Text[0]));
            }
        }

        private void CheckInput()
        {
            var sortedTextBoxes = allTextboxes.OrderBy(i => i.Name).ToArray();

            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == '*')
                    continue;

                if (characterList.GetChar(i) == encryptionList[characterList.FindChar(inputList[i])])
                {
                    decryptionList[characterList.FindChar(inputList[i])] = inputList[i];
                    sortedTextBoxes[i].Enabled = false;
                }
            }

            string temp = solution;
            List<char> decrypted = new List<char>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (!Char.IsLetter(temp[i]))
                {
                    decrypted.Add(temp[i]);
                    continue;
                }

                decrypted.Add(decryptionList[encryptionIndexesUsed[i]]);
            }

            var builder = new StringBuilder();

            foreach (char c in decrypted)
                builder.Append(c);

            lblPuzzleOutput.Text = builder.ToString();
        }

        private void CheckResult()
        {
            if (solution == lblPuzzleOutput.Text)
            {
                scoreTotal += currentPuzzleScore;
                lblScore.Text = scoreTotal.ToString();
                btnGiveVowels.Enabled = false;
                btnInputLetters.Enabled = false;
                solved = true;
            }
            else if (currentPuzzleScore > 1)
            {
                currentPuzzleScore--;
                lblTriesLeft.Text = currentPuzzleScore.ToString();
            }
            else if (currentPuzzleScore <= 1)
            {
                currentPuzzleScore = 0;
                lblTriesLeft.Text = currentPuzzleScore.ToString();
                btnGiveVowels.Enabled = false;
                btnInputLetters.Enabled = false;
                lblPuzzleText.Text = solution;
            }

            if (currentPuzzleScore <= 5)
            {
                btnGiveVowels.Enabled = false;
                btnGiveRSTLN.Enabled = false;
            }
        }
    }
}
