using System;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.ServiceModel;
using System.Windows.Forms;
using System.Windows.Markup;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace WcfService1
{
    public class CrmDataConnection
    {

        public CrmDataConnection()
        {
        }


        public static CrmServiceClient ConnectionToCRM()
        {
            /* //1st Connection method
                 ServiceClient service = new
                 ServiceClient(ConfigurationManager.ConnectionStrings["MyCrmEntities"].ConnectionString);*/

            //2nd Connection method
            var conection = System.Configuration.ConfigurationManager.ConnectionStrings["MyCrmEntities"].ToString();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IOrganizationService service;
            CrmServiceClient svc;
            try
            {
                svc = new CrmServiceClient(conection);
                service = (IOrganizationService)svc.OrganizationWebProxyClient
                        != null ? (IOrganizationService)svc.OrganizationWebProxyClient
                        : (IOrganizationService)svc.OrganizationServiceProxy;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " Authentication:" + conection);
            }

            return svc;

        }

        public Entity GetEntityById(Guid id, string _entityName, String[] _columns)
        {
            CrmServiceClient svc = ConnectionToCRM();
            try
            {
                QueryExpression queryExpression = new QueryExpression();
                queryExpression.ColumnSet.Columns.AddRange(_columns);
                Entity et = svc.Retrieve(_entityName, id, queryExpression.ColumnSet);
                return et;
               
            }
            catch { return null; }

        }
        public EntityCollection GetMultipleRecords(string _entityName, String[] _columns)
        {
            CrmServiceClient svc = ConnectionToCRM();
            var accountResult = new EntityCollection();


            if (svc.IsReady)
            {
                var query = new QueryExpression();
                query.EntityName = _entityName;
                query.ColumnSet = new ColumnSet();
                query.ColumnSet.Columns.AddRange(_columns);

                accountResult = svc.RetrieveMultiple(query);
            }
            return accountResult;
            //foreach (Entity account in accountResult.Entities)
            //{
            //    values += account.Attributes["incidentid"] + ", ";

            //}



            /////Get Attribute LogicalNames

            //RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            //{
            //    EntityFilters = EntityFilters.All,
            //    LogicalName = "account"
            //};
            //RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)svc.Execute(retrieveEntityRequest);
            //EntityMetadata AccountEntity = retrieveAccountEntityResponse.EntityMetadata;


            //foreach (object attribute in AccountEntity.Attributes)
            //{
            //    AttributeMetadata a = (AttributeMetadata)attribute;
            //    //values += a.LogicalName + ",  ";
            //}

        }


        public Guid InsertEntityValues(Entity entity)
        {

            CrmServiceClient svc = ConnectionToCRM();


            try
            {
                return svc.Create(entity);

            }

            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (Exception ex)
            {
                return new Guid(ex.Message);
            }

        }

        public bool UpdateEntity(Entity entity)
        {
            CrmServiceClient svc = ConnectionToCRM();
            try
            {
                if (svc.IsReady && entity!=null)
                {
                    svc.Update(entity);
                    return true;
                }
            }

            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (Exception ex)
            {
                throw ex;
            }
            return false;

        }

        public Entity GetEntity(string _entityName, string _attributeName, string _attributeValue, string[] _columns)
        {
            string message = string.Empty;
            CrmServiceClient svc = ConnectionToCRM();
            if (svc.IsReady)
            {
                try
                {
                    var query_title = _attributeValue;
                    var query = new QueryExpression();
                    query.EntityName = _entityName;
                    query.ColumnSet.AddColumn(_attributeName);
                    
                    //query.ColumnSet.AllColumns = true;
                    query.Criteria.AddCondition(_attributeName, ConditionOperator.Equal, _attributeValue);
                    EntityCollection entities = svc.RetrieveMultiple(query);
                    if (entities != null && entities.Entities != null && entities.Entities.Count > 0)
                    {
                        Guid recordId = svc.RetrieveMultiple(query).Entities.Select(e => e.Id).FirstOrDefault();
                        query.ColumnSet.Columns.AddRange(_columns);
                        return svc.Retrieve(_entityName, recordId, query.ColumnSet);
                        
                        
                    }
                }
                catch(Exception ex)
                {
                    message = ex.Message;
                    return null;
                }

            }
            return null;
        } 
        public string DeleteTestEntity(string _entityName, string _attributeName, string _attributeValue)
        {
            string message = string.Empty;
            CrmServiceClient svc = ConnectionToCRM();
            if (svc.IsReady)
            {
                try
                {
                    //Entity incident = new Entity("incident");
                    //incident["title"] = "titreIncident";
                    //incident = service.Retrieve("incident", recordID, new ColumnSet("incidentid"));

                    var query_title = _attributeValue;
                    var query = new QueryExpression();
                    query.EntityName = _entityName;
                    query.ColumnSet.AddColumn(_attributeName);
                    //query.ColumnSet.AllColumns = true;
                    query.Criteria.AddCondition(_attributeName, ConditionOperator.Equal, _attributeValue);
                    EntityCollection entities = svc.RetrieveMultiple(query);
                    if (entities != null && entities.Entities != null && entities.Entities.Count > 0)
                    {
                        string s = svc.RetrieveMultiple(query).Entities.Select(e => e.Id).FirstOrDefault().ToString();
                        Guid _recordId = new Guid(s);
                        svc.Delete(_entityName, _recordId);
                        message = string.Format("the data with this information :{0} has deleted with success", _recordId);
                    }
                    else message = string.Format("{0} ne contient pas des données avec le nom d'attribut {1} et sa valeur {2}",
                        _entityName, _attributeName, _attributeValue);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            return message;
        }
        public bool DeleteEntity (string _entityName, Guid recordId)
        {
           CrmServiceClient svc = ConnectionToCRM();
            if (svc.IsReady)
            {
                try
                {
                    svc.Delete(_entityName, recordId);
                    return true;
                }
                catch
                {
                    return false;
                }
              
            }
            return true;
        }


        public bool ExecuteRequest(string entityName, string[] columns, string[] values)
        {
            CrmServiceClient svc = ConnectionToCRM();

            if (svc.IsReady)
            {
                try
                {
                    List<AttributeMetadata> attributes = svc.GetAllAttributesForEntity(entityName);
                   
                    var attr = attributes.Where(a=>a.AttributeType.Value.ToString() == "Customer")
                       // .Select(a => new { a.LogicalName,  a.AttributeType.Value })
                        
                        .Distinct().ToList();
                    var joinAttrCol = (from at in attr
                               join c in columns on at.LogicalName equals c
                               //orderby at.LogicalName 
                               select at
                               ).ToArray();

                    //List<string> cols = columns.ToList();
                    //cols.Sort();

                    Entity entity = new Entity(entityName);
                    
                    for (int i = 0; i < joinAttrCol.Length; i++)
                    {

                        var type = ""; //joinAttrCol[i].Value.ToString();

                        switch (type)
                        {
                            case "String" :
                                entity[type] = values[i];
                                break;
                            case "integer" :
                                entity[type] = values[i];
                                break;

                            case "Picklist":
                                entity[type] = new OptionSetValue(Convert.ToInt16(values[i]));
                                break;

                            case "DateTime":
                                break;

                            default:
                                entity[type] = new EntityReference();
                                break;


                        }


                    }

                    CreateRequest request = new CreateRequest
                    {
                        RequestName = entityName,
                        Target = entity
                    };
                    var response = svc.Execute(request);
                    return true;
                }
            catch 
                {
                    throw new Exception(); 
                }
            }
            return false;
        }

        public string getAttributesInformations()
        {
            CrmServiceClient service = ConnectionToCRM();
            string message = string.Empty;

            if (service.IsReady)
            {
                #region getAllAttributesInfosTest
                //try {
                //    RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
                //    {
                //        EntityFilters = EntityFilters.All,
                //        LogicalName = "incident"
                //    };
                //    RetrieveEntityResponse retrieveAccountEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);
                //    EntityMetadata AccountEntity = retrieveAccountEntityResponse.EntityMetadata;

                //    message = message + "Incident entity attributes:\n";
                //    foreach (object attribute in AccountEntity.Attributes)
                //    {
                //        AttributeMetadata a = (AttributeMetadata)attribute;
                //        message += a.LogicalName + "    " +
                //            a.Description + "    " +
                //            a.DisplayName + "    " +
                //            a.EntityLogicalName + "    " +
                //            a.SchemaName + "    " +
                //            //a.DisplayName.UserLocalizedLabel.Label + "      " + 
                //            a.AttributeType + "\n";
                //    }


                //}
                //catch (Exception ex)
                //{
                //    return ex.Message;
                //}
                #endregion

                var attReq = new RetrieveAttributeRequest();
                attReq.EntityLogicalName = "Incident";
                attReq.LogicalName = "statecode";
                attReq.RetrieveAsIfPublished = true;

                var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
                var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

                var optionList = (from o in attMetadata.OptionSet.Options
                                  select new { Value = o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();
                return optionList.ToString();
            }
            return string.Empty;
            
        }
    }
}