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
	internal class ResultsFlowCoordinator : HMUI.FlowCoordinator //Create Class as a HarmonyUI FlowCoordinator. 
	{
		public event Action DidFinishEvent; // create action called DidFinishEvent for going back in the menu
		private MainFlowCoordinator _ResultsMainFlowCoordinator;     //mark _ResultsMainFlowCoordinator as the MainFlowCoordinator 
		public FlowCoordinator _ResultsParentFlowCoordinator;
		private ResultsViewController _ResultsViewController; //this is external to this CS file. we do not need to define it on the contrary.
		private ResultsLeftViewController _ResultsLeftViewController;   //^^
		private ResultsRightViewController _ResultsRightViewController; //^^
		


		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			SetTitle("ASI: Results");
			showBackButton = true;

			_ResultsViewController = BeatSaberUI.CreateViewController<ResultsViewController>();
			_ResultsLeftViewController = BeatSaberUI.CreateViewController<ResultsLeftViewController>();
			_ResultsRightViewController = BeatSaberUI.CreateViewController<ResultsRightViewController>();
			ProvideInitialViewControllers(_ResultsViewController, _ResultsLeftViewController, _ResultsRightViewController);



		}

		protected override void BackButtonWasPressed(ViewController topViewController)
		{
			_ResultsParentFlowCoordinator?.DismissFlowCoordinator(this);
			DidFinishEvent.Invoke();
		}
	}




}