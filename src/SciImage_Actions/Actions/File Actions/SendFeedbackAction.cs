/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class SendFeedbackAction
        : PluginAction
    {
        public override string Name
        {
            get
            {
                return "Send Feedback";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "Help"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.F9);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 2; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 6; }
        }
        private string GetEmailLaunchString(string email, string subject, string body)
        {
            const string emailFormat = "mailto:{0}?subject={1}&body={2}";
            string bodyUE = body.Replace("\r\n", "%0D%0A");
            string launchString = string.Format(emailFormat, email, subject, bodyUE);
            return launchString;
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {

            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            string email = InvariantStrings.FeedbackEmail;
            string subjectFormat = SciImage.SciResources.SciResources.GetString("SendFeedback.Email.Subject.Format");
            string subject = string.Format(subjectFormat, SciInfo.GetFullAppName());
            string body = SciImage.SciResources.SciResources.GetString("SendFeedback.Email.Body");
            string launchMe = GetEmailLaunchString(email, subject, body);
            launchMe = launchMe.Substring(0, Math.Min(1024, launchMe.Length));

            try
            {
                Process.Start(launchMe);
            }
                 
            catch (Exception)
            {
                Utility.ErrorBox(FormsManager.BaseForm, SciImage.SciResources.SciResources.GetString("LaunchLink.Error"));
            }
            return true ;
        }

        public SendFeedbackAction()
        {
        }
    }
}
