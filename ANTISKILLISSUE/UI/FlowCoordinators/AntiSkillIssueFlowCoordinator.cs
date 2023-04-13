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
using AntiSkillIssue.ANTISKILLISSUE;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators //Find Directory
{
	internal class AntiSkillIssueFlowCoordinator : HMUI.FlowCoordinator //our flow coordinator Inherits HarmonyUI FlowCoordinator. 
	{
        #region Events
        public event Action FCDidFinishEvent;
		public event Action VCDidFinishEvent;
        #endregion

        #region  Flow Coordinator Properties

        private MainFlowCoordinator _AntiSkillIssueMainFlowCoordinator;		//mark _AntiSkillIssueMainFlowCoordinator as a MainFlowCoordinator, so it can take over the _mainFlowCoordinator in Plugin.cs
		private AntiSkillIssueViewController _AntiSkillIssueViewController; //this is external to this CS file. used to allow us to reference the View Controllers.
		private AntiSkillIssueLeftViewController _AntiSkillIssueLeftViewController;
		private AntiSkillIssueLeftViewController _newAntiSkillIssueLeftViewController;
		private AntiSkillIssueRightViewController _AntiSkillIssueRightViewController;

        #endregion Flow Coordinator Properties 

        #region Create UI enviornment and View Controllers Within
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
            #region Setup Flow Controller's Enviornment

            SetTitle("ASI: BeatSavior Score Review"); //Set the title at the top of our UI
			showBackButton = true;                    // Enable the Back Button. once pressed, calls our Did Finish Event.

            // Both of these are inherited from the "HMUI.FlowCoordinator" class.

            #endregion Setup Flow Controller's Enviornment

            #region Create View Controllers

            _AntiSkillIssueViewController = BeatSaberUI.CreateViewController<AntiSkillIssueViewController>();
			_AntiSkillIssueLeftViewController = BeatSaberUI.CreateViewController<AntiSkillIssueLeftViewController>();
			_AntiSkillIssueRightViewController = BeatSaberUI.CreateViewController<AntiSkillIssueRightViewController>();

            #region Create View Controllers region Summary

            // we make Three View controllers using BSML to allow us to Add UI into our Flow Controller's Enviornment.
            // we create these as their own version of a View Controller, inheriting Properties from BSMLResourceViewControllers. 
            // Since AntiSkillIssueViewController is already a class in this project, it takes that.
            // same for all the other ones.
            // this readies them for use.

            #endregion Create View Controllers region Summary

            #endregion Create View Controllers 

            #region Enable View Controllers within the Flow Coordinator.
            ProvideInitialViewControllers(_AntiSkillIssueViewController, _AntiSkillIssueLeftViewController, _AntiSkillIssueRightViewController);
			_AntiSkillIssueViewController.SetSessions(); //Auto Populate the Sessions List. Quality of life feature.

            // ProvideInitialViewControllers():
            //  in sequence, choose the position of each View Controller.
            //  Left, Middle, Right.
            //  each peramater Relates to a File, Containing logical code, and Resource References for the UI front end. 
            // :


            #endregion Enable View Controllers within the Flow Coordinator

            #region Setup Data Transfer between two UI instances

            _AntiSkillIssueViewController.BrotherViewController = _AntiSkillIssueLeftViewController;
			_AntiSkillIssueViewController.DataTransfer += _AntiSkillIssueLeftViewController.OnDataTransferEvent;

            // the middle UI that contains the Sessions list has a property, that we set as the left UI's File. 
            // we then Subscribe the left view controller instance to the middle UI.
            // this means that whenever the Middle UI (AntiSkillIssueViewController) calls the OnDataTransferEvent(x,x,x)
            //  function, information is sent to the active instance of the AntiSkillIssueLeftViewController,
            //   So that the instance can use that inforamtion, and show it to the user.

            #endregion Setup Data Transfer between two UI instances

        }

        #endregion Create UI enviornment and View Controllers Within

        #region Close the FlowCoordinator.
        protected override void BackButtonWasPressed(ViewController topViewController)
		{
			_AntiSkillIssueViewController.DataTransfer -= _AntiSkillIssueLeftViewController.OnDataTransferEvent;
            //Remove Delegation of the dataTransferEvent

			FCDidFinishEvent.Invoke();
            //Call our DidFinishEvent to remove the UI and allow the user to continue Playing. 

        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) //Memory leak fix?
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        #endregion Close the FlowCoordinator.

    }




}
