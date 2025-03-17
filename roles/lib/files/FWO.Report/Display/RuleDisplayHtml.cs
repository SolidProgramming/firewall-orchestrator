﻿using FWO.Basics;
using FWO.Data;
using FWO.Config.Api;
using System.Text;
using FWO.Report;
using FWO.Report.Filter;

namespace FWO.Ui.Display
{
    public class RuleDisplayHtml(UserConfig userConfig) : RuleDisplayBase(userConfig)
    {
        // public RuleDisplayHtml(UserConfig userConfig) : base(userConfig)
        // {}

        public string DisplaySource(Rule rule, OutputLocation location, ReportType reportType, int chapterNumber = 0, string style = "")
        {
            return DisplaySourceOrDestination(rule, chapterNumber, location, reportType, style, true);
        }

        public string DisplayDestination(Rule rule, OutputLocation location, ReportType reportType, int chapterNumber = 0, string style = "")
        {
            return DisplaySourceOrDestination(rule, chapterNumber, location, reportType, style, false);
        }

        public string DisplayServices(Rule rule, OutputLocation location, ReportType reportType, int chapterNumber = 0, string style = "")
        {
            StringBuilder result = new ();
            if (rule.ServiceNegated)
            {
                result.AppendLine(userConfig.GetText("negated") + "<br>");
            }

            if(reportType.IsResolvedReport())
            {
                NetworkService[] services = GetNetworkServices(rule.Services).ToArray();
                result.AppendJoin("<br>", Array.ConvertAll(services, service => ServiceToHtml(service, rule.MgmtId, chapterNumber, location, style, reportType)));
            }
            else
            {
                result.AppendJoin("<br>", Array.ConvertAll(rule.Services, service => ServiceToHtml(service.Content, rule.MgmtId, chapterNumber, location, style, reportType)));
            }
            return result.ToString();
        }

        public static string DisplaySectionHeader(Rule rule, int ColumnCount)
        {
            return $"<tr><td class=\"bg-gray\" colspan=\"{ColumnCount}\"><b>{rule.SectionHeader}</b></td></tr>";
        }

        public static string DisplayNextRecert(RuleMetadata rule)
        {
            int count = 0;
            return string.Join("", Array.ConvertAll<Recertification, string>(rule.RuleRecertification.ToArray(), recert => GetNextRecertDateString(CountString(rule.RuleRecertification.Count > 1, ++count), recert).ToString()));
        }

        public static string DisplayOwner(RuleMetadata rule)
        {
            int count = 0;
            return string.Join("", Array.ConvertAll<Recertification, string>(rule.RuleRecertification.ToArray(), recert => GetOwnerDisplayString(CountString(rule.RuleRecertification.Count > 1, ++count), recert).ToString()));
        }

        public static string DisplayRecertIpMatches(RuleMetadata rule)
        {
            int count = 0;
            return string.Join("", Array.ConvertAll<Recertification, string>(rule.RuleRecertification.ToArray(), recert => GetIpMatchDisplayString(CountString(rule.RuleRecertification.Count > 1, ++count), recert).ToString()));
        }

        public static string DisplayLastHit(RuleMetadata rule)
        {
            if (rule.LastHit == null)
                return "";
            else
                return DateOnly.FromDateTime((DateTime)rule.LastHit).ToString("yyyy-MM-dd");  //rule.Metadata.LastHit.ToString("yyyy-MM-dd");
        }

        public static string DisplayLastRecertifier(RuleMetadata rule)
        {
            int count = 0;
            return string.Join("", Array.ConvertAll<Recertification, string>(rule.RuleRecertification.ToArray(), 
                recert => GetLastRecertifierDisplayString(CountString(rule.RuleRecertification.Count > 1, ++count), recert).ToString()));
        }

        protected static string NetworkLocationToHtml(NetworkLocation networkLocation, int mgmtId, int chapterNumber, OutputLocation location, string style, ReportType reportType)
        {
            string nwLocation = DisplayNetworkLocation(networkLocation, reportType, 
                reportType.IsResolvedReport() || networkLocation.User == null ? null :
                ReportDevicesBase.ConstructLink(ObjCatString.User, ReportBase.GetIconClass(ObjCategory.user, networkLocation.User?.Type.Name),
                    chapterNumber, networkLocation.User!.Id, networkLocation.User.Name, location, mgmtId, style),
                reportType.IsResolvedReport() ? null :
                ReportDevicesBase.ConstructLink(ObjCatString.NwObj, ReportBase.GetIconClass(ObjCategory.nobj, networkLocation.Object.Type.Name),
                    chapterNumber, networkLocation.Object.Id, networkLocation.Object.Name, location, mgmtId, style)
                ).ToString();
            return reportType.IsRuleReport() ? $"<span style=\"{style}\">{nwLocation}</span>" : nwLocation;
        }

        protected static string ServiceToHtml(NetworkService service, int mgmtId, int chapterNumber, OutputLocation location, string style, ReportType reportType)
        {
            return DisplayService(service, reportType, reportType.IsResolvedReport() ? null : 
                ReportDevicesBase.ConstructLink(ObjCatString.Svc, ReportBase.GetIconClass(ObjCategory.nsrv, service.Type.Name), chapterNumber, service.Id, service.Name, location, mgmtId, style)).ToString();
        }

        private string DisplaySourceOrDestination(Rule rule, int chapterNumber, OutputLocation location, ReportType reportType, string style, bool isSource)
        {
            StringBuilder result = new();
            if ((isSource && rule.SourceNegated) ||(!isSource && rule.DestinationNegated))
            {
                result.AppendLine(userConfig.GetText("negated") + "<br>");
            }
            string highlightedStyle = style + (reportType == ReportType.AppRules ? " " + GlobalConst.kStyleHighlighted : "");

            if(reportType.IsResolvedReport())
            {
                NetworkLocation[] userNwObjects = [.. GetNetworkLocations(isSource ? rule.Froms : rule.Tos)];
                result.AppendJoin("<br>", Array.ConvertAll(userNwObjects, networkLocation => NetworkLocationToHtml(networkLocation, rule.MgmtId, chapterNumber, location, highlightedStyle, reportType)));
            }
            else
            {
                result.AppendJoin("<br>", Array.ConvertAll(isSource ? rule.Froms : rule.Tos, 
                    nwLoc => NetworkLocationToHtml(nwLoc, rule.MgmtId, chapterNumber, location, highlightedStyle, reportType)));
            }
            if(reportType == ReportType.AppRules)
            {
                if(!rule.ShowDisregarded &&
                    ((isSource && rule.Froms.Length > 0 && rule.DisregardedFroms.Length > 0) || 
                    (!isSource && rule.Tos.Length > 0 && rule.DisregardedTos.Length > 0)))
                {
                    result.Append($"<br><span class=\"text-secondary\">... ({(isSource ? rule.DisregardedFroms.Length : rule.DisregardedTos.Length)} {userConfig.GetText("more")})</span>");
                }
                else
                {
                    if(result.Length > 0)
                    {
                        result.Append("<br>");
                    }
                    result.AppendJoin("<br>", Array.ConvertAll(isSource ? rule.DisregardedFroms : rule.DisregardedTos,
                        nwLoc => NetworkLocationToHtml(nwLoc, rule.MgmtId, chapterNumber, location, nwLoc.Object.IsAnyObject() ? highlightedStyle : style, reportType)));
                }
            }

            return result.ToString();
        }

        private static string GetNextRecertDateString (string countString, Recertification recert)
        {
            string color = "";
            string dateOnly = "-";
            if (recert.NextRecertDate != null)
            {
                dateOnly = DateOnly.FromDateTime((DateTime)recert.NextRecertDate).ToString("yyyy-MM-dd");
                if(recert.NextRecertDate < DateTime.Now)
                {
                    color = " style=\"color: red;\"";
                }
            }
            return "<p" + color + ">" + countString + dateOnly + "</p>";
        }

        private static string GetOwnerDisplayString (string countString, Recertification recert)
        {
            return "<p>" + countString + (recert.FwoOwner != null && recert.FwoOwner?.Name != null ? recert.FwoOwner.Name : "") + "</p>";
        }

        private static string GetIpMatchDisplayString (string countString, Recertification recert)
        {
            return "<p>" + countString + (recert.IpMatch != null && recert.IpMatch != "" ? recert.IpMatch : "&#8208;") + "</p>";
        }

        private static string GetLastRecertifierDisplayString (string countString, Recertification recert)
        {
            return "<p>" + countString + "</p>"; // TODO: fetch last recertifier
        }

        private static string CountString(bool multipleOwners, int ownerCounter)
        {
            return multipleOwners ? ownerCounter.ToString() + ".&nbsp;" : "";
        }
    }
}
