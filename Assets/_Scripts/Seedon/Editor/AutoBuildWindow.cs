using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

public class AutoBuildWindow : EditorWindow
{
    private string buildFolderName = "Build_WasherRotate";
    private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    private string[] scenesToBuild;

    [MenuItem("Tools/Auto Build + Zip")]
    public static void ShowWindow()
    {
        GetWindow<AutoBuildWindow>("Auto Build");
    }

    private void OnEnable()
    {
        // Получаем все сцены из Build Settings
        scenesToBuild = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Build + Zip", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        buildFolderName = EditorGUILayout.TextField("Build Folder Name", buildFolderName);
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildTarget);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Scenes to build: {scenesToBuild?.Length ?? 0}");

        if (scenesToBuild != null)
        {
            foreach (var scene in scenesToBuild)
            {
                EditorGUILayout.LabelField("  • " + Path.GetFileName(scene));
            }
        }

        EditorGUILayout.Space();

        // Показываем следующую версию
        float nextVersion = GetNextVersion();
        EditorGUILayout.LabelField($"Next version: {nextVersion:F1}");

        EditorGUILayout.Space();

        if (GUILayout.Button("Build + Create Zip", GUILayout.Height(40)))
        {
            BuildAndZip();
        }
    }

    private void BuildAndZip()
    {
        if (scenesToBuild == null || scenesToBuild.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No scenes in Build Settings!", "OK");
            return;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string buildPath = Path.Combine(projectRoot, buildFolderName);

        // Создаём папку если нет
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }

        // Определяем имя exe
        string exeName = buildFolderName.Replace("Build_", "") + GetExtension();
        string exePath = Path.Combine(buildPath, exeName);

        Debug.Log($"Starting build: {exePath}");

        // Билдим
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenesToBuild,
            locationPathName = exePath,
            target = buildTarget,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != BuildResult.Succeeded)
        {
            EditorUtility.DisplayDialog("Build Failed", $"Build failed with {report.summary.totalErrors} errors", "OK");
            return;
        }

        Debug.Log("Build succeeded!");

        // Создаём zip
        float version = GetNextVersion();
        string versionStr = version.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        string zipName = $"{buildFolderName}{versionStr}.zip";
        string zipPath = Path.Combine(projectRoot, zipName);

        // Удаляем старый zip если есть
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        Debug.Log($"Creating zip: {zipPath}");

        try
        {
            // includeBaseDirectory = true, чтобы внутри была папка Build_WasherRotate
            ZipFile.CreateFromDirectory(buildPath, zipPath, System.IO.Compression.CompressionLevel.Optimal, true);
            Debug.Log($"✅ Zip created: {zipName}");
            EditorUtility.DisplayDialog("Success", $"Build complete!\nZip: {zipName}", "OK");
            
            // Открываем папку с архивом
            EditorUtility.RevealInFinder(zipPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create zip: {e.Message}");
            EditorUtility.DisplayDialog("Zip Failed", e.Message, "OK");
        }
    }

    private float GetNextVersion()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        
        // Ищем существующие архивы
        var existingZips = Directory.GetFiles(projectRoot, $"{buildFolderName}*.zip")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();

        if (existingZips.Count == 0)
        {
            return 0.1f;
        }

        // Парсим версии
        float maxVersion = 0f;
        var regex = new Regex($@"{Regex.Escape(buildFolderName)}(\d+\.?\d*)");

        foreach (var zip in existingZips)
        {
            var match = regex.Match(zip);
            if (match.Success && float.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float ver))
            {
                if (ver > maxVersion) maxVersion = ver;
            }
        }

        // Следующая версия
        return maxVersion + 0.1f;
    }

    private string GetExtension()
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            case BuildTarget.StandaloneOSX:
                return ".app";
            case BuildTarget.StandaloneLinux64:
                return "";
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.iOS:
                return "";
            default:
                return ".exe";
        }
    }
}

