using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Protocols.WSTrust;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfService1
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetAllAttributesInformation();

        [OperationContract]
        List<Incident> GetMultipleRecord();

        [OperationContract]
        Incident GetIncident(Guid recordId);

        [OperationContract]
        Incident GetIncidentBySearchAttribute(string _searchName, string _searchValue);

        [OperationContract]
        Guid InsertIncident(Incident incident, CustomerAccount customers);

        [OperationContract]
        bool DeleteIncident(Guid recordId);



        // TODO: ajoutez vos opérations de service ici
    }


    // Utilisez un contrat de données comme indiqué dans l'exemple ci-après pour ajouter les types composites aux opérations de service.
    [DataContract]
    public class Incident 
    {
        

        [DataMember(IsRequired = false)]
        public Guid IncidentId { get; set; }
        [DataMember(IsRequired = false)]
        public string Title { get; set; }
        [DataMember(IsRequired = false)]
        public string TicketNumber { get; set; }
        [DataMember(IsRequired = false)]
        public int Priority { get; set; }
        [DataMember(IsRequired = false)]
        public int Origine { get; set; }
        [DataMember(IsRequired = false)]
        public CustomerAccount Customer { get; set; }
        [DataMember(IsRequired = false)]
        public int Status { get; set; }
        [DataMember(IsRequired = false)]
        public DateTime DateCreation { get; set; }

        
        public Incident()
        {
            Customer = new CustomerAccount();
        }
        public Incident(Guid incidentId, string title, string ticketNumber, int priority, int origine, CustomerAccount customer, int status, DateTime dateCreation)
        {
            this.IncidentId = incidentId;
            this.Title = title;
            this.TicketNumber = ticketNumber;
            this.Priority = priority;
            this.Origine = origine;
            this.Customer = customer;
            this.Status = status;
            this.DateCreation = dateCreation;



        }

    }
    [DataContract]
    [XmlSerializerFormat]
    public class CustomerAccount
    {
        public CustomerAccount()
        {
            
        }
        [DataMember(IsRequired = false)]
        public Guid CustomerAccountID { get; set; }
        [DataMember(IsRequired = false)]
        public string Name { get; set; }
        [DataMember(IsRequired = false)]
        public string Telephone { get; set; }
        [DataMember(IsRequired = false)]
        public string SiteWeb { get; set; }
        [DataMember(IsRequired = false)]

        public string Street1 { get; set; }
        [DataMember(IsRequired = false)]
        public string Street2 { get; set;}
        [DataMember(IsRequired = false)]

        public int PostalCode { get; set; }
        [DataMember(IsRequired = false)]
        public string City { get; set; }
        [DataMember(IsRequired = false)]
        public string Country { get; set; }
        
    }
  

}
