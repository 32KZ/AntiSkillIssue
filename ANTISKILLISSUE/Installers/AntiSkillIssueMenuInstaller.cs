using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiraUtil;
using SiraUtil.Zenject;
using Zenject;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using AntiSkillIssue.ANTISKILLISSUE.Configuration;
using AntiSkillIssue.ANTISKILLISSUE.Installers;
using AntiSkillIssue.ANTISKILLISSUE.UI.FlowCoordinators;
using AntiSkillIssue.ANTISKILLISSUE.UI.ViewControllers;


namespace AntiSkillIssue.ANTISKILLISSUE.Installers //sets up the FC/VC so we can use UI's 
{
	internal class AntiSkillIssueMenuInstaller : Installer
	{
		private readonly Configuration.PluginConfig _config;

		public AntiSkillIssueMenuInstaller(Configuration.PluginConfig config)
		{
			_config = config;
		}

		public override void InstallBindings()
		{
			Container.BindInstance(_config);
			Container.Bind<AntiSkillIssueFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
			Container.Bind<AntiSkillIssueViewController>().FromNewComponentAsViewController().AsSingle();

		}
	}
}