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

        public void Init()
        {
            SetSessions();
            cell thirdCell = new cell(sessionName: "03-03-0003", sessionDetails: "3kb");
            sessions.Add(thirdCell);
            sessionList?.tableView.ReloadData();

        }

        //Reload Button
        [UIComponent("reload-button")]
        private ButtonTag ReloadButton;

        //Session list itself
        [UIComponent("session-list")]
        public CustomCellListTableData sessionList;

        //Content of the list
        [UIValue("sessions")]
        public List<object> sessions = new List<object>();


        //to be ran when session clicked on.
        [UIAction("session-selected")]
        public void SessionSelected()
        {
            AntiSkillIssue.Plugin.Log.Info("Session Selected!");
        }


        public void SetSessions() //called from reload button
        {

            if (sessions != null)
            {
                cell SecondCell = new cell(sessionName: "01-01-0001", sessionDetails: "2KB");
                this.sessions.Add(SecondCell);
                sessionList?.tableView.ReloadData(); //reload the custom list

                
            }
            else
            {
                this.sessions.Clear();
                cell FirstCell = new cell(sessionName: "00-00-0000", sessionDetails: "1KB");
                this.sessions.Add(FirstCell);

                sessionList?.tableView.ReloadData(); //reload the custom list
            }

            cell BonusCell = new cell(sessionName: "0x-0x-000x", sessionDetails: "xKB");
            this.sessions.Add(BonusCell);
            sessionList?.tableView.ReloadData(); //reload the custom list


        }
    }

    internal class cell
    {
        [UIValue("session-name")]
        private string sessionName = "0"; 

        [UIValue("session-details")]
        private string sessionDetails = "1";

        public cell(string sessionName, string sessionDetails)
        {
            this.sessionName = sessionName;
            this.sessionDetails = sessionDetails;
        }
    }

}
