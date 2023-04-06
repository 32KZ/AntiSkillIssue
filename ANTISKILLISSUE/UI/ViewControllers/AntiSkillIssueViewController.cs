using SiraUtil;
using SiraUtil.Zenject;
using HarmonyLib;
using HMUI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using AntiSkillIssue.ANTISKILLISSUE;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;

namespace AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers
{

    #region Data Transfer Event Parse requirements

    public delegate void DataTransferEventHandler(object sender, DataTransferEventArgs EventArguments);
    public class DataTransferEventArgs : EventArgs
    {
        public string Path { get; set; }
        public int Line { get; set; }
        public string Name { get; set; }

        //here we set 3 different properties for the Event.
        // first, the path. this is used in the Left View controller to View all other informaton for the play. this is the most important one.
        // second, we have the line the play is stored on. this is just as important as the path, so that we get the right score in the file. 
        // third, is the play name. 
    }

    #endregion 
    internal class AntiSkillIssueViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
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

        public event DataTransferEventHandler DataTransfer;
        public AntiSkillIssueLeftViewController BrotherViewController { get; set; }


        #region Reload Buttons Components
        //Reload Buttons
        [UIComponent("session-reload-button")]
        private ButtonTag SessionReloadButton;
        //-=-=
        #endregion


        #region List Components
        //UI lists
        [UIComponent("session-list")]
        public CustomCellListTableData sessionList;

        [UIComponent("plays-list")]
        public CustomCellListTableData playsList;
        //-=-=
        #endregion


        #region List Contents UIValue
        //Contents of the lists
        [UIValue("sessions")]
        public List<object> Sessions = new List<object>();

        [UIValue("plays")]
        public List<object> Plays = new List<object>();
        //-=-=
        #endregion


        #region Selected List Items

        //to be ran when Session clicked on.
        [UIAction("session-selected")]
        public void SessionSelected(TableView sessionList, Session Sessions )
        {
            SetPlays(SessionPath: Sessions.MyPath, Override:null);
            Plugin.Log.Info("Session Selected!");
            this.sessionList.tableView.ClearSelection();
        }

        //to be ran when Play clicked on.
        [UIAction("play-selected")]
        public void PlaySelected(TableView playList, Play Plays)
        {
            if (Plays.playPath != null) //the Dummy cell has Null as path, so we should ignore it. in all cases, we still need to clear selection.
            {
                SetPlaysData(PlayPath: Plays.playPath, PlayLine: Plays.playLine, PlayName: Plays.songName);
            }
            else 
            {
                Plugin.Log.Info("Why are you clicking the Dummy cell??? That does Nothing!!");
            }
            this.playsList.tableView.ClearSelection();
        }
        #endregion


        #region SetSessions
        public void SetSessions() //called from Sessions-Reload-Button and ASIFlowCoordinator
        {
            this.Sessions.Clear();
            this.Plays.Clear();
            SetPlays(SessionPath: null, Override: "!null, Override!"); //Enqueue a dummy to make it clear of the UI purpose


            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Beat Savior Data"; //Set the CD
            string[] fileNames = Directory.GetFiles(path);                              //get the directories of all the files in CD

            foreach (string fileName in fileNames)                                   // for every file in dir, as fileName,

            {
                
                string OurSessionName = Path.GetFileName(fileName.Replace(".bsd", "")); //  get its actual file name,
                string OurSessionPath = fileName;                                       //  path,
                                                                                        //  
                                                                                        //
                FileInfo OurFileInfo = new FileInfo(fileName);                          //
                string OurSessionSize = $"{OurFileInfo.Length}";                        //  and Size.

                Session Session = new Session   (                                       //  Assign Class Instance its Values
                                       newPath: OurSessionPath,
                                       newSessionName: OurSessionName,
                                       newSessionSize: OurSessionSize + "b"
                                                );
                if (Session.SessionName.Length == 10)
                {
                    this.Sessions.Add(Session);

                    Plugin.Log.Info("New Session " + $"{Session.SessionName}" + " Cell's path: " + $"{Session.MyPath}");
                }
                else { Plugin.Log.Info("The File " + $"{Session.SessionName}" + " Is not 00-00-0000 Format. Excluded.");}


                #region Summary~

                //then, for every session, add it to the first list, with its Name, Size, and Dir. 
                //only Display the Name And Size, the DIR is just A property. 
                //then, once it is selected in the list, highlight it, and:

                //  clear the right list,
                //  pass the Dir to a stream reader,
                //  ignore the first line(for now),
                //  In the right list, create a new cell for every line in the file. (with some data for readability.)
                //          Data for this collection: SongName, SongLength, Difficulty, SongMapper.
                //          Clean all the data
                //now, the Plays should reference a Line in each file.
                //once a Playcell is selected,  highlight it, And:
                //Send the File Directory to ASILVC.CS
                //  call a cleanup function in ASILVC.CS,
                //ANALYSE DATA.

                #endregion Summary~
            }
            sessionList?.tableView.ReloadData();
        }
        #endregion


        #region SetPlays
        public void SetPlays(string SessionPath , string Override) //called from ASIFlowCoordinator / SessionList.
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Beat Savior Data"; //Set the CD

            this.Plays.Clear();

            if (Override != null)
            {
                
                Play OverridePlay = new Play(   newSongName: " Each BSD Session",
                                                newPlayPath: null,
                                                newSongDuration:"3m 32s",
                                                newSongArtist:" Plays In",
                                                newSongMapper:" Go here!",
                                                newDelimiter:" "); //use of delimiter allows me to write a dummy cell.

                this.Plays.Add(OverridePlay);
                Override = null;
            }
            else
            {

                this.Plays.Clear();
                StreamReader reader = File.OpenText(SessionPath);
                int x = 1;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (x == 1) //Line number as int, not index.
                    {
                        x++;
                    }
                    else
                    {
                        Play play = JsonConvert.DeserializeObject<Play>(line);
                        play.playPath = SessionPath;
                        play.playLine = x;
                        Plugin.Log.Info("New Play " + $"{play.songName}" + " Cell's path: " + $"{play.playPath}");

                        #region Clean Data

                        #region Song Duration
                        float temp = float.Parse(play.songDuration) * 1000;
                        int temp2 = Convert.ToInt32(temp) / 1000;
                        play.songDurationFormatted = TimeCalculator(MyValue: temp2);
                        #endregion

                        #region Song Name (Length Limit)
                        if (play.songName.Length >= 14)
                        {
                            play.songName = play.songName.Substring(0, 13) + "...";
                        }
                        #endregion

                        #region Song Artist (Length Limit)
                        if (play.songArtist.Length >= 11)
                        {
                            play.songArtist = play.songArtist.Substring(0, 10) + "...";
                        }
                        #endregion

                        #region Song Mapper (Length Limit)
                        if (play.songMapper.Length >= 14)
                        {
                            play.songMapper = play.songMapper.Substring(0, 13) + "...";
                        }
                        #endregion

                        #region Song Difficulty (Capitalise)
                        if (Char.IsLower(play.songDifficulty[0]))
                        {
                            play.songDifficulty = Char.ToUpper(play.songDifficulty[0]) + play.songDifficulty.Substring(1);
                        }
                        //if letter index 1 of our song difficulty is not a capital letter, make it one.
                        //No song Difficulty will ever start with a number.
                        #endregion

                        #region Delimiter active.

                        play.delimiter= " - ";

                        #endregion

                        #endregion Clean Data

                        this.Plays.Add(play);
                        playsList?.tableView.ReloadData();  //reload the custom list
                        x++;                                //Next line Num


                    }

                }

                reader.Close();                             // CLOSE READER
                
            }

            playsList?.tableView.ReloadData();              //reload the custom list
        }
        #endregion

        
        #region SetPlaysData and Call the Data TransferEvent!
        public void SetPlaysData(string PlayPath, int PlayLine, string PlayName) 
        {

            DataTransfer?.Invoke(this, new DataTransferEventArgs { Path = PlayPath, Line = PlayLine, Name=PlayName });

            Plugin.Log.Info($"Event called with {PlayName}"+" In Session "+$"{PlayPath}"+ " , As Line " + $"{PlayLine}"+".");
           
        }
        #endregion

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

        #region Session and Play Classes

        #region Session and Play Classes
        public class Session
        {
            public string MyPath;

            [UIValue("session-name")]
            public string SessionName ="0";

            [UIValue("session-size")]
            public string SessionSize ="0";


            public Session(string newPath, string newSessionName, string newSessionSize)
            {
                MyPath = newPath;
                SessionName = newSessionName;
                SessionSize = newSessionSize;
            
            }


        }

        public class Play
        {
            public string playPath { get; set; }
            public int playLine { get; set; }

            [UIValue("play-name")]
            public string songName { get; set; }

            [UIValue("play-duration")]
            public string songDurationFormatted { get; set; }
            public string songDuration { get; set; }

            [UIValue("song-difficulty")]
            public string songDifficulty { get; set; }

            [UIValue("song-artist")]
            public string songArtist { get; set; }

            [UIValue("song-mapper")]
            public string songMapper { get; set; }

            [UIValue("delimiter")]
            public string delimiter { get; set; }
            public Dictionary<object,object> trackers { get; set; } 

            public Play(string newPlayPath, string newSongName, string newSongDuration, string newSongArtist, string newSongMapper, string newDelimiter)
            {
                playPath = newPlayPath;
                songName = newSongName;
                songDuration = newSongDuration;
                songArtist = newSongArtist;
                songMapper = newSongMapper;
                delimiter = newDelimiter;


            }



        }


        #endregion Session and Play Classes

        #endregion Session and Play Classes

    }
}


