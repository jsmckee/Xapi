﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

using Korelight.XAPI.Core;
using Microsoft.AspNetCore.Mvc;

namespace Korelight.XAPI
{
    /// <summary>
    /// Java Script API class, generates .JS file for the target service type.
    /// </summary>
    public class JQueryApi<ControllerType> : XCore<ControllerType>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JQueryApi()
                : base()
        {

        }

        /// <summary>
        /// Get a runtime compuled JSON API for a given WCF service Type.
        /// </summary>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        public override Stream GetAPI(String controller)
        {
            StringBuilder api = new StringBuilder();

            api.Append("/* JQUERY API Generated by the Korelight XAPI Service.");
            api.Append(Environment.NewLine);
            api.Append("http://www.Korelight.com ");
            api.Append(Environment.NewLine);
            api.Append("Jeremy McKee - 7/14/2017");
            api.Append(Environment.NewLine);
            api.Append("Timestamp: ");
            api.Append(DateTime.UtcNow.ToString());
            api.Append(" */");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);

            api.Append("var ");
            api.Append(typeof(ControllerType).Name);
            api.Append(" = {");

            foreach (MethodDocumentation doc in GetTypeDocumentation())
            {
                StringBuilder data = new StringBuilder();

                api.Append(doc.MethodName);
                api.Append(": function(success,failure");
                if (doc.MethodParameters.Count > 0)
                {
                    api.Append(",");
                }
                else
                {
                    api.Append("){");
                }

                foreach (var p in doc.MethodParameters)
                {
                    api.Append(p.ParameterName);
                    data.Append(p.ParameterName);

                    api.Append("){");
                    break;
                }

                var endpoint = doc.MethodName;

                RouteAttribute route = null;
                foreach (Attribute a in doc.MethodAttributes)
                {
                    if (a as RouteAttribute != null)
                    {
                        route = a as RouteAttribute;
                    }
                }

                if (route != null)
                {
                    endpoint = route.Template;
                }

                api.Append("$.ajax({context: this,cache: false,type:\"");
                api.Append(doc.HttpActionType);
                api.Append("\",async:true,data:");

                if (data.Length > 0)
                {
                    api.Append("JSON.stringify(" + data.ToString() + ")");
                }
                else
                {
                    api.Append("{}");
                }

                api.Append(",");
                api.Append("url:window.location.protocol + \"//\" + window.location.host + \"/");
                api.Append(controller);
                api.Append("/");
                api.Append(endpoint);
                api.Append("\",");
                api.Append("contentType:\"application/json\",datatype:\"json\",crossDomain:true,success:success,error:failure});},");
            }

            api.Append("};");
            MemoryStream result = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(api.ToString()));
            return result;
        }

        public override List<Tuple<FileInfo, MemoryStream>> GetCoreServiceClients()
        {
            return new List<Tuple<FileInfo, MemoryStream>>();
        }
        public override List<Tuple<FileInfo, MemoryStream>> GetInterfaces(Type[] targettypes)
        {
            return new List<Tuple<FileInfo, MemoryStream>>();
        }
    }
}