using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.GameplaySetup;
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
        private int _StartSecondsTime;
        private int _EndSecondsTime;
        private int _StartMinuitesTime;
        private int _EndMinuitesTime;
        

        [UIAction("Click")]
        private void ButtonClicked()
        {
            Console.WriteLine("Button was clicked!");

        }

        [UIComponent("start-time-slider")]
        private SliderSetting startTimeSlider;

        [UIValue("volume-slider")]
        private float volume = 1f;

        [UIAction("volume-slider")]
        private void SetVolume(float newVolume)
        {
            volume = newVolume;
            // Set the volume of the music here using the newVolume value
            
        }

        



        //private static string timecalc()
        //{
        //_StartSecondsTime = 32; //Default Value <3
        //    if (_StartSecondsTime >= 60)
        //    {
        //        _StartSecondsTime = _StartSecondsTime - 60;
        //        _StartMinuitesTime++;
        //
        //    }
        //    else
        //    {
        //        return _StartSecondsTime + "s";
        //
        //    }
        //return _StartMinuitesTime + "m " + _StartSecondsTime + "s";
        //}

    }
}
