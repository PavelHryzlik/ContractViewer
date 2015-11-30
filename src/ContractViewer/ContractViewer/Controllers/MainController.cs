﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ContractViewer.Models;
using Microsoft.Ajax.Utilities;

namespace ContractViewer.Controllers
{
    public class MainController : Controller
    {
        private readonly SparqlQueryHandler _handler = new SparqlQueryHandler();

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

        [HandleError]
        public ActionResult SubjectDetail(string publisher, string publisherId)
        {
            var publisherViewModel = new PublisherViewModel
            {
                Publisher = _handler.GetPublisher(publisher, publisherId),
                Contracts = _handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContractsByPublisherIc, "publisherId", publisher, LocalNodeType.Literal)
            };

            return View(publisherViewModel);
        }

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

        [HandleError]
        public ActionResult PublicContracts(string Name, string ID)
        {
            var subjectUri = "http://linked.opendata.cz/resource/business-entity/CZ" + ID;

            var publickContractViewModel = new PublicContractViewModel
            {
                Id = ID,  
                Name = Name,
                PublicContract = _handler.GetEntities<PublicContract>(Constants.LinkedOpenDataCz.GetBusinessEntityPublicContracts, "businessEntity", subjectUri, LocalNodeType.Uri, Constants.LinkedOpenDataCz.SparqlEndpointUri)
            };

            return View(publickContractViewModel);
        }

        [HandleError]
        public ActionResult About()
        {
            return View();
        }
    }
}