using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour {
    public class Check
    {
        public Check(string valueName, int valueMin)
        {
            m_valueMin = valueMin;
            m_valueName = valueName;
        }

        public string m_valueName;
        public int m_valueMin;
    }

    public class DialogPair
    {
        public DialogPair(List<Check> requirements, string dialog)
        {
            m_requirements = requirements;
            m_dialog = dialog;
        }

        public List<Check> m_requirements;
        public string m_dialog;
    }

    public class DialogLine
    {
        public DialogLine(List<Check> modifiers, List<DialogPair> lines, List<DialogPair> responses)
        {
            m_modifiers = modifiers;
            m_lines = lines;
            m_responses = responses;
        }

        public List<Check> m_modifiers;
        public List<DialogPair> m_lines;
        public List<DialogPair> m_responses;
    }


    public class DialogScene
    {
        Dictionary<string, DialogLine> m_lines;

        public DialogScene(Dictionary<string,DialogLine> lines)
        {
            m_lines = lines;
        }

        public void ApplyModifiers(string line)
        {
            foreach(Check modifier in m_lines[line].m_modifiers)
            {
                PlayerPrefs.SetInt(modifier.m_valueName, modifier.m_valueMin);
            }
        }

        public List<string> GetDialog(string line)
        {
            ApplyModifiers(line);

            List<string> text = new List<string>();
            text.Add("");
            foreach (DialogPair l in m_lines[line].m_lines)
            {
                bool shouldContinue = false;
                foreach(Check c in l.m_requirements)
                {
                    if(PlayerPrefs.GetInt(c.m_valueName) <= c.m_valueMin)
                    {
                        shouldContinue = true;
                    }
                }

                if (shouldContinue)
                {
                    continue;
                }

                text[0] += l.m_dialog;
            }

            text.Add("");
            text.Add("");
            
            foreach (DialogPair l in m_lines[line].m_responses)
            {
                bool shouldContinue = false;
                foreach (Check c in l.m_requirements)
                {
                    if (PlayerPrefs.GetInt(c.m_valueName) < c.m_valueMin)
                    {
                        shouldContinue = true;
                    }
                }

                if (shouldContinue)
                {
                    continue;
                }
                text[2] = text[1];
                text[1] = l.m_dialog;
                
            }
            return text;
        }
    }

    public class DialogLevel
    {
        public DialogScene m_beforeCharacterCustomization;
        public DialogScene m_beforeMainLevel;
        public DialogScene m_beforeMainTournament;
        public DialogScene m_beforeFailure;
        public DialogScene m_beforeSuccess;

        public DialogLevel(DialogScene BeforeCharacterCustomization, DialogScene BeforeMainLevelCustomization, 
            DialogScene BeforeMainTournament, DialogScene BeforeFailure, DialogScene BeforeSuccess)
        {
            m_beforeCharacterCustomization = BeforeCharacterCustomization;
            m_beforeMainLevel = BeforeMainLevelCustomization;
            m_beforeMainTournament = BeforeMainTournament;
            m_beforeFailure = BeforeFailure;
            m_beforeSuccess = BeforeSuccess;
        }


        public string m_startingLine;
    }


    private List<DialogLevel> m_dialogExchanges;
    [SerializeField]
    private Text m_dialogText;
    [SerializeField]
    private Text m_choiceA;
    [SerializeField]
    private Text m_choiceB;

    [SerializeField]
    private TextAsset m_levelText;

    private static string m_playerChoice;

    public void LoadScenes()
    {
        m_dialogExchanges = new List<DialogLevel>();
        LoadLevelOne();

    }

    public static void SetChoice(string choice)
    {
        m_playerChoice = choice;
    }

    public void LoadLevelOne()
    {
        Dictionary<string,DialogLine > beforeCharacterCustomizationDictionary= new Dictionary<string, DialogLine>();

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "start";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check(key, 1));

            List<DialogPair> lines = new List<DialogPair>();

            List<Check> checks;
            {
                checks = new List<Check>();
                lines.Add(new DialogPair(checks, "Hello user. I see that you have downloaded Monstinder, the app summoned to be created by the Council of Elders as a means to combat our alarming underpopulation rates. I’m sure you must be curious as to why such a severe underpopulation crisis exists. What is your guess mortal?"));
            }
            List<DialogPair> responses = new List<DialogPair>();

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Just take me to the game.")); //written
            }

            {
                checks = new List<Check>();
                responses.Add(new DialogPair(checks, "Monsters Be Ugly..."));
            }

            {
                checks = new List<Check>();
                checks.Add(new Check("Idk, they're asexual?",0));
                responses.Add(new DialogPair(checks, "Idk, they're asexual?"));
            }
            

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Monsters Be Ugly...";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check(key, 1));
        
            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();

            lines.Add(new DialogPair(checks, "Monsters are too ugly to fall in love with one another, and because love is needed to call the Monsterodactyl to deliver the monster babies from the gods, no babies are being delivered"));


            List<Check> dialogChecks = new List<Check>();
            lines.Add(new DialogPair(dialogChecks, "Surely you must know this. "));

            List<DialogPair> responses = new List<DialogPair>();
            checks = new List<Check>();
            responses.Add(new DialogPair(checks, "no"));
            responses.Add(new DialogPair(checks, "Just take me to the game.."));

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Idk, they're asexual?";
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check(key, 1));
            modifiers.Add(new Check("Autism", 1));
            modifiers.Add(new Check("Intelligence", 1));

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            
            lines.Add(new DialogPair(checks, "Um, monsters are asexual or something I don’t know! What kinda game is this anyway? This better be a good use of my time, because you gotta be efficient. Time is money. People are money. Money is money."));


            List<Check> dialogChecks = new List<Check>();
            lines.Add(new DialogPair(dialogChecks, "Surely you must know this."));

            List<DialogPair> responses = new List<DialogPair>();
            checks = new List<Check>(); 
            responses.Add(new DialogPair(checks, "no"));
            responses.Add(new DialogPair(checks, "Just take me to the game.."));

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }


        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Fine, tell me the rules...";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            lines.Add(new DialogPair(checks, ""));


            List<Check> dialogChecks = new List<Check>();
            lines.Add(new DialogPair(dialogChecks, "Look there's going to be three screens okay. For the first click on the polaroids to make a monster. You pick the parts for making them one at a time. For the second one just click on monsters that look like you. It couldn't be simpler"));

            List<DialogPair> responses = new List<DialogPair>();
            responses.Add(new DialogPair(checks, "Monsters Be Ugly..."));
            responses.Add(new DialogPair(checks, "Just take me to the game.."));

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "Just take me to the game..";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            lines.Add(new DialogPair(checks, ""));


            List<Check> dialogChecks = new List<Check>();
            lines.Add(new DialogPair(dialogChecks, "Would you here the rules are?"));

            List<DialogPair> responses = new List<DialogPair>();
            responses.Add(new DialogPair(checks, "no"));
            responses.Add(new DialogPair(checks, "Fine, tell me the rules..."));

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            string key = "no";
            List<Check> modifiers = new List<Check>();

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            lines.Add(new DialogPair(checks, ""));


            List<Check> dialogChecks = new List<Check>();
            lines.Add(new DialogPair(dialogChecks, "Then stop wasting my time. Hit end to get started."));

            List<DialogPair> responses = new List<DialogPair>();
            responses.Add(new DialogPair(checks, "end"));
            responses.Add(new DialogPair(checks, "end"));

            beforeCharacterCustomizationDictionary.Add(key, new DialogLine(modifiers, lines, responses));
        }
        DialogScene beforeCharacterCustomization = new DialogScene(beforeCharacterCustomizationDictionary);

        m_dialogExchanges.Add(new DialogLevel(beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization));

    }
    
    private IEnumerator RunScene(DialogScene scene, string startingLine = "start")
    {
        m_playerChoice = "start";
        while (m_playerChoice != "end")
        {
            List<string> text = scene.GetDialog(m_playerChoice);
            m_dialogText.text = text[0];
            Fader.Instance.FadeOut(m_dialogText.gameObject, .1f);

            m_choiceA.text = text[1];
            Fader.Instance.FadeOut(m_choiceA.gameObject, .1f);
            m_choiceB.text = text[2];
            Fader.Instance.FadeOut(m_choiceB.gameObject, .1f);



            m_playerChoice = "";
            while (m_playerChoice.Length == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            Fader.Instance.FadeIn(m_dialogText.gameObject,.1f);
            Fader.Instance.FadeIn(m_choiceB.gameObject, .1f);
            Fader.Instance.FadeIn(m_choiceA.gameObject, .1f);
            yield return new WaitForSeconds(1.0f);
        }


    }

	// Use this for initialization
	void Start () {
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        LoadScenes();
        int currentLevel = PlayerPrefs.GetInt("Level", 0);

        if (currentLevel > m_dialogExchanges.Count)
        {
            currentLevel = m_dialogExchanges.Count-1;
        }

        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            case "DialogBeforeCharacterCustomization":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeCharacterCustomization);
                while (m_playerChoice != "end") { yield return new WaitForEndOfFrame(); }


                Fader.Instance.FadeIn().LoadLevel("CharacterCustomization").FadeOut();
                break;
            case "DialogBeforeMainLevel":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeMainLevel);
                while (m_playerChoice != "end") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("PrototypeScene").FadeOut();
                break;
            case "DialogBeforeMainTournament":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeMainTournament);
                while (m_playerChoice != "end") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("MatchRejects").FadeOut();
                break;
            case "DialogBeforeFailure":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeFailure);
                while (m_playerChoice != "end") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("Failure").FadeOut();
                break;
            case "DialogBeforeSuccess":
                yield return RunScene(m_dialogExchanges[currentLevel].m_beforeSuccess);
                while (m_playerChoice != "end") { yield return new WaitForEndOfFrame(); }
                Fader.Instance.FadeIn().LoadLevel("Success").FadeOut();
                break;
        }


    }
}
