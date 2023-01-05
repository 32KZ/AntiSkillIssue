using System;
using System.Collections.Generic;
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
        private int _StartSecondsTime;
        private int _EndSecondsTime;
        private int _StartMinuitesTime;
        private int _EndMinuitesTime;

        [UIAction("Click")]
        private void ButtonClicked()
        {
            Console.WriteLine("Button was clicked!");

        }

        [UIComponent("lower_threshold_slider")]
        private SliderSetting Lower_Threshold_Slider;

        [UIValue("lower_threshold_value")]
        private float Lower_Threshold_Value
        {
            get
            {
                return PluginConfig.Instance.lower_threshold;
            }
            set
            {
                PluginConfig.Instance.lower_threshold = value;
            }
        }
        [UIAction("set_lower_threshold")]
        private void Set_Lower_Threshold(float value)
        {
            Lower_Threshold_Value = value;
            NotifyPropertyChanged();
        }

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
