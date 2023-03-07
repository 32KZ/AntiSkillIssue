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
        public void SetSessions() //called from Sessions-Reload-Button and ASIFlowCoordinator
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

        public void SetPlaysData(string PlayPath, int PlayLine, string PlayName) 
        {
            AntiSkillIssueLeftViewController ASILVC = new AntiSkillIssueLeftViewController(newPlayPath: PlayPath, newPlayLine: PlayLine, newPlayName: PlayName);
            Plugin.Log.Info($"{PlayName}"+" In Session "+$"{PlayPath}"+ " , As Line " + $"{PlayLine}"+".");
            ASILVC.ImportPlayData(newPlayPath: PlayPath, newPlayLine: PlayLine, newPlayName: PlayName);
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

        #endregion  Cell / Session and Play Classes

    }
    #region Test Data line 1
    //{
    //"songDataType":1,
    //"playerID":"76561198227979496",
    //"songID":"018ACAAD1EC1E13BD9E428A19E41C844AA05B9DA",
    //"songDifficulty":"expert",
    //"songName":"Chocolate Lily",
    //"songArtist":"Kobaryo",
    //"songMapper":"xScaramouche",
    //"gameMode":"Standard",
    //"songDifficultyRank":7,
    //"songSpeed":1.0,
    //"songStartTime":0.0,
    //"songDuration":135.485,
    //"songJumpDistance":17.06,
    //"trackers":
    //  {
    //  "hitTracker":
    //      {
    //      "leftNoteHit":580,
    //      "rightNoteHit":587,
    //      "bombHit":0,
    //      "maxCombo":1167,
    //      "nbOfWallHit":0,
    //      "miss":3,
    //      "missedNotes":3,
    //      "badCuts":0,
    //      "leftMiss":3,
    //      "leftBadCuts":0,
    //      "rightMiss":0,
    //      "rightBadCuts":0
    //      },
    //  "accuracyTracker":
    //      {
    //      "accRight":110.298126,
    //      "accLeft":109.974136,
    //      "averageAcc":110.1371,
    //      "leftSpeed":45.9280434,
    //      "rightSpeed":40.9439049,
    //      "averageSpeed":43.4210243,
    //      "leftHighestSpeed":72.66841,
    //      "rightHighestSpeed":63.3658638,
    //      "leftPreswing":1.46196735,
    //      "rightPreswing":1.33601224,
    //      "averagePreswing":1.398612,
    //      "leftPostswing":0.8244153,
    //      "rightPostswing":0.850048542,
    //      "averagePostswing":0.8373088,
    //      "leftTimeDependence":0.1879496,
    //      "rightTimeDependence":0.169607624,
    //      "averageTimeDependence":0.178723589,
    //      "leftAverageCut":
    //          [
    //          69.85862,
    //          10.1206894,
    //          29.9948273
    //          ],
    //      "rightAverageCut":
    //          [
    //          69.68825,
    //          10.6269169,
    //          29.9829636
    //          ],
    //      "averageCut":
    //          [
    //          69.77293,
    //          10.3753214,
    //          29.98886
    //          ],
    //      "gridAcc":
    //          [
    //          110.0,
    //          110.990356,
    //          111.455414,
    //          108.269234,
    //          109.384613,
    //          106.25,
    //          "NaN",
    //          109.630432,
    //          107.952377,
    //          106.534882,
    //          105.410255,
    //          110.954544
    //          ],
    //      "gridCut":
    //          [
    //          17,
    //          311,
    //          314,
    //          26,
    //          182,
    //          8,
    //          0,
    //          184,
    //          21,
    //          43,
    //          39,
    //          22]
    //      },
    //  "scoreTracker":
    //      {
    //      "rawScore":1012007,
    //      "score":1012007,
    //      "personalBest":965585,
    //      "rawRatio":0.946548462,
    //      "modifiedRatio":0.946548462,
    //      "personalBestRawRatio":0.9031291,
    //      "personalBestModifiedRatio":0.9031291,
    //      "modifiersMultiplier":1.0,
    //      "modifiers":
    //          [
    //          ]
    //      },
    //  "winTracker":
    //      {
    //      "won":true,
    //      "rank":"SS",
    //      "endTime":135.295929,
    //      "nbOfPause":0
    //      },
    //  "distanceTracker":
    //      {
    //      "rightSaber":2878.82,
    //      "leftSaber":3176.5896,
    //      "rightHand":590.2895,
    //      "leftHand":677.2007
    //      },
    //  "scoreGraphTracker":
    //      {
    //      "graph":
    //          {
    //          "3":0.9826087,
    //          "4":0.9739131,
    //          "5":0.97066313,
    //          "6":0.968520045,
    //          "7":0.968677938,
    //          "8":0.9659691,
    //          "9":0.966931939,
    //          "10":0.9682463,
    //          "11":0.969872534,
    //          "12":0.9692277,
    //          "13":0.9684842,
    //          "14":0.967521369,
    //          "15":0.9660909,
    //          "16":0.965033352,
    //          "17":0.964547157,
    //          "18":0.964106858,
    //          "19":0.9635422,
    //          "20":0.963254333,
    //          "21":0.9628859,
    //          "22":0.9627468,
    //          "23":0.962734938,
    //          "24":0.9624457,
    //          "25":0.9623653,
    //          "26":0.9619398,
    //          "27":0.96152705,
    //          "28":0.96109277,
    //          "29":0.961150646,
    //          "30":0.961059332,
    //          "34":0.9609527,
    //          "35":0.960922062,
    //          "36":0.9608026,
    //          "37":0.960633636,
    //          "38":0.960508,
    //          "39":0.960498,
    //          "40":0.9602566,
    //          "41":0.9601165,
    //          "42":0.959866762,
    //          "43":0.959491134,
    //          "44":0.959058166,
    //          "45":0.9587112,
    //          "46":0.9582557,
    //          "47":0.9578895,
    //          "48":0.957544446,
    //          "49":0.957335,
    //          "50":0.957118869,
    //          "51":0.9569558,
    //          "52":0.956814647,
    //          "53":0.9567856,
    //          "54":0.9567547,
    //          "55":0.95684886,
    //          "56":0.956975162,
    //          "57":0.9571193,
    //          "58":0.957166851,
    //          "59":0.9572661,
    //          "66":0.9573279,
    //          "67":0.957445145,
    //          "68":0.9576164,
    //          "69":0.9577514,
    //          "70":0.957816541,
    //          "71":0.9578906,
    //          "72":0.957946658,
    //          "73":0.957953155,
    //          "74":0.9580733,
    //          "75":0.9582655,
    //          "76":0.9584758,
    //          "77":0.958653,
    //          "78":0.9587714,
    //          "79":0.958857656,
    //          "80":0.9588817,
    //          "81":0.9588945,
    //          "82":0.958938956,
    //          "83":0.9590274,
    //          "84":0.9591331,
    //          "85":0.9591937,
    //          "86":0.9592206,
    //          "87":0.9591829,
    //          "88":0.9590917,
    //          "89":0.959001958,
    //          "90":0.95896703,
    //          "91":0.958953142,
    //          "92":0.9589669,
    //          "93":0.959032357,
    //          "97":0.9591122,
    //          "98":0.9591881,
    //          "99":0.9592532,
    //          "100":0.9593831,
    //          "101":0.959501266,
    //          "102":0.959590137,
    //          "103":0.9597177,
    //          "104":0.9597921,
    //          "105":0.959764957,
    //          "106":0.9596774,
    //          "107":0.959526062,
    //          "108":0.9593062,
    //          "109":0.959155262,
    //          "110":0.959054768,
    //          "111":0.9589069,
    //          "112":0.958841264,
    //          "113":0.958770454,
    //          "114":0.9586694,
    //          "115":0.95854646,
    //          "116":0.9585137,
    //          "117":0.9575754,
    //          "118":0.9566659,
    //          "119":0.9554658,
    //          "120":0.953734636,
    //          "121":0.951179862,
    //          "122":0.9495512,
    //          "123":0.947901249,
    //          "124":0.9465926,
    //          "125":0.945822954,
    //          "126":0.9459358,
    //          "127":0.9459996,
    //          "128":0.9460944,
    //          "129":0.9461507,
    //          "130":0.946259856,
    //          "131":0.946343064
    //          }
    //      }
    //  },
    //"deepTrackers":
    //  {
    //  "noteTracker":
    //      {
    //      "notes":
    //          [
    //              {
    //              "noteType":0,
    //              "noteDirection":1,
    //              "index":2,"id":0,
    //              "time":3.76978731,
    //              "cutType":0,
    //              "multiplier":1,
    //              "score":
    //                  [
    //                  70,
    //                  13,
    //                  30
    //                  ],
    //              "noteCenter":
    //                  [
    //                  0.3,
    //                  0.791297853,
    //                  3.00528455
    //                  ],
    //              "noteRotation":
    //                  [
    //                  6.61882639,
    //                  9.463408,
    //                  359.5056
    //                  ],
    //              "timeDeviation":0.028942585,
    //              "speed":31.5990047,
    //              "preswing":1.50371718,
    //              "postswing":0.984967649,
    //              "distanceToCenter":0.0309337564,
    //              "cutPoint":
    //                  [
    //                  0.330787838,
    //                  0.7890785,
    //                  3.00326419
    //                  ],
    //              "saberDir":
    //                  [
    //                  -0.06056553,
    //                  -0.9934134,
    //                  -0.097271
    //                  ],
    //              "cutNormal":
    //                  [
    //                  -0.995282233,
    //                  0.07174681,
    //                  0.06531261
    //                  ],
    //              "timeDependence":0.06531261
    //              },
    //              { 
    //
    //                //Next Note and so on. im not going to do this for every note. that would be 10,000 lines long.
    //                // there is 1.17k notes in this BSD file. https://beatsaver.com/maps/86af on ex.
    //                // And to make it feasable to code here, im going to omit most of the following data.
    //
    //              "noteType":1, "noteDirection":1,"index":1,"id":1,"time":3.8761704,"cutType":0,"multiplier":2,...
    //              }
    //          ]
    //      }
    //  }
    //}

    #endregion
}


