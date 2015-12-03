using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContractViewer.Models;
using VDS.RDF.Query;
using System.Reflection;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace ContractViewer.Controllers
{
    /// <summary>
    /// Enum of type of RDF object
    /// </summary>
    public enum LocalNodeType
    {
        None,
        Uri,
        Literal
    }

    /// <summary>
    /// Main handler for executing and parsing SPARQL queries over SPARQL endpoints
    /// </summary>
    public class SparqlQueryHandler
    {
        /// <summary>
        /// Generic method gets entities by specified type without parameters
        /// Typically allows Contract, Party, Attachment, Amendment, Milestone, Version type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="query">SPARQL query</param>
        /// <returns>Collection of entities with corresponding type</returns>
        public IEnumerable<T> GetEntities<T>(string query)
        {
            return GetEntities<T>(query, String.Empty, String.Empty, LocalNodeType.None);
        }

        /// <summary>
        /// Generic method gets entities by specified type
        /// Typically allows Contract, Party, Attachment, Amendment, Milestone, Version type
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="query">SPARQL query</param>
        /// <param name="variable">Variable name for SPARQL query</param>
        /// <param name="substituteValue">Value of the variable</param>
        /// <param name="nodeType">Node type of variable value (Uri/Literal)</param>
        /// <param name="endpointUri">SPARQL query URI</param>
        /// <returns>Collection of entities with corresponding type</returns>
        public IEnumerable<T> GetEntities<T>(string query, string variable, string substituteValue, LocalNodeType nodeType, string endpointUri = Constants.StudentOpenDataCz.SparqlEndpointUri)
        {
            // Initialize the connection to the SPARQL endpoint byt parametr endpointUri
            var endpoint = new SparqlRemoteEndpoint(new Uri(endpointUri));

            // Assemble SPARQL query
            var queryString = new SparqlParameterizedString { CommandText = query };

            // Fill variable in SPARQL query
            switch (nodeType)
            {
                case LocalNodeType.Uri:
                    queryString.SetUri(variable, new Uri(substituteValue));
                    break;
                case LocalNodeType.Literal:
                    queryString.SetLiteral(variable, substituteValue);
                    break;
            }

            // Execute SPARQL query
            SparqlResultSet results = endpoint.QueryWithResultSet(queryString.ToString());

            var entities = new List<T>();
            var entityType = typeof(T);

            // Parse SPARQL results
            foreach (SparqlResult result in results.Results)
            {
                var contract = Activator.CreateInstance(typeof(T));

                if (!String.IsNullOrEmpty(substituteValue))
                {
                    PropertyInfo prop = entityType.GetProperty("Publisher");
                    prop?.SetValue(contract, substituteValue, null);
                }

                foreach (var var in result.Variables)
                {
                    // Get property of the entity of the corresponding type by variable name
                    PropertyInfo prop = entityType.GetProperty(var);

                    if (prop != null)
                    {
                        string text;
                        INode value = result.Value(var); // Get value of variable
                        if (value == null)
                            continue;

                        // Parse value and fill entity of the corresponding type
                        switch (value.NodeType)
                        {
                            case NodeType.Literal:
                                var node = W3CSpecHelper.FormatNode(value); // formatting according to W3C specification

                                if (node is BooleanNode)
                                {
                                    prop.SetValue(contract, ((BooleanNode)node).AsBoolean() ? "Ano" : "Ne", null);
                                }
                                else if (node is DateNode)
                                {
                                    prop.SetValue(contract, ((DateNode)node).AsDateTime(), null);
                                }
                                else if (node is DateTimeNode)
                                {
                                    prop.SetValue(contract, ((DateTimeNode)node).AsDateTime(), null);
                                }
                                else if (node is TimeSpanNode)
                                {
                                    prop.SetValue(contract, ((TimeSpanNode)node).AsTimeSpan(), null);
                                }
                                else
                                {
                                    prop.SetValue(contract, ((ILiteralNode)node).Value, null);
                                }
                                break;

                            case NodeType.Uri:
                                var uri = (IUriNode)value;

                                text = uri.Uri.ToString();
                                prop.SetValue(contract, text, null);

                                // Get specific values form URIs
                                if (var == "Uri")
                                {
                                    PropertyInfo propUri = entityType.GetProperty("BaseDomain");
                                    propUri?.SetValue(contract, uri.Uri.Authority.Replace("/", ""), null);

                                    propUri = entityType.GetProperty("ContractId");
                                    propUri?.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 2).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("Version");
                                    propUri?.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("AttachmentId");
                                    propUri?.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("AmendmentId");
                                    propUri?.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("LocalID");
                                    propUri?.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);
                                }

                                break;

                            default:
                                text = value.ToString();
                                prop.SetValue(contract, text, null);
                                break;
                        }
                    }
                }

                entities.Add((T)contract);
            }

            return entities;
        }

        /// <summary>
        /// Method gets contract by base domain, contract Id and contract version
        /// </summary>
        /// <param name="baseDomain">Base domain</param>
        /// <param name="contractId">Contract Id</param>
        /// <param name="version">Contract version</param>
        /// <param name="publisher">Publisher name (optional)</param>
        /// <returns>Result contract</returns>
        public Contract GetContract(string baseDomain, string contractId, string version, string publisher)
        {
            // Assemble SPARQL queries with corresponding parameter 
            var queryString = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetContract }; // Query to get contract
            var queryStringPriceSpec = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetPriceSpecByContract }; // Query to get price spec.
            var contractUri = new Uri(String.Format("http://{0}/contract/{1}/{2}", baseDomain, contractId, version));
            queryString.SetUri("contract", contractUri);
            queryStringPriceSpec.SetUri("contract", contractUri);

            // Initialize the connection to the SPARQL endpoint(http://student.opendata.cz/sparql)
            var endpoint = new SparqlRemoteEndpoint(new Uri(Constants.StudentOpenDataCz.SparqlEndpointUri));

            // Execute SPARQL queries
            SparqlResultSet results = endpoint.QueryWithResultSet(queryString.ToString());
            SparqlResultSet resultsPriceSpec = endpoint.QueryWithResultSet(queryStringPriceSpec.ToString());

            var contract = new Contract
            {
                Uri = contractUri.ToString(),
                BaseDomain = baseDomain,
                ContractId = contractId,
                Version = version,
                Publisher = publisher
            };

            // Parse price spec. SPARQL results
            foreach (SparqlResult result in resultsPriceSpec.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("Amount").ToString()))
                {
                    contract.Amount = ((ILiteralNode)result.Value("Amount")).Value;
                }

                if (!String.IsNullOrEmpty(result.Value("Currency").ToString()))
                {
                    contract.Currency = result.Value("Currency").ToString();
                }
            }

            // Parse contract SPARQL results
            foreach (SparqlResult result in results.Results)
            {
                string predicate = String.Empty;
                foreach (var var in result.Variables)
                {
                    if (var == "p")
                    {
                        var urinode = ((IUriNode)result.Value(var));

                        // Get predicate name pro predicate URI
                        predicate = !String.IsNullOrEmpty(urinode.Uri.Fragment)
                            ? urinode.Uri.Fragment.Replace("#", "")
                            : urinode.Uri.Segments.Last();
                    }

                    // Parse contract values by predicate names
                    if (var == "o")
                    {
                        INode value = result.Value(var);

                        switch (value.NodeType)
                        {
                            case NodeType.Literal:
                                var node = W3CSpecHelper.FormatNode(value); // formatting according to W3C specification

                                if (predicate == "awardID")
                                {
                                    contract.AwardId = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "title")
                                {
                                    contract.Title = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "contractType")
                                {
                                    contract.ContractType = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "description")
                                {
                                    contract.Description = ((ILiteralNode)node).Value;
                                }

                                if (predicate == "created")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.DateSigned = ((DateNode)node).AsDateTime();
                                    }
                                }
                                if (predicate == "validFrom")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.ValidFrom = ((DateNode)node).AsDateTime();
                                    }
                                }
                                if (predicate == "validUntil")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.ValidUntil = ((DateNode)node).AsDateTime();
                                    }
                                }

                                if (predicate == "competency")
                                {
                                    contract.Competency = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "anonymised")
                                {
                                    if (node is BooleanNode)
                                    {
                                        contract.Anonymised = ((BooleanNode)node).AsBoolean() ? "Ano" : "Ne";
                                    }  
                                }
                                
                                break;

                            case NodeType.Uri:
                                var uri = (IUriNode)value;

                                if (predicate == "url")
                                {
                                    contract.Document = uri.Uri.ToString();
                                }

                                if (predicate == "publicContract")
                                {
                                    contract.AwardUrl = uri.Uri.ToString();
                                }

                                break;
                        }
                    }
                }
            }

            return contract;
        }
        
        /// <summary>
        /// Methods get all publishers
        /// </summary>
        /// <returns>Collection of publishers</returns>
        public IEnumerable<Publisher> GetPublishers()
        {
            var subjects = new List<Publisher>();

            // Assemble SPARQL query
            var slodQueryString = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetPublishers };

            // Initialize the connection to the SPARQL endpoint(http://student.opendata.cz/sparql)
            var slodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.StudentOpenDataCz.SparqlEndpointUri));

            // Execute SPARQL query
            SparqlResultSet slotResults = slodEndpoint.QueryWithResultSet(slodQueryString.ToString());

            // Parse SPARQL results
            foreach (SparqlResult result in slotResults.Results)
            {
                var publisher = new Publisher();
                if (!String.IsNullOrEmpty(result.Value("Subject").ToString()))
                {
                    publisher.Name = result.Value("Subject").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("Ic").ToString()))
                {
                    publisher.Id = result.Value("Ic").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("AresLink").ToString()))
                {
                    publisher.AresUrl = result.Value("AresLink").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("ContractSum").ToString()))
                {
                    publisher.NumberOfContracts = Convert.ToInt32(((ILiteralNode)result.Value("ContractSum")).Value);
                }

                // Fill coordinates
                GetPublisherCoordinates(ref publisher);

                //// For test, when linked.data.cz/sparql failed
                //if (publisher.Name == "Třebíč")
                //{
                //    publisher.GeoPoint = new GeoPoint { Latitude = (decimal)49.22, Longitude = (decimal)15.88 };
                //}
                //if (publisher.Name == "Děčín")
                //{
                //    publisher.GeoPoint = new GeoPoint { Latitude = (decimal)50.7736, Longitude = (decimal)14.1961 };
                //}
                //if (publisher.Name == "Praha")
                //{
                //    publisher.GeoPoint = new GeoPoint { Latitude = (decimal)50.0880400, Longitude = (decimal)14.4207600 };
                //}

                // Fill publisher image
                GetPublisherImage(ref publisher);
                
                subjects.Add(publisher);
            }

            return subjects;
        }

        /// <summary>
        /// Method gets publisher by his IC
        /// </summary>
        /// <param name="publisherName">Publisher name</param>
        /// <param name="publisherId">Publisher Id - IC</param>
        /// <returns>Return publisher object</returns>
        public Publisher GetPublisher(string publisherName, string publisherId)
        {
            var publisher = new Publisher { Name = publisherName, Id = publisherId };

            // Get publisher info
            GetPublisherByIc(ref publisher);

            // Fill openign hours
            GetPublisherOpeningHours(ref publisher);

            // Fill coordinates
            GetPublisherCoordinates(ref publisher);

            // Fill publisher image
            GetPublisherImage(ref publisher);

            return publisher;
        }

        /// <summary>
        /// Method gets publisher info (filling publisher object)
        /// </summary>
        /// <param name="publisher">Reference to publisher object</param>
        private void GetPublisherByIc(ref Publisher publisher)
        {
            // Assemble SPARQL query with corresponding parameter 
            var slodQueryString = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetPublisherByIc };
            slodQueryString.SetLiteral("ic", publisher.Id);

            // Initialize the connection to the SPARQL endpoint(http://student.opendata.cz/sparql)
            var slodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.StudentOpenDataCz.SparqlEndpointUri));

            // Execute SPARQL query
            SparqlResultSet slotResults = slodEndpoint.QueryWithResultSet(slodQueryString.ToString());

            // Parse SPARQL results
            foreach (SparqlResult result in slotResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("publisher").ToString()))
                {
                    publisher.Url = result.Value("publisher").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("aresLink").ToString()))
                {
                    publisher.AresUrl = result.Value("aresLink").ToString();
                }
            }
        }

        /// <summary>
        /// Method gets publisher opening hours (filling publisher object)
        /// </summary>
        /// <param name="publisher">Reference to publisher object</param>
        private void GetPublisherOpeningHours(ref Publisher publisher)
        {
            // Assemble SPARQL query with corresponding parameter 
            var lodGetOpeningHoursQueryString = new SparqlParameterizedString { CommandText = Constants.LinkedOpenDataCz.GetBusinessEntityOpeningHours };
            lodGetOpeningHoursQueryString.SetUri("businessEntity", new Uri(publisher.AresUrl));

            // Initialize the connection to the SPARQL endpoint(http://linked.opendata.cz/sparql)
            var lodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.LinkedOpenDataCz.SparqlEndpointUri));

            // Execute SPARQL query
            SparqlResultSet lodGetOpeningHoursResults = lodEndpoint.QueryWithResultSet(lodGetOpeningHoursQueryString.ToString());

            var localPlaces = new List<LocalPlace>();

            // Parse SPARQL results and fill to LocalPlace collection
            foreach (SparqlResult result in lodGetOpeningHoursResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("localPlace").ToString()))
                {
                    var localPlaceStr = result.Value("localPlace").ToString();

                    // If we already set local place, get that one
                    LocalPlace localPlace;
                    if (localPlaces.Any(l => l.Url == localPlaceStr))
                    {
                        localPlace = localPlaces.First(l => l.Url == localPlaceStr);
                    }
                    else
                    {
                        localPlace = new LocalPlace
                        {
                            Url = localPlaceStr,
                            OpeningHours = new SortedDictionary<DayOfWeekCz, ICollection<OpeningHour>>()
                        };

                        if (!String.IsNullOrEmpty(result.Value("streetAddress").ToString()))
                        {
                            localPlace.StreetAddress = result.Value("streetAddress").ToString();
                        }

                        if (!String.IsNullOrEmpty(result.Value("postalCode").ToString()))
                        {
                            localPlace.PostalCode = result.Value("postalCode").ToString();
                        }
                    }

                    // Fill opening hours
                    GetOpeningHours(result, localPlace.OpeningHours);

                    if (localPlaces.All(l => l.Url != localPlaceStr))
                    {
                        localPlaces.Add(localPlace);
                    }
                }
            }

            publisher.LocalPlaces = localPlaces;
        }

        /// <summary>
        /// Method parse SPARQL result to dictionary of opening hours
        /// </summary>
        /// <param name="result">Input SPARQL result</param>
        /// <param name="openingHours">Output dictionary of opening hours</param>
        private void GetOpeningHours(SparqlResult result, SortedDictionary<DayOfWeekCz, ICollection<OpeningHour>> openingHours)
        {
            if (!String.IsNullOrEmpty(result.Value("dayOfWeek").ToString()))
            {
                var dayOfWeekStr = result.Value("dayOfWeek").ToString().Replace("http://purl.org/goodrelations/v1#", String.Empty);

                var openingHour = new OpeningHour();
                if (!String.IsNullOrEmpty(result.Value("open").ToString()) && !String.IsNullOrEmpty(result.Value("close").ToString()))
                {
                    openingHour = new OpeningHour
                    {
                        Open = ((TimeSpanNode)W3CSpecHelper.FormatNode(result.Value("open"))).AsTimeSpan(), // formatting according to W3C specification
                        Close = ((TimeSpanNode)W3CSpecHelper.FormatNode(result.Value("close"))).AsTimeSpan() // formatting according to W3C specification
                    };
                }

                var dayOfWeek = (DayOfWeekCz)Enum.Parse(typeof(DayOfWeekCz), dayOfWeekStr);

                if (openingHours.Any(d => d.Key == dayOfWeek))
                {
                    openingHours[dayOfWeek].Add(openingHour);
                    openingHours[dayOfWeek] = openingHours[dayOfWeek].OrderBy(o => o.Open).ToList(); // sort opening hours by open hour of the day
                }
                else
                {
                    openingHours[dayOfWeek] = new List<OpeningHour> {openingHour};
                }
            }
        }

        /// <summary>
        /// Method gets publisher coordinates (filling publisher object)
        /// </summary>
        /// <param name="publisher">Reference to publisher object</param>
        private void GetPublisherCoordinates(ref Publisher publisher)
        {
            // Assemble SPARQL query with corresponding parameter 
            var lodRuianLinkQueryString = new SparqlParameterizedString { CommandText = Constants.LinkedOpenDataCz.GetBusinessEntityRuianLink };
            lodRuianLinkQueryString.SetUri("businessEntity", new Uri(publisher.AresUrl));

            // Initialize the connection to the SPARQL endpoint(http://linked.opendata.cz/sparql)
            var lodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.LinkedOpenDataCz.SparqlEndpointUri));

            // Execute SPARQL query
            SparqlResultSet lodGetRuianLinkResults = lodEndpoint.QueryWithResultSet(lodRuianLinkQueryString.ToString());

            var sparqlResult = lodGetRuianLinkResults.FirstOrDefault(result => !String.IsNullOrEmpty(result.Value("ruianLink").ToString()));
            if (sparqlResult != null)
            {
                // Assemble SPARQL query with corresponding parameter 
                var rlodCoordinatesQueryString = new SparqlParameterizedString { CommandText = Constants.RuianLinkedOpenDataCz.GetBusinessEntityCoordinates };
                rlodCoordinatesQueryString.SetUri("addressPoint", new Uri(sparqlResult.Value("ruianLink").ToString()));

                // Initialize the connection to the SPARQL endpoint(RÚIAN - http://ruian.linked.opendata.cz/sparql)
                var rlodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.RuianLinkedOpenDataCz.SparqlEndpointUri));

                // Execute SPARQL query
                SparqlResultSet rlodCoordinatesResults = rlodEndpoint.QueryWithResultSet(rlodCoordinatesQueryString.ToString());

                // Parse SPARQL results
                foreach (SparqlResult result in rlodCoordinatesResults.Results)
                {
                    var geoPoint = new GeoPoint();
                    if (!String.IsNullOrEmpty(result.Value("latitude").ToString()))
                    {
                        geoPoint.Latitude = Convert.ToDecimal(result.Value("latitude").ToString(), CultureInfo.InvariantCulture);
                    }

                    if (!String.IsNullOrEmpty(result.Value("longitude").ToString()))
                    {
                        geoPoint.Longitude = Convert.ToDecimal(result.Value("longitude").ToString(), CultureInfo.InvariantCulture);
                    }
                    publisher.GeoPoint = geoPoint;
                }
            }
        }

        /// <summary>
        /// Method tries to get publisher image (filling publisher object) form DBpedia by publisher name
        /// </summary>
        /// <param name="publisher">Reference to publisher object</param>
        private void GetPublisherImage(ref Publisher publisher)
        {
            // Assemble SPARQL query with corresponding parameter 
            var dbpdQueryString = new SparqlParameterizedString { CommandText = Constants.CsDbpediaOrg.GetPublisherImage };
            dbpdQueryString.SetLiteral("publisher", publisher.Name);

            // Initialize the connection to the SPARQL endpoint (DBPedia - http://cs.dbpedia.org/sparql)
            var dbpdEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.CsDbpediaOrg.SparqlEndpointUri));

            // Execute SPARQL query
            SparqlResultSet dbPediaResults = dbpdEndpoint.QueryWithResultSet(dbpdQueryString.ToString());

            // Parse SPARQL results
            foreach (SparqlResult result in dbPediaResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("img").ToString()))
                {
                    publisher.ImageUrl = result.Value("img").ToString();
                }
            }
        }
    }
}