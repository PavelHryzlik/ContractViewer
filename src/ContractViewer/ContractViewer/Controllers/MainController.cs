using System.Web.Mvc;
using ContractViewer.Models;

namespace ContractViewer.Controllers
{
    public class MainController : Controller
    {
        private readonly SparqlQueryHandler _handler = new SparqlQueryHandler();

        public ActionResult Index()
        {
            return View(_handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContracts));
        }

        public ActionResult SubjectDetail(string publisher)
        {
            var publisherViewModel = new PublisherViewModel
            {
                Publisher = _handler.GetPublisher(publisher),
                Contracts = _handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContractsByPublisherName, "publisher", publisher, LocalNodeType.Literal)
            };

            return View(publisherViewModel);
        }

        public ActionResult ContractDetail(string baseDomain, string contractId, string version, string publisher)
        {
            var contract = _handler.GetContract(baseDomain, contractId, version, publisher);

            var contractViewModel = new ContractViewModel
            {
                Contract = contract,
                Parties = _handler.GetEntities<Party>(Constants.StudentOpenDataCz.GetPartiesByContract, "contract", contract.Uri, LocalNodeType.Uri),
                Attachments = _handler.GetEntities<Attachment>(Constants.StudentOpenDataCz.GetAttachmentsContract, "contract", contract.Uri, LocalNodeType.Uri),
                Amendments = _handler.GetEntities<Amendment>(Constants.StudentOpenDataCz.GetAmendmentsByContract, "contract", contract.Uri, LocalNodeType.Uri),
                Milestones = _handler.GetEntities<Milestone>(Constants.StudentOpenDataCz.GetMilestonesByContract, "contract", contract.Uri, LocalNodeType.Uri)
            };

            return View(contractViewModel);
        }

        public ActionResult PublicContracts(string Name, string ID)
        {
            var subjectUri = "http://linked.opendata.cz/resource/business-entity/CZ" + ID;

            var publickContractViewModel = new PublickContractViewModel
            {
                Id = ID,  
                Name = Name,
                PublicContract = _handler.GetEntities<PublicContract>(Constants.LinkedOpenDataCz.GetBusinessEntityPublicContracts, "businessEntity", subjectUri, LocalNodeType.Uri, Constants.LinkedOpenDataCz.SparqlEndpointUri)
            };

            return View(publickContractViewModel);
        }
    }
}