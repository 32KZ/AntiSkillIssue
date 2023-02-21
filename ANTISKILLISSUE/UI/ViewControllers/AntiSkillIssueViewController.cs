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
    internal class AntiSkillIssueViewController : BSMLResourceViewController //no way! its a legendary view controller! super rare!
    {
        public override string ResourceName => string.Join(".", GetType().Namespace, GetType().Name) + ".bsml";

        #region Reload Buttons Components
        //Reload Buttons
        [UIComponent("session-reload-button")]
        private ButtonTag SessionReloadButton;

        [UIComponent("plays-reload-button")]
        private ButtonTag PlaysReloadButton;
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
        public void SessionSelected(TableView sessionList,Session Sessions )
        {
            SetPlays(SessionPath: Sessions.MyPath, Override:null);
            Plugin.Log.Info("Session Selected!");
        }

        //to be ran when Play clicked on.
        [UIAction("play-selected")]
        public void PlaySelected(TableView playList, Play Plays)
        {
            SetPlaysData(PlayPath: Plays.playPath, PlayLine:Plays.playLine, PlayName:Plays.songName);
        }
        #endregion


        
        #region SetSessions
        public void SetSessions() //called from Sessions-Reload-Button
        {
            this.Sessions.Clear();

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
                //  call a cleanup function in ASILVC.CS,
                //  clear the right list,
                //  pass the Dir to a stream reader,
                //  ignore the first line(for now),
                //  In the right list, create a new cell for every line in the file. (with some data for readability.)
                //          Data for this collection: SongName, SongLength, PercentileScore, Score.
                //now, the PlayCells should reference a Line in each file.
                //once a cell is selected,  highlight it, And:
                //Send the File Directory to ASILVC.CS
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
                
                Play OverridePlay = new Play(   newSongName: "Each BSD Session",
                                                newPlayPath:path,
                                                newSongDuration:"3m 32s",
                                                newSongArtist:"Plays In",
                                                newSongMapper:"Go here!",
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
                    if (x == 1) //Line number as int32, not index.
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

                        //Song Duration
                        float temp = float.Parse(play.songDuration) * 1000;
                        int temp2 = Convert.ToInt32(temp) / 1000;
                        play.songDurationFormatted = TimeCalculator(MyValue: temp2);

                        //Song Name (Length Limit)
                        if (play.songName.Length >= 14)
                        {
                            play.songName = play.songName.Substring(0, 13) + "...";
                        }
                        //Song Artist (Length Limit)
                        if (play.songArtist.Length >= 11)
                        {
                            play.songArtist = play.songArtist.Substring(0, 10) + "...";
                        }
                        //Song Mapper (length Limit)
                        if (play.songMapper.Length >= 14)
                        {
                            play.songMapper = play.songMapper.Substring(0, 13) + "...";
                        }
                        //Song Difficulty (Capitalise)


                        //Delimiter active.
                        play.delimiter= " - ";


                        #endregion Clean Data

                        this.Plays.Add(play);
                        playsList?.tableView.ReloadData(); //reload the custom list
                        x++; //Next line Num


                    }

                }

                reader.Close();                         // CLOSE READER
                
            }

            playsList?.tableView.ReloadData();      //reload the custom list
        }
        #endregion

        private void SetPlaysData(string PlayPath, int PlayLine, string PlayName) 
        {
            Plugin.Log.Info($"{PlayName}"+" In Session "+$"{PlayPath}"+ " , As Line " + $"{PlayLine}"+".");
            AntiSkillIssueLeftViewController.CurrentPlayName = PlayName;
        }

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



        #region Cell / Session and Play Classes

        #region Depreciated Session/PlayCells
        //internal class SessionCell
        //{
        //    [UIValue("session-name")]
        //    private string sessionName = "0";
        //
        //    [UIValue("session-details")]
        //    private string sessionDetails = "1";
        //
        //    public SessionCell(string sessionName, string sessionDetails)
        //    {
        //        this.sessionName = sessionName;
        //        this.sessionDetails = sessionDetails;
        //    }
        //
        //}




        //internal class PlayCell
        //{
        //    [UIValue("play-name")]
        //    private string playName = "0";
        //
        //    [UIValue("play-details")]
        //    private string playDetails = "1";
        //
        //    public PlayCell(string playName, string playDetails)
        //    {
        //        this.playName = playName;
        //        this.playDetails = playDetails;
        //    }
        //
        //}


        #endregion Depreciated Session/PlayCells

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



            //public
            //    Dictionary<string, (
            //       Dictionary<string, int>,                                                //Hit Tracker
            //        Dictionary<string, (float, float[])>,                                   //Accuracy tracker
            //        Dictionary<string, (int, float, string[])>,                             //Score Tracker
            //        Dictionary<string, (bool, string, float, int)>,                         //Win Tracker
            //        Dictionary<string, float>,                                              //Distance Tracker
            //        Dictionary<string, Dictionary<string, float>>,                          //ScoreGraph Tracker
            //        Dictionary<string, (int, float, int, int[], float, float[], float)>     //Note tracker
            //        )> trackers
            //{ get; set; } //i think its ABSOLUTELY HILARIOUS that i have to comment
            //A Fucking Type.
            //TRIPLE NESTED DICTIONARIES. WHAT. THE. FUCK.

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

        #endregion  Cell / Session and Play Classes

    }
}


