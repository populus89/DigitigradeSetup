using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;

public class DigitigradeSetup : EditorWindow
{
    [MenuItem("Tools/Digitigrade Setup")]
    static void Init()
    {
        DigitigradeSetup window = (DigitigradeSetup)EditorWindow.GetWindow(typeof(DigitigradeSetup));
        window.Show();
    }

    private GameObject targetObject;

    private List<Transform> digiBones;
    private List<Transform> plantiBones;
    private List<List<string>> boneNames;

    private Transform pelvisBone;


    void OnGUI()
    {

        var areastyle = new GUIStyle(GUI.skin.GetStyle("Box"));
        areastyle.padding = new RectOffset(5, 5, 5, 5);
        areastyle.margin = new RectOffset(5, 5, 5, 5);

        var buttonstyle = new GUIStyle(GUI.skin.GetStyle("Button"));

        //buttonstyle.margin = new RectOffset(25, 25, 25, 25);
        buttonstyle.padding = new RectOffset(15, 15, 15, 15);

        buttonstyle.fixedWidth = 150;


        var textstyle = new GUIStyle(GUI.skin.GetStyle("Label"));

        textstyle.padding = new RectOffset(5, 5, 5, 5);
        textstyle.wordWrap = true;
        textstyle.fontSize = 14;


        var fieldstyle = new GUIStyle(GUI.skin.GetStyle("ObjectField"));
        fieldstyle.margin = new RectOffset(25, 25, 25, 25);
        fieldstyle.padding = new RectOffset(15, 15, 15, 15);

        var creditstyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        creditstyle.padding = new RectOffset(5, 5, 5, 5);
        creditstyle.wordWrap = true;
        creditstyle.fontSize = 11;

        GUILayout.BeginHorizontal( areastyle);

        targetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true) as GameObject;

        GUILayout.EndHorizontal(); ;

        GUILayout.BeginHorizontal( areastyle);

        if (GUILayout.Button("Auto Rig", buttonstyle))
        {
            AutoRig();

        }

        EditorGUILayout.LabelField("Auto rigs the avatar.\n(Make sure it has both digitigrade and plantigrade bones)", textstyle);


        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(areastyle);

        if (GUILayout.Button("Helper Rig", buttonstyle))
        {
            HelperRig();
        }

        EditorGUILayout.LabelField("Rigs the avatar with helper bones, no planti required.\nUse this if your avatar has a regular rig.", textstyle);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(areastyle);

        EditorGUILayout.LabelField("If you like this script, consider giving me a follow!:\n\n@ThePopulus89 on twitter.\n@populus89.bsky.social on Bluesky.", creditstyle);
        GUILayout.EndHorizontal();
    }

    void AutoRig()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target object is not assigned.");
            return;
        }

        // List of bone names and their alternative naming. 
        // The code will iterate on the alternatives so you can add your own, if you use a different naming convention.
        // Make sure the words are generic and free of prefixes or symbols.

        boneNames = new List<List<string>>();

        boneNames.Add(new List<string> { "leg", "thigh", "hip" });
        boneNames.Add(new List<string> { "shin", "calf", "knee" });
        boneNames.Add(new List<string> { "foot", "ankle" });
        boneNames.Add(new List<string> { "toe", "toes", "paw" });

        // Excluded words for digitigrade bones and required names for plantigrade bones.

        List<string> excludedWords = new List<string> { "planti", "ctrl", "fix", "prop", "cloth", "plantigrade", "plant", "dummy", "_end",".end" };
        List<string> keywordList = new List<string> { "planti", "plantigrade", "plant" };

        pelvisBone = FindBone(new List<string> { "pelvis", "hips" }, "", excludedWords);

        int i = 0;

        //The code looks for both left and right versions of the bones.

        foreach (List<string> names in boneNames)
        {
            List<string> a = new List<string> { "left", "right" };

            foreach (string s in a)
            {
                var digiBone = FindBone(names, s, excludedWords);
                var plantiBone = FindBone(names, s, additionalKeywords: keywordList);

                if (digiBone != null) Debug.Log("Found " + s + " digitigrade bone: " + digiBone.name);
                if (plantiBone != null) Debug.Log("Found " + s + " plantigrade bone: " + plantiBone.name);

                if (digiBone != null && plantiBone != null)
                {
                    digiBones.Add(digiBone);
                    plantiBones.Add(plantiBone);

                    if (i < boneNames.Count - 2)
                    {
                        AddRotationConstraints(digiBone, plantiBone);
                    }

                    if (i == boneNames.Count - 2)
                    {
                        AddRotationConstraints(digiBone, FindBone(boneNames[0], s, excludedWords));

                    }

                    if (i == boneNames.Count - 1)
                    {
                        AddParentConstraint(digiBone, plantiBone);

                    }

                }
            }


            i++;
        }


        boneNames = new List<List<string>>();

    }

    void HelperRig()
    {

        digiBones = new List<Transform>();

        var helperBones = new List<Transform>();

        if (targetObject == null)
        {
            Debug.LogWarning("Target object is not assigned.");
            return;
        }

        // List of bone names and their alternative naming. 
        // The code will iterate on the alternatives so you can add your own, if you use a different naming convention.
        // Make sure the words are generic and free of prefixes or symbols.

        boneNames = new List<List<string>>();

        boneNames.Add(new List<string> { "leg", "thigh", "hip" });
        boneNames.Add(new List<string> { "shin", "calf", "knee" });
        boneNames.Add(new List<string> { "foot", "ankle" });
        boneNames.Add(new List<string> { "toe", "toes", "paw" });

        // Excluded words for digitigrade bones and required names for plantigrade bones.

        List<string> excludedWords = new List<string> { "planti", "ctrl", "fix", "prop", "cloth", "plantigrade", "plant", "dummy" ,"_end", ".end"};

        pelvisBone = FindBone(new List<string> { "pelvis", "hips" }, "", excludedWords);

        var plantlistL = new List<Transform>();
        var plantlistR = new List<Transform>();

        var diglistL = new List<Transform>();
        var diglistR = new List<Transform>();

        foreach (List<string> names in boneNames)
        {
            List<string> a = new List<string> { "left", "right" };


            foreach (string s in a)
            {

                var digiBone = FindBone(names, s, excludedWords);

                if (digiBone != null) Debug.Log("Found " + s + " digitigrade bone: " + digiBone.name);

                if (digiBone != null)
                {
                    digiBones.Add(digiBone);
                    var helper = CreateObjectAt(digiBone.transform, "dummy_" + digiBone.name);

                    //Debug

                    //var tmpdummy = helper.AddComponent<BoxCollider>();
                    //tmpdummy.size = new Vector3(0.04f, 0.04f, 0.04f);

                    if (s == "left")
                    {
                        plantlistL.Add(helper.transform);
                        diglistL.Add(digiBone);

                    }
                    else
                    {
                        plantlistR.Add(helper.transform);
                        diglistR.Add(digiBone);

                    }


                }

            }

        }

        var digiListLR = new List<List<Transform>>();
        var plantListLR = new List<List<Transform>>();

        digiListLR.Add(diglistL);
        digiListLR.Add(diglistR);

        plantListLR.Add(plantlistL);
        plantListLR.Add(plantlistR);

        int j = 0;
        foreach (List<Transform> l in plantListLR)
        {
            int k = 0;

            l[0].parent = pelvisBone.transform;

            l[1].parent = l[2];
            l[2].position = l[3].position;
            l[1].parent = null;
            l[1].parent = l[0];
            l[2].parent = l[1];

            l[3].position = digiListLR[j][3].position;
            l[3].position = Vector3.Lerp(new Vector3(l[1].position.x, l[3].position.y, l[1].position.z), l[3].position, 0.1f);
            l[3].parent = l[2];


            foreach (Transform t in l)
            {

                    if (k < l.Count - 2)
                    {
                        AddRotationConstraints(digiListLR[j][k], t);
                    }

                    if (k == l.Count - 2)
                    {
                        AddRotationConstraints(digiListLR[j][k], l[0]);

                    }

                    if (k == l.Count - 1)
                    {
                        AddParentConstraint(digiListLR[j][k], t);

                    }

                k++;
            }


                j++;
        }

       

    }
        Transform FindBone(List<string> keywords, string side, List<string> excludedWords = null, List<string> additionalKeywords = null)
    {
        string[] prefixes;
        bool isPrefixed = false;

        if (side == "left")
        {
            prefixes = new string[] { "left", "l.", ".l", "_l" };
            isPrefixed = true;
        }
        else if (side == "right")
        {
            prefixes = new string[] { "right", "r.", ".r", "_r" };
            isPrefixed = true;
        }
        else
        {
            prefixes = new string[] { "" };
            isPrefixed = false;
        }

        foreach (Transform bone in targetObject.GetComponentsInChildren<Transform>())
        {
            string boneName = bone.name.ToLower();

            foreach (string prefix in prefixes)
            {
                bool isExcluded = false;

                if (excludedWords != null)
                {
                    foreach (string excludedWord in excludedWords)
                    {
                        if (boneName.Contains(excludedWord))
                        {
                            isExcluded = true;
                            Debug.Log("excluded " + boneName);
                            break;

                        }
                    }
                }

                if (!isExcluded)
                {
                    bool containsKeywords = false;

                    if (keywords != null)
                    {
                        foreach (string keyword in keywords)
                        {
                            if (boneName.Contains(keyword))
                            {
                                containsKeywords = true;

                            }
                        }
                    }
                    if (isPrefixed == true)
                    {
                        if (containsKeywords && boneName.Contains(prefix))
                        {
                            if (additionalKeywords == null || additionalKeywords.Count == 0)
                            {
                                Debug.Log("Not looking for additional keywords");
                                return bone;
                            }
                            else
                            {
                                foreach (string additionalKeyword in additionalKeywords)
                                {
                                    if (boneName.Contains(additionalKeyword))
                                    {
                                        return bone;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        if (containsKeywords)
                        {
                            if (additionalKeywords == null || additionalKeywords.Count == 0)
                            {
                                Debug.Log("Found " + bone.name);
                                return bone;
                            }
                            else
                            {
                                foreach (string additionalKeyword in additionalKeywords)
                                {
                                    if (boneName.Contains(additionalKeyword))
                                    {
                                        return bone;
                                    }
                                }
                            }
                        }


                    }
                }
            }
        }

        return null;
    }

    void AddRotationConstraints(Transform bone, Transform targetBone)
    {
        RotationConstraint rotationConstraint = bone.gameObject.AddComponent<RotationConstraint>();

        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = targetBone,
            weight = 1f
        };

        rotationConstraint.AddSource(source);

        var rotationDelta = Quaternion.Inverse(targetBone.rotation) * rotationConstraint.transform.rotation;
        rotationConstraint.rotationOffset = rotationDelta.eulerAngles;

        rotationConstraint.constraintActive = true;
        rotationConstraint.locked = true;
    }

    void AddParentConstraint(Transform bone, Transform targetBone)
    {
        ParentConstraint parentConstraint = bone.gameObject.AddComponent<ParentConstraint>();


        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = targetBone,
            weight = 1f
        };


        parentConstraint.AddSource(source);

        var positionDelta = bone.position - targetBone.position;
        var rotationDelta = Quaternion.Inverse(targetBone.rotation) * bone.rotation;
        parentConstraint.SetTranslationOffset(0, positionDelta);
        parentConstraint.SetRotationOffset(0, rotationDelta.eulerAngles);

        parentConstraint.SetTranslationOffset(0, Quaternion.Inverse(bone.rotation) * positionDelta);
        parentConstraint.constraintActive = true;
        parentConstraint.locked = true;


    }

    static GameObject CreateObjectAt(Transform obj, string name)
    {

        var a = new GameObject(name);
        a.transform.position = obj.transform.position;
        a.transform.rotation = obj.transform.rotation;

        return a;
    }


}

