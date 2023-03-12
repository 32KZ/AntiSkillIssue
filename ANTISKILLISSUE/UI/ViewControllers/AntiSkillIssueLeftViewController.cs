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

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{
    internal class AntiSkillIssueLeftViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";

        #region Import Values
        public string myPlayName { get; set; }
        public int myPlayLine { get; set; }
        public string myPlayPath { get; set; }

        #endregion

        #region UI Backing Values

        #region Tab 1 Overview And Slider Selection

        private string songName { get; set; }
        public string songLength { get; set; } //used in UI display
        public string songDifficulty { get; set; }
        public string deLimiter { get; set; }
        public string playValidity { get; set; } = "The Validity of the Play will appear here!";
        public string validityFontColor { get; set; } = "#ffffff";

        #region Slider Properties
        public float songDuration { get; set; } = 0f; // Used in Slider Calculation.

        public float startTime { get; set; }
        public float endTime { get; set; }

        #endregion


        #endregion Tab 1 Overview And Slider Selection

        #region Tab 2 Preswing Poswing Backing Values 

        public float averageLeftPreSwing { get; set; } = 0f;
        public float averageLeftPostSwing { get; set; } = 0f;

        public float averageRightPreSwing { get; set; } = 0f;
        public float averageRightPostSwing { get; set; } = 0f;

        #endregion Tab 2 Preswing Poswing Backing Values 

        #region Tab 3 Accuracy 
        public float[] averageLeftCut { get; set; } = new float[3]; //Default Value is a Float array of length 3.
        public float[] averageRightCut { get; set; } = new float[3]; 

        #endregion Tab 3 Accuracy

        #endregion UI Backing Values



        #region UNIVERSAL UI ACTIONS

        [UIAction("Click")]
        public void ButtonClicked()
        {
            Plugin.Log.Info($"Click!");
            
        }



        #endregion UNIVERSAL UI ACTIONS

        #region TAB 1 : START AND END SLIDERS

        #region T1: UI ACTIONS

        [UIAction("set-start-slider")]
        private void SetStartTime(float value)
        {

            StartTime = value;

        }

        [UIAction("set-end-slider")]
        private void SetEndSlider(float value)
        {

            EndTime = value;

        }
        #endregion T1: UI ACTIONS

        #region T1: UI COMPONENTS

        [UIComponent("start-time-slider")]
        private SliderSetting startTimeSlider;

        [UIComponent("end-time-slider")]
        private SliderSetting endTimeSlider;

        #endregion T1: UI COMPONENTS

        #region T1: UI VALUES

        #region Start Slider

        [UIValue("start-slider")]
        public float StartTime
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


        #region End Slider

        [UIValue("end-slider")]
        public float EndTime 
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


        #region Song Duration

        [UIValue("song-duration")]
        public float SongDuration
        {
            get
            {
                if (songDuration == 0f) { songDuration = 60f; }
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


        #region Dynamic Value

        [UIValue("dynamic-value")]
        private float DynamicValue;

        #endregion


        #region Song Name

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

        #endregion


        #region Song Difficulty

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

        //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Possible Values
        #endregion

        #endregion T1: UI VALUES

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
                
                this.averageLeftPreSwing = (float)Math.Round(value,4)*100; //cast round Double to the Float type, and round our value to 2 DP for display(perameter provided as 4 because its 2 orders of magnitude higher by the end of the calculation, because we want a percentage)
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

                this.averageLeftPostSwing = (float)Math.Round(value, 4) * 100; //As a Percentage.
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
                return averageRightCut[1];                                           // Actual or default Value Returned
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
        private static string TimeCalculator(int MyValue)
        {
            int MinuitesTime = 0;
            while (MyValue >= 60)
            {
                MinuitesTime++;
                MyValue = MyValue - 60;
            };

            return $"  {MinuitesTime}m {MyValue}s ";
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
            
            //Plugin.Log.Info($"");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Beat Savior Data\"; //Set the CD


            #region Read Our Play into WorkingPlay

            StreamReader reader = File.OpenText(myPlayPath);
            int x = 1; //Line number as int32 not index
            string line;
            Play WorkingPlay = new Play(null, null, 0f, null, null, null, null); // Make WorkingPlay non local variable to the While Loop
            while ((line = reader.ReadLine()) != null)
            {
                if (x != myPlayLine)    // if its not the selected Play, move to the next line until it is. 
                {
                    x++;  
                }
                else                    // When it is, deserialise the line as WorkingPlay, and icrement the line so it hoiks to the end of the file.
                {

                    WorkingPlay = JsonConvert.DeserializeObject<Play>(line); //WorkingPlay is Currently Equal to the Deserialisation(as Play class instance) of the Currently Selected Line.
                    WorkingPlay.playPath = myPlayPath;
                    WorkingPlay.playLine = myPlayLine;

                    Plugin.Log.Info($"{x}> Processing Play {WorkingPlay.songName} at path: {WorkingPlay.playPath}");

                    #region Clean Data

                    #region Song Duration Formatter (121 = 2m 1s)

                    int temp = Convert.ToInt32((WorkingPlay.songDuration +1) / 1 ) ; // floor divide it as int to get a whole int. Round Up. to be inclusive

                    WorkingPlay.songDurationFormatted = TimeCalculator(MyValue: temp);

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
                    //No song Difficulty will ever start with a number. Songdiff possibilities: [easy,normal,hard,expert,expertplus].

                    #endregion

                    #endregion Clean Data
                    
                    x++;                                //Next line Num

                    
                }
                
            }
            reader.Close();                             // CLOSE READER

            #endregion


            Plugin.Log.Info($"WorkingPlay:{WorkingPlay.songName}, by {WorkingPlay.songArtist}, mapped by {WorkingPlay.songMapper} ");
            Plugin.Log.Info($"{WorkingPlay.songDifficulty}");
            Plugin.Log.Info($"{WorkingPlay.songDurationFormatted}");



            
            

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

            #endregion


            #endregion

            #region Assign UIValues

            #region Assign Tab 1 UIValues

            SongName = WorkingPlay.songName;
            SongDifficulty = WorkingPlay.songDifficulty;
            SongLength = WorkingPlay.songDurationFormatted;
            SongDuration = WorkingPlay.songDuration;
            Delimiter = $"  -  ";

            #region Validity Check

            if (HitTracker.maxCombo == 0) //in a valid score, the max combo will always be greater than 1, unless all notes are missed. (if all notes are missed, this mod is not for you.)
            { 
                PlayValidity = "Invalid! BSD Does Not Support Modmaps, or Desktop Scores.";
                ValidityFontColor = "#ff0000";
            }
            else 
            { 
                PlayValidity = "Valid!";
                ValidityFontColor = "#00ff00";
            }
            #endregion Validity Check


            #endregion

            #region Assign Tab 2 UIValues


            AverageLeftPreSwing = AccuracyTracker.leftPreswing;
            AverageLeftPostSwing = AccuracyTracker.leftPostswing;
            AverageRightPreSwing = AccuracyTracker.rightPreswing;
            AverageRightPostSwing = AccuracyTracker.rightPostswing;

            #endregion

            #region Assign Tab 3 UIValues

            AverageLeftAccuracy = AccuracyTracker.leftAverageCut[1];
            AverageRightAccuracy = AccuracyTracker.rightAverageCut[1];

            #endregion Assign Tab 3 UIValues
            
            
            #endregion


            //ReadNoteTracker
            //   (
            //    WorkingPlay: WorkingPlay,
            //    HitTracker: HitTracker,
            //    AccuracyTracker: AccuracyTracker,
            //    ScoreTracker: ScoreTracker,
            //    WinTracker: WinTracker
            //    //DistanceTracker: DistanceTracker,
            //    //ScoreGraphTracker: ScoreGraphTracker
            //    );

        }

        #endregion OnDataTransferEvent


        #region Read Note Tracker

        public void ReadNoteTracker
                                        (
            Play WorkingPlay,
            hitTracker HitTracker,
            accuracyTracker AccuracyTracker,
            scoreTracker ScoreTracker,
            winTracker WinTracker,
            distanceTracker DistanceTracker
            //scoreGraphTracker ScoreGraphTracker
                                        )
        {




            //Deep trackers 
            object fileNoteTracker = WorkingPlay.deepTrackers["noteTracker"];                                   // get "notes" : <this>[{},{}]
            noteTracker Notes = JsonConvert.DeserializeObject<noteTracker>(fileNoteTracker.ToString());         // Deserialise

            ArrayList DictionaryNotesList = new ArrayList();                                                    //create a new array list
            int x = 0;                                                                                          //set the indexer
            foreach (object FileDictionaryNotes in (object[])Notes.notes)                                       //for every entry in our deserialised dictionary list,
            {

                notesDictionary NotesDictionary = JsonConvert.DeserializeObject<notesDictionary>(FileDictionaryNotes.ToString());
                // notes dictionary is equal to each dictionary in the list

                DictionaryNotesList.Add(NotesDictionary);
                // add it to the arraylist

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

                x++; 
            }

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


