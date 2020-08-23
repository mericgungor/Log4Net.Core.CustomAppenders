/*
   Copyright 2019 Hatfield Consultants

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
   
   http://www.apache.org/licenses/LICENSE-2.0
   
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System.IO;
using System.Net.Mail;
using log4net.Appender;
using log4net.Core;

namespace Hatfield.Log4Net.Core.CustomAppenders.Appenders
{
    /// <summary>
    /// https://github.com/HatfieldConsultants/Log4Net.Core.CustomAppenders
    /// </summary>
    public class SMTPAppender : BufferingAppenderSkeleton
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        
        protected void SendEmail(string messageBody)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(From);
                mailMessage.To.Add(To);
                mailMessage.Body = messageBody;
                mailMessage.Subject = Subject;


                using (SmtpClient smtp = new SmtpClient(SmtpHost, Port))
                {
                    //gmail setting: must be on 
                    //https://myaccount.google.com/lesssecureapps

                    smtp.UseDefaultCredentials = false;
                    if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                        smtp.Credentials = new NetworkCredential(UserName, Password);

                    smtp.EnableSsl = EnableSsl;
                    smtp.Send(mailMessage);
                }
            }          
        }

        protected override bool RequiresLayout => true;

        protected override void SendBuffer(LoggingEvent[] events)
        {
            StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);

            string t = Layout.Header;
            if (t != null)
            {
                writer.Write(t);
            }

            for (int i = 0; i < events.Length; i++)
            {
                // Render the event and append the text to the buffer
                RenderLoggingEvent(writer, events[i]);
            }

            t = Layout.Footer;
            if (t != null)
            {
                writer.Write(t);
            }

            SendEmail(writer.ToString());
        }
    }
}
