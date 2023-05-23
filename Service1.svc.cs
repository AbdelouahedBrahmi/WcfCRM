using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace WcfService1
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" dans le code, le fichier svc et le fichier de configuration.
    // REMARQUE : pour lancer le client test WCF afin de tester ce service, sélectionnez Service1.svc ou Service1.svc.cs dans l'Explorateur de solutions et démarrez le débogage.
    public class Service1 : IService1
    {
        private IncidentLogic logic = new IncidentLogic();
        private CrmDataConnection crmData = new CrmDataConnection();



        public string GetAllAttributesInformation()
        {
            return crmData.getAttributesInformations();
        }
        public Guid InsertIncident(Incident incident)
        {
            return logic.InsertIncident(incident);
        }

        public bool DeleteIncident (Guid recordId)
        {
            return logic.DeleteIncident(recordId);
        }

        public Incident GetIncidentBySearchAttribute(string _searchName, string _searchValue)
        {
            return logic.GetIncident(_searchName, _searchValue);

        }
        //
        public List<Incident> GetMultipleRecord()
        {
            try
            {
                return logic.GetMultipleRecord();

            }
            catch(EndpointNotFoundException ex)
            {
                string mess = ex.Message;
                return null;
            }
                
            
        }
            

        public Incident GetIncident(Guid recordId)
        {
            return logic.GetIncidentRecord(recordId);
        }

        public List<CustomerAccount> GetCustomerAccounts()
        {
            return logic.GetCustomerAccounts();
        }

        public bool updateIncident(Incident incident)
        {
            return logic.UpdateIncident(incident);
        }
    }
    }
