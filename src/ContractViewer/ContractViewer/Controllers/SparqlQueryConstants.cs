using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContractViewer.Controllers
{
    public static class SparqlQueryConstants
    {
        public const string SelectBySubject = "SELECT * WHERE { @subject ?p ?o }";

        public const string SelectDBpediaInfo =
              @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
                PREFIX dbpedia-owl: <http://dbpedia.org/ontology/>
                
                SELECT DISTINCT ?city ?img
                WHERE {
                   ?city rdfs:label @publisher@cs;
                         dbpedia-owl:thumbnail ?img .       
                }";

        public const string SelectContracts =
              @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX ps: <https://w3id.org/payswarm#>

                SELECT ?Uri ?Publisher ?Title ?ContractType ?DateSigned ?ValidFrom ?Amount
                WHERE 
                { 
                   ?Uri a <http://tiny.cc/open-contracting#Contract> ;
                             dc:title ?Title ;
                             cn:contractType ?ContractType ; 
                             dc:created ?DateSigned ;
                             ps:validFrom  ?ValidFrom ;
                             gr:hasCurrencyValue ?Amount .

                   BIND(REPLACE(str(?Uri), 'contract.*$', 'publisher') AS ?publisherStr)
                   BIND(URI(?publisherStr) AS ?pub ) 

                   ?pub gr:legalName ?Publisher .
                }";

        public const string SelectContractsByPublisher =
              @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX ps: <https://w3id.org/payswarm#>

                SELECT ?Uri ?Title ?ContractType ?DateSigned ?ValidFrom ?Amount
                WHERE 
                { 
                   ?Uri a <http://tiny.cc/open-contracting#Contract> ;
                             dc:title ?Title ;
                             cn:contractType ?ContractType ; 
                             dc:created ?DateSigned ;
                             ps:validFrom  ?ValidFrom ;
                             gr:hasCurrencyValue ?Amount .

                   BIND(REPLACE(str(?Uri), 'contract.*$', 'publisher') AS ?publisherStr)
                   BIND(URI(?publisherStr) AS ?pub ) 

                   ?pub gr:legalName @publisher .
                }";

        public const string SelectAmendments =
              @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX com: <https://w3id.org/commerce#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Title ?DateSigned ?Document
                WHERE 
                { 
                   ?Uri a <http://tiny.cc/open-contracting#Amendment> ;
                             dc:title ?Title ;
                             dc:created ?DateSigned ;
                             com:contentUrl  ?Document ;
                             cn:contract @contract .
                }";

        public const string SelectAttachments =
              @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX com: <https://w3id.org/commerce#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Title ?Document
                WHERE 
                { 
                   ?Uri a <http://tiny.cc/open-contracting#Attachment> ;
                             dc:title ?Title ;
                             com:contentUrl  ?Document ;
                             cn:contract @contract .
                }";

        public const string SelectParties =
              @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>

                SELECT ?ID ?Uri ?Name ?Country ?PaysVAT ?StreetAddres ?Locality ?PostalCode 
                WHERE 
                { 
                   @contract cn:party ?Uri .
                   ?Uri a <http://purl.org/goodrelations/v1#BusinessEntity> ;
                             gr:legalName ?Name ;
                             schema:addressCountry ?Country ;
                             schema:address ?Address ;
                             gr:valueAddedTaxIncluded ?PaysVAT .

                   OPTIONAL {?Uri dc:identifier ?ID}

                   ?Address a <http://schema.org/PostalAddress> ;
                             schema:streetAddres ?StreetAddres ;
                             schema:addressLocality ?Locality ;
                             schema:postalCode ?PostalCode .       
                }";

        public const string SelectMilestones =
              @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Title ?DueDate
                WHERE 
                {          
                   @contract cn:implementation ?Implementation .
                   
                   ?Implementation a <http://tiny.cc/open-contracting#Implementation> ;
                             cn:milestone ?Uri .                  

                   ?Uri a <http://tiny.cc/open-contracting#Milestone> ;
                             dc:title ?Title ;
                             cn:dueDate ?DueDate .       
                }";
    }
}