namespace ContractViewer.Handlers
{
    public static class Constants
    {
        /// <summary>
        /// SPARQL queries over the http://student.opendata.cz/sparql endpoint
        /// </summary>
        public static class StudentOpenDataCz
        {
            public const string SparqlEndpointUri = "http://student.opendata.cz/sparql";

            /// <summary>
            /// SPARQL query - Get contract by contract URI
            /// </summary>
            public const string GetContract = "SELECT * WHERE { @contract ?p ?o }";

            /// <summary>
            /// SPARQL query - Get all publishers
            /// </summary>
            public const string GetPublishers =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>

                SELECT ?Publisher ?Ic
                        (SAMPLE(?subject) AS ?Subject)
                        (SAMPLE(?aresLink ) AS ?AresLink)
                        (COUNT(?Contract) as ?ContractSum) 
                WHERE 
                { 
                    ?Contract a cn:Contract ;
                                dc:publisher ?Publisher.
    
                    ?Publisher gr:legalName ?subject;
                                dc:identifier ?Ic .

                    OPTIONAL
                    {
                        ?Publisher owl:sameAs ?aresLink .
                    }

                }
                GROUP BY ?Publisher ?Ic";

            /// <summary>
            /// SPARQL query - Get all contracts
            /// </summary>
            public const string GetContracts =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>

                SELECT ?Uri ?Publisher ?PublisherId ?Title ?ContractType ?DateSigned ?ValidFrom ?Amount
                WHERE 
                { 
                    ?Uri a cn:Contract ;
                            dc:title ?Title ;
                            cn:contractType ?ContractType ; 
                            dc:created ?DateSigned ;
                            cn:validFrom  ?ValidFrom ;
                            dc:publisher ?PublisherLink ;
                            cn:amount ?PriceSpec .

                    ?PriceSpec gr:hasCurrencyValue ?Amount .

                    ?PublisherLink gr:legalName ?Publisher ;
                                   dc:identifier ?PublisherId .
                }";

            /// <summary>
            /// SPARQL query - Get publisher by his IC
            /// </summary>
            public const string GetPublisherByIc =
                @"PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX owl: <http://www.w3.org/2002/07/owl#>

                SELECT ?publisher ?aresLink
                WHERE 
                { 
                    ?publisher dc:identifier @ic .

                    OPTIONAL
                    {
                        ?publisher owl:sameAs ?aresLink .
                    }
                }";

            /// <summary>
            /// SPARQL query - Get all contracts of the publisher by his IC
            /// </summary>
            public const string GetContractsByPublisherIc =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>

                SELECT ?Uri ?Title ?ContractType ?DateSigned ?ValidFrom ?Amount
                WHERE 
                { 
                    ?Uri a cn:Contract ;
                            dc:title ?Title ;
                            cn:contractType ?ContractType ; 
                            dc:created ?DateSigned ;
                            cn:validFrom  ?ValidFrom ;
                            dc:publisher ?PublisherLink ;
                            cn:amount ?PriceSpec .

                    ?PriceSpec gr:hasCurrencyValue ?Amount .

                    ?PublisherLink dc:identifier @publisherId .
                }";

            /// <summary>
            /// SPARQL query - Get price specification of the contract
            /// </summary>
            public const string GetPriceSpecByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                SELECT ?Uri ?Amount ?Currency
                WHERE 
                { 
                   @contract cn:amount ?Uri .

                   ?Uri gr:hasCurrencyValue ?Amount ;
                        gr:hasCurrency ?Currency .
                }";

            /// <summary>
            /// SPARQL query - Get all versions of the contract
            /// </summary>
            public const string GetVersionsByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>

                SELECT ?Uri ?Issued ?ContractUri ?VersionOrder
                WHERE 
                {          
                ?Contract cn:version ?Uri.

                ?Uri dc:issued ?Issued ;
                cn:uri ?ContractUri ;
                cn:versionOrder ?VersionOrder.    

                FILTER regex(?Contract, @contract) 
                }";


            /// <summary>
            /// SPARQL query - Get all amendments of the contract
            /// </summary>
            public const string GetAmendmentsByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX schema: <http://schema.org/>

                SELECT ?Uri ?Title ?DateSigned ?Document
                WHERE 
                { 
                    ?Uri a cn:Amendment ;
                           dc:title ?Title ;
                           dc:created ?DateSigned ;
                           schema:url  ?Document ;
                           cn:contract @contract .
                }";

            /// <summary>
            /// SPARQL query - Get all attachments of the contract
            /// </summary>
            public const string GetAttachmentsByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX schema: <http://schema.org/>

                SELECT ?Uri ?Title ?Document
                WHERE 
                { 
                   ?Uri a cn:Attachment ;
                          dc:title ?Title ;
                          schema:url  ?Document ;
                          cn:contract @contract .
                }";

            /// <summary>
            /// SPARQL query - Get all parties of the contract
            /// </summary>
            public const string GetPartiesByContract =
                @"PREFIX cn: <http://tiny.cc/open-contracting#>
                PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>

                SELECT ?Id ?Uri ?Name ?Country ?PaysVat ?StreetAddres ?Locality ?PostalCode 
                WHERE 
                { 
                   @contract cn:party ?Uri .
                   ?Uri a gr:BusinessEntity ;
                          gr:legalName ?Name ;
                          schema:addressCountry ?Country ;
                          schema:address ?Address ;
                          cn:paysVAT ?PaysVat .

                   OPTIONAL {?Uri dc:identifier ?Id}

                   ?Address a schema:PostalAddress ;
                              schema:streetAddres ?StreetAddres ;
                              schema:addressLocality ?Locality ;
                              schema:postalCode ?PostalCode .       
                }";

            /// <summary>
            /// SPARQL query - Get all milestones of the contract
            /// </summary>
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

        /// <summary>
        /// SPARQL queries over the http://cs.dbpedia.org/sparql endpoint
        /// </summary>
        public static class CsDbpediaOrg
        {
            public const string SparqlEndpointUri = "http://cs.dbpedia.org/sparql";

            /// <summary>
            /// SPARQL query - Get image from DBpedia by subject name
            /// </summary>
            public const string GetPublisherImage =
                @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
                PREFIX dbpedia-owl: <http://dbpedia.org/ontology/>
                
                SELECT DISTINCT ?city ?img
                WHERE {
                   ?city rdfs:label @publisher@cs;
                         dbpedia-owl:thumbnail ?img .       
                }";
        }

        /// <summary>
        /// SPARQL queries over the http://linked.opendata.cz/sparql endpoint
        /// </summary>
        public static class LinkedOpenDataCz
        {
            public const string SparqlEndpointUri = "http://linked.opendata.cz/sparql";

            /// <summary>
            /// SPARQL query - Get opening hours business entity from public authorities
            /// Using "reverse" predicate http://linked.opendata.cz/ontology/domain/seznam.gov.cz/ovm/business-entity
            /// </summary>
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
                }";

            /// <summary>
            /// SPARQL query - Get RÚIAN link (addressPoint) by business entity (through ARES adsress link)
            /// </summary>
            public const string GetBusinessEntityRuianLink =
                @"PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX schema: <http://schema.org/>

                SELECT *
                WHERE 
                {
                  @businessEntity s:address ?address .

                  ?address ruianlink:adresni-misto ?ruianLink .
                                                                
                  FILTER(CONTAINS(str(?address), 'ares'))
                }";

            /// <summary>
            /// SPARQL query - Get public contracts by business entity
            /// Using "reverse" predicate http://purl.org/procurement/public-contracts#
            /// </summary>
            public const string GetBusinessEntityPublicContracts =
                @"PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>
                PREFIX pc: <http://purl.org/procurement/public-contracts#>
                PREFIX pccz: <http://purl.org/procurement/public-contracts-czech#>
                PREFIX skos: <http://www.w3.org/2004/02/skos/core#>

                SELECT DISTINCT ?Uri ?ContractId ?EvNumber ?Title ?SupplierUri ?Id ?Amount ?Vat
                WHERE 
                {
                  ?Uri pc:contractingAuthority @businessEntity ;
                     dc:title ?Title .
     
                  OPTIONAL 
                  {
                    ?Uri pccz:kodprofil ?ContractId ;
                         pccz:kodusvzis ?EvNumber  ;
                         pco:awardedTender ?Tender .
                    ?Tender pco:offeredPrice ?PriceSpec ;
                            pco:supplier ?SupplierUri . 
                    ?SupplierUri gr:legalName ?Supplier . 
               
                    BIND(CONCAT(str(?SupplierUri), '/identifier') as ?IcStr)
                    BIND(URI(?IcStr) as ?IcUri)

                    ?IcUri skos:notation ?Id .
                    ?PriceSpec gr:hasCurrencyValue ?Amount ;
                               gr:valueAddedTaxIncluded	1 .
                  }      
                }";
        }

        /// <summary>
        /// SPARQL queries over the http://ruian.linked.opendata.cz/sparql endpoint
        /// </summary>
        public static class RuianLinkedOpenDataCz
        {
            public const string SparqlEndpointUri = "http://ruian.linked.opendata.cz/sparql";

            /// <summary>
            /// SPARQL query - Get geo coordinates by address point from RÚIAN
            /// </summary>
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