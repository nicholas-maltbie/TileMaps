// Copyright (C) 2022 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Export scripts and demos of this project for use in other projects.
/// </summary>
public class ExportPackage : MonoBehaviour
{
    /// <summary>
    /// Path of assets for the demo export.
    /// </summary>
    public static readonly string[] AssetPaths = {
        Path.Combine(ScriptBatch.AssetDirectory, Constants.ProjectName),
    };

    /// <summary>
    /// Path of just the scripts for the demo export.
    /// </summary>
    public static readonly string[] ScriptsAssetPaths = {
        Path.Combine(ScriptBatch.AssetDirectory, Constants.ProjectName, "Scripts"),
    };

    /// <summary>
    /// File path for exporting the demo package.
    /// </summary>
    public static string PackagePath => Path.Combine(
        ScriptBatch.BuildDirectory, Constants.ProjectName + $"-Examples-{ScriptBatch.VersionNumber}.unitypackage");

    /// <summary>
    /// File path for exporting just the scripts from the demo project.
    /// </summary>
    /// <returns></returns>
    public static string ScriptPackagePath => Path.Combine(
        ScriptBatch.BuildDirectory, Constants.ProjectName + $"-{ScriptBatch.VersionNumber}.unitypackage");

    /// <summary>
    /// Batched method to export all types of packages.
    /// </summary>
    [MenuItem("Build/Package/Export All Packages")]
    public static void ExportAllPackages()
    {
        ExportExampleAssetPackage();
        ExportScriptsAssetPackage();
    }

    /// <summary>
    /// Command to export the entire demo project.
    /// </summary>
    [MenuItem("Build/Package/Export Example Package")]
    public static void ExportExampleAssetPackage()
    {
        AssetDatabase.ExportPackage(
            AssetPaths,
            PackagePath,
            ExportPackageOptions.Recurse |
                ExportPackageOptions.Interactive |
                ExportPackageOptions.IncludeDependencies);
    }

    /// <summary>
    /// Command to export just the scripts for the demo project.
    /// </summary>
    [MenuItem("Build/Package/Export Scripts Package")]
    public static void ExportScriptsAssetPackage()
    {
        AssetDatabase.ExportPackage(
            ScriptsAssetPaths,
            ScriptPackagePath,
            ExportPackageOptions.Recurse |
                ExportPackageOptions.Interactive |
                ExportPackageOptions.IncludeDependencies);
    }
}
