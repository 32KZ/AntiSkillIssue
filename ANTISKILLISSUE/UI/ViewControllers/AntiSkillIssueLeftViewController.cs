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
        private float SongLength = 59f; //will be value taken from songdata

        [UIValue("start-slider")]
        private float StartTime = 1f;

        [UIValue("end-slider")]
        private float endTime = 1f;

        [UIValue("notes-selected")]
        private int NotesSlected = 0;

        [UIValue("duration-selected")]
        private int DurationSelected = 0;

        [UIValue("bpm-changes")]
        private int BPMChanges = 0;

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


        // TAB3: accuracy
        // T3: UIVALUES

        // T3: LEFT
        [UIValue("average-left-accuracy")]
        private int AverageLeftAccuracy = 15;


        // T3: RIGHT
        [UIValue("average-right-accuracy")]
        private int AverageRightAccuracy = 15;

        // TAB 4 : TIME DEPENDENCY
        // T4: UIVALUES
        // T4: LEFT
        [UIValue("average-left-timing-dependence")]
        private string AverageLeftTimingDependence = "0.00" + "TD";

        [UIValue("average-left-timing-deviation")]
        private string AverageLeftTimingDeviation = "120" + "ms";

        // T4: RIGHT
        [UIValue("average-right-timing-dependence")]
        private string AverageRightTimingDependence = "0.00" + "TD";

        [UIValue("average-right-timing-deviation")]
        private string AverageRightTimingDeviation = "120" + "ms";

        // TAB 5 : VELOCITY
        // T5: UIVALUES
        // T5: LEFT
        [UIValue("average-left-velocity")]
        private float AverageLeftVelocity = 80f;

        [UIValue("recommended-left-velocity")]
        private float RecommendedLeftVelocity = 65f;
        // T5: RIGHT
        [UIValue("average-right-velocity")]
        private float AverageRightVelocity = 80f;

        [UIValue("recommended-right-velocity")]
        private float RecommendedRightVelocity = 65f;

        //private int _StartSecondsTime;
        //private int _EndSecondsTime;
        //private int _StartMinuitesTime;
        //private int _EndMinuitesTime;
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
