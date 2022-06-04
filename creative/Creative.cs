using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SpaceCraft;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Reflection;
using MijuTools;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;





namespace creative
{
    [BepInPlugin("creativemode", "creative", "0.0.0.3")]
    [BepInProcess("Planet Crafter.exe")]
    public class Creative : BaseUnityPlugin
    {

        public static ActionCrafter testss = new ActionCrafter();

        public static ConfigEntry<Key> Keyfly;

        public static DataConfig.GameSettingMode creatice_mode = (DataConfig.GameSettingMode)4;
        public static DataConfig.CraftableIn creative_Crafter = (DataConfig.CraftableIn)7;

        private readonly Harmony harmony = new Harmony("AddCraftableObjectsPlugin");


        private void Awake() {

            testss.craftTime = 1;
            testss.craftableIdentifier = creative_Crafter;
            Creative.Keyfly = base.Config.Bind<Key>("MySection", "Choose a key for Flying from the list of keys above.", Key.F1, "TEST");

            harmony.PatchAll(typeof(creative.Creative));
            Logger.LogInfo($"Plugin AddCraftableObjectsPlugin is loaded!");

        }


        private void Update()
        {
            int gameMode = (int)GameSettingsHandler.GetGameMode();
            
            bool wasPressedThisFrame = Keyboard.current[Creative.Keyfly.Value].wasPressedThisFrame;
            PlayerMovable playerMovable = UnityEngine.Object.FindObjectOfType<PlayerMovable>();
            bool flag = playerMovable != null;
            if (wasPressedThisFrame)
            {
                base.Logger.LogInfo("key pressed");
                if (gameMode == 4 && flag)
                {
                    base.Logger.LogInfo(testss.GetCrafterIdentifier());
                    base.Logger.LogInfo("key pressed and creative");
                    playerMovable.flyMode = !playerMovable.flyMode;
                    base.Logger.LogInfo("flyMode changed");
                }
                else { return; }
            }

            bool wasPressedThisFrame1 = Keyboard.current[Key.C].wasPressedThisFrame;
            if (wasPressedThisFrame1)
            {
                if ((int)GameSettingsHandler.GetGameMode() == 4)
                {
                    Debug.Log((int)GameSettingsHandler.GetGameMode() == 4);
                    UiWindowCraft uiWindowCraft = (UiWindowCraft)Managers.GetManager<WindowsHandler>().OpenAndReturnUi(DataConfig.UiType.Craft);
                    uiWindowCraft.SetCrafter(testss, true);
                    Debug.Log("crafter key");
                }
                else return;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DyingConsequencesHandler), "HandleDyingConsequences")]
        private static void DeatChest()
        {
            int gameMode = (int)GameSettingsHandler.GetGameMode();
            if (gameMode == 4)
            {
                return;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveFilesCreateNew), "OnEnable")]
        private static void gameMode(ref DropDownAndHover ___dropDownGameMode,ref DropDownAndHover ___dropDownSpawn, ref SaveFilesSelector ___saveFilesSelector, ref TMP_InputField ___newNameInput, ref bool __runOriginal) {
            Creative.SetNameOfNewSave(ref ___saveFilesSelector, ref ___newNameInput, ref ___dropDownGameMode, ref __runOriginal);
            ___dropDownGameMode.ClearOptions();
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            foreach (DataConfig.GameSettingMode gameMode in (DataConfig.GameSettingMode[])Enum.GetValues(typeof(DataConfig.GameSettingMode)))
            {
                list.Add(Readable.GetModeLabel(gameMode));
                list2.Add(Readable.GetModeDescription(gameMode));
            }
            list.Add("Creative");
            list2.Add("it's creative");
            ___dropDownGameMode.AddOptions(list, list2);
            ___dropDownSpawn.ClearOptions();
            List<string> list3 = new List<string>();
            List<string> list4 = new List<string>();
            foreach (DataConfig.GameSettingStartLocation spawn in (DataConfig.GameSettingStartLocation[])Enum.GetValues(typeof(DataConfig.GameSettingStartLocation)))
            {
                list3.Add(Readable.GetSpawnLabel(spawn));
                list4.Add(Readable.GetSpawnDescription(spawn));
            }
            ___dropDownSpawn.AddOptions(list3, list4);
            
            __runOriginal = false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveFilesCreateNew), "SetNameOfNewSave")]
        private static void SetNameOfNewSave(ref SaveFilesSelector ___saveFilesSelector, ref TMP_InputField ___newNameInput , ref DropDownAndHover ___dropDownGameMode, ref bool __runOriginal)
        {

            string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
            int num = files.Length + 1;
            for (int i = 0; i < 200; i++)
            {
                string text = "Survival-" + num.ToString();
                bool flag = false;
                foreach (string file in files)
                {
                    if (___saveFilesSelector.GetNameOfSave(file) == text)
                    {
                        num++;
                        flag = true;
                        break;
                    }
                }
                ___newNameInput.text = text;
                if (!flag)
                {
                    break;
                }
            }
            __runOriginal = false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayModeHandler), "YesIfModes")]
        private static bool creativeFree(ref bool __result)
        {
            int gameMode = (int)GameSettingsHandler.GetGameMode();
            if (gameMode == 4)
            {
                __result = true;
                
            }
            return false;
        }
/*                ___allowedTaggedSurfaces.Add(DataConfig.HomemadeTag.SurfaceTerrain);
                ___allowedTaggedSurfaces.Add(DataConfig.HomemadeTag.SurfaceFloor);
                ___allowedTaggedSurfaces.Add(DataConfig.HomemadeTag.SurfaceWall);
                ___allowedTaggedSurfaces.Add(DataConfig.HomemadeTag.SurfaceTopContainer);
                ___allowedTaggedSurfaces.Add(DataConfig.HomemadeTag.ConstructibleCollisionDetector);
                ___allowedTaggedSurfaces.Add(DataConfig.HomemadeTag.IgnoreCollisionForConstraintNotColling);*/
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ConstraintOnSurfaces), "LateUpdate")]
        private static void build(ref List<DataConfig.HomemadeTag> ___allowedTaggedSurfaces, ref bool ___isConstraintRespected, ref PlayerAimController ___playerAimController)
        {
            if (___playerAimController != null)
            {
                int gameMode = (int)GameSettingsHandler.GetGameMode();
                if (gameMode == 4)
                {

                    ___isConstraintRespected = true;




                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMultitool), "SetState")]
        private static void allChipsUnlocked(ref PlayerMultitool __instance)
        {
            int gameMode = (int)GameSettingsHandler.GetGameMode();
            if (gameMode == 4)
            {
                
                __instance.AddEnabledState(DataConfig.MultiToolState.Deconstruct);
                __instance.AddEnabledState(DataConfig.MultiToolState.Build);

            }
        }

        

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseHudHandler), "UpdateHud")]
        private static void creativeTxt(ref GameObject ___subjectPositionDecoration,ref PlayerCanAct ___playerCanAct, ref TextMeshProUGUI ___textPositionDecoration,ref int ___frameTillLastSecond, ref float ___updateHudEvery,ref PlayModeHandler ___playModeHandler, ref PlayerMovable ___playerMovable, ref bool __runOriginal)
        {

            if (___subjectPositionDecoration == null)
            {
                return;
            }
            if (!___playerCanAct.GetCanMove())
            {
                return;
            }
            Vector3 position = ___subjectPositionDecoration.transform.position;
            ___textPositionDecoration.text = Mathf.Round(position.x).ToString();
            TextMeshProUGUI textMeshProUGUI = ___textPositionDecoration;
            textMeshProUGUI.text = textMeshProUGUI.text + ":" + Mathf.Round(position.y).ToString();
            TextMeshProUGUI textMeshProUGUI2 = ___textPositionDecoration;
            textMeshProUGUI2.text = textMeshProUGUI2.text + ":" + Mathf.Round(position.z).ToString();
            TextMeshProUGUI textMeshProUGUI3 = ___textPositionDecoration;
            textMeshProUGUI3.text = string.Concat(new string[]
            {
                textMeshProUGUI3.text,
                " : ",
                QualitySettings.GetQualityLevel().ToString(),
                " : ",
                ((float)___frameTillLastSecond / ___updateHudEvery).ToString()
            });
            ___frameTillLastSecond = 0;
            int gameMode = (int)GameSettingsHandler.GetGameMode();
            if (gameMode == 4)
                if (___playModeHandler.GetIsFreePlay() && gameMode != 4)
            {
                TextMeshProUGUI textMeshProUGUI4 = ___textPositionDecoration;
                textMeshProUGUI4.text += " - Free Mode";
            }
            if (gameMode == 4)
            {
                TextMeshProUGUI textMeshProUGUI4 = ___textPositionDecoration;
                textMeshProUGUI4.text += " - Creative";
            }
            if (___playerMovable.GetFlyMode())
            {
                TextMeshProUGUI textMeshProUGUI5 = ___textPositionDecoration;
                textMeshProUGUI5.text += " - Fly Mode";
            }
            __runOriginal = false;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveFilesSelector), "AddSaveToList")]
        
        private static void AddSaveToList(string _fileName, ref GameObject ___prefabSaveDisplayer, ref GameObject ___displayersContainer, ref SaveFilesSelector __instance , ref GameObject __result , ref bool __runOriginal)
        {
            __runOriginal = false;


            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(___prefabSaveDisplayer);
            SavedDataHandler manager = Managers.GetManager<SavedDataHandler>();
            manager.GetSavedDataInfos(_fileName);
            bool savedDataInfosCorrupted = manager.GetSavedDataInfosCorrupted();
            WorldUnit savedDataTerraformUnit = manager.GetSavedDataTerraformUnit();
            string modeLabel = Readable.GetModeLabel(manager.GetSavedDataInfosMode());
            if ((int)GameSettingsHandler.GetGameMode() == 4) modeLabel = "Creative";
            gameObject.GetComponent<SaveFileDisplayer>().SetData(_fileName, savedDataTerraformUnit, __instance, savedDataInfosCorrupted, modeLabel);
            gameObject.transform.SetParent(___displayersContainer.transform);
            gameObject.transform.SetSiblingIndex(0);
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            __result = gameObject;

        }




        [HarmonyPrefix]
        [HarmonyPatch(typeof(GroupItem), "CanBeCraftedIn")]
        private static bool CanBeCraftedIn(DataConfig.CraftableIn _craftableIdentifier, ref bool __result)
        {
            if ((int)_craftableIdentifier == 7)
            {
                __result = true;
            }
            return false;
        }




    }
}
