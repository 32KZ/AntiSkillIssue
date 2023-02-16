using SiraUtil;
using SiraUtil.Zenject;
using HarmonyLib;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        #region INIT
        [Init]
        private void Init()
        {
            SetSessions();
            SetPlays();
            SessionCell UniversalCell1 = new SessionCell(sessionName: "03-03-0003", sessionDetails: "3kb");
            PlayCell UniversalCell2 = new PlayCell(playName: "03-03-0003", playDetails: "3kb");
            Sessions.Add(UniversalCell1);
            Plays.Add(UniversalCell2);
            sessionList?.tableView.ReloadData();
            playsList?.tableView.ReloadData();

        }
        #endregion


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
        public void SessionSelected()
        {
            Plugin.Log.Info("Session Selected!");
        }

        //to be ran when Play clicked on.
        [UIAction("play-selected")]
        public void PlaySelected()
        {
            Plugin.Log.Info("Play Selected!");
        }
        #endregion


        #region SetSessions
        public void SetSessions() //called from Sessions-Reload-Button
        {

            if (Sessions != null)
            {
                SessionCell SessionsCell1 = new SessionCell(sessionName: "01-01-0001", sessionDetails: "2KB");
                this.Sessions.Add(SessionsCell1);
                sessionList?.tableView.ReloadData(); //reload the custom list

                
            }
            else
            {
                this.Sessions.Clear();
                SessionCell SessionsCell1 = new SessionCell(sessionName: "00-00-0000", sessionDetails: "1KB");
                this.Sessions.Add(SessionsCell1);

                sessionList?.tableView.ReloadData(); //reload the custom list
            }

            SessionCell testCell = new SessionCell(sessionName: "0x-0x-000x", sessionDetails: "xKB");
            this.Sessions.Add(testCell);
            sessionList?.tableView.ReloadData(); //reload the custom list


        }
        #endregion


        #region SetPlays
        public void SetPlays() //called from Plays-Reload-Button
        {

            if (Plays != null)
            {
                PlayCell PlaysCell1 = new PlayCell(playName: "01-01-0001", playDetails: "2KB");
                this.Plays.Add(PlaysCell1);
                playsList?.tableView.ReloadData(); //reload the custom list


            }
            else
            {
                this.Plays.Clear();
                PlayCell PlaysCell1 = new PlayCell(playName: "00-00-0000", playDetails: "1KB");
                this.Plays.Add(PlaysCell1);

                playsList?.tableView.ReloadData(); //reload the custom list
            }

            PlayCell BonusCell = new PlayCell(playName: "0x-0x-000x", playDetails: "xKB");
            this.Plays.Add(BonusCell);
            playsList?.tableView.ReloadData(); //reload the custom list


        }
#endregion
    }
    #region Cell Classes
    internal class SessionCell
    {
        [UIValue("session-name")]
        private string sessionName = "0"; 

        [UIValue("session-details")]
        private string sessionDetails = "1";

        public SessionCell(string sessionName, string sessionDetails)
        {
            this.sessionName = sessionName;
            this.sessionDetails = sessionDetails;
        }

    }

    internal class PlayCell
    {
        [UIValue("play-name")]
        private string playName = "0";

        [UIValue("play-details")]
        private string playDetails = "1";

        public PlayCell(string playName, string playDetails)
        {
            this.playName = playName;
            this.playDetails = playDetails;
        }

    }
    #endregion
}
