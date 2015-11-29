using System;

namespace ContractViewer.Controllers
{
    public static class Constants
    {
        public static class StudentOpenDataCz
        {
            public const string SparqlEndpointUri = "http://student.opendata.cz/sparql";

            public const string GetContract = "SELECT * WHERE { @contract ?p ?o }";

            public const string GetNumberOfContracts =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>

                SELECT ?Publisher 
                        (SAMPLE(?subject) AS ?Subject)
                        (SAMPLE(?ic) AS ?Ic)
                        (SAMPLE(?aresLink ) AS ?AresLink)
                        (COUNT(?Contract) as ?ContractSum) 
                WHERE 
                { 
                    ?Contract a cn:Contract ;
                                dc:publisher ?Publisher.
    
                    ?Publisher gr:legalName ?subject;
                                dc:identifier ?ic .

                    OPTIONAL
                    {
                        ?Publisher owl:sameAs ?aresLink .
                    }

                }
                GROUP BY ?Publisher";

            public const string GetContracts =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX ps: <https://w3id.org/payswarm#>

                SELECT ?Uri ?Publisher ?Title ?ContractType ?DateSigned ?ValidFrom ?Amount
                WHERE 
                { 
                    ?Uri a cn:Contract ;
                            dc:title ?Title ;
                            cn:contractType ?ContractType ; 
                            dc:created ?DateSigned ;
                            ps:validFrom  ?ValidFrom ;
                            dc:publisher ?PublisherLink ;
                            cn:amount ?PriceSpec .

                    ?PriceSpec gr:hasCurrencyValue ?Amount .

                    ?PublisherLink gr:legalName ?Publisher .
                }";

            public const string GetPublisherByName =
                @"PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX owl: <http://www.w3.org/2002/07/owl#>

                SELECT ?publisher ?ic ?aresLink
                WHERE 
                { 
                    ?publisher dc:identifier ?ic ;
                                gr:legalName @publisher .

                    OPTIONAL
                    {
                        ?publisher owl:sameAs ?aresLink .
                    }
                }";

            public const string GetContractsByPublisherName =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX ps: <https://w3id.org/payswarm#>

                SELECT ?Uri ?Title ?ContractType ?DateSigned ?ValidFrom ?Amount
                WHERE 
                { 
                    ?Uri a cn:Contract ;
                            dc:title ?Title ;
                            cn:contractType ?ContractType ; 
                            dc:created ?DateSigned ;
                            ps:validFrom  ?ValidFrom ;
                            dc:publisher ?PublisherLink ;
                            cn:amount ?PriceSpec .

                    ?PriceSpec gr:hasCurrencyValue ?Amount .

                    ?PublisherLink gr:legalName @publisher .
                }";

            public const string GetPriceSpecByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                SELECT ?Uri ?Amount ?Currency
                WHERE 
                { 
                   <http://localhost:7598/contract/39/1> cn:amount ?Uri .

                   ?Uri gr:hasCurrencyValue ?Amount ;
                        gr:hasCurrency ?Currency .
                }";

            public const string GetAmendmentsByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX com: <https://w3id.org/commerce#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Title ?DateSigned ?Document
                WHERE 
                { 
                    ?Uri a cn:Amendment ;
                           dc:title ?Title ;
                           dc:created ?DateSigned ;
                           com:contentUrl  ?Document ;
                           cn:contract @contract .
                }";

            public const string GetAttachmentsContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX com: <https://w3id.org/commerce#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Title ?Document
                WHERE 
                { 
                   ?Uri a cn:Attachment ;
                          dc:title ?Title ;
                          com:contentUrl  ?Document ;
                          cn:contract @contract .
                }";

            public const string GetPartiesByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>

                SELECT ?ID ?Uri ?Name ?Country ?PaysVAT ?StreetAddres ?Locality ?PostalCode 
                WHERE 
                { 
                   @contract cn:party ?Uri .
                   ?Uri a gr:BusinessEntity ;
                          gr:legalName ?Name ;
                          schema:addressCountry ?Country ;
                          schema:address ?Address ;
                          gr:valueAddedTaxIncluded ?PaysVAT .

                   OPTIONAL {?Uri dc:identifier ?ID}

                   ?Address a schema:PostalAddress ;
                              schema:streetAddres ?StreetAddres ;
                              schema:addressLocality ?Locality ;
                              schema:postalCode ?PostalCode .       
                }";

            public const string GetMilestonesByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Title ?DueDate
                WHERE 
                {          
                   @contract cn:implementation ?Implementation .
                   
                   ?Implementation a cn:Implementation ;
                             cn:milestone ?Uri .                  

                   ?Uri a cn:Milestone ;
                          dc:title ?Title ;
                          cn:dueDate ?DueDate .       
                }";
        }

        public static class CsDbpediaOrg
        {
            public const string SparqlEndpointUri = "http://cs.dbpedia.org/sparql";

            public const string GetPublisherInfo =
                @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
                PREFIX dbpedia-owl: <http://dbpedia.org/ontology/>
                
                SELECT DISTINCT ?city ?img
                WHERE {
                   ?city rdfs:label @publisher@cs;
                         dbpedia-owl:thumbnail ?img .       
                }";
        }

        public static class LinkedOpenDataCz
        {
            public const string SparqlEndpointUri = "http://linked.opendata.cz/sparql";

            public const string GetBusinessEntityOpeningHours =
                @"PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>

                SELECT ?localPlace ?streetAddress ?postalCode ?dayOfWeek ?open ?close
                WHERE 
                {
                    ?subjekt <http://linked.opendata.cz/ontology/domain/seznam.gov.cz/ovm/business-entity> @businessEntity;
                            gr:hasPOS ?localPlace .

                    ?localPlace s:address ?address ;
                                gr:openingHoursSpecification ?openingHours .

                    ?address s:streetAddress ?streetAddress ;
                            s:postalCode ?postalCode .
  
                    ?openingHours gr:hasOpeningHoursDayOfWeek ?dayOfWeek ;
                                gr:opens ?open ;
                                gr:closes ?close .
                } ";

            public const string GetBusinessEntityRuianLink =
                @"PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>

                SELECT *
                WHERE 
                {
                  @businessEntity s:address ?address .

                  ?address  ruianlink:adresni-misto ?ruianLink .
                                                                
                  FILTER(CONTAINS(str(?address), 'ares'))
                }";

            public const string GetBusinessEntityPublicContracts =
                @"PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX pc: <http://purl.org/procurement/public-contracts#>
                PREFIX pccz: <http://purl.org/procurement/public-contracts-czech#>
                PREFIX skos: <http://www.w3.org/2004/02/skos/core#>

                SELECT DISTINCT ?Uri ?KodProfil ?Title ?SupplierUri ?ID ?Amount ?Vat
                WHERE 
                {
                  ?Uri pc:contractingAuthority @businessEntity ;
                     dc:title ?Title .
     
                  OPTIONAL 
                  {
                    ?Uri pccz:kodprofil ?KodProfil ;
                         pco:awardedTender ?Tender .
                    ?Tender pco:offeredPrice ?PriceSpec ;
                            pco:supplier ?SupplierUri . 
                    ?SupplierUri gr:legalName ?Supplier . 
               
                    BIND(CONCAT(str(?SupplierUri), '/identifier') as ?IcStr)
                    BIND(URI(?IcStr) as ?IcUri)

                    ?IcUri skos:notation ?ID .
                    ?PriceSpec gr:hasCurrencyValue ?Amount ;
                               gr:valueAddedTaxIncluded	1 .
                  }      
                }";
        }

        public static class RuianLinkedOpenDataCz
        {
            public const string SparqlEndpointUri = "http://ruian.linked.opendata.cz/sparql";

            public const string GetBusinessEntityCoordinates =
                @"PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>
                PREFIX ruian: <http://ruian.linked.opendata.cz/ontology/>

                SELECT ?longitude ?latitude
                WHERE 
                {
                  @addressPoint ruian:adresniBod ?addressPoint .

                  ?addressPoint s:geo ?geoCoordinates.

                  ?geoCoordinates s:longitude ?longitude ;
                                  s:latitude ?latitude .
                }";
        }
    }
}