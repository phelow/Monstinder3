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
                    if(PlayerPrefs.GetInt(c.m_valueName) < c.m_valueMin)
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
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Fortitude", 1));

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            checks.Add(new Check("Fortitude", 1));
            lines.Add(new DialogPair(checks, "Hello and welcome to Monstinder. "));


            lines.Add(new DialogPair(checks, "The elder council of monsters thanks you for doing your part to save our race."));

            List<DialogPair> responses = new List<DialogPair>();
            responses.Add(new DialogPair(checks, "Ok."));
            responses.Add(new DialogPair(checks, "What?"));

            beforeCharacterCustomizationDictionary.Add("start", new DialogLine(modifiers, lines, responses));
        }

        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Incredulousness", 1));

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            checks.Add(new Check("Fortitude", 1));
            lines.Add(new DialogPair(checks, "Ah yes, Monstinder. The solution to the monster population crisis. Soon we will be growing and prospering again."));


            List<Check> dialogChecks = new List<Check>();
            checks.Add(new Check("Incredulousness", 1));
            lines.Add(new DialogPair(dialogChecks, "Surely you must know this."));

            List<DialogPair> responses = new List<DialogPair>();
            responses.Add(new DialogPair(checks, "Ok."));
            responses.Add(new DialogPair(checks, "What?"));

            beforeCharacterCustomizationDictionary.Add("What?", new DialogLine(modifiers, lines, responses));
        }


        { //TODO: this scoping is the work of the devil, refactor this out.
            //Guess I'm doing this the old fashioned way
            List<Check> modifiers = new List<Check>();
            modifiers.Add(new Check("Incredulousness", 1));

            List<DialogPair> lines = new List<DialogPair>();
            List<Check> checks = new List<Check>();
            checks.Add(new Check("Fortitude", 1));
            lines.Add(new DialogPair(checks, "First we need to make a monster. On the next scene click the polaroids to make your monster. Click end to get started"));


            List<Check> dialogChecks = new List<Check>();
            checks.Add(new Check("Incredulousness", 1));
            lines.Add(new DialogPair(dialogChecks, "Its simple trust me."));

            List<DialogPair> responses = new List<DialogPair>();
            responses.Add(new DialogPair(checks, "end"));
            responses.Add(new DialogPair(checks, "What?"));

            beforeCharacterCustomizationDictionary.Add("Ok.", new DialogLine(modifiers, lines, responses));
        }

        DialogScene beforeCharacterCustomization = new DialogScene(beforeCharacterCustomizationDictionary);

        m_dialogExchanges.Add(new DialogLevel(beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization, beforeCharacterCustomization));

    }
    
    private IEnumerator RunScene(DialogScene scene, string startingLine = "start")
    {
        m_playerChoice = "start";
        while (m_playerChoice != "end") {
            List<string> text = scene.GetDialog(m_playerChoice);
            m_dialogText.text = text[0];
            Fader.Instance.FadeOut(m_dialogText.gameObject);

            m_choiceA.text = text[1];
            Fader.Instance.FadeOut(m_choiceA.gameObject);
            m_choiceB.text = text[2];
            Fader.Instance.FadeOut(m_choiceB.gameObject);



            m_playerChoice = "";
            while (m_playerChoice.Length == 0)
            {
                yield return new WaitForEndOfFrame();
            }
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
