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
	internal class AntiSkillIssueFlowCoordinator : HMUI.FlowCoordinator //Create Class as a HarmonyUI FlowCoordinator. 
	{
		public event Action DidFinishEvent;
		private MainFlowCoordinator _AntiSkillIssueMainFlowCoordinator;		//mark _AntiSkillIssueMainFlowCoordinator as the MainFlowCoordinator 
		private AntiSkillIssueViewController _AntiSkillIssueViewController; //this is external to this CS file. we do not need to define it on the contrary.
		private AntiSkillIssueLeftViewController _AntiSkillIssueLeftViewController;
		private AntiSkillIssueRightViewController _AntiSkillIssueRightViewController;
		//private void Construct(MainFlowCoordinator mainFlowCoordinator, AntiSkillIssueViewController AntiSkillIssueViewController) //Create the FlowCoordinator and ViewController
		//{
		//	_AntiSkillIssueMainFlowCoordinator = mainFlowCoordinator;
		//	_AntiSkillIssueViewController = AntiSkillIssueViewController;
		//}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			SetTitle("ASI: BeatSavior Score Review");
			showBackButton = true;

			_AntiSkillIssueViewController = BeatSaberUI.CreateViewController<AntiSkillIssueViewController>();
			_AntiSkillIssueLeftViewController = BeatSaberUI.CreateViewController<AntiSkillIssueLeftViewController>();
			_AntiSkillIssueRightViewController = BeatSaberUI.CreateViewController<AntiSkillIssueRightViewController>();
			ProvideInitialViewControllers(_AntiSkillIssueViewController, _AntiSkillIssueLeftViewController, _AntiSkillIssueRightViewController);
			_AntiSkillIssueViewController.SetSessions(); //Auto Populate the Sessions List.
			_AntiSkillIssueViewController.SetPlays(SessionPath:null, Override:"!null, Override!"); //Enqueue a dummy to make it clear of the UI purpose.
		}

		protected override void BackButtonWasPressed(ViewController topViewController)
		{
			DidFinishEvent.Invoke();
		}


	}




}
