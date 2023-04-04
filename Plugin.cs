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
        #region Plugin Class Properties

        #region Flow Coordinators

        private AntiSkillIssueFlowCoordinator _AntiSkillIssueFlowCoordinator;
        private MainFlowCoordinator _mainFlowCoordinator;

        #endregion Flow Coordinators

        #region Setup Properties
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        #endregion Setup Properties

        private MenuButton AsiMenuButton; //MenuButton Property.

        #endregion

        #region Initialise The Mod

        [Init]
        public void Init(IPALogger logger, IPA.Config.Config config) 
        {
            
            Instance = this;
            Log = logger;
            Log.Info("AntiSkillIssue initialized.");

        }

        #endregion

        #region Start the Mod
        [OnStart]
        public void OnApplicationStart()
        {
            new GameObject("AntiSkillIssueController").AddComponent<AntiSkillIssueController>();
            Log.Info("Started AntiSkillIssue");
        }
        #endregion Start the Mod

        #region Whilst mod is Enabled. Akin to main.
        [OnEnable]
        public void OnEnable()
        {
            try // Create a Mod Button For Main Functionality
            {
                AsiMenuButton = new MenuButton("AntiSkillIssue", "Grow your PP with care!", OnModButtonPressed, true);
                MenuButtons.instance.RegisterButton(AsiMenuButton);
                Log.Info("Menu Button Created And Registered.");
            }
            catch
            {
                Log.Info("Failed to instance a MenuButton in plugin.cs. Check yout IPA installation.");
            } 
        }
        #endregion Whilst mod is Enabled. Akin to main.

        #region Disable the Mod
        [OnExit] // Same as [OnDisable]
        public void OnApplicationQuit()
        {
            MenuButtons.instance.UnregisterButton(AsiMenuButton); //Unregister Our MenuButton on Exit.
            //GameplaySetup.instance.RemoveTab("ASI"); If we had a Tab, we would remove it here also.

        }
        #endregion

        #region Create our UI on Button Press.
        public void OnModButtonPressed()
        {


            _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
            //Set the MainFlowCoordinator to a private Variable So we can Provide Children Flow Coordinators.

            _AntiSkillIssueFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<AntiSkillIssueFlowCoordinator>();
            _mainFlowCoordinator.PresentFlowCoordinator(_AntiSkillIssueFlowCoordinator);
            //Create A flow Coordinator, and Present it to the Active, MainFlowCoordinator.

            _AntiSkillIssueFlowCoordinator.FCDidFinishEvent += _AntiSkillIssueFlowCoordinator_FCDidFinishEvent;
            _AntiSkillIssueFlowCoordinator.VCDidFinishEvent += _AntiSkillIssueFlowCoordinator_VCDidFinishEvent;
            //Subscribe our FlowCoordinators DidFinish Events for the FlowCoordinators And the the ViewControllers's 

            Log.Info("OnModButtonPressed() Ran!");

        }

        #endregion Create Our UI on Button Press

        #region Cleanup after Back Button pressed.

        
        private void _AntiSkillIssueFlowCoordinator_FCDidFinishEvent() // Dismiss the Flow Coordinator that Controlls the UI

        {

            _AntiSkillIssueFlowCoordinator.FCDidFinishEvent -= _AntiSkillIssueFlowCoordinator_FCDidFinishEvent;             
            _AntiSkillIssueFlowCoordinator.VCDidFinishEvent -= _AntiSkillIssueFlowCoordinator_VCDidFinishEvent; // we do this here because there are multiple View Controllers.
            // DISCONNECT DELEGATION TO STOP MULTIPLE CALLS.

            _mainFlowCoordinator.DismissFlowCoordinator(_AntiSkillIssueFlowCoordinator);
            //Dissmiss our Flow Coordinator to allow for the menu to swap back.

            #region _AntiSkillIssueFlowCoordinator_DidFinishEvent() Summary
            // when the did finish event is called for the _AntiSkillIssueFlowCoordinator, 
            // Dismiss the _AntiSkillIssueFlowCoordinator from the _mainFlowCoordinator,
            // AND the view controllers. there are multiple view controllers, so we only need to call the method once
            // with all of them defined, instead of vice versa.
            #endregion

        }

        #endregion Cleanup after Back Button pressed.

        #region Mark UI as Cleared
        private void _AntiSkillIssueFlowCoordinator_VCDidFinishEvent()
        {
            
            Log.Info("Closed our ViewControllers.");
        
        }
        #endregion

    }
}