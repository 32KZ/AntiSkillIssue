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
using BeatSaberMarkupLanguage.Tags;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;
using TMPro;
using static SliderController.Pool;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
    internal class AntiSkillIssueLeftViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";
        public AntiSkillIssueLeftViewController _AntiSkillIssueLeftViewController;
        internal static IPALogger Log { get; private set; }
        public string myPlayName { get; set; }
        public int myPlayLine { get; set; }
        public string myPlayPath { get; set; }

        #region Default Variables
        private string songName = "none";
        private int testInt = 0;
        #endregion Default Variables

        #region import variables

        public string CurrentPlayName;
        public string CurrentPlayPath;
        public string CurrentPlayLine;

        #endregion import variables


        #region UNIVERSAL UI ACTIONS
        [UIAction("test-decrement")]
        public void testDecrement() { testvalue--; }

        [UIAction("test-increment")]
        public void testIncrement() { testvalue++; }

        [UIAction("Click")]
        public void ButtonClicked()
        {
            Console.WriteLine("Button was clicked!");

        }
        //dynamic value
        [UIValue("dynamic-value")]
        private float DynamicValue;

        #endregion UNIVERSAL UI ACTIONS

        #region TAB 1 : START AND END SLIDERS

        #region T1: UI ACTIONS

        [UIAction("start-slider")]
        private void SetStartTime(float newStartTime)
        {
            DynamicValue = StartTime;
            StartTime = newStartTime;
            NotifyPropertyChanged("dynamic-value");

        }

        [UIAction("end-slider")]
        private void SetEndTime(float newEndTime)
        {

            endTime = newEndTime;

        }
        #endregion T1: UI ACTIONS

        #region T1: UI COMPONENTS

        [UIComponent("song-selected-text")]
        private string SelectedSongText;

        [UIComponent("increment-button")]
        private ButtonTag incrementButton;

        [UIComponent("test-value")]
        private TextTag TestInt;

        [UIComponent("decrement-button")]
        private ButtonTag decrementButton;

        [UIComponent("start-time-slider")]
        private SliderSetting startTimeSlider;

        [UIComponent("end-time-slider")]
        private SliderSetting endTimeSlider;

        #endregion T1: UI COMPONENTS

        #region T1: UI VALUES

        [UIValue("selected-song-name")]
        public string SongName


        {
            get { return songName; }
            set
            {
                this.songName = value;
                this.NotifyPropertyChanged();
                Console.WriteLine("Clearly you have never played a muffn map");
            }
        }

        [UIValue("test-value")]
        public int testvalue 


        {
            get { return testInt; }
            set
            {
                testvalue = value;
                this.NotifyPropertyChanged();
                Console.WriteLine("tv");
            }
        }

        [UIValue("song-length")]
        private float SongLength = 121f; //will be values taken from songdata

        [UIValue("start-slider")]
        private float StartTime = 0f;

        [UIValue("end-slider")]
        private float endTime = 60f;

        [UIValue("notes-selected")]
        private int NotesSlected = 32;

        [UIValue("duration-selected")]
        private int DurationSelected = 59;

        [UIValue("bpm-changes")]
        private int BPMChanges = 0;
        #endregion T1: UI VALUES

        #endregion TAB 1 : START AND END SLIDERS

        #region TAB 2 : PRESWING POST SWING (%)

        #region T2: UI VALUES

        #region T2: LEFT

        [UIValue("average-left-pre-swing")]
        private int AverageLeftPreSwing = 100;

        [UIValue("average-left-post-swing")]
        private int AverageLeftPostSwing = 100;

        #endregion T2: LEFT
        #region T2: RIGHT

        [UIValue("average-right-pre-swing")]
        private int AverageRightPreSwing = 100;

        [UIValue("average-right-post-swing")]
        private int AverageRightPostSwing = 100;

        #endregion T2: LEFT

        #endregion T2: UI VALUES

        #endregion TAB 2 : PRESWING POST SWING (%)

        #region TAB 3 : Acc

        #region T3: UIVALUES


        #region T3: LEFT

        [UIValue("average-left-accuracy")]
        private int AverageLeftAccuracy = 15;

        #endregion T3: LEFT

        #region T3: RIGHT

        [UIValue("average-right-accuracy")]
        private int AverageRightAccuracy = 15;
        #endregion T3: RIGHT

        #endregion T3: UIVALUES

        #endregion TAB 3 : Acc

        #region TAB 4 : TIME DEPENDENCY

        #region T4: UIVALUES

        #region T4: LEFT
        [UIValue("average-left-timing-dependence")]
        private string AverageLeftTimingDependence = "0.00" + "TD";

        [UIValue("average-left-timing-deviation")]
        private string AverageLeftTimingDeviation = "120" + "ms";
        #endregion T4: LEFT

        #region T4: RIGHT

        [UIValue("average-right-timing-dependence")]
        private string AverageRightTimingDependence = "0.00" + "TD";

        [UIValue("average-right-timing-deviation")]
        private string AverageRightTimingDeviation = "120" + "ms";

        #endregion T4: RIGHT

        #endregion T4: UIVALUES

        #endregion TAB 4 : TIME DEPENDENCY

        #region TAB 5 : VELOCITY

        #region T5: UIVALUES

        #region T5: LEFT

        [UIValue("average-left-velocity")]
        private float AverageLeftVelocity = 80f;

        [UIValue("recommended-left-velocity")]
        private float RecommendedLeftVelocity = 65f;

        #endregion T5: LEFT

        #region T5: RIGHT

        [UIValue("average-right-velocity")]
        private float AverageRightVelocity = 80f;

        [UIValue("recommended-right-velocity")]
        private float RecommendedRightVelocity = 65f;

        #endregion T5: RIGHT

        #endregion T5: UIVALUES

        #endregion TAB 5 : VELOCITY

        public void ImportPlayData(string newPlayPath, int newPlayLine, string newPlayName)
        {
            WorkingPlay workingPlay = new WorkingPlay(newPlayPath: newPlayPath, newPlayLine: newPlayLine, newPlayName: newPlayName);
            SongName = workingPlay.myPlayName;
            
            Plugin.Log.Info("WorkingPlay Created Successfully!");



        }

        public AntiSkillIssueLeftViewController(string newPlayPath, int newPlayLine, string newPlayName)
        {
            myPlayName = newPlayName;
            myPlayPath = newPlayPath;
            myPlayLine = newPlayLine;

        }


    }

    public class WorkingPlay
    {

        public string myPlayName { get; set; }
        public int myPlayLine { get; set; }
        public string myPlayPath { get; set; }


        public WorkingPlay(string newPlayPath, int newPlayLine, string newPlayName)
        {
            myPlayName = newPlayName;
            myPlayPath = newPlayPath;
            myPlayLine = newPlayLine;

        }

    }

}


