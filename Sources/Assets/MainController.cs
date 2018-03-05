using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;

using System.Diagnostics;

public class MainController : MonoBehaviour {


	public InputField WordsIF;
	public Button GenerateButton;
	public string[] Words;

	
	public GameObject WordInputPanel;
	public GameObject OriginalCard;

	public Text ResultInfo;
	public Button OpenFolderBtn;
	public Text CardsCount;
	public List<GameObject> CardsGO;
	public Button BackToInputBtn;
	public int lastCaretPos;
	public Toggle RegistToggle;
	public Text QuestionsText;
	public InputField mailIF;   //Used InputField for copypaste ability


	int SizeMultiplier=4;  //Multiplier Actual screen size when saving to image file

	// Use this for initialization
	void Start () {

		Screen.SetResolution(1240, 877, false);  //Screen size adapted to A4 paper format

		GenerateButton.onClick.AddListener(GenerateCards);


		OpenFolderBtn.onClick.AddListener(OpenFolderWithCards);
		BackToInputBtn.onClick.AddListener(BackToInputs);



		QuestionsText.gameObject.SetActive(false);
		mailIF.gameObject.SetActive(false);

		ResultInfo.gameObject.SetActive(false);
		OpenFolderBtn.gameObject.SetActive(false);
		BackToInputBtn.gameObject.SetActive(false);
		GenerateButton.interactable = false;

	}

 

	 

 

	 
	/// <summary>
	/// Back button from the second screen to the first
	/// </summary>
	public void BackToInputs() 
	{
		WordInputPanel.SetActive(true);
		ResultInfo.gameObject.SetActive(false);

		BackToInputBtn.gameObject.SetActive(false);
		OpenFolderBtn.gameObject.SetActive(false);
		QuestionsText.gameObject.SetActive(false);
		mailIF.gameObject.SetActive(false);

		foreach (GameObject g in CardsGO)
		{
			GameObject.Destroy(g, 0.001f);
		}
		CardsGO.Clear();
		CardsGO.Capacity = 0;
	}


	/// <summary>
	/// Calculate cards count to be generated
	/// </summary>
	public void CalculateCardsCount()
	{
		if (WordsIF.text.Length>0)
		GenerateButton.interactable = true;
		else
			GenerateButton.interactable = false;
		CardsCount.text = "Cards count to be generated: " + GetWords(WordsIF.text).Count();
	
	}


	 


	public void GenerateCards()
	{

		CardsGO.Clear();
		CardsGO.Capacity = 0;
		Words = GetWords(WordsIF.text);
		if (RegistToggle.isOn)
		{
			for (int i=0;i<Words.Length;i++)
			{
				Words[i] = Words[i].ToUpper();
			}
		}
		WordInputPanel.SetActive(false);
		OriginalCard.SetActive(false);



		

		for (int i = 0; i < Words.Length; i++)
		{
			GameObject someCard = GameObject.Instantiate(OriginalCard);
			someCard.SetActive(true);
			someCard.transform.parent = OriginalCard.transform.parent;
			someCard.GetComponentsInChildren<MainWordMarker>(true)[0].GetComponent<Text>().text = Words[i];
			someCard.GetComponentsInChildren<SecondareWordMarker>(true)[0].GetComponent<Text>().text = Words[i];
			CardsGO.Add(someCard);

		}

		StartCoroutine(TakeScreenShots());

		StartCoroutine(AfterAllScreenShotsTaken());
		
	}

	
	IEnumerator AfterAllScreenShotsTaken()
	{
		yield return new WaitForSeconds(2);
		QuestionsText.gameObject.SetActive(true);
		mailIF.gameObject.SetActive(true);
		ResultInfo.gameObject.SetActive(true);
		OpenFolderBtn.gameObject.SetActive(true);
		BackToInputBtn.gameObject.SetActive(true);
		ResultInfo.text = "Cards generated into this folder: " + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
	}




	IEnumerator TakeScreenShots()
	{

		int package16 = (Words.Length % 16 > 0) ? (1 + (int)(Words.Length / 16)) : (int)(Words.Length / 16);
		for (int i = 0; i < package16; i++)
		{

			foreach (GameObject g in CardsGO)
			{
				g.SetActive(false);
			}

			for (int card = 0; card < 16; card++)
			{
				if (card + i * 16 < Words.Count())

				{
					CardsGO[card + i * 16].SetActive(true);

				}
			}


			UnityEngine.Debug.Log(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')));

			if (!PlayerPrefs.HasKey("PrintCounter"))
			{
				PlayerPrefs.SetInt("PrintCounter", 1);
			}
			else
			{
				PlayerPrefs.SetInt("PrintCounter", PlayerPrefs.GetInt("PrintCounter") + 1);
			}
			string ScreenShotName = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) +"/Cards_CodeNames_"+ PlayerPrefs.GetInt("PrintCounter")+ ".png";
			ScreenCapture.CaptureScreenshot(ScreenShotName, SizeMultiplier);
			yield return new WaitForSeconds(0.5f);
		}
		
	}












	public InputField AppPathTtex;
	public void OpenFolderWithCards()
	{
		AppPathTtex.text = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'))+"/";
		 
		string itemPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) +"/";
		itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
		System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
	}
	


	static string[] GetWords(string input)
	{
		MatchCollection matches = Regex.Matches(input, @"\b[\w']*\b");

		var words = from m in matches.Cast<Match>()
					where (!string.IsNullOrEmpty(m.Value)&&(m.Value.Length>1))
					select TrimSuffix(m.Value);

		return words.ToArray();
	}

	static string TrimSuffix(string word)
	{
		int apostropheLocation = word.IndexOf('\'');
		if (apostropheLocation != -1)
		{
			word = word.Substring(0, apostropheLocation);
		}
		

		return word;
	}
}
