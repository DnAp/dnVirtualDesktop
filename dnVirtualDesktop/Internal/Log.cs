﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

namespace dnVirtualDesktop.Internal
{
    public static class Log
    {
        public static void LogEvent(string eventName,
            string eventDetails,
            string additionalDetails,
            string moduleName,
            Exception e)
        {
            try
            {
                //string computerInfo = "Operating System: " + System.Environment.OSVersion;
                File.AppendAllText(
                    "event.log",
                    "[" + DateTime.Now + "]\t" + eventName + "\t" + eventDetails + "\t" + additionalDetails + "\t"
                    + moduleName + e.Message + "\n"
                );
                //AddEventToDatabase("dnVirtualDesktop", Program.version, DateTime.Now, eventName, eventDetails, additionalDetails, moduleName, e, 1, SystemInformation.UserName,
                //SystemInformation.ComputerName, computerInfo);
            }
            catch (Exception ex)
            {
            }
        }

        private static void AddEventToDatabase(string applicationName,
            string applicationVersion,
            DateTime eventDateTime,
            string eventName,
            string eventDetails,
            string additionalDetails,
            string moduleName,
            Exception exception,
            int stackLevel,
            string username,
            string computerName,
            string computerInfo)
        {
            try
            {
                StackFrame stackframe1 = new StackFrame(1 + stackLevel);
                ParameterInfo[] parameters = stackframe1.GetMethod().GetParameters();
                string method = moduleName + "::" + stackframe1.GetMethod().Name;
                method = method + "(";
                int i = 0;
                foreach (ParameterInfo param in parameters.OrderBy(p => p.Position))
                {
                    method = (method + param.ParameterType.Name + " ") + param.Name;
                    if (i < parameters.Count() - 1)
                    {
                        method = method + ", ";
                    }

                    i += 1;
                }

                method = method + ")";


                String exceptionText = "";
                if (exception != null)
                {
                    exceptionText = exception.GetType().ToString();
                }

                Zomp.EventData theEvent = new Zomp.EventData();
                theEvent.ApplicationName = applicationName;
                theEvent.ApplicationVersion = applicationVersion;
                theEvent.EventDateTime = eventDateTime;
                theEvent.EventName = eventName;
                theEvent.EventDetails = eventDetails;
                theEvent.AdditionalDetails = additionalDetails;
                theEvent.ModuleName = moduleName;
                theEvent.Exception = exceptionText;
                theEvent.CallingMethod = method;
                theEvent.Username = username;
                theEvent.ComputerName = computerName;
                theEvent.ComputerInfo = computerInfo;

                Thread t = new Thread(AddEventToDatabase);

                //z.AddEventToDatabase(theEvent);


                t.Start(theEvent);
                //MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
            }
        }

        private static void AddEventToDatabase(Object d)
        {
            try
            {
                Zomp.EventData dd = (Zomp.EventData) d;
                WSHttpBinding b = new WSHttpBinding(SecurityMode.Transport);
                b.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                b.Name = "WSHttpBinding_IService";
                EndpointAddress address = new EndpointAddress("https://zomp.co/z/ZompWebService.svc");

                using (Zomp.ServiceClient z = new Zomp.ServiceClient(b, address))
                {
                    z.AddEventToDatabase(dd);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}