using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using UnityEngine.UI;
using UnityEditor;
using System.Runtime.CompilerServices;


public class GameManager : MonoBehaviour
{
    [SerializeField] private ScoreAnimator scoreAnimator;
    private DifficultySettings difficultySettings;
    public char userPressedLetter { get; private set; }
    public List<string> wordList = new(); // to store all words from a certain category, not really needed after adding the Dictionary.
    private Dictionary<string, List<string>> categorizedWords = new();
    public List<string> wordToGuess = new(); //this here contains the word that needs to be guessed as chars
    public string[] lettersGuessed;
    public List<TMP_Text> letterHolderList = new(); // to store the instantiates of objects (_) 
    public GameObject letterPrefab;
    public Transform letterHolder;
    public TMP_Text attemptsText, timerText, scoreText, categoryText, hintsText, lettersEnteredText, messageText;
    bool gameOver = false;
    float timer = 0f;
    public int score = 0;
    private int mistakes = 0;  //overall mistakes that affect number of attempts.
    private int maxMistakes;
    private int hintsLeft;
    public bool updateStick = true; // These two variables are passed to the StickmanScript in order for it to update
    public bool success; //
    public List<string> lettersEntered = new();
    int wordsCompleted;
    int changeDifficultyCount = 0;
    int wordsTarget;
    public int currentWordMistakes = 0;  //if current word is guessed without any errors, then increase score multiplier. R esets for new word.
    int wordsGuessedPerfectly;
    public string selectedGamemode;
    public string selectedDifficulty;
    public GameObject messagePanel;
    int numberOfHintsUsed = 0;
    float timeSpent = 0f;

    public LoadEndScreen loadEndScreen;
    /* public GameObject leaderboardButton; */
    
    // Start is called before the first frame update
    void Start()
    {    
        HideMessage();
        updateStick = true; //stickman will show strings based on difficulty.
        ApplyDifficulty();
        InitializeGameMode();
        Initialize();

    }
    void Update(){
        if (!gameOver){
            if(selectedGamemode == "ScoreAttack"){
                timer -= Time.deltaTime;    
                timeSpent += Time.deltaTime;   
                if(timer <= 0){ 
                    timer = 0;
                    gameOver = true;
                    if(wordsCompleted == 0){
                        UpdateScore(score * -1);
                        DisplayMessage($"Game Over! 0 Words Guessed!");
                        RevealMissingLetters(); 
                    }
                    if(timer == 0 && wordsCompleted > 0 && wordsCompleted < wordsTarget){
                        int wordsLeft = (wordsTarget - wordsCompleted) * -5;
                        UpdateScore(wordsLeft);
                        DisplayMessage($"Game Over! Words left: {wordsTarget - wordsCompleted}", 3f);         
                        StartCoroutine(DelayScore(1.0f));
                        RevealMissingLetters();              
                    }
  
                    AudioManager.Instance.StopMusic();
                }
                UpdateTimerText();
            }    
        }

        if (Input.anyKeyDown && Input.GetMouseButtonDown(0) == false && Input.GetMouseButtonDown(1) == false && Input.GetMouseButtonDown(2) == false)
        {
           
            if (!string.IsNullOrEmpty(Input.inputString)) {
                char keyPressed = Input.inputString.ToUpper()[0];  // Get the input key as an uppercase character
                
                if (char.IsLetterOrDigit(keyPressed)) {
                    userPressedLetter = keyPressed;
                    InputButton();
                } else {
                    DisplayMessage("Invalid input.");
                }
            }
        }
    }

    void ApplyDifficulty(){

        selectedDifficulty = MenuScript.difficulty;
        difficultySettings = new DifficultySettings(selectedDifficulty);
        Debug.Log("Difficulty is: " + selectedDifficulty);

        maxMistakes = difficultySettings.MaxMistakes;
        hintsLeft = difficultySettings.HintsLeft; 

        attemptsText.text = "Attempts: " + maxMistakes; 
        hintsText.text = "Hints left: " + hintsLeft;

    }

    void InitializeGameMode(){
        selectedGamemode = MenuScript.gamemode;
        switch (selectedGamemode)
        {
            case "Endless":
                AudioManager.Instance.PlayEndlessMusic();
                Debug.Log("Endless Mode Initialized");
                DisplayMessage("Endless Mode Has Begun!\nStart guessing letters!");
                timerText.gameObject.SetActive(false);
                wordsCompleted = 0;
                wordsGuessedPerfectly = 0;
               
                break;
            case "ScoreAttack":
                AudioManager.Instance.PlayScoreAttackMusic();
                timerText.gameObject.SetActive(true);
                wordsTarget = 5; //fixed value - for every difficulty
                Debug.Log("Score Attack Mode Initialized with target words: " + wordsTarget);
                DisplayMessage("Solve 5 words to complete challenge.", 2.0f);
                wordsCompleted = 0;
                timer = difficultySettings.MaxTime;
                break;
        }
    }

    public void RevealLetterHint(){
        if(hintsLeft > 0 && !gameOver) {

            numberOfHintsUsed++;
            int HintUsedScore = numberOfHintsUsed * -5;
            UpdateScore(HintUsedScore);    
                          
            hintsLeft--;
            hintsText.text = "Hints left: "+ hintsLeft; //update hint every time user clicks button
               
            List<int> unrevealedLettersIndices = new List<int>();
            //loop with size of the actual random word to guess
            //if a letter from that word has not been found/guessed, then add its index to the list
            for(int i = 0; i < wordToGuess.Count; i++){
                if(lettersGuessed[i] == null){
                    unrevealedLettersIndices.Add(i);
                }
            }

            //pick a random index of the letters that has not been guessed.
            if(unrevealedLettersIndices.Count > 0){
                int randomIndex = unrevealedLettersIndices[Random.Range(0, unrevealedLettersIndices.Count)];

                //new feature: add letters from hint to the lettersEntered list and display all the letters guessed in the game 
                //to keep track of letters entered already.
                string revealedLetter = wordToGuess[randomIndex];
                if(!lettersEntered.Contains(revealedLetter)){
                    lettersEntered.Add(revealedLetter);
                    UpdateLettersEnteredDisplay();
                }

                GuessLetter(wordToGuess[randomIndex], true);   
            }

            else {
                Debug.Log("All letters revealed or no hints left.");
                DisplayMessage("All letters revealed or no hints left.");
            }
        }
        else {
            Debug.Log("No hints left.");
            DisplayMessage("No hints left.");
        }
    }

    void Initialize()
    {   
        // Pick a random category
        string currentCategory = ChooseRandomCategory();
    
        // Pick a random word within the selected category
        string selectedWord = ChooseRandomWordFromCategory(currentCategory);

        //This wordList is to see the words in the inspector. Not really needed.
        wordList = categorizedWords[currentCategory];

        //Split words
        char[] splittedWord = selectedWord.ToCharArray();
        lettersGuessed = new string[splittedWord.Length];

        //Create visual - underscores and intiliaze word to guess
        for(int i = 0; i < splittedWord.Length; i++){

            GameObject temp = Instantiate(letterPrefab, letterHolder, false);
            TMP_Text prefabText = temp.GetComponent<TMP_Text>();
            
            if(splittedWord[i] == ' ' || splittedWord[i] == ':' || splittedWord[i] == ',' || splittedWord[i] == '.' || splittedWord[i] == '\'' || splittedWord[i] == '-'){ 
                prefabText.text = splittedWord[i].ToString();
                lettersGuessed[i] = splittedWord[i].ToString();   
            }
            else {
                prefabText.text = "_";    
                lettersGuessed[i] = null;       
            }

            wordToGuess.Add(splittedWord[i].ToString());
            letterHolderList.Add(prefabText);
        }
    }
    string ChooseRandomCategory(){

        string suffix = "_" + selectedDifficulty + ".txt"; // Ensure the underscore is included for accurate replacement.
        string[] fileNames = {
        "Movies" + suffix,
        "States" + suffix,
        "Cities" + suffix,
        "Phrases" + suffix,
        "Food" + suffix,
        "Video_Games" + suffix,
        "TV_Shows" + suffix,
        "Famous_People" + suffix
        };
        string fileName = GetRandomFile(fileNames);
        LoadWordsFromFile(fileName);

        string cleanCategory = Path.GetFileNameWithoutExtension(fileName).Replace(suffix.Replace(".txt", ""), "").Replace("_", " ");
        categoryText.text = "Category: " + cleanCategory;
        
        return cleanCategory;
    }

    string ChooseRandomWordFromCategory(string category){
        List<string> wordsInCategory = categorizedWords[category];
        return wordsInCategory[Random.Range(0, wordsInCategory.Count)].ToUpper();
    }

    //Get input letter after clicking button
    public void InputButton()
    {   
        string inputLetter = userPressedLetter.ToString();
        if (gameOver || string.IsNullOrWhiteSpace(userPressedLetter.ToString()))
        {
            userPressedLetter = '\0';
            return;
        }

        if(Regex.IsMatch(inputLetter, @"^[A-Z0-9]$"))
        {
            //to keep track of letters entered
            if (!lettersEntered.Contains(inputLetter))
            {
                lettersEntered.Add(inputLetter);
                UpdateLettersEnteredDisplay();
                GuessLetter(inputLetter);
            }
            else
            {
                Debug.Log("You already guessed that letter");
                DisplayMessage("You already guessed that letter");
                
            }
        }
        else {
            Debug.Log("Please enter a single character");
        }

        userPressedLetter = '\0';

    }

    void GuessLetter(string letter, bool isFromHint = false)
    {
        int countRepeatedLetters = 0; //counter for repeated letters
        bool hasFoundLetter = false;
        for (int i = 0; i < wordToGuess.Count; i++)
        {
            if (wordToGuess[i] == letter && lettersGuessed[i] == null)
            {
                //add letters guessed
                lettersGuessed[i] = letter; 
                letterHolderList[i].text = letter; 
                hasFoundLetter = true;
                countRepeatedLetters++;
                updateStick = true;//user made a mistake, update stick to show one less life
            }
        }

        if (hasFoundLetter && !isFromHint) // Check if the guess is not from a hint
        {   
            AudioManager.Instance.PlaySFX(AudioManager.Instance.correct);
            int baseScore = countRepeatedLetters * 10; //if a word contains duplicate letters then multiply by that count.
            //add adjusted score based on difficulty:
            int adjustedScore = Mathf.CeilToInt(baseScore * difficultySettings.ScoreMultiplier); 
            UpdateScore(adjustedScore);
        }

        //lose points when giving incorrect letter
        if (!hasFoundLetter && !isFromHint)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.incorrect);
            currentWordMistakes++;
            mistakes++; 
            UpdateScore(-5);
            updateStick = true;//user made a mistake, update stick to show one less life.
            //Reset wordsGuessedPerfectly to ensure bonuses only for consecutive perfect guesses.
            wordsGuessedPerfectly = 0; 
            if (mistakes >= maxMistakes)
            {
                Debug.Log("Game Over! You ran out of attempts.");
                DisplayMessage("Game Over! You ran out of attempts.", 5.0f);
                loadEndScreen.StartRising();
                gameOver = true;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.death);
                AudioManager.Instance.StopMusic();
                RevealMissingLetters();
                /* leaderboardButton.SetActive(true); */
            }

        }
        
        UpdateAttemptsText();

        if (CheckIfWon())
        {
            Debug.Log("Congratulations! You guessed the word.");
            wordsCompleted++;
            changeDifficultyCount++;
            if (selectedGamemode == "Endless")
            { 
                success = true;//for stick script. This one resets the ragdoll and reenables his limbs, so that if it gets an extra life, it doesn't appear limp. On score attack you dont get extra lives so im disabling it here.
                UpdateDifficulty();
            } 

            StartCoroutine(ShowCompleteWordGuessed());          
        }      
    }

    void RevealMissingLetters(){
        for (int i = 0; i < lettersGuessed.Length; i++){
            if(lettersGuessed[i] == null){
                letterHolderList[i].text = wordToGuess[i];
                letterHolderList[i].color = Color.red;
            }
        }
    }

    //This is just to show the complete word for a moment instead of directly initializing the next word
    IEnumerator ShowCompleteWordGuessed(){
        yield return new WaitForSeconds(0.7f);
        ClearPreviousWord();
        updateStick = true;//after the reset, gives the stick figure his extra lives if he earned any.
    }

    //This will verify that every single letter has been guessed.
    bool CheckIfWon(){
        for(int i = 0; i < lettersGuessed.Length; i++){

            if(wordToGuess[i] == " " && lettersGuessed != null) continue;

            if(lettersGuessed[i] == null) {
                return false;
            }
        }
        return true;
    }

    void UpdateTimerText(){

        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text =  $"Time: {seconds}";
    }

    void UpdateAttemptsText(){ 
        int remainingAttempts = maxMistakes - mistakes;
        attemptsText.text = "Attempts: " + remainingAttempts.ToString();
    }

    void UpdateScore(int _score){

        score += _score;
        if(score < 0){
            score = 0;
        }
        scoreText.text = score.ToString();
        scoreAnimator.AnimateScoreChange(_score);
    }
    void UpdateLettersEnteredDisplay(){
        lettersEnteredText.text = string.Join(", ", lettersEntered);
    }

    //Reset time and attempts, then get another random word.
    void ClearPreviousWord(){ 
        Debug.Log("ClearPreviousWord started");

        foreach (TMP_Text text in letterHolderList){
            //Debug.Log($"Destroying GameObject: {text.gameObject.name}");
            Destroy(text.gameObject);
        }

        letterHolderList.Clear();
        wordToGuess.Clear();
        lettersEntered.Clear();
        UpdateLettersEnteredDisplay();
        gameOver = false;
        lettersGuessed = null; 
        HandleGameModes(); 
        currentWordMistakes = 0;
        Debug.Log("words completed: " + wordsCompleted);
       
        if(!gameOver){
            Initialize();
        } 
    }

    void HandleGameModes(){
        switch(selectedGamemode){
            case "Endless":      
                if(currentWordMistakes == 0){
                    wordsGuessedPerfectly++;
                    int perfectGuessBonus = 5 + (50 * wordsGuessedPerfectly); //bonus score if no mistakes. 
                    UpdateScore(perfectGuessBonus);
                    Debug.Log($"Perfect guess! Bonus awarded: {perfectGuessBonus}. Total words guessed: {wordsCompleted}");
                    DisplayMessage($"Perfect guess! Bonus +: {perfectGuessBonus}");
                }

                int bonusLives = difficultySettings.HintsToReturn; //get lives back     
                //updateStick = true; //update strings when getting lives back
                mistakes = Mathf.Max(mistakes - bonusLives, 0);      
      
                Debug.Log($"max mistakes: {maxMistakes} and mistakes {mistakes}");
                UpdateAttemptsText(); 
                break;  
            case "ScoreAttack":
                if(wordsCompleted >= wordsTarget) {
                    gameOver = true;
                    //CalculateAttackModeFinalScore();
                    StartCoroutine(DelayScore(2f));
                }

                break;
        }
    }

    IEnumerator DelayScore(float delay = 1.0f){ 
        yield return new WaitForSeconds(delay);
         CalculateAttackModeFinalScore();
    }

    void CalculateAttackModeFinalScore(){
        float bonusTime = (difficultySettings.MaxTime - timeSpent) * 0.2f;
        int bonusAttemptsLeft = (maxMistakes - mistakes) * 2;  
        timer = 0f;
        UpdateScore((int)bonusTime + bonusAttemptsLeft);
        Debug.Log("Score Attack Mode Complete! Final Score: " + score);
        DisplayMessage("Score Attack Mode Ended!", 4f);    
    }

    //Increase difficulty after guessing x words.
    private void UpdateDifficulty(){

        int increaseDifficultyCount = difficultySettings.MaxMistakes;
        
        //on easy - 8 words to increase dif., on normal - 5, on hard - no increase
        //this will also modify how many lives you get back but it doesnt cap lives. 
        //if in the menu you choose easy, then even if the difficulty increases you max lives will still be 8
        if(changeDifficultyCount == increaseDifficultyCount && selectedDifficulty != "Hard"){
            if (selectedDifficulty == "Easy") {
                selectedDifficulty = "Normal";
            } else if (selectedDifficulty == "Normal") {
                selectedDifficulty = "Hard";
            }

            difficultySettings = new DifficultySettings(selectedDifficulty);
            Debug.Log($"{changeDifficultyCount} words guessed. Difficulty Increased");
            DisplayMessage("Difficulty Increased!", 3.0f);
            changeDifficultyCount = 0;  
        }
    }

    void LoadWordsFromFile(string fileName){

        try{   
            string filePath;

            if(Application.isEditor){
                filePath = Path.Combine(Application.dataPath, "WordLists/" + selectedDifficulty, fileName);
            }
            else {
                filePath = Path.Combine(Application.streamingAssetsPath, "WordLists/" + selectedDifficulty, fileName);
            }
           

                if (!File.Exists(filePath)) {
                    Debug.LogError("File not found: " + filePath);
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);
                string currentCategory = Path.GetFileNameWithoutExtension(fileName).Replace("_" + selectedDifficulty, "").Replace("_"," "); //REPLACE _ AND DIFFICULTY NAME FROM TEXT FILE

                if (!categorizedWords.ContainsKey(currentCategory))
                {
                    categorizedWords[currentCategory] = new List<string>();
                }
                
                foreach(string line in lines){
                    string word = line.Trim().ToUpper();
                    if(!string.IsNullOrEmpty(word)) {
                        categorizedWords[currentCategory].Add(word);
                    }
                }
                 
                Debug.Log("Loaded words from: " + fileName); 

        } catch(System.Exception ex){
            Debug.LogError("An error ocurred while loding words from file: " + ex.Message);
            
        }
    }

    string GetRandomFile(string[] fileNames){
        int index = Random.Range(0, fileNames.Length);
        return fileNames[index];
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void OpenLeaderBoard()
    {
        if (selectedGamemode == "Endless")
        {
            // Store the score in playerprefs to retrieve it in the next scene
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();

            // Load the scene
            SceneManager.LoadScene("DemoScene");
        }
        if (selectedGamemode == "ScoreAttack")
        {
            // Store the score in playerprefs to retrieve it in the next scene
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();

            // Load the scene
            SceneManager.LoadScene("scatboard");
        }

    }

    public int ReturnLives()
    {
        return maxMistakes - mistakes;
    }

    public bool UpdateStick()
    {
        return updateStick;
    }
    public bool Success()
    {
        return success;
    }
    public void StickUpdated()
    {
        updateStick = false;
        success = false;
    }
    

    public void DisplayMessage(string message, float delay = 3.0f){
        messageText.text = message;
        messagePanel.SetActive(true);
        StartCoroutine(HideMessageDelay(delay));

    }
    void HideMessage(){
        messagePanel.SetActive(false);
    }

    IEnumerator HideMessageDelay(float delay){
        yield return new WaitForSeconds(delay);
        HideMessage();
    }
}
