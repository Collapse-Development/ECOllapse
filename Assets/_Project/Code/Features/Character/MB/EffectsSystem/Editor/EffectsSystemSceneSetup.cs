using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using _Project.Code.Features.Character.MB.MovementSystem;

namespace _Project.Code.Features.Character.MB.EffectsSystem.Editor
{
    /// <summary>
    /// Editor utility to automatically set up the EffectsSystem test scene.
    /// </summary>
    public static class EffectsSystemSceneSetup
    {
        [MenuItem("Tools/Effects System/Setup Test Scene")]
        public static void SetupTestScene()
        {
            // Load the Character prefab
            GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Character.prefab");
            
            if (characterPrefab == null)
            {
                Debug.LogError("Character prefab not found at Assets/_Project/Prefabs/Character.prefab");
                return;
            }
            
            // Check if Character already exists in scene
            Character existingCharacter = Object.FindFirstObjectByType<Character>();
            GameObject characterInstance;
            
            if (existingCharacter != null)
            {
                Debug.Log("Character already exists in scene. Using existing instance.");
                characterInstance = existingCharacter.gameObject;
            }
            else
            {
                // Instantiate the Character prefab
                characterInstance = (GameObject)PrefabUtility.InstantiatePrefab(characterPrefab);
                characterInstance.transform.position = Vector3.zero;
                Debug.Log("Character prefab instantiated in scene.");
            }
            
            // Verify Model child exists and is visible
            Transform modelTransform = characterInstance.transform.Find("Model");
            if (modelTransform == null)
            {
                Debug.LogWarning("Model child not found in Character. Creating a simple cube model.");
                GameObject modelObj = new GameObject("Model");
                modelObj.transform.SetParent(characterInstance.transform);
                modelObj.transform.localPosition = Vector3.zero;
                
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Cube";
                cube.transform.SetParent(modelObj.transform);
                cube.transform.localPosition = new Vector3(0, 0.5f, 0);
                
                // Remove the collider from the visual cube (Character already has Rigidbody)
                Object.DestroyImmediate(cube.GetComponent<Collider>());
            }
            else
            {
                // Ensure Model and its children are active
                modelTransform.gameObject.SetActive(true);
                foreach (Transform child in modelTransform)
                {
                    child.gameObject.SetActive(true);
                }
                Debug.Log("Model child verified and activated.");
            }
            
            // Add EffectsSystem component as child if not exists
            EffectsSystem effectsSystem = characterInstance.GetComponentInChildren<EffectsSystem>();
            
            if (effectsSystem == null)
            {
                GameObject effectsSystemObj = new GameObject("EffectsSystem");
                effectsSystemObj.transform.SetParent(characterInstance.transform);
                effectsSystemObj.transform.localPosition = Vector3.zero;
                effectsSystem = effectsSystemObj.AddComponent<EffectsSystem>();
                Debug.Log("EffectsSystem component added as child of Character.");
            }
            else
            {
                Debug.Log("EffectsSystem already exists.");
            }
            
            // Ensure Rigidbody exists and is configured properly
            Rigidbody rb = characterInstance.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = characterInstance.AddComponent<Rigidbody>();
                Debug.Log("Rigidbody added to Character.");
            }
            
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.freezeRotation = true;
            Debug.Log("Rigidbody configured for kinematic movement.");
            
            // Verify CharacterMovementSystem exists, add if missing
            CharacterMovementSystem movementSystem = characterInstance.GetComponent<CharacterMovementSystem>();
            if (movementSystem == null)
            {
                Debug.LogWarning("CharacterMovementSystem not found on Character. Adding it now...");
                movementSystem = characterInstance.AddComponent<CharacterMovementSystem>();
                
                // Set initial speed using SerializedObject
                SerializedObject serializedMovement = new SerializedObject(movementSystem);
                SerializedProperty speedProperty = serializedMovement.FindProperty("_speed");
                if (speedProperty != null)
                {
                    speedProperty.floatValue = 5f;
                    serializedMovement.ApplyModifiedProperties();
                }
                
                Debug.Log("CharacterMovementSystem added and configured with speed 5.0");
            }
            else
            {
                Debug.Log($"CharacterMovementSystem found. Current speed: {movementSystem.Speed}");
            }
            
            // Add EffectsSystemTest script to Character if not exists
            EffectsSystemTest testScript = characterInstance.GetComponent<EffectsSystemTest>();
            
            if (testScript == null)
            {
                testScript = characterInstance.AddComponent<EffectsSystemTest>();
                Debug.Log("EffectsSystemTest script added to Character.");
            }
            else
            {
                Debug.Log("EffectsSystemTest script already exists.");
            }
            
            // Set up Canvas and UI
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            
            if (canvas == null)
            {
                // Create Canvas
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                Debug.Log("Canvas created.");
            }
            
            // Create UI Text for speed display
            Text speedText = canvas.GetComponentInChildren<Text>();
            
            if (speedText == null)
            {
                GameObject textObj = new GameObject("SpeedMultiplierText");
                textObj.transform.SetParent(canvas.transform);
                
                RectTransform rectTransform = textObj.AddComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.anchoredPosition = new Vector2(10, -10);
                rectTransform.sizeDelta = new Vector2(400, 100);
                
                speedText = textObj.AddComponent<Text>();
                speedText.text = "Speed Multiplier: 1.00x\nCurrent Speed: 0.00";
                speedText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                speedText.fontSize = 20;
                speedText.color = Color.white;
                
                Debug.Log("UI Text created for speed display.");
            }
            
            // Assign UI Text to test script using SerializedObject
            SerializedObject serializedTest = new SerializedObject(testScript);
            SerializedProperty textProperty = serializedTest.FindProperty("speedMultiplierText");
            
            if (textProperty != null)
            {
                textProperty.objectReferenceValue = speedText;
                serializedTest.ApplyModifiedProperties();
                Debug.Log("UI Text assigned to EffectsSystemTest script.");
            }
            
            // Set base speed
            SerializedProperty baseSpeedProperty = serializedTest.FindProperty("baseSpeed");
            if (baseSpeedProperty != null && baseSpeedProperty.floatValue == 0f)
            {
                baseSpeedProperty.floatValue = 5f;
                serializedTest.ApplyModifiedProperties();
                Debug.Log("Base speed set to 5.0");
            }
            

            else
            {
                Debug.LogError("Rigidbody not found on Character! Movement requires a Rigidbody component.");
            }
            
            // Create or assign yellow material to the cube
            modelTransform = characterInstance.transform.Find("Model");
            if (modelTransform != null)
            {
                Transform cubeTransform = modelTransform.Find("Cube");
                if (cubeTransform != null)
                {
                    MeshRenderer renderer = cubeTransform.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        // Create yellow material
                        Material yellowMaterial = new Material(Shader.Find("Standard"));
                        yellowMaterial.color = Color.yellow;
                        yellowMaterial.name = "YellowCharacterMaterial";
                        
                        // Save material as asset
                        string materialPath = "Assets/_Project/Materials";
                        if (!AssetDatabase.IsValidFolder(materialPath))
                        {
                            AssetDatabase.CreateFolder("Assets/_Project", "Materials");
                        }
                        
                        string fullPath = materialPath + "/YellowCharacterMaterial.mat";
                        Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(fullPath);
                        
                        if (existingMaterial == null)
                        {
                            AssetDatabase.CreateAsset(yellowMaterial, fullPath);
                            Debug.Log("Yellow material created at " + fullPath);
                        }
                        else
                        {
                            yellowMaterial = existingMaterial;
                            Debug.Log("Using existing yellow material.");
                        }
                        
                        renderer.sharedMaterial = yellowMaterial;
                        Debug.Log("Yellow material applied to character cube.");
                    }
                }
            }
            
            // Add a ground plane if not exists
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                ground.transform.position = new Vector3(0, -0.5f, 0);
                ground.transform.localScale = new Vector3(10, 1, 10);
                Debug.Log("Ground plane created.");
            }
            
            // Add directional light if not exists
            Light light = Object.FindFirstObjectByType<Light>();
            if (light == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
                Debug.Log("Directional Light created.");
            }
            
            // Add camera with follow script for third-person view
            Camera camera = Object.FindFirstObjectByType<Camera>();
            SimpleCameraFollow cameraFollow = null;
            
            if (camera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                camera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
                cameraObj.AddComponent<AudioListener>();
                cameraFollow = cameraObj.AddComponent<SimpleCameraFollow>();
                Debug.Log("Camera created with follow script.");
            }
            else
            {
                cameraFollow = camera.GetComponent<SimpleCameraFollow>();
                if (cameraFollow == null)
                {
                    cameraFollow = camera.gameObject.AddComponent<SimpleCameraFollow>();
                    Debug.Log("SimpleCameraFollow added to existing camera.");
                }
            }
            
            // Configure camera follow to track the character
            if (cameraFollow != null)
            {
                SerializedObject serializedCamera = new SerializedObject(cameraFollow);
                SerializedProperty targetProperty = serializedCamera.FindProperty("target");
                SerializedProperty offsetProperty = serializedCamera.FindProperty("offset");
                SerializedProperty smoothSpeedProperty = serializedCamera.FindProperty("smoothSpeed");
                SerializedProperty lookAtHeightProperty = serializedCamera.FindProperty("lookAtHeight");
                
                if (targetProperty != null)
                {
                    targetProperty.objectReferenceValue = characterInstance.transform;
                }
                
                if (offsetProperty != null)
                {
                    offsetProperty.vector3Value = new Vector3(0, 5, -8);
                }
                
                if (smoothSpeedProperty != null)
                {
                    smoothSpeedProperty.floatValue = 5f;
                }
                
                if (lookAtHeightProperty != null)
                {
                    lookAtHeightProperty.floatValue = 1f;
                }
                
                serializedCamera.ApplyModifiedProperties();
                
                // Position camera initially
                camera.transform.position = characterInstance.transform.position + new Vector3(0, 5, -8);
                camera.transform.LookAt(characterInstance.transform.position + Vector3.up);
                
                Debug.Log("Camera configured to follow character with third-person view.");
            }
            
            Debug.Log("=== EffectsSystem Test Scene Setup Complete! ===");
            Debug.Log("Press Play and use WASD to move, 1-4 to add effects, C to cancel last effect.");
            
            // Mark scene as dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }
    }
}
