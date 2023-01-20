using SiraUtil;
using SiraUtil.Zenject;
using HarmonyLib;
using HMUI;
using System;
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
using BeatSaviorData;
using AntiSkillIssue.ANTISKILLISSUE;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;
using TMPro;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
    public class TabHostController : PersistentSingleton<TabHostController>
    {

        internal static IPALogger Log { get; private set; }
        internal static TabHostController instance;
        

        private MainFlowCoordinator _mainFlowCoordinator;
        private ResultsFlowCoordinator _ResultsFlowCoordinator;
        private SoloFreePlayFlowCoordinator _SoloFreePlayFlowCoordinator;
        private MainMenuViewController _mainMenuViewController;


        [UIValue("song-name")]
        private string _SongName = "Songname";// songName;
        [UIValue("cover-image")]
        private string _CoverImage = "Cover "; //private img coverImage = cover.Image
        [UIValue("song-length")]
        private string _SongLength = "Length ";// toString(song.Length)


        public void Init(IPALogger logger)
        {
            instance = this;
            Log = logger;

            Log.Info("TabHostController Initialised.");

            
        }

        public void AddTab()
        {


            ////----| Possible Logic
            //float _TempSongLength = _currentLevel.songDuration;
            //_SongLength = _TempSongLength.ToString();

            //_SongName = _currentLevel.songName;

            //_CoverImage = "Cover Image";
            ////----|

            GameplaySetup.instance.AddTab("ASI", "AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers.ASITabMenu.bsml", this, MenuType.Solo | MenuType.Campaign | MenuType.Online);
            Log.Info("ModTab Created. in TabHostController");
            


        }

        

        [UIAction("results-button")]
        private void ResultsPageClicked()
        {
            Log.Info("ResultsPageClicked() Ran!");


            _SoloFreePlayFlowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            Log.Info("Set CurrentFlow Coordinator to SolofreeplayCoordinator.First");
            
            _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            Log.Info("Set MainflowCoordinator");

            _ResultsFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ResultsFlowCoordinator>();
            _ResultsFlowCoordinator._ResultsParentFlowCoordinator = _mainFlowCoordinator;
            Log.Info("Created Flow Coordinator and set parent");

            _mainFlowCoordinator.PresentFlowCoordinator(_ResultsFlowCoordinator);
            Log.Info("Presented FlowCoordinator");

            _ResultsFlowCoordinator.DidFinishEvent += _ResultsFlowCoordinator_DidFinishEvent;

        }

        private void _ResultsFlowCoordinator_DidFinishEvent()
        {
            _ResultsFlowCoordinator.DidFinishEvent -= _ResultsFlowCoordinator_DidFinishEvent;
            Log.Info("ASI _ResultsFlowCoordinator_DidFinishEvent; Ran!");

            

            #region _AntiSkillIssueFlowCoordinator_DidFinishEvent() Summary
            //when the did finish event is called for the _AntiSkillIssueFlowCoordinator, 
            //Dissmiss the _AntiSkillIssueFlowCoordinator from the _mainFlowCoordinator.
            #endregion

        }




    }
}