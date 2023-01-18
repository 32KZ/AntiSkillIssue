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
        private SoloFreePlayFlowCoordinator _CurrentFlowCoordinator;



        [UIValue("song-name")]
        private string _SongName = "Songname";//yeah songName;
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



            //Set our Current flow coordinator to the first SoloFreePlatFlowCoordinator, the Solo Play Screen. 
           
            _CurrentFlowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().First();
            Log.Info("ResultsPageClicked() Ran!");
            //Define the Coordinators
            _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            Log.Info("ResultsPageClicked() Ran!");
            _ResultsFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ResultsFlowCoordinator>();
            //make sure the _ResultsFlowCoordinator Knows that is Parent Coordinator is this Current/last Coordinator
            _ResultsFlowCoordinator._ResultsParentFlowCoordinator = _mainFlowCoordinator;
            Log.Info("ResultsPageClicked() Ran!");
            //Since the Current FlowCoordinator is active, and HMUI has marked it as a HMUI Flow Coordinator, we SHOULD >
            // > be able to present this _ResultsFlowCoordinator Directly To the First SoloFreePlayFlowCoordinator, as that should be the solo menu.
            // > this allows us to effectively Swap Out What menu is displayed. 
            _mainFlowCoordinator.PresentFlowCoordinator(_ResultsFlowCoordinator);
            Log.Info("ResultsPageClicked() Ran!");
            // Presenting the FlowCoordinator to the main Flow Coordinator causes a softlock since SoloMenu stays active at the same time.
            // we have to make it more of a chain instead of a tree. this way, we can work back up it. | VVVV Normal Method
            //_ResultsFlowCoordinator._ResultsParentFlowCoordinator.PresentFlowCoordinator(_ResultsFlowCoordinator);
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