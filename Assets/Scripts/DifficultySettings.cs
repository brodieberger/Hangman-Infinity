using TMPro;

public class DifficultySettings
{
    /* public enum Difficulty { Easy, Normal, Hard } */
    public int MaxMistakes { get; private set; }
    public int HintsLeft { get; private set; }
    public int HintsToReturn { get; private set; } // Number of hints to give back to the player after a correct guess

    public float MaxTime {get; private set;}
    public float ScoreMultiplier { get; private set; }

    public DifficultySettings(string difficulty)
    {
        MaxTime = 60;

        switch (difficulty)
        {
            case "Easy":
                MaxMistakes = 8;
                HintsLeft = 5;
                HintsToReturn = 3;
                ScoreMultiplier = 1f;
                break;
            case "Normal":
                MaxMistakes = 5;
                HintsLeft = 3;
                HintsToReturn = 2;
                ScoreMultiplier = 1.2f;
                break;
            case "Hard":
                MaxMistakes = 3;
                HintsLeft = 2;
                HintsToReturn = 1;
                ScoreMultiplier = 1.5f;
                break;
        }
    }
}
