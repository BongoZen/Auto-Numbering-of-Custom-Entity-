using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Text;


namespace AutoNumberPlugin
{
    public class Program : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new NotImplementedException();

            try
            {


                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));


                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));



                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                var inputParams = context.InputParameters;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity newCase = (Entity)context.InputParameters["Target"];
                    newCase.Attributes["new_name"] = AutoNumber(service);
                   
                }

            }



            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Plugin Error- " + ex.Message.ToString());

            }

        }
        public string AutoNumber(IOrganizationService service1)
        {
            Entity autoConfiguration = new Entity("new_autonumber");
            StringBuilder autoNumber = new StringBuilder();
            string prefix, last_used;
            QueryExpression queryExpression = new QueryExpression()
            {
                EntityName = "new_autonumber",
                ColumnSet = new ColumnSet("new_name", "new_prefix", "new_number")
            };
            EntityCollection entityCollection = service1.RetrieveMultiple(queryExpression);
            if (entityCollection.Entities.Count == 0)
            {
                return null;
            }
            foreach (Entity entity in entityCollection.Entities)
            {
                if (entity.Attributes["new_name"].ToString().ToLower() == "incident")
                {
                    prefix = entity.GetAttributeValue<string>("new_prefix");
                    last_used = entity.GetAttributeValue<string>("new_number");
                    int lastNumber = int.Parse(last_used);
                    lastNumber++;
                    last_used = lastNumber.ToString();
                    autoConfiguration.Id = entity.Id;
                    autoConfiguration["new_number"] = last_used;
                    service1.Update(autoConfiguration);
                    autoNumber.Append(prefix + last_used);
                    break;
                }
            }
            return autoNumber.ToString();
        }
    }
}