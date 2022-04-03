using System.Collections.Generic;
using Godot;

public class Global : Node
{
    private const string SAVE_DIRECTORY = "user://";
    private const string SAVE_FILE_PREFIX = "save-";
    private const string SAVE_FILE_SUFFIX = ".json";

    private const string SETTINGS_SAVE_FILE = "settings";
    private const string DEFAULT_SAVE_FILE = "default";


    private Dictionary<string, object> settings = new Dictionary<string, object>();
    // var game_data: Dictionary = {}

    private const int FIRST_LEVEL = 1;
    private static int currentLevel = FIRST_LEVEL;
    public static int CurrentLevel { get => currentLevel; }

    private static string currentLevelPath = GetLevelScenePath(currentLevel);
    public static string CurrentLevelPath { get => currentLevelPath; }

    private static int bestLevel = FIRST_LEVEL;
    public static int BestLevel { get => bestLevel; }

    public override void _Ready()
    {
        // if (save_file_exists(SETTINGS_SAVE_FILE))
        // {
        //     settings.initialize_from_dictionary(load_data(SETTINGS_SAVE_FILE));
        // }
    }

    public static void SetLevelToBest()
    {
        currentLevel = bestLevel;
        currentLevelPath = GetLevelScenePath(currentLevel);
    }

    public static void SetLevelToFirst()
    {
        currentLevel = FIRST_LEVEL;
        currentLevelPath = GetLevelScenePath(currentLevel);
    }

    public static void WinLevel(int level)
    {
        currentLevel = level;
        int lastLevel = currentLevel;
        string nextLevelPath = GetLevelScenePath(currentLevel + 1);
        if (new File().FileExists(nextLevelPath))
        {
            ++currentLevel;
            currentLevelPath = nextLevelPath;
            bestLevel = Mathf.Max(bestLevel, currentLevel);
            SaveData(bestLevel);
        }
        else
        {
            SetLevelToFirst();
        }
    }

    private static string GetLevelScenePath(int level)
    {
        return "res://scenes/Level" + level + ".tscn";
    }

    public static void SaveData(int bestLevel)
    {
        var data = new Dictionary<string, object>();
        data.Add("bestLevel", bestLevel);
        SaveData("bestLevel", data);
    }

    public static bool LoadBestLevel()
    {
        var data = LoadData("bestLevel");
        if (data == null)
        {
            GD.Print("data is null");
            return false;
        }
        object result = data["bestLevel"];
        bestLevel = result == null ? FIRST_LEVEL : (int)((System.Single)result);
        return true;
    }

    void SaveSettings()
    {
        SaveData(SETTINGS_SAVE_FILE, settings);
    }

    private static void SaveData(string save_name, Dictionary<string, object> data)
    {
        var path = GetSaveFilePath(save_name);
        // LOG.info("Saving data to: %s" % path);
        var file = new File();
        var openError = file.Open(path, File.ModeFlags.Write);
        if (openError != 0)
        {
            GD.Print("Open of ", path, " error: ", openError);
        }
        // LOG.check_error_code(file.open(path, File.WRITE), "Opening '%s'" % path);
        // LOG.info("Saving to: " + file.get_path_absolute());
        file.StoreString(JSON.Print(data));
        file.Close();
    }

    private string[] GetSaveFiles()
    {
        var dir = OpenSaveDirectory();
        dir.ListDirBegin(false, false);
        // LOG.check_error_code(dir.list_dir_begin(false, false), "Listing the files of " + SAVE_DIRECTORY);
        var file_name = dir.GetNext();

        List<string> result = new List<string>();
        while (file_name != "")
        {
            if (!dir.CurrentIsDir() && file_name.StartsWith(SAVE_FILE_PREFIX)
                    && file_name.EndsWith(SAVE_FILE_SUFFIX))
            {
                result.Add(file_name.Substr(SAVE_FILE_PREFIX.Length,
                    file_name.Length - SAVE_FILE_PREFIX.Length - SAVE_FILE_SUFFIX.Length));
            }
            file_name = dir.GetNext();
        }
        dir.ListDirEnd();

        result.Sort();
        return result.ToArray();
    }

    Node GetSingleNodeInGroup(string group)
    {
        Godot.Collections.Array nodes = GetTree().GetNodesInGroup(group);
        return nodes.Count > 0 ? (Node)nodes[0] : null;
    }

    private Godot.Directory OpenSaveDirectory()
    {
        var dir = new Godot.Directory();
        // LOG.check_error_code(dir.open(SAVE_DIRECTORY), "Opening " + SAVE_DIRECTORY);
        return dir;
    }

    private bool LoadGame(string save_name = DEFAULT_SAVE_FILE)
    {
        if (!SaveFileExists(save_name))
        {
            return false;
        }
        var loaded_data = LoadData(save_name);
        if (loaded_data.Count == 0)
        {
            return false;
        }
        var game_data = loaded_data;
        currentLevel = (int)(game_data["level"] ?? FIRST_LEVEL);
        return true;
    }

    private static bool SaveFileExists(string save_name)
    {
        var path = GetSaveFilePath(save_name);
        return new File().FileExists(path);
    }

    private static Godot.Collections.Dictionary LoadData(string save_name)
    {
        var path = GetSaveFilePath(save_name);
        var file = new File();
        if (!file.FileExists(path))
        {
            return null;
        }
        file.Open(path, File.ModeFlags.Read);
        // LOG.check_error_code(file.open(path, File.READ), "Opening file " + path);
        var raw_data = file.GetAsText();
        file.Close();
        var loaded_data = JSON.Parse(raw_data);
        if (loaded_data.Result != null && loaded_data.Result is Godot.Collections.Dictionary)
        {
            return (Godot.Collections.Dictionary)loaded_data.Result;
        }
        else
        {
            // LOG.error("Corrupted savegame data in file '%s'!" % path);
            return null;
        }
    }

    private static string GetSaveFilePath(string save_name)
    {
        return SAVE_DIRECTORY + "/" + SAVE_FILE_PREFIX + save_name + SAVE_FILE_SUFFIX;
    }
}