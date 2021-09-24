using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Helper;
using Model.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Model.Security
{
    public static class AuthorityExtensions
    {
        static bool checkAuth(SHA256 hash, OrganizationToken token, AuthTokenViewModel viewModel)
        {
            return viewModel.Authorization == token.ComputeAuthorization(hash, viewModel.Seed);
        }

        public static OrganizationToken CheckAuthToken(this GenericManager<EIVOEntityDataContext> models, Organization seller, AuthTokenViewModel viewModel)
        {
            SHA256 hash = SHA256.Create();

            var agents = models.GetTable<InvoiceIssuerAgent>().Where(i => i.IssuerID == seller.CompanyID)
                            .Select(a => a.AgentID);

            var tokenItems = models.GetTable<Organization>().Where(o => o.CompanyID == seller.CompanyID || agents.Contains(o.CompanyID))
                        .Join(models.GetTable<OrganizationToken>(), o => o.CompanyID, t => t.CompanyID, (o, t) => t)
                        .ToArray();

            foreach (var token in tokenItems)
            {
                if (checkAuth(hash, token, viewModel))
                    return token;
            }

            return null;
        }
    }
}
