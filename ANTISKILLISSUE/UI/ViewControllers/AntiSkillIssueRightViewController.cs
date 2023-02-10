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
using BeatSaberMarkupLanguage.Tags;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
	internal class AntiSkillIssueRightViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
	{
		public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";
        internal static IPALogger Log { get; private set; }


		[UIComponent("get-uuid-button")]
		ButtonTag button1;

        [UIValue("user-uuid")]
		private string UserUUID = null;

		[UIAction("get-user-uuid")]
		public void getUserUUID() //null reference. step through me.
		{
			UserUUID = "0";

            string _myFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Log.Info("_myFilePath:"+_myFilePath);
            try 
			{ 
			StreamReader read = new StreamReader(_myFilePath+"Roaming\\Beat Savior Data\\TextText.txt");
			string TestValue = read.ReadLine();
			UserUUID = TestValue;
			NotifyPropertyChanged(nameof(UserUUID));
			Log.Info(TestValue);
			read.Close();
			
			}
			catch 
			{
			UserUUID = "default val";
			Log.Info(UserUUID);
			}
			return;
			
	    }

	}

	


}