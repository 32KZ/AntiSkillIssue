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
	internal class AntiSkillIssueRightViewController : BSMLResourceViewController 
	{
        #region ResourceName 
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";

        //Our ResoureName is an override from the BSMLViewController Class. it sets this View Controller's Resource to
        // the file that matches the namespace's class + ".bsml" on the end.
        // this means the resource used to show the user the front end UI is the BSML file with the same name.
        // this is how the UI is structured within the project. the CS files Contain the code that Controlls elements Inside the BSML file.
        // the BSML file then is understood by the BSML library and Used to make a UI in the game that the end user is able to use. 
        //Additionally, so that C# knows that the BSML files are resources, the need to be marked as embedded resources within the Visual Studio Solution explorer. 

        #endregion ResourceName

        //Currently, the RightViewController does not do anything. in the future, further developments can be made to display to the user what the most
        // important things are to improve on here, based apon their current skillset. 

    }




}