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

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
    internal class AntiSkillIssueLeftViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";
        public string myPlayName { get; set; }
        public int myPlayLine { get; set; }
        public string myPlayPath { get; set; }

        private string songName { get; set; }

        public string songLength { get; set; } //used in UI display
        public float songDuration { get; set; } // Used in Slider Calculation.

        public string songDifficulty { get; set; }
        public string deLimiter { get; set; }


        #region UNIVERSAL UI ACTIONS
        [UIAction("Click")]
        public void ButtonClicked()
        {
            Plugin.Log.Info($"Click!");
            
        }



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
        private TextTag SelectedSongText;

        [UIComponent("start-time-slider")]
        private SliderSetting startTimeSlider;

        [UIComponent("end-time-slider")]
        private SliderSetting endTimeSlider;

        #endregion T1: UI COMPONENTS

        #region T1: UI VALUES

        //dynamic value
        [UIValue("dynamic-value")]
        private float DynamicValue;

        [UIValue("song-name")]
        public string SongName


        {
            get 
            {
                if (songName == null) { songName = "Use these Sliders to Select a smaller section of the Map to review!"; }
                return songName;
            }
            set
            {
                this.songName = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.songName} Property Changed!");
            }
        }

        [UIValue("song-difficulty")]
        public string SongDifficulty


        {
            get
            {
                if (songDifficulty == null) { songDifficulty = "all data on following tabs will change accordingly."; }
                return songDifficulty;
            }
            set
            {
                this.songDifficulty = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.songDifficulty} Property Changed!");
            }
        }

        [UIValue("song-length")]
        public string SongLength //will be values taken from songdata


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

        [UIValue("delimiter")]
        public string Delimiter 

        {
            get
            {
                if (SongName == "Use these Sliders to Select a smaller section of the Map to review!") { deLimiter = ""; }
                else { deLimiter = "  -  "; }

                return deLimiter; //Delimiter is not a value declared in Working play, so it will check against the song name instead. 

            }
            set
            {
                this.deLimiter = value;
                this.NotifyPropertyChanged();
                Plugin.Log.Info($"{this.deLimiter} Property Changed!");
            }
        }

        [UIValue("song-duration")]
        public float SongDuration 
        { 
            get 
            {
                if (songDuration == null) { songDuration = 60f; }
                return songDuration; 
            } 
            set 
            {
            }
        }


                [UIValue("start-slider")]
        private float StartTime = 0f;

        [UIValue("end-slider")]
        private float endTime = 60f;

        //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        
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

        #region timeFormatter
        private static string TimeCalculator(float MyValue)
        {
            int MinuitesTime = 0;
            while (MyValue >= 60)
            {
                MinuitesTime++;
                MyValue = MyValue - 60;
            };

            return MinuitesTime + "m " + MyValue + "s";
        }
        #endregion timeFormatter

        public void OnDataTransferEvent(object sender, DataTransferEventArgs eventArgs)
        {
            // when the event is called from the AntiSkillIssueViewController,
            // the information comes here and gets applied to the left view controller so we can use it.  
            myPlayName = eventArgs.Name;
            myPlayPath = eventArgs.Path;
            myPlayLine = eventArgs.Line;
            
            //Plugin.Log.Info("");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Beat Savior Data\"; //Set the CD

            StreamReader reader = File.OpenText(myPlayPath);
            int x = 1;
            string line;
            Play WorkingPlay = new Play(null, null, null, null, null, null, null);
            while ((line = reader.ReadLine()) != null)
            {
                if (x != myPlayLine) //Line number as int32 not index
                {
                    x++; //if the Line Indecator is not empty, 
                }
                else
                {
                    WorkingPlay = JsonConvert.DeserializeObject<Play>(line);
                    WorkingPlay.playPath = myPlayPath;
                    WorkingPlay.playLine = myPlayLine;
                    Plugin.Log.Info($"{x}> Processing Play {WorkingPlay.songName} at path: {WorkingPlay.playPath}");

                    #region Clean Data

                    #region Song Duration Formatter (121 = 2m 1s)

                    float temp = float.Parse(WorkingPlay.songDuration) * 1000;
                    int temp2 = Convert.ToInt32(temp) / 1000;
                    WorkingPlay.songDurationFormatted = TimeCalculator(MyValue: temp2);

                    #endregion

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

                    #region Song Difficulty (Capitalise)

                    if (Char.IsLower(WorkingPlay.songDifficulty[0]))
                    {
                        WorkingPlay.songDifficulty = Char.ToUpper(WorkingPlay.songDifficulty[0]) + WorkingPlay.songDifficulty.Substring(1);
                    }
                    //if letter index 1 of our song difficulty is not a capital letter, make it one.
                    //No song Difficulty will ever start with a number.

                    #endregion



                    #endregion Clean Data
                    //Play is a local variable here. you need to make a working play and swap it out here, including cleaning the data. that allows you to move onto the deserialising. 
                    
                    x++;                                //Next line Num

                    
                }
                
            }
            reader.Close();                             // CLOSE READER

            Plugin.Log.Info($"WorkingPlay:{WorkingPlay.songName}, by {WorkingPlay.songArtist}, mapped by {WorkingPlay.songMapper} ");
            Plugin.Log.Info($"{WorkingPlay.songDifficulty}");
            Plugin.Log.Info($"{WorkingPlay.songDurationFormatted}");

            SongName = WorkingPlay.songName;

            SongDifficulty = WorkingPlay.songDifficulty;
            SongLength = WorkingPlay.songDurationFormatted;
            songDuration = (float)Convert.ToInt32(WorkingPlay.songDuration);
            Delimiter = "  -  ";

            #region Deserialise TRACKERS
            //HIT TRACKER
            object fileHitTracker = WorkingPlay.trackers["hitTracker"];
            hitTracker HitTracker = JsonConvert.DeserializeObject<hitTracker>(fileHitTracker.ToString());

            //ACCURACY TRACKER
            object fileAccuracyTracker = WorkingPlay.trackers["accuracyTracker"];
            accuracyTracker AccuracyTracker = JsonConvert.DeserializeObject<accuracyTracker>(fileAccuracyTracker.ToString());


            //SCORE TRACKER

            object fileScoreTracker = WorkingPlay.trackers["scoreTracker"];
            scoreTracker ScoreTracker = JsonConvert.DeserializeObject<scoreTracker>(fileScoreTracker.ToString());

            //WIN TRACKER

            object fileWinTracker = WorkingPlay.trackers["winTracker"];
            winTracker WinTracker = JsonConvert.DeserializeObject<winTracker>(fileWinTracker.ToString());

            //DISTANCE TRACKER

            object fileDistanceTracker = WorkingPlay.trackers["distanceTracker"];
            distanceTracker DistanceTracker = JsonConvert.DeserializeObject<distanceTracker>(fileDistanceTracker.ToString());

            //SCOREGRAPH TRACKER

            object fileScoreGraphTracker = WorkingPlay.trackers["scoreGraphTracker"];
            scoreGraphTracker ScoreGraphTracker = JsonConvert.DeserializeObject<scoreGraphTracker>(fileScoreGraphTracker.ToString());



            #endregion Deserialise TRACKERS


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


    #region Trackers


    public class Play
    {
        public string playPath { get; set; }
        public int playLine { get; set; }

        public string songName { get; set; }

        public string songDurationFormatted { get; set; }
        public string songDuration { get; set; }

        public string songDifficulty { get; set; }

        public string songArtist { get; set; }

        public string songMapper { get; set; }
        public Dictionary<string, object> trackers { get; set; }
        public Dictionary<string, object> deepTrackers { get; set; }

        public Play(
            string newPlayPath,
            string newSongName,
            string newSongDuration,
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


