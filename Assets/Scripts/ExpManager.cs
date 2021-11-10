using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public struct ProtocolElement
{
    public ProtocolElement(int i, int r)
    {
        prefab = i;
        reps = r;
    }

    public int prefab { get; set; }
    public int reps { get; set; }
}

public class ExpManager : MonoBehaviour
{
    public static ExpManager instance;
    public GameObject[] mazePrefabs;
    public string dataDirectory;
    public string protocolDirectory;
    public List<string> subjectsHistory;
    public List<string> protocolsHistory;
    
    bool protocolCheck;
    bool subjectCheck;
    string subjectID;
    string subjectBirthday;
    string subjectGender;
    List<ProtocolElement> protocol; 
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        DontDestroyOnLoad(this);
        subjectCheck = false;
        protocol = new List<ProtocolElement>();
        Invoke("FetchHistory",0);
    }

    public void FetchHistory()
    {
        string[] tempStrings = Directory.GetDirectories(dataDirectory);
        subjectsHistory = new List<string>();
        subjectsHistory.Add(" ");
        for (int i = 0; i < tempStrings.Length; i++)
        {
            string tempString = tempStrings[i].Split('\\')[1];
            subjectsHistory.Add(tempString);
        }

        tempStrings = Directory.GetFiles(protocolDirectory);
        protocolsHistory = new List<string>();
        protocolsHistory.Add(" ");
        for (int i = 0; i < tempStrings.Length; i++)
        {
            string tempString = tempStrings[i].Split('\\')[1];
            tempString = tempString.Substring(9, 10);
            protocolsHistory.Add(tempString);
        }
        UIManager.instance.LoadDropdowns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveSubjectInfo(string id, string birthday, string gender)
    {
        subjectID = id;
        subjectBirthday = birthday;
        subjectGender = gender;
        if(!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        if (!Directory.Exists(dataDirectory + '\\' + subjectID))
        {
            Directory.CreateDirectory(dataDirectory + '\\' + subjectID);
        }

        using (FileStream fs = File.Create(dataDirectory + '\\' + subjectID+"\\SubjectInfo.txt"))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("Subject ID: " + subjectID);
                writer.WriteLine("Birthday: " + subjectBirthday);
                writer.WriteLine("Gender: " + subjectGender);
            }
        }
        subjectCheck = true;
    }

    public void SetSubject(int i)
    {
        using (StreamReader fs = new StreamReader(dataDirectory + '\\' + subjectsHistory[i] + "\\SubjectInfo.txt"))
        {
            string tempSubjectId = fs.ReadLine().Split(':')[1];
            tempSubjectId = tempSubjectId.Substring(1);
            string tempSubjectBirthday = fs.ReadLine().Split(':')[1];
            tempSubjectBirthday = tempSubjectBirthday.Substring(1);
            string tempSubjectGender = fs.ReadLine().Split(':')[1];
            tempSubjectGender = tempSubjectGender.Substring(1);
            bool tempIsSubjectMale = tempSubjectGender == "Male";
            UIManager.instance.LoadSubjectInfo(tempSubjectId, tempSubjectBirthday, tempIsSubjectMale);
        }
    }

    public void SaveProtocolInfo(List<ProtocolElementUI> p)
    {
        StringBuilder csv = new StringBuilder();
        if (!Directory.Exists(protocolDirectory))
        {
            Directory.CreateDirectory(protocolDirectory);
        }

        for (int i=0; i<p.Count; i++)
        {
            csv.AppendLine(string.Format("{0},{1}",p[i].prefab, p[i].reps));
            ProtocolElement tempElement = new ProtocolElement(p[i].prefab, p[i].reps);
        }
        File.WriteAllText(protocolDirectory + "\\protocol_" + System.DateTime.Now.ToString("yyyy_MM_dd") + ".csv", csv.ToString());
        protocolCheck = true;
    }

    public void SetProtocol(int i)
    {
        using (StreamReader fs = new StreamReader(protocolDirectory + "\\protocol_" + protocolsHistory[i] + ".csv"))
        {
            List<ProtocolElement> tempProtocol = new List<ProtocolElement>();
            string line;
            while ((line=fs.ReadLine())!=null)
            {
                var values = line.Split(',');
                tempProtocol.Add(new ProtocolElement(int.Parse(values[0]), int.Parse(values[1])));
            }
            UIManager.instance.LoadProtocolInfo(tempProtocol);
        }
    }
}
