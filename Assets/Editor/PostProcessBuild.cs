using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class PostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            string pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

            // Load Xcode project
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxPath);

#if UNITY_2019_3_OR_NEWER
            string targetGUID = pbxProject.GetUnityMainTargetGuid();
            string frameworkTargetGUID = pbxProject.GetUnityFrameworkTargetGuid();
#else
            string targetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

            // Add necessary capabilities
            pbxProject.AddCapability(targetGUID, PBXCapabilityType.PushNotifications);

            // Add frameworks
            pbxProject.AddFrameworkToProject(targetGUID, "UserNotifications.framework", false);
            pbxProject.AddFrameworkToProject(targetGUID, "Security.framework", false);
            pbxProject.AddFrameworkToProject(targetGUID, "SystemConfiguration.framework", false);

            // Save changes to project.pbxproj
            pbxProject.WriteToFile(pbxPath);

            // Add Background Modes and other settings to Info.plist
            AddBackgroundModesToPlist(plistPath);
            AddUrlTypesToPlist(plistPath);
            AddAppTransportSecuritySettings(plistPath);
        }
    }

    private static void AddBackgroundModesToPlist(string plistPath)
    {
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;

        // Add UIBackgroundModes array
        PlistElementArray bgModes = rootDict.CreateArray("UIBackgroundModes");
        bgModes.AddString("remote-notification");

        plist.WriteToFile(plistPath);
    }

    private static void AddUrlTypesToPlist(string plistPath)
    {
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Add Firebase's REVERSED_CLIENT_ID
        PlistElementDict rootDict = plist.root;
        PlistElementArray urlTypes = rootDict.CreateArray("CFBundleURLTypes");
        PlistElementDict urlDict = urlTypes.AddDict();
        urlDict.SetString("CFBundleURLName", "Firebase");
        PlistElementArray urlSchemes = urlDict.CreateArray("CFBundleURLSchemes");

        // Add your reversed client ID here
        urlSchemes.AddString("com.googleusercontent.apps.YOUR_REVERSED_CLIENT_ID");

        plist.WriteToFile(plistPath);
    }

    private static void AddAppTransportSecuritySettings(string plistPath)
    {
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;
        PlistElementDict atsDict = rootDict.CreateDict("NSAppTransportSecurity");
        atsDict.SetBoolean("NSAllowsArbitraryLoads", true);

        plist.WriteToFile(plistPath);
    }
}
