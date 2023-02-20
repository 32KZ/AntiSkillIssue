using SiraUtil;
using SiraUtil.Zenject;
using HarmonyLib;
using HMUI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using BS_Utils;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using AntiSkillIssue.ANTISKILLISSUE;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;


namespace AntiSkillIssue 
{
    [Plugin(RuntimeOptions.SingleStartInit)] 
    public class Plugin 
    {
        private AntiSkillIssueFlowCoordinator _AntiSkillIssueFlowCoordinator;
        private MainFlowCoordinator _mainFlowCoordinator;
        private ResultsFlowCoordinator _ResultsFlowCoordinator;
        
        internal static Plugin Instance { get; private set; } 
        internal static IPALogger Log { get; private set; }


        


        [Init] //initialisation tag

        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        

        public void Init(IPALogger logger, IPA.Config.Config config) 
        {
            
            Instance = this;
            Log = logger;

            //float _TempSongLength = _currentLevel.songDuration;
            //string _SongLength = "6:30";//_TempSongLength.ToString();

            //string _SongName = "test1"; //_currentLevel.songName;

            //string _CoverImage = "Cover Image";


            Log.Info("AntiSkillIssue initialized.");

           
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            
            Log.Info("Started AntiSkillIssue");
            new GameObject("AntiSkillIssueController").AddComponent<AntiSkillIssueController>();

        }

        [OnEnable]
        public void OnEnable()
        {

            
            try
            {
                MenuButtons.instance.RegisterButton(new MenuButton("AntiSkillIssue", "Grow your PP with care!", OnModButtonPressed, true));
                Log.Info("Menu Button Created.");
            }
            catch
            {
                Log.Critical("Failed to instance a MenuButton in plugin.cs, Something has gone Terribly Wrong...");
            }
            
            try
            {
                TabHostController tabHostController = new TabHostController();
                tabHostController.Init(Log);
                tabHostController.AddTab();
                
                //GameplaySetup.instance.AddTab("ASI", "AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers.ASITabMenu.bsml", this, MenuType.Solo | MenuType.Campaign | MenuType.Online);
                //Log.Info("ASI ModTab Created. in plugin.cs");

            }
            catch
            {
                Log.Warn("unable to Create Mod Tab in Plugin.cs, Defaulting");
                GameplaySetup.instance.AddTab("ASI", "AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers.ASITabMenuFAIL.bsml", this, MenuType.Solo | MenuType.Campaign | MenuType.Online);
            }



            #region OnEnable() Summary
            //Create a try and catch statement where a menubutton is attempeted to be made.
            //if its made, it will appear in game. 
            //if it fails, a critical log will be made.
            //do the same for a solo mod menu tab
            //when the menu button is pressed, OnModButtonPressed() is ran.

            #endregion

            

        }

        [OnExit]
        public void OnApplicationQuit()
        {

            GameplaySetup.instance.RemoveTab("ASI");
            Log.Debug("oyasuminasai!");

            #region OnapplicationQuit() Summary
            //Remove the ASI mod menu tab
            // log to the verbose debugger that the on exit tag has run.
            #endregion

        }


        public void OnModButtonPressed()
        {

            Log.Info("OnModButtonPressed() Ran!");
            _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            _AntiSkillIssueFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<AntiSkillIssueFlowCoordinator>();
            _mainFlowCoordinator.PresentFlowCoordinator(_AntiSkillIssueFlowCoordinator);

            _AntiSkillIssueFlowCoordinator.DidFinishEvent += _AntiSkillIssueFlowCoordinator_DidFinishEvent;

            #region OnModButtonPressed Summary

            // when the menu button we created on mod enable is pressed:

            // set the _mainFlowCoordinator to the first flow coordinator.

            // create a new flow coordinator with the name _AntiSkillIssueFlowCoordinator

            // present the _AntiSkillIssueFlowCoordinator to the _mainFlowCoordinator for use.

            // reference a did finish event so the back button can function, by dissmissing the
            // _AntiSkillIssueFlowCoordinator from the _mainFlowCoordinator.
            #endregion
        }

        private void _AntiSkillIssueFlowCoordinator_DidFinishEvent()
        {
            _AntiSkillIssueFlowCoordinator.DidFinishEvent -= _AntiSkillIssueFlowCoordinator_DidFinishEvent;
            // DISCONNECT DELEGATION TO STOP MULTIPLE CALLS
            _mainFlowCoordinator.DismissFlowCoordinator(_AntiSkillIssueFlowCoordinator);
            #region _AntiSkillIssueFlowCoordinator_DidFinishEvent() Summary
            //when the did finish event is called for the _AntiSkillIssueFlowCoordinator, 
            //Dissmiss the _AntiSkillIssueFlowCoordinator from the _mainFlowCoordinator.
            #endregion

        }

    }
}