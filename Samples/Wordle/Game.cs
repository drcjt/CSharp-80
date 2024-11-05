using System;

namespace Wordle
{
    internal class Game(string[] words)
    {
        private readonly string[] _words = words;

        public void Play()
        {
            var random = new Random();

            bool playing;
            do
            {
                Console.WriteLine("You have 6 guesses.");
                Console.WriteLine("Uppercase = letter present and in correct position.");
                Console.WriteLine("Lowercase = letter present but not in correct position.");
                Console.WriteLine("Underscore = letter not present");

                int attempts = 0;

                var wordIndex = random.Next(_words.Length);
                var word = _words[wordIndex];

                do
                {
                    var guessedWord = Console.ReadLine();
                    if (guessedWord.Length != 5)
                    {
                        // Ignore invalid guess and let player try again
                        Console.WriteLine("Invalid guess. Try again");
                        continue;
                    }

                    if (guessedWord == word)
                    {
                        Console.WriteLine("You won");
                        break;
                    }
                    else
                    {
                        attempts++;

                        ScoreGuessedWord(word, guessedWord);

                        Console.WriteLine();
                    }

                    if (attempts > 5)
                    {
                        Console.WriteLine("You lost");
                        break;
                    }
                } while (true);

                playing = PlayAgain();
            } while (playing);
        }

        private static bool PlayAgain()
        {
            Console.WriteLine("Press any key to play again. Q to quit");

            while (!Console.KeyAvailable)
            {
            }

            return Console.ReadKey(true).KeyChar != 'Q';
        }

        private static void ScoreGuessedWord(string word, string guessedWord)
        {
            for (int i = 0; i < 5; i++)
            {
                if (guessedWord[i] == word[i])
                {
                    // Upper case for present and in correct position
                    Console.Write(Char.ToUpper(guessedWord[i]));
                }
                else
                {
                    if (word.Contains(guessedWord[i]))
                    {
                        // Lower case for present and not in correct position
                        Console.Write(Char.ToLower(guessedWord[i]));
                    }
                    else
                    {
                        // Underscore for letter not present
                        Console.Write('_');
                    }
                }
            }
        }
    }
}
