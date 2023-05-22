using System;
using System.Activities.Presentation.Debug;
using System.Activities.Statements;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;


namespace WcfService1
{
    public class IncidentLogic
    {
        private CrmDataConnection crmDataConnection = new CrmDataConnection();
        public static Guid recordId;
        public static List<string> customersName; 


        public IncidentLogic()
        {
            EntityCollection accountList = crmDataConnection.GetMultipleRecords("account", new string[] { "name" });
            customersName = accountList.Entities.Select(e => e.GetAttributeValue<string>("title")).ToList();
        }

        public List<Incident> GetMultipleRecord()
        {
            try
            {

                string[] columns = { "incidentid", "title", "ticketnumber", "prioritycode", "caseorigincode", "customerid", "statuscode", "createdon" };
                EntityCollection result = crmDataConnection.GetMultipleRecords("incident", columns);
                if (result != null && result.Entities.Any() && result.Entities.Count > 0)
                {
                    List<Incident> list = new List<Incident>();
                    CustomerAccount customer = new CustomerAccount();
                    Entity customerEntity = new Entity();
                    columns = new string[]{ "accountid", "name", "telephone1", "websiteurl", "address1_line1", "address1_line2", "address1_postalcode", "address1_postalcode", "address1_city", "address1_country" };

                    foreach (var entity in result.Entities)
                    {
                        Guid record = entity.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("customerid").Id;
                        customerEntity = crmDataConnection.GetEntityById(entity.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("customerid").Id, "account", columns);
                       if (customerEntity == null) 
                        {
                            customerEntity = crmDataConnection.GetEntityById(entity.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("customerid").Id, "aaduser", new string[] { "id" });
                            customerEntity = crmDataConnection.GetEntityById(customerEntity.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("id").Id, "account", columns);

                        }
                        if (customerEntity != null) 
                        {
                            customer = new CustomerAccount()
                            {
                                CustomerAccountID = customerEntity.Id,
                                Name = customerEntity.GetAttributeValue<string>("name"),
                                Telephone = customerEntity.GetAttributeValue<string>("telephone1"),
                                SiteWeb = customerEntity.GetAttributeValue<string>("websiteurl"),
                                Street1 = customerEntity.GetAttributeValue<string>("address1_line1"),
                                Street2 = customerEntity.GetAttributeValue<string>("address1_line2"),
                                PostalCode = Convert.ToInt32(customerEntity.GetAttributeValue<String>("address1_postalcode")),
                                City = customerEntity.GetAttributeValue<string>("address1_city"),
                                Country = customerEntity.GetAttributeValue<string>("address1_country"),

                            };
                        }
                        recordId = entity.Id;
                        Incident incident = new Incident(entity.Id,
                            entity.Contains("title") ? entity.GetAttributeValue<string>("title") : string.Empty,
                            entity.Contains("ticketnumber") ? entity.GetAttributeValue<string>("ticketnumber") : string.Empty,
                            entity.Contains("prioritycode") ? Convert.ToInt32(entity.GetAttributeValue<OptionSetValue>("prioritycode").Value) : 0,
                            entity.Contains("caseorigincode") ? Convert.ToInt32(entity.GetAttributeValue<OptionSetValue>("caseorigincode").Value) : 0,
                            customer,
                            entity.Contains("statuscode") ? Convert.ToInt32(entity.GetAttributeValue<OptionSetValue>("statuscode").Value) : 15,
                            entity.Contains("createdon") ? entity.GetAttributeValue<DateTime>("createdon") : DateTime.MinValue);
                        list.Add(incident);

                    }
                    return list;
                }
            }
            catch (Exception ex){ 
                string ms = ex.Message;
                
            }
            return null;
        }

        public Incident GetIncident(string _searchName, string _searchValue)
        {

            Entity incidentEntity = crmDataConnection.GetEntity(
                "incident", _searchName, _searchValue, new string[] { "incidentid", "title", "ticketnumber", "prioritycode", "caseorigincode", "customerid", "statuscode", "createdon" });
            //Entity customerEntity = crmDataConnection.GetEntityById(incidentEntity.GetAttributeValue<EntityReference>("customerid").Id, "account",
            //     new string[] { "accountid", "name", "telephone1", "websiteurl", "address1_line1", "address1_line2", "address1_postalcode", "address1_postalcode", "address1_city", "address1_country" });
            var account = incidentEntity.Attributes["customerid"] as EntityReference; //incidentEntity.GetAttributeValue<EntityReference>("customerid")
            CustomerAccount customer = new CustomerAccount();
            if (/*customerEntity*/ account != null)
            {
                
                customer = new CustomerAccount()
                {
                    CustomerAccountID = account.Id, // customerEntity.Id,
                    Name = account.Name,//customerEntity.GetAttributeValue<string>("name"),
                    //Telephone = customerEntity.GetAttributeValue<string>("telephone1"),
                    //SiteWeb = customerEntity.GetAttributeValue<string>("websiteurl"),
                    //Street1 = customerEntity.GetAttributeValue<string>("address1_line1"),
                    //Street2 = customerEntity.GetAttributeValue<string>("address1_line2"),
                    //PostalCode = Convert.ToInt32(customerEntity.GetAttributeValue<String>("address1_postalcode")),
                    //City = customerEntity.GetAttributeValue<string>("address1_city"),
                    //Country = customerEntity.GetAttributeValue<string>("address1_country"),

                };
            }
            Incident incident = new Incident(incidentEntity.Id,
            incidentEntity.Contains("title") ? incidentEntity.GetAttributeValue<string>("title") : string.Empty,
            incidentEntity.Contains("ticketnumber") ? incidentEntity.GetAttributeValue<string>("ticketnumber") : string.Empty,
            incidentEntity.Contains("prioritycode") ? Convert.ToInt32(incidentEntity.GetAttributeValue<OptionSetValue>("prioritycode").Value) : 0,
            incidentEntity.Contains("caseorigincode") ? Convert.ToInt32(incidentEntity.GetAttributeValue<OptionSetValue>("caseorigincode").Value) : 0,
            customer,
            incidentEntity.Contains("statecode") ? Convert.ToInt32(incidentEntity.GetAttributeValue<OptionSetValue>("statecode").Value) : 0,
            incidentEntity.Contains("createdon") ? incidentEntity.GetAttributeValue<DateTime>("createdon") : DateTime.MinValue);
            return incident;
        }
        public Incident GetIncidentRecord(Guid recordId)
        {
           
            Entity incidentEntity = crmDataConnection.GetEntityById(recordId, "incident", new string[] { "incidentid", "title", "ticketnumber", "prioritycode", "caseorigincode", "customerid", "statuscode", "createdon" }); 
            Entity customerEntity = crmDataConnection.GetEntityById(incidentEntity.GetAttributeValue<EntityReference>("customerid").Id, "account",
                 new string[] { "accountid", "name", "telephone1", "websiteurl", "address1_line1", "address1_line2", "address1_postalcode", "address1_postalcode", "address1_city", "address1_country" });
            CustomerAccount customer = new CustomerAccount();
            if (customerEntity != null)
            {
                customer = new CustomerAccount()
                {
                    CustomerAccountID = customerEntity.Id,
                    Name = customerEntity.GetAttributeValue<string>("name"),
                    Telephone = customerEntity.GetAttributeValue<string>("telephone1"),
                    SiteWeb = customerEntity.GetAttributeValue<string>("websiteurl"),
                    Street1 = customerEntity.GetAttributeValue<string>("address1_line1"),
                    Street2 = customerEntity.GetAttributeValue<string>("address1_line2"),
                    PostalCode = Convert.ToInt32(customerEntity.GetAttributeValue<String>("address1_postalcode")),
                    City = customerEntity.GetAttributeValue<string>("address1_city"),
                    Country = customerEntity.GetAttributeValue<string>("address1_country"),

                };
            }
            Incident incident = new Incident(incidentEntity.Id,
            incidentEntity.Contains("title") ? incidentEntity.GetAttributeValue<string>("title") : string.Empty,
            incidentEntity.Contains("ticketnumber") ? incidentEntity.GetAttributeValue<string>("ticketnumber") : string.Empty,
            incidentEntity.Contains("prioritycode") ? Convert.ToInt32(incidentEntity.GetAttributeValue<OptionSetValue>("prioritycode").Value) : 0,
            incidentEntity.Contains("caseorigincode") ? Convert.ToInt32(incidentEntity.GetAttributeValue<OptionSetValue>("caseorigincode").Value) : 0,
            customer,
            incidentEntity.Contains("statecode") ? Convert.ToInt32(incidentEntity.GetAttributeValue<OptionSetValue>("statecode").Value) : 0,
            incidentEntity.Contains("createdon") ? incidentEntity.GetAttributeValue<DateTime>("createdon") : DateTime.MinValue);
            return incident;
        }

        public string DeleteIncidentByAttribute(string _searchName, string _searchValue)
        {
            return crmDataConnection.DeleteTestEntity("incident", _searchName, _searchValue);
        }
        public bool DeleteIncident(Guid recordId)
        {
            return crmDataConnection.DeleteEntity("incident", recordId);
        }

        public Guid InsertIncident(Incident incident, CustomerAccount customer)
        {

            try
            {
                Entity incidentEntity = new Entity("incident");

                incidentEntity.Id = incident.IncidentId;
                incidentEntity["title"] = incident.Title;
                Entity recordCustomer = crmDataConnection.GetEntity("account", "name", customer.Name, new string[] { "accountid" });
                if (recordCustomer != null) incidentEntity["customerid"] = new EntityReference("account", recordCustomer.Id);
                else if (recordCustomer == null){ 
                    MessageBox.Show("veuillez saisir un compte valide!"); 
                    
                }
                ;
                // incidentEntity["ticketnumber"] = incident.TicketNumber;
                // incidentEntity["prioritycode"] = incident.Priority;
                // incidentEntity["caseorigincode"] = incident.Origine;

                // incidentEntity["customerid"] = new Guid("83883308-7ad5-ea11-a813-000d3a33f3b4");
                //Entity customer = crmDataConnection.GetEntity("account", "name", customerAccount, new string[] { "accountid" });
                
                //incident.Customer.CustomerAccountID = customer.Id;
                //incidentEntity["customerid"] = new EntityReference("account", customer.Id);
                //incidentEntity["customerid"] = new EntityReference("account", new Guid("83883308-7ad5-ea11-a813-000d3a33f3b4"));
                //incidentEntity["statecode"] = incident.Status;
                //incidentEntity["createdon"] = DateTime.Now; // incident.DateCreation;
                return crmDataConnection.InsertEntityValues(incidentEntity);
            }catch (Exception ex)
            {
                ex.Message.ToString();
                return Guid.Empty;
            }

            
        }
    }
}