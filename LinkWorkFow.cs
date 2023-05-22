using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

using System;
using System.Activities;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk.Query;

namespace WcfService1
{
    public class LinkWorkFow : CodeActivity
    {
        protected override void Execute(CodeActivityContext activityContext)
        {
            try
            {
                bool hasThereBeenAnUpdate = false;

                IWorkflowContext context = activityContext.GetExtension<IWorkflowContext>();

                IOrganizationServiceFactory serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

                ITracingService tracingService = activityContext.GetExtension<ITracingService>();

                tracingService.Trace("Get Enquiry using ID");


                ColumnSet attributes = new ColumnSet(new string[] { "netug_name", "netug_email", "netug_phone", "netug_contactid" });
                var enquiry = service.Retrieve("netug_enquiry", context.PrimaryEntityId, attributes);

                tracingService.Trace("check if we can find an existing contact");

                object emailObject;

                if (enquiry.Attributes.TryGetValue("netug_email", out emailObject))
                {

                    var emailFromEnquiry = emailObject.ToString();
                    if (emailFromEnquiry != "")
                    {

                        QueryExpression qe = new QueryExpression();
                        qe.EntityName = "contact";
                        qe.ColumnSet = new ColumnSet(new string[] { "emailaddress1" });

                        qe.Criteria = new FilterExpression();
                        qe.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, emailFromEnquiry);
                        qe.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

                        EntityCollection results = service.RetrieveMultiple(qe);

                        tracingService.Trace("Check the amount of contacts");
                        if (results.Entities.Count > 0)
                        {
                            tracingService.Trace("More that zero contacts so link the first record to the enquiry");


                            enquiry.Attributes["netug_contactid"] = new EntityReference("contact");
                            hasThereBeenAnUpdate = true;

                        }
                        else
                        {
                            tracingService.Trace("No contact so build contact record");
                            var contact = BuildContactRecord(enquiry, tracingService);

                            tracingService.Trace("Check if contact has value");
                            if (contact != null)
                            {
                                tracingService.Trace("create the contact");
                                var contactid = service.Create((Microsoft.Xrm.Sdk.Entity)contact);

                                tracingService.Trace("Link the contact to the enquiry");
                                enquiry.Attributes["netug_contactid"] = new EntityReference("contact");
                                hasThereBeenAnUpdate = true;
                            }
                        }

                        if (hasThereBeenAnUpdate)
                        {
                            tracingService.Trace("Been an update - Update the enquiry");
                            service.Update(enquiry);
                        }

                        tracingService.Trace("Enquiry Workflow completed");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }            
        }

        private object BuildContactRecord(Entity enquiry, ITracingService tracingService)
        {
            throw new NotImplementedException();
        }
    }
}