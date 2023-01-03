using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using HMUI;
using SiraUtil;
using SiraUtil.Zenject;
using SiraUtil.Logging;
using SiraUtil.Web.SiraSync;
using Zenject;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
    internal class AntiSkillIssueLeftViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";

        [UIAction("Click")]
        private void ButtonClicked()
        {
            Console.WriteLine("Button was clicked!");

        }

        [UIComponent("start-time-slider")]
        private SliderSetting StartTimeSlider;

        [UIValue("start-time-value")]
        private string StartTimeValue;

        [UIComponent("end-time-slider")]
        private SliderSetting EndTimeSlider;

        [UIValue("end-time-value")]
        private string EndTimeValue;

    }
}
