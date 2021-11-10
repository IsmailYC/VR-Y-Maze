using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public struct ProtocolElementUI
{
    public ProtocolElementUI(int r, int s, GameObject g)
    {
        reps = r;
        prefab = s;
        panel = g;
        mazePanelBehaviour = g.GetComponent<MazePanelBehaviour>();
        rectTransform = g.GetComponent<RectTransform>();
    }

    public int reps { get; set; }
    public int prefab { get; set; }
    public GameObject panel { get; set; }
    public MazePanelBehaviour mazePanelBehaviour { get; set; }
    public RectTransform rectTransform;
}

public class UIManager : MonoBehaviour
{
    public enum UIState { None, Subject, Protocol, Menu };
    public static UIManager instance;

    public UIState uiState;

    public GameObject popupPanel;
    public Text popupMessage;

    public GameObject subjectPanel;
    public GameObject protocolPanel;
    public GameObject menuPanel;

    //Menu components
    public Button subjectButton;
    Text subjectButtonText;
    public Button protocolButton;
    Text protocolButtonText;
    public Button menuButton;
    Text menuButtonText;

    //Subject panel components
    public InputField subjectIdField;
    public InputField subjectBirthdateField;
    public Toggle subjectIsMaleToggle;
    public Toggle subjectIsFemaleToggle;
    public Dropdown subjectsDropdown;
    public Button subjectSaveButton;
    Text subjectSaveButtonText;

    //Protocol panel components
    public Dropdown mazesDropdown;
    public GameObject mazePanelPrefab;
    public GameObject mazeScrollView;
    RectTransform scrollViewRect;
    public Dropdown protocolsDropdown;
    public Button protocolSaveButton;
    Text protocolSaveButtonText;

    string subjectID;
    string subjectBirthday;
    string subjectGender;

    int selectedMaze;
    List<ProtocolElementUI> protocol;
    int mazeCount;
    float panelWidth;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        uiState = UIState.None;
        subjectButton.onClick.AddListener(Switch2SubjectPanel);
        subjectButtonText = subjectButton.gameObject.GetComponentInChildren<Text>();
        protocolButton.onClick.AddListener(Switch2ProtocolPanel);
        protocolButtonText = protocolButton.gameObject.GetComponentInChildren<Text>();
        menuButton.onClick.AddListener(Switch2MenuPanel);
        menuButtonText = menuButton.gameObject.GetComponentInChildren<Text>();
        subjectSaveButtonText = subjectSaveButton.GetComponentInChildren<Text>();
        protocolSaveButtonText = protocolSaveButton.GetComponentInChildren<Text>();

        scrollViewRect = mazeScrollView.GetComponent<RectTransform>();
        selectedMaze = 0;
        protocol = new List<ProtocolElementUI>();
        mazeCount = 0;
        panelWidth = mazePanelPrefab.GetComponent<RectTransform>().rect.width;
    }

    public void LoadDropdowns()
    {
        List<string> mazeDropOptions = new List<string>();
        for (int i = 0; i < ExpManager.instance.mazePrefabs.Length; i++)
        {
            mazeDropOptions.Add(ExpManager.instance.mazePrefabs[i].GetComponent<MazeInfo>().title);
        }
        mazesDropdown.AddOptions(mazeDropOptions);

        subjectsDropdown.AddOptions(ExpManager.instance.subjectsHistory);
        protocolsDropdown.AddOptions(ExpManager.instance.protocolsHistory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Switch2SubjectPanel()
    {
        switch(uiState)
        {
            case UIState.Protocol:
                protocolButton.interactable = true;
                protocolButtonText.color = Color.black*protocolButton.colors.normalColor;
                protocolPanel.SetActive(false);
                break;
            case UIState.Menu:
                menuButton.interactable = true;
                menuButtonText.color = Color.black * menuButton.colors.normalColor;
                menuPanel.SetActive(false);
                break;
        }
        uiState = UIState.Subject;
        subjectButton.interactable = false;
        subjectButtonText.color = Color.black * subjectButton.colors.disabledColor;
        subjectPanel.SetActive(true);
    }

    void Switch2ProtocolPanel()
    {
        switch (uiState)
        {
            case UIState.Subject:
                subjectButton.interactable = true;
                subjectButtonText.color = Color.black * subjectButton.colors.normalColor;
                subjectPanel.SetActive(false);
                break;
            case UIState.Menu:
                menuButton.interactable = true;
                menuButtonText.color = Color.black * menuButton.colors.normalColor;
                menuPanel.SetActive(false);
                break;
        }
        uiState = UIState.Protocol;
        protocolButton.interactable = false;
        protocolButtonText.color = Color.black * protocolButton.colors.disabledColor;
        protocolPanel.SetActive(true);
        SetProtocol(protocolsDropdown.options.Count - 1);
        protocolsDropdown.value = protocolsDropdown.options.Count - 1;
    }

    void Switch2MenuPanel()
    {
        switch (uiState)
        {
            case UIState.Protocol:
                protocolButton.interactable = true;
                protocolButtonText.color = Color.black * protocolButton.colors.normalColor;
                protocolPanel.SetActive(false);
                break;
            case UIState.Subject:
                subjectButton.interactable = true;
                subjectButtonText.color = Color.black * subjectButton.colors.normalColor;
                subjectPanel.SetActive(false);
                break;
        }
        uiState = UIState.Menu;
        menuButton.interactable = false;
        menuButtonText.color = Color.black * menuButton.colors.disabledColor;
        menuPanel.SetActive(true);
    }

    public void SetSubjectID(string value)
    {
        subjectID = value;
        EnableSubjectSaveBtn();
    }

    public void SetSubjectBirthdate(string value)
    {
        try
        {
            DateTime.Parse(value, new CultureInfo("fr-FR", true));
            subjectBirthday = value;
            EnableSubjectSaveBtn();
        }
        catch (FormatException)
        {
            subjectBirthdateField.text = subjectBirthday;
            ShowPopUp("The input should be a date formatted dd/mm/yyyy");
        }
    }

    public void SubjectIsMale(bool value)
    {
        if (value)
            subjectGender = "Male";
        else
            subjectGender = "Female";
        EnableSubjectSaveBtn();
    }

    public void SetSubject(int i)
    {
        if (i == 0)
        {
            LoadSubjectInfo("", "", true);
            EnableSubjectSaveBtn();
        }
        else
        {
            ExpManager.instance.SetSubject(i);
            DisableSubjectSaveBtn();
        }
    }

    public void LoadSubjectInfo(string sId, string sBd, bool isM)
    {
        subjectID = sId;
        subjectIdField.text = sId;

        subjectBirthday = sBd;
        subjectBirthdateField.text = sBd;

        if (isM)
        {
            subjectGender = "Male";
            subjectIsMaleToggle.isOn = true;
            subjectIsFemaleToggle.isOn = false;
        }
        else
        {
            subjectGender = "Female";
            subjectIsFemaleToggle.isOn = true;
            subjectIsMaleToggle.isOn = false;
        }
    }

    public void SaveSubjectInfo()
    {
        ExpManager.instance.SaveSubjectInfo(subjectID, subjectBirthday, subjectGender);
    }

    public void DisableSubjectSaveBtn()
    {
        subjectSaveButton.interactable = false;
        subjectSaveButtonText.color = Color.black * protocolButton.colors.disabledColor;
    }

    public void EnableSubjectSaveBtn()
    {
        subjectSaveButton.interactable = true;
        subjectSaveButtonText.color = Color.black * protocolButton.colors.normalColor;
    }

    public void SetMazeOption(int choice)
    {
        selectedMaze = choice;
    }

    public void AddMaze2Protocol()
    {
        GameObject tempPanelMaze = Instantiate(mazePanelPrefab);
        tempPanelMaze.transform.SetParent(mazeScrollView.transform, false);
        protocol.Add(new ProtocolElementUI(0, selectedMaze, tempPanelMaze));
        protocol[mazeCount].mazePanelBehaviour.SetPanelComponents(ExpManager.instance.mazePrefabs[selectedMaze].GetComponent<MazeInfo>(), mazeCount);
        scrollViewRect.sizeDelta = new Vector2(panelWidth * (mazeCount+1), 0);
        protocol[mazeCount].rectTransform.localPosition = panelWidth * mazeCount * Vector3.right;
        mazeCount++;
        EnableProtocolSaveBtn();
    }

    public void AddMaze2Protocol(int p, int r)
    {
        GameObject tempPanelMaze = Instantiate(mazePanelPrefab);
        tempPanelMaze.transform.SetParent(mazeScrollView.transform, false);
        protocol.Add(new ProtocolElementUI(0, p, tempPanelMaze));
        protocol[mazeCount].mazePanelBehaviour.SetPanelComponents(ExpManager.instance.mazePrefabs[p].GetComponent<MazeInfo>(), mazeCount);
        scrollViewRect.sizeDelta = new Vector2(panelWidth * (mazeCount + 1), 0);
        protocol[mazeCount].rectTransform.localPosition = panelWidth * mazeCount * Vector3.right;
        SetMazeRepWithUpdate(r, mazeCount);
        mazeCount++;
    }

    public void CreateMazePrefab()
    {

    }

    public void SetMazeRep(int r, int index)
    {
        ProtocolElementUI tempElement = protocol[index];
        tempElement.reps = r;
        protocol[index] = tempElement;
        EnableProtocolSaveBtn();
    }

    public void SetMazeRepWithUpdate(int r, int index)
    {
        SetMazeRep(r, index);
        protocol[index].mazePanelBehaviour.SetMazeRepsDisplay(r);
    }

    public void RemoveMaze2Protocol(int index)
    {
        Destroy(protocol[index].panel);
        protocol.RemoveAt(index);
        mazeCount--;
        for (int i = index; i < mazeCount; i++)
        {
            protocol[i].rectTransform.localPosition = panelWidth * i * Vector3.right;
            protocol[i].mazePanelBehaviour.SetIndex(i);
        }
        scrollViewRect.sizeDelta = new Vector2(panelWidth * mazeCount, 0);
        EnableProtocolSaveBtn();
    }

    public void SetProtocol(int i)
    {
        if (i == 0)
        {
            ClearProtocol();
            EnableProtocolSaveBtn();
        }
        else
        {
            ExpManager.instance.SetProtocol(i);
            DisableProtocolSaveBtn();
        }
    }

    public void ClearProtocol()
    {
        int originalMazeCount = mazeCount;
        for (int i = 0; i < originalMazeCount; i++)
        {
            RemoveMaze2Protocol(0);
        }
    }

    public void LoadProtocolInfo(List<ProtocolElement> p)
    {
        ClearProtocol();
        for (int i = 0; i < p.Count; i++)
        {
            AddMaze2Protocol(p[i].prefab, p[i].reps);
        }
    }

    public void SaveProtocolInfo()
    {
        ExpManager.instance.SaveProtocolInfo(protocol);
    }

    public void EnableProtocolSaveBtn()
    {
        protocolSaveButton.interactable = true;
        protocolSaveButtonText.color = Color.black * protocolButton.colors.normalColor;
    }

    public void DisableProtocolSaveBtn()
    {
        protocolSaveButton.interactable = false;
        protocolSaveButtonText.color = Color.black * protocolButton.colors.disabledColor;
    }

    public void ShowPopUp(string message)
    {
        popupMessage.text = message;
        popupPanel.SetActive(true);
    }

    public void HidePopUp()
    {
        popupPanel.SetActive(false);
    }
}
