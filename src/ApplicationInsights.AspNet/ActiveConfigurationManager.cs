﻿namespace Microsoft.ApplicationInsights.AspNet
{
    using Microsoft.ApplicationInsights.AspNet.ContextInitializers;
    using Microsoft.ApplicationInsights.AspNet.TelemetryInitializers;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Framework.ConfigurationModel;
    using System;

    public static class ActiveConfigurationManager
    {
        public static void AddInstrumentationKey(TelemetryConfiguration aiConfig, IConfiguration config)
        {
            if (aiConfig == null)
            {
                // TODO: Diagnostics
                return;
            }

            // Do not initialize key if customer has already set it.
            if (string.IsNullOrWhiteSpace(aiConfig.InstrumentationKey))
            {
                // Read from configuration
                // Config.json will look like this:
                //
                //      "ApplicationInsights": {
                //            "InstrumentationKey": "11111111-2222-3333-4444-555555555555"
                //      }
                var instrumentationKey = config.Get("ApplicationInsights:InstrumentationKey");
                if (!string.IsNullOrWhiteSpace(instrumentationKey))
                {
                    aiConfig.InstrumentationKey = instrumentationKey;
                }
            }
        }

        public static void AddTelemetryInitializers(TelemetryConfiguration aiConfig, IServiceProvider serviceProvider)
        {
            if (aiConfig == null)
            {
                // TODO: Diagnostics
                return;
            }

            if (serviceProvider == null)
            {
                // TODO: Diagnostics
                return;
            }

            var httpAccessor = serviceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;

            aiConfig.TelemetryInitializers.Add(new ClientIpHeaderTelemetryInitializer(httpAccessor));
            aiConfig.TelemetryInitializers.Add(new UserAgentTelemetryInitializer(httpAccessor));
            aiConfig.TelemetryInitializers.Add(new OperationNameTelemetryInitializer(httpAccessor));
            aiConfig.TelemetryInitializers.Add(new OperationIdTelemetryInitializer(httpAccessor));
        }

        public static void AddContextInitializers(TelemetryConfiguration aiConfig)
        {
            if (aiConfig == null)
            {
                // TODO: Diagnostics
                return;
            }

            aiConfig.ContextInitializers.Add(new DomainNameRoleInstanceContextInitializer());
        }
    }
}