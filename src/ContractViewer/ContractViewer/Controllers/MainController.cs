using System.Web.Mvc;
using ContractViewer.Handlers;
using ContractViewer.Models;

namespace ContractViewer.Controllers
{
    /// <summary>
    /// Main controller of the application. Handle requests and return corresponding views 
    /// </summary>
    public class MainController : Controller
    {
        private readonly SparqlQueryHandler _handler = new SparqlQueryHandler();

        /// <summary>
        /// Handle the request to main page
        /// Get all publishers and contracts
        /// </summary>
        /// <returns>Return main page</returns>
        [HandleError]
        public ActionResult Index()
        {
            var indexViewModel = new IndexViewModel
            {
                Subjects = _handler.GetPublishers(),
                GoogleMap = new GoogleMapViewModel(),
                Contracts = _handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContracts)
            };

            return View(indexViewModel);
        }

        /// <summary>
        /// Handle the request to publisher detail page
        /// Get publisher by publisher IC and corresponding contracts
        /// </summary>
        /// <param name="publisher">Publisher name</param>
        /// <param name="publisherId">Publisher Id - IC</param>
        /// <returns>>Return page of the publisher detail</returns>
        [HandleError]
        public ActionResult SubjectDetail(string publisher, string publisherId)
        {
            var publisherViewModel = new PublisherViewModel
            {
                Publisher = _handler.GetPublisher(publisher, publisherId),
                Contracts = _handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContractsByPublisherIc, "publisherId", publisherId, LocalNodeType.Literal)
            };

            return View(publisherViewModel);
        }

        /// <summary>
        /// Handle the request to contract detail page
        /// Get contract by base domain, contract Id and contract version and then
        /// get corresponding parties, attachments, amendments, milestones and versions
        /// </summary>
        /// <param name="baseDomain">Base domain</param>
        /// <param name="contractId">Contract Id</param>
        /// <param name="version">Contract version</param>
        /// <param name="publisher">Publisher name (optional)</param>
        /// <returns>Return page of the contract detail</returns>
        [HandleError]
        public ActionResult ContractDetail(string baseDomain, string contractId, string version, string publisher)
        {
            var contract = _handler.GetContract(baseDomain, contractId, version, publisher);

            var contractViewModel = new ContractViewModel
            {
                Contract = contract,
                Parties = _handler.GetEntities<Party>(Constants.StudentOpenDataCz.GetPartiesByContract, "contract", contract.Uri, LocalNodeType.Uri),
                Attachments = _handler.GetEntities<Attachment>(Constants.StudentOpenDataCz.GetAttachmentsByContract, "contract", contract.Uri, LocalNodeType.Uri),
                Amendments = _handler.GetEntities<Amendment>(Constants.StudentOpenDataCz.GetAmendmentsByContract, "contract", contract.Uri, LocalNodeType.Uri),
                Milestones = _handler.GetEntities<Milestone>(Constants.StudentOpenDataCz.GetMilestonesByContract, "contract", contract.Uri, LocalNodeType.Uri),
                Versions = _handler.GetEntities<Version>(Constants.StudentOpenDataCz.GetVersionsByContract, "contract", contract.Uri.Substring(0, contract.Uri.LastIndexOf('/')), LocalNodeType.Uri)
            };

            return View(contractViewModel);
        }

        /// <summary>
        /// Handle the request to public contracts page
        /// Get public contracts by subject ID - IC
        /// </summary>
        /// <param name="name">Subject name</param>
        /// <param name="id">Subject ID - IC</param>
        /// <returns>Return page of public contracts</returns>
        [HandleError]
        public ActionResult PublicContracts(string name, string id)
        {
            var subjectUri = "http://linked.opendata.cz/resource/business-entity/CZ" + id;

            var publickContractViewModel = new PublicContractViewModel
            {
                Id = id,  
                Name = name,
                PublicContract = _handler.GetEntities<PublicContract>(Constants.LinkedOpenDataCz.GetBusinessEntityPublicContracts, "businessEntity", subjectUri, LocalNodeType.Uri, Constants.LinkedOpenDataCz.SparqlEndpointUri)
            };

            return View(publickContractViewModel);
        }

        /// <summary>
        /// Handle the request to about page
        /// Get info about the application
        /// </summary>
        /// <returns>Return about page</returns>
        [HandleError]
        public ActionResult About()
        {
            return View();
        }
    }
}