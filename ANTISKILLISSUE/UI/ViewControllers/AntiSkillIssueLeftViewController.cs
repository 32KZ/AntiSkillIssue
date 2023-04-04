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
using System.IO;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Diagnostics.Eventing.Reader;
using System.Collections;
using IPA.Utilities;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
    internal class AntiSkillIssueLeftViewController : BSMLResourceViewController //this view controller inherits from BSMLResourceViewController class.
    {
        #region Notes for Commonly occuring themes within this file.

        // 
        //  THIS IS THE FILE WHERE MOST OF THE MAGIC TAKES PLACE.
        //  ALL OF THIS IS DOCUMENTED INSIDE OF THE iMPLEMENTATION SECTION OF THE WRITEUP. 
        //  
        //
        //  =-=-=-=-=-=-=- This.NotifyPropertyChanged(); -=-=-=-=-=-=-=
        //  
        //  this.NotifyPropertyChanged is a Function inherited from the BSMLResourceViewController class.
        //  what it does, is it notifys the UI that a property has changed. as a result, ir refreshes the UIvalues inside of the UI.
        //  it also takes string input. for example, this.notifyPropertyChanged(nameof(AverageTimeDeviation))
        //  this makes it only refresh one UI value. However, the Performance Change is Minimal.
        //  
        //  =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //  

        #endregion 

        #region ResourceName 
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";

        //Our ResoureName is an override from the BSMLViewController Class. it sets this View Controller's Resource to
        // the file that matches the namespace's class + ".bsml" on the end.
        // this means the resource used to show the user the front end UI is the BSML file with the same name.
        // this is how the UI is structured within the project. the CS files Contain the code that Controlls elements Inside the BSML file.
        // the BSML file then is understood by the BSML library and Used to make a UI in the game that the end user is able to use. 
        //Additionally, so that C# knows that the BSML files are resources, the need to be marked as embedded resources within the Visual Studio Solution explorer. 

        #endregion ResourceName

        public Play WorkingPlay = new Play(null, null, float.NaN, null, null, null, null); 
        // Create a Blank workingplay. 

        #region Import Values
        public string myPlayName { get; set; }
        public int myPlayLine { get; set; }
        public string myPlayPath { get; set; }
        // Import these from the data transfer event 

        #endregion

        #region workingPlay Property
        public Play workingPlay
        {
            get
            {
                return WorkingPlay;
                //return our backing value
            }
            set
            {
                this.WorkingPlay = value;
                //no validation required for this input. backingvalue = the input.
            }

        }
        #endregion

        #region UI Backing Values

        #region Tracker Backing Values for Properties

        public hitTracker myHitTracker { get; set; }
        public accuracyTracker myAccuracyTracker { get; set; }
        public scoreTracker myScoreTracker { get; set; }
        public winTracker myWinTracker { get; set; }


        #endregion

        #region Tab 1 Overview And Slider Selection

        private string songName { get; set; } = null; //Name of the Song. Used in UI display.
        public string songLength { get; set; } //used in UI display
        public string songDifficulty { get; set; } //Used in UI display.
        public string deLimiter { get; set; } // Used in Front end Display.
        public string playValidity { get; set; } = "The Validity of the Play will appear here!"; //Validity. if null, Default. if !null, Decide if valid or not.
        public string validityFontColor { get; set; } = "#ffffff"; //Depending on the color of the Validity,

        //Front end display for this tab.


        #region Slider Properties
        public float songDuration { get; set; } = 60f; // Used in Slider Calculation.
        public float startTime { get; set; } // the StartSlider's actual Selected time
        public float endTime { get; set; }   // the end slider's actual selected time

        public float startSliderMaximum { get; set; } = 60f; //Maximum for the start Time Selector.
        public float endSliderMaximum { get; set; } = 60f;   //Maximum For the end time Selector
        #endregion

        #region Tracker Properties
        //Create Properties for all the trackers

        public hitTracker HitTracker 
        {
            get 
            {
                return myHitTracker;
            
            }
            set 
            {
                this.myHitTracker = value;
                //Currently active Tracker.

            }

        }

        public accuracyTracker AccuracyTracker
        {
            get
            {
                return myAccuracyTracker;

            }
            set
            {
                this.myAccuracyTracker = value;
                //Currently active Tracker.

            }

        }
        public scoreTracker ScoreTracker
        {
            get
            {
                return myScoreTracker;

            }
            set
            {
                this.myScoreTracker = value;
            }

        }
        public winTracker WinTracker
        {
            get
            {
                return myWinTracker;

            }
            set
            {
                this.myWinTracker = value;
            }

        }

        #endregion



        #endregion Tab 1 Overview And Slider Selection

        #region Tab 2 Preswing Poswing Backing Values 

        public float averageLeftPreSwing { get; set; } = 0f;
        public float averageLeftPostSwing { get; set; } = 0f;

        public float averageRightPreSwing { get; set; } = 0f;
        public float averageRightPostSwing { get; set; } = 0f;
        
        //Front end display of the average values for this tab.


        #endregion Tab 2 Preswing Poswing Backing Values 

        #region Tab 3 Accuracy 
        public float[] averageLeftCut { get; set; } = new float[3]; //Default Value is a Float array of length 3.
        public float[] averageRightCut { get; set; } = new float[3];
        // -----
        //Front end display of the average values for this tab.

        #endregion Tab 3 Accuracy

        #region Tab 4 Timing Dependence
        //Front end display of the average values for this tab.

        public string averageLeftTimingDependence { get; set; } = "0.00" + "TD";
        public string averageLeftTimingDeviation { get; set; } = "?" + "ms";
        
        public string averageRightTimingDependence { get; set; } = "0.00" + "TD";
        public string averageRightTimingDeviation { get; set; } = "?" + "ms";

        #endregion

        #region Tab 5 Velocity

        public float averageLeftVelocity { get; set; } = 0f;
        public float averageRightVelocity { get; set; } = 0f;
        public float recommendRightVelocity { get; set; } = 0f;
        public float recommendLeftVelocity { get; set; } = 0f;
        //Front end display of the average values for this tab.

        #endregion Tab 5 Velocity

        #endregion UI Backing Values


        #region UNIVERSAL UI ACTIONS

        [UIAction("Click")]
        public void ButtonClicked()
        {
            Plugin.Log.Info($"Click!");

        }
        //Easter egg for the Colored, Clickable text. Does nothing Functionally.
        // this code snippet also appears inside of the BSML doc's, as an example on how to set up UIActions.
        // as a result, not my code. i did not write this originally. 


        #endregion UNIVERSAL UI ACTIONS

        #region TAB 1 : START AND END SLIDERS

        #region T1: UI COMPONENTS

        [UIComponent("apply-button")]
        public ButtonTag ApplyButton;

        //start buttons
        [UIComponent("st-decrement-small")]
        public ButtonTag StartTimeDecrementSmall;

        [UIComponent("st-decrement-large")]
        public ButtonTag StartTimeDecrementLarge;

        [UIComponent("st-increment-large")]
        public ButtonTag StartTimeIncrementLarge;

        [UIComponent("st-increment-small")]
        public ButtonTag StartTimeIncrementSmall;

        //end buttons
        [UIComponent("et-decrement-small")]
        public ButtonTag EndTimeDecrementSmall;

        [UIComponent("et-decrement-large")]
        public ButtonTag EndTimeDecrementLarge;

        [UIComponent("et-increment-large")]
        public ButtonTag EndTimeIncrementLarge;

        [UIComponent("et-increment-small")]
        public ButtonTag EndTimeIncrementSmall;

        //start slider texts

        [UIComponent("st-slider-value")]
        public TextTag StartTimeSliderValue;

        [UIComponent("st-slider-maximum")]
        public TextTag StartTimeSliderMaximum;

        //end slider texts

        [UIComponent("et-slider-value")]
        public TextTag EndTimeSliderValue;

        [UIComponent("et-slider-maximum")]
        public TextTag EndTimeSliderMaximum;


        #endregion T1: UI COMPONENTS

        #region T1: UI VALUES

        #region Start Slider Value

        [UIValue("st-slider-value")]
        public float StartTime //Controls the starttimeSlider's actual Selected Value.
        {
            get
            {
                if (startTime == float.NaN) { startTime = 0f; }
                return startTime;
            }
            set
            {
                this.startTime = value;
                this.NotifyPropertyChanged();

            }

        }
        #endregion

        #region End Slider Value

        [UIValue("et-slider-value")]
        public float EndTime //Controls the EndTimeSlider's actual selected value
        {
            get
            {
                if (endTime == float.NaN) { endTime = 0f; }
                return endTime;
            }
            set
            {

                this.endTime = value;
                this.NotifyPropertyChanged();

            }

        }

        #endregion

        #region Start Slider Maximum
        [UIValue("st-slider-maximum")]
        public float StartSliderMaximum //Controls the starttimeSlider's actual Selected Value.
        {
            get
            {
                if (startSliderMaximum == float.NaN) { startSliderMaximum = 0f; }
                return startSliderMaximum;
            }
            set
            {
                this.startSliderMaximum = value;
                this.NotifyPropertyChanged();

            }

        }
        #endregion

        #region End Slider Maximum
        [UIValue("et-slider-maximum")]
        public float EndSliderMaximum //Controls the starttimeSlider's actual Selected Value.
        {
            get
            {
                if (endSliderMaximum == float.NaN) { endSliderMaximum = 0f; }
                return endSliderMaximum;
            }
            set
            {
                this.endSliderMaximum = value;
                this.NotifyPropertyChanged();

            }

        }
        #endregion

        #region Song Duration

        [UIValue("song-duration")]
        public float SongDuration
        {
            get
            {
                if (songDuration == float.NaN) { songDuration = 60f; }
                return songDuration;
            }
            set
            {
                this.songDuration = value;

                this.NotifyPropertyChanged();
                Plugin.Log.Info($"Song Duration Changed! {this.songDuration}");

            }
        }

        #endregion

        #region Song Name

        [UIValue("song-name")]
        public string SongName


        {
            get
            {
                if (songName == null) { songName = "Use these Sliders to Select a smaller section of the Map to review!"; }
                return songName;
                //when initially starting, the default value is used here to Display to the user the intent of the page.
            }
            set
            {
                this.songName = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.songName} Property Changed!");
            }
        }

        #endregion


        #region Song Difficulty

        [UIValue("song-difficulty")]
        public string SongDifficulty
        {
            get
            {
                if (songDifficulty == null) { songDifficulty = "all data on following tabs will change accordingly."; }
                return songDifficulty;
                // this is the same case as the Song name. this allows for greater UI understandability. 
            }
            set
            {
                this.songDifficulty = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.songDifficulty} Property Changed!");
            }
        }

        #endregion


        #region Song Length

        [UIValue("song-length")]
        public string SongLength
        {
            get
            {
                if (songLength == null) { songLength = ""; }
                return songLength;

            }
            set
            {
                this.songLength = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.songLength} Property Changed!");
            }
        }

        #endregion


        #region Delimiter

        [UIValue("delimiter")]
        public string Delimiter

        {
            get
            {
                if (SongName == "Use these Sliders to Select a smaller section of the Map to review!") { deLimiter = ""; }
                else { deLimiter = $"  -  "; }

                return deLimiter; //Delimiter is not a value declared in Working play, so it will check against the song name instead. 

            }
            set
            {
                this.deLimiter = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.deLimiter} Property Changed!");
            }
        }

        #endregion

        #region Play Validity 
        [UIValue("play-validity")]
        private string PlayValidity
        {
            get
            {
                return playValidity;
            }
            set
            {
                playValidity = value;
                this.NotifyPropertyChanged();
            }
        }

        [UIValue("validity-font-color")]
        private string ValidityFontColor
        {
            get
            {
                return validityFontColor;
            }
            set
            {
                validityFontColor = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Possible Values

        //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Possible Values

        [UIValue("notes-selected")]
        private int NotesSlected = 32;

        [UIValue("duration-selected")]
        private int DurationSelected = 59;

        [UIValue("bpm-changes")]
        private int BPMChanges = 0;

        //Here During Development, i had a few ideas for More Data types to implement into a future release.
        // these currently do nothing, and are unimplemented. 
        // my thought process was that if somone knows how many notes, and over how long, they could work out how fast the section is. 
        // or even better, i could tell them with the mod.
        // additionally, BPM changes would be helpful to know as it would help dictate how fast the user has to swing. 

        //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Possible Values
        #endregion

        #endregion T1: UI VALUES

        #region T1: UI ACTIONS 

        [UIAction("apply")]
        public void Apply()
        {
            if (SongName == null) { Plugin.Log.Info("Please Select a Play Before applying a range."); }
            else if (StartTime == EndTime) { Plugin.Log.Info("Please Select a range."); }
            else
            {

                ReadNoteTracker
                   (
                    WorkingPlay: workingPlay

                    );

            }

            // when the apply button is clicked, check if there is a Song Selected. if there is not, do not run.
            // if there is a song selected, and there is no range selected (start time and end time are the same.) do not run.
            // in all other cases, at least 1 second is selected, as a result, we can take information from this. 

        }

        #region Slider Buttons

        // Start time Slider Buttons
        [UIAction("st-decrement-small")]
        public void ActionStartTimeDecrementSmall()
        {

            if (StartTime - 1f <= 0f) 
            { StartTime = 0f; }
            else if (StartTime > EndTime)
            { StartTime = EndTime; }
            else if (StartTime > StartSliderMaximum)
            { StartTime = StartSliderMaximum; }
            else { StartTime = StartTime - 1f; }
            //if the result is not less than 0, or greater than the end time, or max, start time gets decremented. 
            // this is the same theory for all other buttons. this stops out of bounds from happening. 
        }

        [UIAction("st-decrement-large")]
        public void ActionStartTimeDecrementLarge()
        {

            if (StartTime - 10f <= 0f) 
            { StartTime = 0f; }
            else if (StartTime > EndTime)
            { StartTime = EndTime; }
            else if (StartTime > StartSliderMaximum) 
            { StartTime = StartSliderMaximum; }
            else { StartTime = StartTime - 10f; }

            
        }

        [UIAction("st-increment-small")]
        public void ActionStartTimeIncrementSmall()
        {
            if (StartTime + 1f <= 0f)  
            { StartTime = 0f; }
            else if (StartTime + 1f  > EndTime)
            { StartTime = EndTime; }
            else if (StartTime +1f > StartSliderMaximum) 
            { StartTime = StartSliderMaximum; }
            else { StartTime = StartTime + 1f; }
        }

        [UIAction("st-increment-large")]
        public void ActionStartTimeIncrementLarge()
        {
            if (StartTime + 10f <= 0f) 
            { StartTime = 0f; }
            else if (StartTime + 10f > EndTime)
            { StartTime = EndTime; }
            else if (StartTime + 10f > StartSliderMaximum)
            { StartTime = StartSliderMaximum; }
            else { StartTime = StartTime + 10f; }
        }

        //End time Slider Buttons

        [UIAction("et-decrement-small")]
        public void ActionEndTimeDecrementSmall()
        {
            if (EndTime - 1f <= 0f)  
            { EndTime = 0f; }
            else if (EndTime -1f < StartTime)
            { EndTime = StartTime; }
            else if (EndTime > StartSliderMaximum)
            { EndTime = StartSliderMaximum; }
            else { EndTime = EndTime - 1f; }
        }

        [UIAction("et-decrement-large")]
        public void ActionEndTimeDecrementLarge()
        {
            if (EndTime - 10f <= 0f)  
            { EndTime = 0f; }
            else if (EndTime -10f < StartTime)
            { EndTime = StartTime; }
            else if (EndTime > StartSliderMaximum)
            { EndTime = StartSliderMaximum; }
            else { EndTime = EndTime - 10f; }
        }

        [UIAction("et-increment-small")]
        public void ActionEndTimeIncrementSmall()
        {
            if (EndTime + 1f <= 0f)
            { EndTime = 0f; }
            else if (EndTime + 1f < StartTime)
            { EndTime = StartTime; }
            else if (EndTime +1f > StartSliderMaximum)
            { EndTime = StartSliderMaximum; }
            else { EndTime = EndTime + 1f; }
        }

        [UIAction("et-increment-large")]
        public void ActionEndTimeIncrementLarge()
        {
            if (EndTime + 10f <= 0f)
            { EndTime = 0f; }
            else if (EndTime + 10f < StartTime)
            { EndTime = StartTime; }
            else if (EndTime + 10f > StartSliderMaximum)
            { EndTime = StartSliderMaximum; }
            else { EndTime = EndTime + 10f; }
        }

        #endregion Slider Buttons


        #endregion


        #endregion TAB 1 : START AND END SLIDERS

        #region TAB 2 : PRESWING POST SWING (%)

        #region T2: UI VALUES

        #region T2: LEFT

        [UIValue("average-left-pre-swing")]
        private float AverageLeftPreSwing
        {
            get
            {
                if (averageLeftPreSwing == 0f) { averageLeftPreSwing = 100f; }
                return averageLeftPreSwing;
            }
            set
            {
                
                this.averageLeftPreSwing = (float)Math.Round(value,4)*100; 
                //cast round Double to the Float type, and round our value to 2 DP for display(perameter provided as 4 because its 2 orders of magnitude higher by the end of the calculation, because we want a percentage)
                
                this.NotifyPropertyChanged();

            }

        }

        [UIValue("average-left-post-swing")]
        private float AverageLeftPostSwing
        {
            get
            {
                if (averageLeftPostSwing == 0f) { averageLeftPostSwing = 100f; }
                return averageLeftPostSwing;
            }
            set
            {

                this.averageLeftPostSwing = (float)Math.Round(value, 4) * 100; //As a Percentage. Same theory as last time.
                this.NotifyPropertyChanged();

            }

        }

        #endregion T2: LEFT

        #region T2: RIGHT

        [UIValue("average-right-pre-swing")]
        private float AverageRightPreSwing
        {
            get
            {
                if (averageRightPreSwing == 0f) { averageRightPreSwing = 100f; }
                return averageRightPreSwing;
            }
            set
            {

                this.averageRightPreSwing = (float)Math.Round(value, 4) * 100; //Make Percentage
                this.NotifyPropertyChanged();

            }

        }

        [UIValue("average-right-post-swing")]
        private float AverageRightPostSwing
        {
            get
            {
                if (averageRightPostSwing == 0f) { averageRightPostSwing = 100f; }
                return averageRightPostSwing;
            }
            set
            {

                this.averageRightPostSwing = (float)Math.Round(value, 4) * 100; //Make Percentage
                this.NotifyPropertyChanged();

            }

        }

        #endregion T2: LEFT

        #endregion T2: UI VALUES

        #endregion TAB 2 : PRESWING POST SWING (%)

        #region TAB 3 : Accuracy 

        #region T3: UIVALUES


        #region T3: LEFT

        [UIValue("average-left-accuracy")]
        private float AverageLeftAccuracy
        {
            get
            {
                if (averageLeftCut[1] == float.NaN) { averageLeftCut[1] = 15f; }
                return averageLeftCut[1];                                           // if one value in the array is NaN, then all are NaN. as a result, return default Value.
            }
            set
            {

                this.averageLeftCut[1] = (float)Math.Round(value, 2); // We are looking for a 2dp float of Accuracy. eg, 13.43 or 10.23. (0.00 - 15.00)
                this.NotifyPropertyChanged();

            }

        }

        #endregion T3: LEFT

        #region T3: RIGHT

        [UIValue("average-right-accuracy")]
        private float AverageRightAccuracy
        {
            get
            {
                if (averageRightCut[1] == float.NaN) { averageRightCut[1] = 15f; }
                return averageRightCut[1]; // Actual or default Value Returned
            }
            set
            {

                this.averageRightCut[1] = (float)Math.Round(value, 2); // 2dp
                this.NotifyPropertyChanged();

            }

        }

        #endregion T3: RIGHT

        #endregion T3: UIVALUES

        #endregion TAB 3 : Accuracy

        #region TAB 4 : TIME DEPENDENCY

        #region T4: UIVALUES

        #region T4: LEFT

        [UIValue("average-left-timing-dependence")]
        private string AverageLeftTimingDependence
        {
            get
            { 
                return averageLeftTimingDependence;
            }
            set 
            { 
                this.averageLeftTimingDependence = $"{value} TD";
                this.NotifyPropertyChanged();
                //whatever the value comes in as, it can be cast to string.
                // So it is.
            } 
        }

        [UIValue("average-left-timing-deviation")]
        private string AverageLeftTimingDeviation
        {
            get
            {
                return averageLeftTimingDeviation;
            }
            set
            {
                this.averageLeftTimingDeviation = $"{value} ms";
                this.NotifyPropertyChanged();
            }
        }

        #endregion T4: LEFT

        #region T4: RIGHT

        [UIValue("average-right-timing-dependence")]
        private string AverageRightTimingDependence
        {
            get
            {
                return averageRightTimingDependence;
            }
            set
            {
                this.averageRightTimingDependence = $"{value} TD";
                this.NotifyPropertyChanged();
            }
        }

        [UIValue("average-right-timing-deviation")]
        private string AverageRightTimingDeviation
        {
            get
            {
                return averageRightTimingDeviation;
            }
            set
            {
                this.averageRightTimingDeviation = $"{value} ms";
                this.NotifyPropertyChanged();
            }
        }

        #endregion T4: RIGHT

        #endregion T4: UIVALUES

        #endregion TAB 4 : TIME DEPENDENCY

        #region TAB 5 : VELOCITY

        #region T5: UIVALUES

        #region T5: LEFT

        [UIValue("average-left-velocity")]
        private float AverageLeftVelocity
        {
            get
            {
                return averageLeftVelocity;
            }
            set
            {
                this.averageLeftVelocity = value;
                this.NotifyPropertyChanged();
            }
        }

        [UIValue("recommended-left-velocity")]
        private float RecommendedLeftVelocity
        {
            get
            {
                return recommendLeftVelocity;
            }
            set
            {
                this.recommendLeftVelocity = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion T5: LEFT

        #region T5: RIGHT

        [UIValue("average-right-velocity")]
        private float AverageRightVelocity
        {
            get
            {
                return averageRightVelocity;
            }
            set
            {
                this.averageRightVelocity = value;
                this.NotifyPropertyChanged();
            }
        }

        [UIValue("recommended-right-velocity")]
        private float RecommendedRightVelocity
        {
            get
            {
                return recommendRightVelocity;
            }
            set
            {
                this.recommendRightVelocity = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion T5: RIGHT

        #endregion T5: UIVALUES

        #endregion TAB 5 : VELOCITY

        #region timeFormatter
        private static string TimeCalculator(int MyValue)
        {
            int MinuitesTime = 0;
            while (MyValue >= 60)
            {
                MinuitesTime++;
                MyValue = MyValue - 60;
            };

            return $"  {MinuitesTime}m {MyValue}s ";
            // this is used in makeing User Friendly Time Display. for every 60 float seconds, add one to the Minuite time.


        }
        #endregion timeFormatter


        #region OnDataTransferEvent
        public void OnDataTransferEvent(object sender, DataTransferEventArgs eventArgs)
        {
            // when the event is called from the AntiSkillIssueViewController,
            // the information comes here and gets applied to the left view controller so we can use it.  
            myPlayName = eventArgs.Name;
            myPlayPath = eventArgs.Path;
            myPlayLine = eventArgs.Line;
            // If path is equal to the root folder, skip. (ADD THIS BEFORE YOU RELEASE)
            //Plugin.Log.Info($"");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Beat Savior Data\"; //Set the CD

            if (myPlayPath == path)
            { //DO NOTHING. if its equal to the path, we get a critical Access Denied Error, Breaking the UI. 
            } // this case Usually applies when the user Clicks One of the Dummy Cells meant to teach them How to use the UI. 
            else
            { //in all other cases, Go for it!



                #region Read Our Play into WorkingPlay

                StreamReader reader = File.OpenText(myPlayPath); // Open the File. (eg 2020-01-01.BSD)
                int x = 1; //Line number as integer, not index.
                string line;
                Play WorkingPlay = new Play(null, null, float.NaN, null, null, null, null); // Make WorkingPlay Usable Outside of While loop.
                while ((line = reader.ReadLine()) != null) //For every Line in the File,
                {
                    if (x != myPlayLine)    // if its not the selected Play, move to the next line until it is. 
                    {
                        x++;
                    }
                    else                    // When it is, deserialise the line as WorkingPlay, as class Play. then, icrement the line so it Skips to the end of the file.
                    {

                        WorkingPlay = JsonConvert.DeserializeObject<Play>(line); //WorkingPlay is Currently Equal to the Deserialisation(as Play class instance) of the Currently Selected Line.
                        WorkingPlay.playPath = myPlayPath;
                        WorkingPlay.playLine = myPlayLine;

                        Plugin.Log.Info($"{x}> Processing Play {WorkingPlay.songName} at path: {WorkingPlay.playPath}");

                        #region Clean Data
                        // Here, we are cleaning some Overview Data for the End User to look at so that it is Understandable and neat.
                        #region Song Duration Formatter (121 = 2m 1s)

                        int temp = Convert.ToInt32((WorkingPlay.songDuration + 1) / 1); // floor divide it as int to get a whole int. Round Up. to be inclusive. 
                        // if we rounded down here, we might miss out some notes on levels that end in the same second as a note Exists. we dont want that.

                        WorkingPlay.songDurationFormatted = TimeCalculator(MyValue: temp);

                        #endregion

                        //Song name does not need to be length limited here.
                        // it does in the Play's list however, so i left it commented here, and you can find it there too.
                        #region Song Name (Length Limit)
                        //if (play.songName.Length >= 14)
                        //{
                        //    play.songName = play.songName.Substring(0, 13) + "...";
                        //}
                        #endregion

                        #region Song Artist (Length Limit)
                        //if (play.songArtist.Length >= 11)
                        //{
                        //   play.songArtist = play.songArtist.Substring(0, 10) + "...";
                        //}
                        #endregion

                        #region Song Mapper (Length Limit)
                        //if (play.songMapper.Length >= 14)
                        //{
                        //    play.songMapper = play.songMapper.Substring(0, 13) + "...";
                        //}
                        #endregion

                        //
                        #region Song Difficulty (Capitalise)

                        if (Char.IsLower(WorkingPlay.songDifficulty[0]))
                        {
                            WorkingPlay.songDifficulty = Char.ToUpper(WorkingPlay.songDifficulty[0]) + WorkingPlay.songDifficulty.Substring(1);
                        }
                        //if letter index 1 of our song difficulty is not a capital letter, make it one.
                        //No song Difficulty will ever start with a number. Songdiff possibilities: [easy,normal,hard,expert,expertplus].
                        // Most songs have Custom Difficulty names, however, this will only ever identify them as Songdiff Possibilities mentioned above.
                        #endregion

                        #endregion Clean Data

                        x++;                                //Next line Num


                    }

                }
                reader.Close();                             // CLOSE READER AFTER DONE USING. WITHOUT THIS, YOU CAN ONLY USE IT ONCE. 

                #endregion

                #region Log info to --Vebose Debug menu.
                Plugin.Log.Info($"WorkingPlay:{WorkingPlay.songName}, by {WorkingPlay.songArtist}, mapped by {WorkingPlay.songMapper} ");
                Plugin.Log.Info($"{WorkingPlay.songDifficulty}");
                Plugin.Log.Info($"{WorkingPlay.songDurationFormatted}");

                #endregion Log info to --Vebose Debug menu.


                workingPlay = WorkingPlay;
                //Update Property Feild.

                #region Deserialise TRACKERS

                #region HIT TRACKER - used for overview information (notes hit or missed, wall hit, bomb hit ect) (OVERVIEW)
                object fileHitTracker = WorkingPlay.trackers["hitTracker"];
                hitTracker HitTracker = JsonConvert.DeserializeObject<hitTracker>(fileHitTracker.ToString());
                #endregion


                #region SCORE TRACKER - Overall Score in the Level, with modifiers and such. (OVERVIEW)

                object fileScoreTracker = WorkingPlay.trackers["scoreTracker"];
                scoreTracker ScoreTracker = JsonConvert.DeserializeObject<scoreTracker>(fileScoreTracker.ToString());

                #endregion

                #region WIN TRACKER - results of the Level. EG rank and such. (OVERVIEW)

                object fileWinTracker = WorkingPlay.trackers["winTracker"];
                winTracker WinTracker = JsonConvert.DeserializeObject<winTracker>(fileWinTracker.ToString());
                // this is Currently unused By the Project. however, in the Future, we can use this to Show the User their Score on the Level,
                // along side the current difficulty information.
               
                #endregion

                #region ACCURACY TRACKER - Contains average acc for hands, and a grid acc/Cut lists. (ACCURACY TAB) 
                object fileAccuracyTracker = WorkingPlay.trackers["accuracyTracker"];
                accuracyTracker AccuracyTracker = JsonConvert.DeserializeObject<accuracyTracker>(fileAccuracyTracker.ToString());
                #endregion

                #region DISTANCE TRACKER - Contains Overall distances of hand movement. "Usable" for overall Velocity 

                object fileDistanceTracker = WorkingPlay.trackers["distanceTracker"];
                distanceTracker DistanceTracker = JsonConvert.DeserializeObject<distanceTracker>(fileDistanceTracker.ToString());
                #endregion

                #region SCOREGRAPH TRACKER - Contains data for a graph as score data changes. might be useful for future development. 

                //object fileScoreGraphTracker = WorkingPlay.trackers["scoreGraphTracker"];
                //scoreGraphTracker ScoreGraphTracker = JsonConvert.DeserializeObject<scoreGraphTracker>(fileScoreGraphTracker.ToString());

                //Useful for Future possibilities with the Mod's Development, however, its currently outside the Scope. Commented out here
                // to save processing.

                #endregion


                #endregion

                #region Assign UIValues

                #region Assign Tab 1 UIValues

                //Front facing
                SongName = WorkingPlay.songName; //Song name Property is set so it can be used.
                SongDifficulty = WorkingPlay.songDifficulty; //Difficulty Property Set.
                SongLength = WorkingPlay.songDurationFormatted; //Song Length Gets formatted Before Being Set. (Formatted in the Clean data Region)

                //Back end, used for calculation.
                SongDuration = (float)Math.Round(WorkingPlay.songDuration, 0) + 1f;
                StartSliderMaximum = (float)Math.Round(WorkingPlay.songDuration, 0) + 1f;
                EndSliderMaximum = (float)Math.Round(WorkingPlay.songDuration, 0) + 1f;

                // Activate the Delimiter.
                Delimiter = $"  -  ";

                #region Validity Check

                if (HitTracker.maxCombo == 0)
                //in a valid score, the max combo will always be greater than 1,
                //unless all notes are missed, or you are playing a mapping extentions map, that Beat Savior data does not understand or support.
                //(if all notes are missed, this Project is not for you. or, you are eusing a desktop mode to set the score. as a result, its invalid still.)

                { //Assign Validity to Properties.
                    PlayValidity = "Invalid! BSD Does Not Support Modmaps, or Desktop Scores.";
                    ValidityFontColor = "#ff0000"; //red!
                }
                else
                { //Assign Validity to Properties.
                    PlayValidity = "Valid!";
                    ValidityFontColor = "#00ff00"; //Green!
                }
                #endregion Validity Check


                #endregion

                #region Assign Tab 2 UIValues

                // Assign properties with their Correct information, Updating the UI in the process.
                AverageLeftPreSwing = AccuracyTracker.leftPreswing;
                AverageLeftPostSwing = AccuracyTracker.leftPostswing;
                AverageRightPreSwing = AccuracyTracker.rightPreswing;
                AverageRightPostSwing = AccuracyTracker.rightPostswing;

                #endregion

                #region Assign Tab 3 UIValues
                // Assign properties with their Correct information, Updating the UI in the process.
                AverageLeftAccuracy = AccuracyTracker.leftAverageCut[1];
                AverageRightAccuracy = AccuracyTracker.rightAverageCut[1];

                #endregion Assign Tab 3 UIValues

                #region Assign Tab 4 UIValues

                // Assign properties with their Correct information, Updating the UI in the process.

                AverageLeftTimingDependence = Convert.ToString((float)Math.Round((float)AccuracyTracker.leftTimeDependence, 4)); // the string of our time dependence rounded to 4dp. (casted as float so we get all DP )
                AverageRightTimingDependence = Convert.ToString((float)Math.Round((float)AccuracyTracker.rightTimeDependence, 4)); //
                //timing deviation needs to be added here, but is stored inside notesDictionary, under deep trackers. 
                // as a result, it cannot be shown unless a user selects a range. the rest of time Dependence is OK still though.

                #endregion

                #region Assign Tab 5 UIValues

                AverageLeftVelocity = AccuracyTracker.leftSpeed;
                RecommendedLeftVelocity = (float)Math.Round(AverageLeftVelocity * 0.9f, 3); 
                AverageRightVelocity = AccuracyTracker.rightSpeed;
                RecommendedRightVelocity = (float)Math.Round(AverageRightVelocity * 0.9f, 3);
                //For now, ask them to move a tenth Slower. in the future,
                //we can have the user move a 10th faster than the average Notes Per Second of the selection Provided.  (3dp)
                //

                #endregion Assign Tab 5 UIValues


                #endregion

            }

        }

        #endregion OnDataTransferEvent


        #region Read Note Tracker

        public void ReadNoteTracker
                                        (
            Play WorkingPlay
                                        )
        {

            //Deep trackers 
            object fileNoteTracker = WorkingPlay.deepTrackers["noteTracker"];                                   // get "notes" : <this>[{},{}]
            noteTracker Notes = JsonConvert.DeserializeObject<noteTracker>(fileNoteTracker.ToString());         // Deserialise

            ArrayList DictionaryNotesList = new ArrayList();                                                    //create a new array list
            int x = 0;                                                                                          //set the indexer
            

            foreach (object FileDictionaryNotes in Notes.notes)                                       //for every entry in our deserialised dictionary list,
            {

                notesDictionary NotesDictionary = JsonConvert.DeserializeObject<notesDictionary>(FileDictionaryNotes.ToString());
                // notes dictionary is equal to each dictionary in the list

                if (NotesDictionary.time < StartTime) { x++; }
                else if (NotesDictionary.time >= StartTime & NotesDictionary.time <= EndTime) { DictionaryNotesList.Add(NotesDictionary); }
                else if (NotesDictionary.time > EndTime) { x++; } // possible efficiency increase if we stop deserialising past this point
                
                // add it to the arraylist if its within range.

                //notesDictionary activeDictNote = (notesDictionary)DictionaryNotesList[x]; // active Working notes Dictionary.  (CLI)


                #region //Display Data in CLI
                //Plugin.Log.Info($"Note ID: {activeDictNote.id}");// 2277
                //Plugin.Log.Info($"Note Index: {activeDictNote.index}");// 6 
                //Plugin.Log.Info($"NoteType: {activeDictNote.noteType}"); // 0
                //Plugin.Log.Info($"NoteDirection: {activeDictNote.noteDirection}");// 1 
                //Plugin.Log.Info($"NoteTime:  {activeDictNote.time}");// 307.5
                //Plugin.Log.Info($"cutType:  {activeDictNote.cutType}");// 0
                //Plugin.Log.Info($"Multiplyer:  {activeDictNote.multiplier}");// 8
                //foreach (int Y in activeDictNote.score) { Plugin.Log.Info($"Score:  {Y}"); }
                //foreach (int Y in activeDictNote.noteCenter) { Plugin.Log.Info($"noteCenter:  {Y}"); }
                //foreach (int Y in activeDictNote.noteRotation) { Plugin.Log.Info($"NoteRotation:  {Y}"); }
                //Plugin.Log.Info($"timeDeviation:  {activeDictNote.timeDeviation}"); // 0.0820...
                //Plugin.Log.Info($"Speed:  {activeDictNote.speed}"); // 40.95655
                //Plugin.Log.Info($"preSwing:  {activeDictNote.preswing}"); // 1.1495...
                //Plugin.Log.Info($"PostSwing:  {activeDictNote.postswing}"); //0.8838...
                //Plugin.Log.Info($"Distance to Center:  {activeDictNote.distanceToCenter}"); //0.03606...

                //if (activeDictNote.cutPoint != null) { foreach (int Y in activeDictNote.cutPoint) { Plugin.Log.Info($"CutPoint:  {Y}"); } }
                //else { Plugin.Log.Info($"cutPoint: Null"); }

                //if (activeDictNote.saberDir != null) { foreach (int Y in activeDictNote.saberDir) { Plugin.Log.Info($"SaberDir:  {Y}"); } }
                //else { Plugin.Log.Info($"cutPoint: Null"); }

                //if (activeDictNote.cutNormal != null) { foreach (int Y in activeDictNote.cutNormal) { Plugin.Log.Info($"CutNormal:  {Y}"); } }
                //else { Plugin.Log.Info($"CutNormal: Null"); }

                //Plugin.Log.Info($"TimeDependednce:  {activeDictNote.timeDependence}"); //0.01249...

                #endregion

                //x++; 
            }

            float newLeftAccuracyAverage = 0f;
            float newLeftTimeDeviationAverage = 0f;
            float newLeftTimeDependenceAverage = 0f;
            float newLeftSpeedAverage = 0f;
            float newLeftPreswingAverage = 0f;
            float newLeftPostswingAverage = 0f;

            float newRightAccuracyAverage = 0f;
            float newRightTimeDeviationAverage = 0f;
            float newRightTimeDependenceAverage = 0f;
            float newRightSpeedAverage = 0f;
            float newRightPreswingAverage = 0f;
            float newRightPostswingAverage = 0f;

            ArrayList leftAccuracyAverageList = new ArrayList();
            ArrayList leftTimeDeviationAverageList = new ArrayList();
            ArrayList leftTimeDependenceAverageList = new ArrayList();
            ArrayList leftSpeedAverageList = new ArrayList();
            ArrayList leftPreswingAverageList = new ArrayList();
            ArrayList leftPostswingAverageList = new ArrayList();

            ArrayList rightAccuracyAverageList = new ArrayList();
            ArrayList rightTimeDeviationAverageList = new ArrayList();
            ArrayList rightTimeDependenceAverageList = new ArrayList();
            ArrayList rightSpeedAverageList = new ArrayList();
            ArrayList rightPreswingAverageList = new ArrayList();
            ArrayList rightPostswingAverageList = new ArrayList();



            foreach (notesDictionary CurrentNoteDictionary in DictionaryNotesList) 
            {
                if (CurrentNoteDictionary.noteType == 0)// if hand is = right
                {
                    rightAccuracyAverageList.Add(CurrentNoteDictionary.score[1]);
                    rightTimeDeviationAverageList.Add(CurrentNoteDictionary.timeDeviation);
                    rightTimeDependenceAverageList.Add(CurrentNoteDictionary.timeDependence);
                    rightSpeedAverageList.Add(CurrentNoteDictionary.speed);
                    rightPreswingAverageList.Add(CurrentNoteDictionary.preswing);
                    rightPostswingAverageList.Add(CurrentNoteDictionary.postswing);
                }
                else // you have two hands, the only other option is left.
                {
                    leftAccuracyAverageList.Add(CurrentNoteDictionary.score[1]);
                    leftTimeDeviationAverageList.Add(CurrentNoteDictionary.timeDeviation);
                    leftTimeDependenceAverageList.Add(CurrentNoteDictionary.timeDependence);
                    leftSpeedAverageList.Add(CurrentNoteDictionary.speed);
                    leftPreswingAverageList.Add(CurrentNoteDictionary.preswing);
                    leftPostswingAverageList.Add(CurrentNoteDictionary.postswing);
                }

            }

            #region Calculate Accuracy averages For Left and right Hands



            int leftTotalNoteAccuracy = 0;
            int length = 0;
            foreach (int NoteAccuracy in leftAccuracyAverageList) 
            {
                leftTotalNoteAccuracy = leftTotalNoteAccuracy + NoteAccuracy;
                length++;
            }
            newLeftAccuracyAverage = (float)leftTotalNoteAccuracy / (float)length;


            int rightTotalNoteAccuracy = 0;
            length = 0;
            foreach (int NoteAccuracy in rightAccuracyAverageList)
            {
                rightTotalNoteAccuracy = rightTotalNoteAccuracy + NoteAccuracy;
                length++;
            }
            newRightAccuracyAverage = (float)rightTotalNoteAccuracy / (float)length;
            length = 0;



            this.AverageLeftAccuracy = newLeftAccuracyAverage;
            this.AverageRightAccuracy = newRightAccuracyAverage;



            #endregion

            #region Calculate Time Deiviation Averages for each hand.

            float leftTotalTimeDeviation = 0f;  //invalid cast here~
            length = 0;
            foreach (float NoteDeviation in leftTimeDeviationAverageList)
            {

                if (NoteDeviation != float.NaN) //if the note was missed than there is NAN for Deviation
                { 
                    leftTotalTimeDeviation = leftTotalTimeDeviation + NoteDeviation;
                    length++;
                }               
            }

            newLeftTimeDeviationAverage = (float)leftTotalTimeDeviation / (float)length;



            float rightTotalTimeDeviation = 0f;
            length = 0;
            foreach (float NoteDeviation in rightTimeDeviationAverageList)
            {
                if (NoteDeviation != float.NaN) //if the note was missed than there is NAN for Deviation
                {
                    rightTotalTimeDeviation = rightTotalTimeDeviation + (float)NoteDeviation;
                    length++;
                }
            }
            newRightTimeDeviationAverage = (float)rightTotalTimeDeviation / (float)length;



            this.AverageLeftTimingDeviation = Convert.ToString(Math.Round(newLeftTimeDeviationAverage,4));
            this.AverageRightTimingDeviation = Convert.ToString(Math.Round(newRightTimeDeviationAverage,4));



            #endregion

            #region Calculate Time Dependence Averages for each hand.

            float leftTotalTimeDependence = 0f;
            length = 0;
            foreach (float NoteDependence in leftTimeDependenceAverageList)
            {
                if (NoteDependence != float.NaN) //if the note was missed than there is NAN for Dependence
                {
                    leftTotalTimeDependence = leftTotalTimeDependence + (float)NoteDependence;
                    length++;
                }
            }
            newLeftTimeDependenceAverage = (float)leftTotalTimeDependence / (float)length;



            float rightTotalTimeDependence = 0f;
            length = 0;
            foreach (float NoteDependence in rightTimeDependenceAverageList)
            {
                if (NoteDependence != float.NaN) //if the note was missed than there is NAN for Dependence
                {
                    rightTotalTimeDependence = rightTotalTimeDependence + (float)NoteDependence;
                    length++;
                }
            }
            newRightTimeDependenceAverage = (float)rightTotalTimeDependence / (float)length;



            this.AverageLeftTimingDependence = Convert.ToString(Math.Round(newLeftTimeDependenceAverage, 4));
            this.AverageRightTimingDependence = Convert.ToString(Math.Round(newRightTimeDependenceAverage, 4));

            #endregion

            #region Calculate Speed Averages For each Hand.

            float leftTotalSpeed = 0f;
            length = 0;
            foreach (float NoteSpeed in leftSpeedAverageList)
            {

                if (NoteSpeed != float.NaN) //if the note was missed than there is NAN for Deviation
                {
                    leftTotalSpeed = leftTotalSpeed + NoteSpeed;
                    length++;
                }

            }

            newLeftSpeedAverage = (float)leftTotalSpeed / (float)length;

            float rightTotalSpeed = 0f;
            length = 0;
            foreach (float NoteSpeed in rightSpeedAverageList)
            {

                if (NoteSpeed != float.NaN) //if the note was missed than there is NAN
                {
                    rightTotalSpeed = rightTotalSpeed + NoteSpeed;
                    length++;
                }

            }

            newRightSpeedAverage = (float)rightTotalSpeed / (float)length;

            AverageLeftVelocity = newLeftSpeedAverage;
            AverageRightVelocity = newRightSpeedAverage;
            RecommendedLeftVelocity = (float)Math.Round(AverageLeftVelocity * 0.9f, 3);
            RecommendedRightVelocity = (float)Math.Round(AverageRightVelocity * 0.9f, 3);

            #endregion

            #region Calculate Preswing and postswing averafge for left and right hand

            float leftTotalPreswing = 0f;
            length = 0;
            foreach (float NotePreswing in leftPreswingAverageList)
            {

                if (NotePreswing != float.NaN) //if the note was missed than there is NAN for Deviation
                {
                    leftTotalPreswing = leftTotalPreswing + NotePreswing;
                    length++;
                }

            }

            newLeftPreswingAverage = (float)leftTotalPreswing / (float)length;

            float rightTotalPreswing = 0f;
            length = 0;
            foreach (float NotePreswing in rightPreswingAverageList)
            {

                if (NotePreswing != float.NaN) //if the note was missed than there is NAN for Deviation
                {
                    rightTotalPreswing = rightTotalPreswing + NotePreswing;
                    length++;
                }

            }

            newRightPreswingAverage = (float)rightTotalPreswing / (float)length;


            float leftTotalPostswing = 0f;
            length = 0;
            foreach (float NotePostswing in leftPostswingAverageList)
            {

                if (NotePostswing != float.NaN) //if the note was missed than there is NAN for Deviation
                {
                    leftTotalPostswing = leftTotalPostswing + NotePostswing;
                    length++;
                }

            }

            newLeftPostswingAverage = (float)leftTotalPostswing / (float)length;

            float rightTotalPostswing = 0f;
            length = 0;
            foreach (float NotePostswing in rightPostswingAverageList)
            {

                if (NotePostswing != float.NaN) //if the note was missed than there is NAN for Deviation
                {
                    rightTotalPostswing = rightTotalPostswing + NotePostswing;
                    length++;
                }

            }

            newRightPostswingAverage = (float)rightTotalPostswing / (float)length;

            AverageRightPreSwing = newRightPreswingAverage;
            AverageRightPostSwing = newRightPostswingAverage;
            AverageLeftPreSwing = newLeftPreswingAverage;
            AverageLeftPostSwing = newLeftPostswingAverage;


            #endregion

            //For each hand get averages and update the properties
            //Nullify active Values.


            //{
            //
            //"score":[70,12,30],
            //
            //"timeDeviation":0.017578125,
            //"speed":57.0695038,
            //"preswing":1.54497993,
            //"postswing":0.843348563,
            //"timeDependence":0.171928167
            //
            //}

        }

        #endregion

    }

    #region Trackers


    public class Play
    {
        public string playPath { get; set; }
        public int playLine { get; set; }

        public string songName { get; set; }

        public string songDurationFormatted { get; set; }
        public float songDuration { get; set; }

        public string songDifficulty { get; set; }

        public string songArtist { get; set; }

        public string songMapper { get; set; }
        public Dictionary<string, object> trackers { get; set; }
        public Dictionary<string, object> deepTrackers { get; set; }

        public Play(
            string newPlayPath,
            string newSongName,
            float newSongDuration,
            string newSongArtist,
            string newSongMapper,
            Dictionary<string, object> TheTrackers,
            Dictionary<string, object> TheDeepTrackers
                   )
        {
            playPath = newPlayPath;
            songName = newSongName;
            songDuration = newSongDuration;
            songArtist = newSongArtist;
            songMapper = newSongMapper;
            trackers = TheTrackers;
            deepTrackers = TheDeepTrackers;


        }



    }

    public class hitTracker
    {
        public int leftNoteHit { get; set; }
        public int rightNoteHit { get; set; }

        public int bombHit { get; set; }

        public int maxCombo { get; set; }
        public int nbOfWallsHit { get; set; }

        public int miss { get; set; }

        public int missedNotes { get; set; }
        public int badCuts { get; set; }
        public int leftMiss { get; set; }
        public int rightMiss { get; set; }
        public int leftBadCuts { get; set; }
        public int rightBadCuts { get; set; }



        public hitTracker
                             (
            int newLeftNoteHit,
            int newRightNoteHit,
            int newBombHit,
            int newMaxCombo,
            int newNbOfWallsHit,
            int newMiss,
            int newLeftMiss,
            int newRightMiss,
            int newMissedNotes,
            int newBadCuts,
            int newLeftBadCuts,
            int newRightBadCuts
                             )
        {


            leftNoteHit = newLeftNoteHit;
            rightNoteHit = newRightNoteHit;
            bombHit = newBombHit;
            maxCombo = newMaxCombo;
            nbOfWallsHit = newNbOfWallsHit;
            miss = newMiss;
            leftMiss = newLeftMiss;
            rightMiss = newRightMiss;
            missedNotes = newMissedNotes;
            badCuts = newBadCuts;
            leftBadCuts = newLeftBadCuts;
            rightBadCuts = newRightBadCuts;
        }



    }

    public class accuracyTracker
    {
        public float accRight { get; set; }
        public float accLeft { get; set; }
        public float averageAcc { get; set; }
        public float leftSpeed { get; set; }
        public float rightSpeed { get; set; }
        public float averageSpeed { get; set; }
        public float leftHighestSpeed { get; set; }
        public float rightHighestSpeed { get; set; }
        public float leftPreswing { get; set; }
        public float rightPreswing { get; set; }
        public float averagePreswing { get; set; }
        public float leftPostswing { get; set; }
        public float rightPostswing { get; set; }
        public float averagePostswing { get; set; }
        public float leftTimeDependence { get; set; }
        public float rightTimeDependence { get; set; }
        public float averageTimeDependence { get; set; }
        public float[] leftAverageCut { get; set; }
        public float[] rightAverageCut { get; set; }
        public float[] averageCut { get; set; }
        public float[] gridAcc { get; set; }
        public float[] gridCut { get; set; }




        public accuracyTracker
                             (
             float newAccRight,
             float newAccLeft,
             float newAverageAcc,
             float newLeftSpeed,
             float newRightSpeed,
             float newAverageSpeed,
             float newLeftHighestSpeed,
             float newRightHighestSpeed,
             float newLeftPreswing,
             float newRightPreswing,
             float newAveragePreswing,
             float newLeftPostswing,
             float newRightPostswing,
             float newAveragePostswing,
             float newLeftTimeDependence,
             float newRightTimeDependence,
             float newAverageTimeDependence,
             float[] newLeftAverageCut,
             float[] newRightAverageCut,
             float[] newAverageCut,
             float[] newGridAcc,
             float[] newGridCut

                             )
        {


            accRight = newAccRight;
            accLeft = newAccLeft;
            averageAcc = newAverageAcc;
            leftSpeed = newLeftSpeed;
            rightSpeed = newRightSpeed;
            averageSpeed = newAverageSpeed;
            leftHighestSpeed = newLeftHighestSpeed;
            rightHighestSpeed = newRightHighestSpeed;
            leftPreswing = newLeftPreswing;
            rightPreswing = newRightPreswing;
            averagePreswing = newAveragePreswing;
            leftPostswing = newLeftPostswing;
            rightPostswing = newRightPostswing;
            averagePostswing = newAveragePostswing;
            leftTimeDependence = newLeftTimeDependence;
            rightTimeDependence = newRightTimeDependence;
            averageTimeDependence = newAverageTimeDependence;
            leftAverageCut = newLeftAverageCut;
            rightAverageCut = newRightAverageCut;
            averageCut = newAverageCut;
            gridAcc = newGridAcc;
            gridCut = newGridCut;


        }



    }

    public class scoreTracker
    {
        public int rawScore { get; set; }
        public int score { get; set; }
        public int personalBest { get; set; }
        public float rawRatio { get; set; }
        public float modifiedRatio { get; set; }
        public float personalBestRawRatio { get; set; }
        public float personalBestModifiedRatio { get; set; }
        public float modifiersMultiplier { get; set; }
        public string[] modifiers { get; set; }



        public scoreTracker
                             (
            int newRawScore,
            int newScore,
            int newPersonalBest,
            float newRawRatio,
            float newModifiedRatio,
            float newPersonalBestRawRatio,
            float newPersonalBestModifiedRatio,
            float newModifiersMultiplier,
            string[] newModifiers

                             )
        {


            rawScore = newRawScore;
            score = newScore;
            personalBest = newPersonalBest;
            rawRatio = newRawRatio;
            personalBestRawRatio = newPersonalBestRawRatio;
            modifiedRatio = newModifiedRatio;
            personalBestModifiedRatio = newPersonalBestModifiedRatio;
            modifiersMultiplier = newModifiersMultiplier;
            modifiers = newModifiers;

        }



    }

    public class winTracker
    {
        public bool won { get; set; }
        public string rank { get; set; }
        public float endTime { get; set; }
        public int nbOfPause { get; set; }




        public winTracker
                             (
            bool newWon,
            string newRank,
            float newEndTime,
            int newNbOfPause

                             )
        {


            won = newWon;
            rank = newRank;
            endTime = newEndTime;
            nbOfPause = newNbOfPause;

        }



    }

    public class distanceTracker
    {
        public float rightSaber { get; set; }
        public float leftSaber { get; set; }
        public float rightHand { get; set; }
        public float leftHand { get; set; }


        public distanceTracker
                             (
            float newRightSaber,
            float newLeftSaber,
            float newRightHand,
            float newLeftHand

                             )
        {


            rightSaber = newRightSaber;
            leftSaber = newLeftSaber;
            rightHand = newRightHand;
            leftHand = newLeftHand;
        }



    }

    public class scoreGraphTracker
    {
        public object graph { get; set; }

        public scoreGraphTracker
                             (
            object newGraph
                             )
        {
            graph = newGraph;
        }



    }

    public class noteTracker
    {
        public object[] notes { get; set; } // key notes's value, the dict itself.

        public noteTracker
                             (

            object[] newNotes

                             )
        {


            notes = newNotes;

        }



    }

    public class notesDictionary
    {
        public int noteType { get; set; }
        public int noteDirection { get; set; }
        public int index { get; set; }
        public int id { get; set; }
        public float time { get; set; }
        public int cutType { get; set; }
        public int multiplier { get; set; }
        public int[] score { get; set; }
        public float[] noteCenter { get; set; }
        public float[] noteRotation { get; set; }
        public float timeDeviation { get; set; }
        public float speed { get; set; }
        public float preswing { get; set; }
        public float postswing { get; set; }
        public float distanceToCenter { get; set; }
        public float[] cutPoint { get; set; }
        public float[] saberDir { get; set; }
        public float[] cutNormal { get; set; }
        public float timeDependence { get; set; }

        public notesDictionary
                             (

            int newNoteType,
            int newNoteDirection,
            int newIndex,
            int newId,
            float newTime,
            int newCutType,
            int newMultiplier,
            int[] newScore,
            float[] newNoteCenter,
            float[] newNoteRotation,
            float newTimeDeviation,
            float newSpeed,
            float newPreswing,
            float newPostswing,
            float newDistanceToCenter,
            float[] newCutPoint,
            float[] newSaberDir,
            float[] newCutNormal,
            float newTimeDependence

                             )
        {


            noteType = newNoteType;
            noteDirection = newNoteDirection;
            index = newIndex;
            id = newId;
            time = newTime;
            cutType = newCutType;
            multiplier = newMultiplier;
            score = newScore;
            noteCenter = newNoteCenter;
            noteRotation = newNoteRotation;
            timeDeviation = newTimeDeviation;
            speed = newSpeed;
            preswing = newPreswing;
            postswing = newPostswing;
            distanceToCenter = newDistanceToCenter;
            cutPoint = newCutPoint;
            saberDir = newSaberDir;
            cutNormal = newCutNormal;
            timeDependence = newTimeDependence;


        }



    }

    #endregion Trackers


}


