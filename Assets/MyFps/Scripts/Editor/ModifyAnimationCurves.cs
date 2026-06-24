using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MyFps.Editor
{
    public static class ModifyAnimationCurves
    {
        [MenuItem("MyFps/Make Run In-Place")]
        public static void MakeRunInPlace()
        {
            string animPath = "Assets/_Sample/02CharacherAnimTest/Animation/Running.anim";
            if (!File.Exists(animPath))
            {
                Debug.LogError("Running.anim not found at: " + animPath);
                return;
            }

            string[] lines = File.ReadAllLines(animPath);
            bool inPositionCurve = false;
            
            float firstX = -0.010220384f; // Initial X value from first key
            float firstZ = -0.018450066f; // Initial Z value from first key
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains("m_PositionCurves:"))
                {
                    inPositionCurve = true;
                    continue;
                }
                if (inPositionCurve && line.Contains("m_ScaleCurves:"))
                {
                    inPositionCurve = false;
                    continue;
                }
                
                if (inPositionCurve)
                {
                    // Match value: {x: ..., y: ..., z: ...}
                    var matchValue = Regex.Match(line, @"value: \{x: ([-e\d\.]+), y: ([-e\d\.]+), z: ([-e\d\.]+)\}");
                    if (matchValue.Success)
                    {
                        string yVal = matchValue.Groups[2].Value;
                        lines[i] = $"        value: {{x: {firstX}, y: {yVal}, z: {firstZ}}}";
                        continue;
                    }
                    
                    // Match inSlope: {x: ..., y: ..., z: ...}
                    var matchIn = Regex.Match(line, @"inSlope: \{x: ([-e\d\.]+), y: ([-e\d\.]+), z: ([-e\d\.]+)\}");
                    if (matchIn.Success)
                    {
                        string yVal = matchIn.Groups[2].Value;
                        lines[i] = $"        inSlope: {{x: 0, y: {yVal}, z: 0}}";
                        continue;
                    }
                    
                    // Match outSlope: {x: ..., y: ..., z: ...}
                    var matchOut = Regex.Match(line, @"outSlope: \{x: ([-e\d\.]+), y: ([-e\d\.]+), z: ([-e\d\.]+)\}");
                    if (matchOut.Success)
                    {
                        string yVal = matchOut.Groups[2].Value;
                        lines[i] = $"        outSlope: {{x: 0, y: {yVal}, z: 0}}";
                        continue;
                    }
                }
            }

            File.WriteAllLines(animPath, lines);
            AssetDatabase.ImportAsset(animPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("Successfully modified Running.anim via text parsing to play in-place.");
        }
    }
}
