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
        internal static IPALogger Log { get; private set; }
        private int _StartSecondsTime;
        private int _EndSecondsTime;
        private int _StartMinuitesTime;
        private int _EndMinuitesTime;
        
        //UI ACTIONS *

        [UIAction("Click")]
        private void ButtonClicked()
        {
            Console.WriteLine("Button was clicked!");

        }

        // TAB1 : START AND END SLIDERS
        // T1: UI ACTIONS
        [UIAction("start-slider")]
        private void SetStartTime(float newStartTime)
        {
            StartTime = newStartTime;
            // Set the new value of the slider on Change

        }

        [UIAction("end-slider")]
        private void SetEndTime(float newEndTime)
        {

            endTime = newEndTime;

            // Set the new value of the slider on Change

        }

        // T1: UI COMPONENTS

        [UIComponent("start-time-slider")]
        private SliderSetting startTimeSlider;

        [UIComponent("end-time-slider")]
        private SliderSetting endTimeSlider;

        // T1: UI VALUES

        [UIValue("song-length")]
        private float SongLength =59f; //will be value taken from songdata

        [UIValue("start-slider")]
        private float StartTime = 1f;

        [UIValue("end-slider")]
        private float endTime = 1f;

        //

        // TAB2 : PRESWING POST SWING (%)

        // T2: UI VALUES
        // T2: LEFT

        [UIValue("average-left-pre-swing")]
        private int AverageLeftPreSwing   = 100;

        [UIValue("average-left-post-swing")]
        private int AverageLeftPostSwing  = 100;

        // T2: RIGHT

        [UIValue("average-right-pre-swing")]
        private int AverageRightPreSwing  = 100;

        [UIValue("average-right-post-swing")]
        private int AverageRightPostSwing = 100;





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
