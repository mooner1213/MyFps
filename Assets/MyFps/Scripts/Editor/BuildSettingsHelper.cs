using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace MyFps
{
    /// <summary>
    /// 컴파일 시점에 자동으로 필요한 씬들을 빌드 세팅(Build Settings)에 등록하는 에디터 헬퍼 클래스
    /// </summary>
    [InitializeOnLoad]
    public static class BuildSettingsHelper
    {
        static BuildSettingsHelper()
        {
            AddScenesToBuildSettings();
        }

        private static void AddScenesToBuildSettings()
        {
            string playScenePath = "Assets/MyFps/Scenes/PlayScene.unity";
            string gameOverScenePath = "Assets/MyFps/Scenes/GameOverScene.unity";

            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            bool hasPlay = false;
            bool hasGameOver = false;

            foreach (var scene in scenes)
            {
                if (scene.path == playScenePath) hasPlay = true;
                if (scene.path == gameOverScenePath) hasGameOver = true;
            }

            bool changed = false;

            if (!hasPlay && File.Exists(playScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(playScenePath, true));
                changed = true;
            }

            if (!hasGameOver && File.Exists(gameOverScenePath))
            {
                scenes.Add(new EditorBuildSettingsScene(gameOverScenePath, true));
                changed = true;
            }

            if (changed)
            {
                EditorBuildSettings.scenes = scenes.ToArray();
                UnityEngine.Debug.Log("[BuildSettingsHelper] Automatically added PlayScene and GameOverScene to Build Settings.");
            }
        }
    }
}
