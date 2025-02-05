using Newtonsoft.Json;
using System.Text.Json;

using FWO.Basics;
using FWO.Api.Data;
using FWO.Mail;

namespace FWO.Config.Api.Data
{
	[AttributeUsage(AttributeTargets.Property)]
	public class UserConfigDataAttribute : Attribute { }

	public class ConfigData : ICloneable
	{
		public readonly bool Editable;

		[JsonProperty("DefaultLanguage")]
		public virtual string DefaultLanguage { get; set; } = GlobalConst.kEnglish;

		[JsonProperty("sessionTimeout")]
		public int SessionTimeout { get; set; } = 720;

		[JsonProperty("sessionTimeoutNoticePeriod")]
		public int SessionTimeoutNoticePeriod { get; set; } = 60;

		[JsonProperty("uiHostName")]
		public string UiHostName { get; set; } = "http://localhost:5000";

		[JsonProperty("welcomeMessage")]
		public string WelcomeMessage { get; set; } = "";

		[JsonProperty("maxMessages"), UserConfigData]
		public int MaxMessages { get; set; } = 3;

		[JsonProperty("elementsPerFetch"), UserConfigData]
		public int ElementsPerFetch { get; set; } = 100;

		[JsonProperty("maxInitialFetchesRightSidebar")]
		public int MaxInitialFetchesRightSidebar { get; set; } = 10;

		[JsonProperty("autoFillRightSidebar")]
		public bool AutoFillRightSidebar { get; set; } = false;

		[JsonProperty("unusedTolerance")]
		public int UnusedTolerance { get; set; } = 400;

		[JsonProperty("creationTolerance")]
		public int CreationTolerance { get; set; } = 90;

		[JsonProperty("dataRetentionTime")]
		public int DataRetentionTime { get; set; } = 731;

		[JsonProperty("importSleepTime")]
		public int ImportSleepTime { get; set; } = 40;

		[JsonProperty("importCheckCertificates")]
		public bool ImportCheckCertificates { get; set; } = false;

		[JsonProperty("importSuppressCertificateWarnings")]
		public bool ImportSuppressCertificateWarnings { get; set; } = true;

		[JsonProperty("autoDiscoverSleepTime")]
		public int AutoDiscoverSleepTime { get; set; } = 24;

		[JsonProperty("autoDiscoverStartAt")]
		public DateTime AutoDiscoverStartAt { get; set; } = new();

		[JsonProperty("fwApiElementsPerFetch")]
		public int FwApiElementsPerFetch { get; set; } = 150;

		[JsonProperty("impChangeNotifyRecipients")]
		public string ImpChangeNotifyRecipients { get; set; } = "";

		[JsonProperty("impChangeNotifySubject")]
		public string ImpChangeNotifySubject { get; set; } = "";

		[JsonProperty("impChangeNotifyBody")]
		public string ImpChangeNotifyBody { get; set; } = "";

		[JsonProperty("impChangeNotifyActive")]
		public bool ImpChangeNotifyActive { get; set; } = false;

		[JsonProperty("impChangeNotifyType")]
		public int ImpChangeNotifyType { get; set; }

		[JsonProperty("impChangeNotifySleepTime")]
		public int ImpChangeNotifySleepTime { get; set; } = 60;

		[JsonProperty("impChangeNotifyStartAt")]
		public DateTime ImpChangeNotifyStartAt { get; set; } = new();

		[JsonProperty("externalRequestSleepTime")]
		public int ExternalRequestSleepTime { get; set; } = 60;

		[JsonProperty("externalRequestStartAt")]
		public DateTime ExternalRequestStartAt { get; set; } = new();


		[JsonProperty("recertificationPeriod")]
		public int RecertificationPeriod { get; set; } = 365;

		[JsonProperty("recertificationNoticePeriod")]
		public int RecertificationNoticePeriod { get; set; } = 30;

		[JsonProperty("recertificationDisplayPeriod")]
		public int RecertificationDisplayPeriod { get; set; } = 30;

		[JsonProperty("ruleRemovalGracePeriod")]
		public int RuleRemovalGracePeriod { get; set; } = 60;

		[JsonProperty("commentRequired")]
		public bool CommentRequired { get; set; } = false;

		[JsonProperty("recAutocreateDeleteTicket")]
		public bool RecAutoCreateDeleteTicket { get; set; } = false;

		[JsonProperty("recDeleteRuleTicketTitle")]
		public string RecDeleteRuleTicketTitle { get; set; } = "";

		[JsonProperty("recDeleteRuleTicketReason")]
		public string RecDeleteRuleTicketReason { get; set; } = "";

		[JsonProperty("recDeleteRuleReqTaskTitle")]
		public string RecDeleteRuleReqTaskTitle { get; set; } = "";

		[JsonProperty("recDeleteRuleReqTaskReason")]
		public string RecDeleteRuleReqTaskReason { get; set; } = "";

		[JsonProperty("recDeleteRuleTicketPriority")]
		public int RecDeleteRuleTicketPriority { get; set; } = 3;

		[JsonProperty("recDeleteRuleInitState")]
		public int RecDeleteRuleInitState { get; set; } = 0;

		[JsonProperty("recCheckActive")]
		public bool RecCheckActive { get; set; } = false;

		[JsonProperty("recCheckParams")]
		public string RecCheckParams { get; set; } = System.Text.Json.JsonSerializer.Serialize(new RecertCheckParams());

		[JsonProperty("recCheckEmailSubject")]
		public string RecCheckEmailSubject { get; set; } = "";

		[JsonProperty("recCheckEmailUpcomingText")]
		public string RecCheckEmailUpcomingText { get; set; } = "";

		[JsonProperty("recCheckEmailOverdueText")]
		public string RecCheckEmailOverdueText { get; set; } = "";

		[JsonProperty("recRefreshStartup")]
		public bool RecRefreshStartup { get; set; } = false;

		[JsonProperty("recRefreshDaily")]
		public bool RecRefreshDaily { get; set; } = false;

		[JsonProperty("pwMinLength")]
		public int PwMinLength { get; set; } = 10;

		[JsonProperty("pwUpperCaseRequired")]
		public bool PwUpperCaseRequired { get; set; } = false;

		[JsonProperty("pwLowerCaseRequired")]
		public bool PwLowerCaseRequired { get; set; } = false;

		[JsonProperty("pwNumberRequired")]
		public bool PwNumberRequired { get; set; } = false;

		[JsonProperty("pwSpecialCharactersRequired")]
		public bool PwSpecialCharactersRequired { get; set; } = false;

		[JsonProperty("emailServerAddress")]
		public string EmailServerAddress { get; set; } = "";

		[JsonProperty("emailPort")]
		public int EmailPort { get; set; }

		[JsonProperty("emailTls")]
		public EmailEncryptionMethod EmailTls { get; set; } = EmailEncryptionMethod.None;

		[JsonProperty("emailUser")]
		public string EmailUser { get; set; } = "";

		[JsonProperty("emailPassword")]
		public string EmailPassword { get; set; } = "";

		[JsonProperty("emailSenderAddress")]
		public string EmailSenderAddress { get; set; } = "";

		[JsonProperty("useDummyEmailAddress")]
		public bool UseDummyEmailAddress { get; set; } = false;

		[JsonProperty("dummyEmailAddress")]
		public string DummyEmailAddress { get; set; } = "";

		[JsonProperty("minCollapseAllDevices"), UserConfigData]
		public int MinCollapseAllDevices { get; set; } = 15;

		[JsonProperty("messageViewTime"), UserConfigData]
		public int MessageViewTime { get; set; } = 7;

		[JsonProperty("dailyCheckStartAt")]
		public DateTime DailyCheckStartAt { get; set; } = new();

		[JsonProperty("maxImportDuration")]
		public int MaxImportDuration { get; set; } = 4;

		[JsonProperty("maxImportInterval")]
		public int MaxImportInterval { get; set; } = 12;

		[JsonProperty("reqAvailableTaskTypes")]
		public string ReqAvailableTaskTypes { get; set; } = "";

		[JsonProperty("reqOwnerBased")]
		public bool ReqOwnerBased { get; set; } = false;

		[JsonProperty("reqReducedView")]
		public bool ReqReducedView { get; set; } = false;

		[JsonProperty("reqAllowObjectSearch")]
		public bool ReqAllowObjectSearch { get; set; } = false;

		[JsonProperty("reqAllowManualOwnerAdmin")]
		public bool AllowManualOwnerAdmin { get; set; } = false;

		[JsonProperty("reqPriorities")]
		public string ReqPriorities { get; set; } = "";

		[JsonProperty("reqAutoCreateImplTasks")]
		public AutoCreateImplTaskOptions ReqAutoCreateImplTasks { get; set; } = AutoCreateImplTaskOptions.never;

		[JsonProperty("reqActivatePathAnalysis")]
		public bool ReqActivatePathAnalysis { get; set; } = true;

		[JsonProperty("reqShowCompliance")]
		public bool ReqShowCompliance { get; set; } = false;

		[JsonProperty("ruleOwnershipMode")]
		public RuleOwnershipMode RuleOwnershipMode { get; set; } = RuleOwnershipMode.mixed;


		[JsonProperty("allowServerInConn")]
		public bool AllowServerInConn { get; set; } = true;

		[JsonProperty("allowServiceInConn")]
		public bool AllowServiceInConn { get; set; } = true;

		[JsonProperty("overviewDisplayLines")]
		public int OverviewDisplayLines { get; set; } = 3;

		[JsonProperty("reducedProtocolSet")]
		public bool ReducedProtocolSet { get; set; } = true;

        [JsonProperty("createApplicationZones")]
        public bool CreateAppZones { get; set; }

        [JsonProperty("importAppDataPath")]
		public string ImportAppDataPath { get; set; } = "";

		[JsonProperty("importAppDataSleepTime")]
		public int ImportAppDataSleepTime { get; set; } = 24;

		[JsonProperty("importAppDataStartAt")]
		public DateTime ImportAppDataStartAt { get; set; } = new DateTime();

		[JsonProperty("ownerLdapId")]
		public int OwnerLdapId { get; set; } = GlobalConst.kLdapInternalId;

		[JsonProperty("manageOwnerLdapGroups")]
		public bool ManageOwnerLdapGroups { get; set; } = true;

		[JsonProperty("ownerLdapGroupNames")]
		public string OwnerLdapGroupNames { get; set; } = "";
        
		[JsonProperty("importSubnetDataPath")]
		public string ImportSubnetDataPath { get; set; } = "";

		[JsonProperty("importSubnetDataSleepTime")]
		public int ImportSubnetDataSleepTime { get; set; } = 24;

		[JsonProperty("importSubnetDataStartAt")]
		public DateTime ImportSubnetDataStartAt { get; set; } = new DateTime();

		[JsonProperty("modNamingConvention")]
		public string ModNamingConvention { get; set; } = "";

		[JsonProperty("modIconify")]
		public bool ModIconify { get; set; } = true;

		[JsonProperty("modCommonAreas")]
		public string ModCommonAreas { get; set; } = "";

		[JsonProperty("modAppServerTypes")]
		public string ModAppServerTypes { get; set; } = "";

		[JsonProperty("modReqInterfaceName")]
		public string ModReqInterfaceName { get; set; } = "";

		[JsonProperty("modReqEmailReceiver")]
		public EmailRecipientOption ModReqEmailReceiver { get; set; } = EmailRecipientOption.FallbackToMainResponsibleIfOwnerGroupEmpty;

		[JsonProperty("modReqEmailRequesterInCc")]
		public bool ModReqEmailRequesterInCc { get; set; } = true;

		[JsonProperty("modReqEmailSubject")]
		public string ModReqEmailSubject { get; set; } = "";

		[JsonProperty("modReqEmailBody")]
		public string ModReqEmailBody { get; set; } = "";

		[JsonProperty("modReqTicketTitle")]
		public string ModReqTicketTitle { get; set; } = "";

		[JsonProperty("modReqTaskTitle")]
		public string ModReqTaskTitle { get; set; } = "";

		[JsonProperty("modRolloutActive")]
		public bool ModRolloutActive { get; set; } = true;

		[JsonProperty("modRolloutResolveServiceGroups")]
		public bool ModRolloutResolveServiceGroups { get; set; } = true;

		[JsonProperty("modRolloutBundleTasks")]
		public bool ModRolloutBundleTasks { get; set; } = false;

		[JsonProperty("modRolloutErrorText")]
		public string ModRolloutErrorText { get; set; } = "";

		[JsonProperty("externalRequestWaitCycles")]
		public int ExternalRequestWaitCycles { get; set; } = 0;

		[JsonProperty("extTicketSystems")]
		public string ExtTicketSystems { get; set; } = "";

		[JsonProperty("modExtraConfigs")]
		public string ModExtraConfigs { get; set; } = "";

		public ConfigData(bool editable = false)
		{
			Editable = editable;
		}

		public object Clone()
		{
			// Watch out for references they need to be deep cloned (currently none)
			ConfigData configData = (ConfigData)MemberwiseClone();
			return configData;
		}

		public object CloneEditable()
		{
			object clone = Clone();
			typeof(ConfigData).GetProperty("Editable")?.SetValue(clone, true);
			return clone;
		}
	}
}
